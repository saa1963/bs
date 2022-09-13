#include "DetachedSignLib.h"
#include <cstdio>

#define MAX_SIZE_CERT_NAME 1024

static PCCERT_CONTEXT
FindCertificate3(BYTE* thumbprint)
{
    PCCERT_CONTEXT capiCertificate = NULL;
    WCHAR certname[MAX_SIZE_CERT_NAME] = { 0 };
    HCERTSTORE hStore;
    DWORD dwFlag;

    for (int i = 0; i < 2; i++)
    {
        if (i == 0) dwFlag = CERT_SYSTEM_STORE_LOCAL_MACHINE;
        else dwFlag = CERT_SYSTEM_STORE_LOCAL_MACHINE;
        dwFlag = CERT_SYSTEM_STORE_CURRENT_USER;
        hStore = CertOpenStore(CERT_STORE_PROV_SYSTEM_W,
            X509_ASN_ENCODING | PKCS_7_ASN_ENCODING,
            NULL, CERT_STORE_OPEN_EXISTING_FLAG |
            dwFlag | CERT_STORE_READONLY_FLAG, L"MY");

        if (hStore != nullptr)
        {
            CRYPT_INTEGER_BLOB cib;
            cib.cbData = 20;
            cib.pbData = thumbprint;

            capiCertificate = CertFindCertificateInStore(hStore,
                X509_ASN_ENCODING | PKCS_7_ASN_ENCODING, 0,
                CERT_FIND_SHA1_HASH, &cib, NULL);

            if ((NULL == capiCertificate) || CertVerifyTimeValidity(NULL, capiCertificate->pCertInfo) != 0)
            {
                capiCertificate = NULL;
                CertCloseStore(hStore, 0);
            }
            else
            {
                break;
            }
        }
    }
    return capiCertificate;
}

static LPCWSTR HandleError(LPCWSTR custMsg, WCHAR* errmsg, DWORD errBuffer_len)
{
    DWORD err = GetLastError();
    wcscpy_s(errmsg, errBuffer_len, custMsg);
    wcscat_s(errmsg, errBuffer_len, L"\n");
    wcscat_s(errmsg, errBuffer_len, L"Win32 Error: ");
    _ultow_s(err, errmsg + wcslen(errmsg), errBuffer_len - wcslen(errmsg), 10);
    wcscat_s(errmsg, errBuffer_len, L"\n");
    FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM,
        nullptr, err, 0, errmsg + wcslen(errmsg), errBuffer_len - wcslen(errmsg), nullptr);
    return errmsg;
}

DWORD _Sign(BYTE* message, DWORD cbmess_len,
    BYTE* signMessage, DWORD* sm_len, BYTE* thumbprint, LPWSTR errMessage, DWORD em_len, BOOL Detached)
{
    PCCERT_CONTEXT certContext = FindCertificate3(thumbprint);
    if (certContext != nullptr)
    {
        CRYPT_OBJID_BLOB hashParam;
        hashParam.cbData = 0;
        hashParam.pbData = nullptr;
        CRYPT_ALGORITHM_IDENTIFIER hashAlg;
        hashAlg.pszObjId = (LPSTR)"1.2.643.7.1.1.2.2";
        hashAlg.Parameters = hashParam;
        CRYPT_SIGN_MESSAGE_PARA signPara;
        signPara.cbSize = sizeof(signPara);
        signPara.dwMsgEncodingType = X509_ASN_ENCODING | PKCS_7_ASN_ENCODING;
        signPara.pSigningCert = certContext;
        signPara.HashAlgorithm = hashAlg;
        signPara.pvHashAuxInfo = nullptr;
        signPara.cMsgCert = 1;
        signPara.rgpMsgCert = &certContext;
        signPara.cMsgCrl = 0;
        signPara.cAuthAttr = 0;
        signPara.rgAuthAttr = NULL;
        signPara.cUnauthAttr = 0;
        signPara.dwFlags = 0;
        signPara.dwInnerContentType = 0;

        // Create the MessageArray and the MessageSizeArray.
        const BYTE* MessageArray[] = { message };
        DWORD_PTR MessageSizeArray[1];
        MessageSizeArray[0] = cbmess_len;

        if (signMessage == nullptr)
        {
            if (!CryptSignMessage(
                &signPara,
                Detached,
                1,
                MessageArray,
                MessageSizeArray,
                NULL,
                sm_len))
            {
                HandleError(L"Calculation size of buffer is failed.", errMessage, em_len);
                CertCloseStore(certContext->hCertStore, 0);
                CertFreeCertificateContext(certContext);
                return -1;
            }
            CertCloseStore(certContext->hCertStore, 0);
            CertFreeCertificateContext(certContext);
            return 0;
        }

        if (!CryptSignMessage(
            &signPara,
            Detached,
            1,
            MessageArray,
            MessageSizeArray,
            signMessage,
            sm_len))
        {
            HandleError(L"Sign message is failed.", errMessage, em_len);
            CertCloseStore(certContext->hCertStore, 0);
            CertFreeCertificateContext(certContext);
            return -2;
        }

        CertCloseStore(certContext->hCertStore, 0);
        CertFreeCertificateContext(certContext);
        return 1;
    }
    else
    {
        HandleError(L"Find certificate is failed.", errMessage, em_len);
        return -1;
    }
}

DWORD SignDetached(BYTE* message, DWORD cbmess_len,
    BYTE* signMessage, DWORD* sm_len, BYTE *thumbprint, LPWSTR errMessage, DWORD em_len)
{
    return _Sign(message, cbmess_len, signMessage, sm_len, thumbprint, errMessage, em_len, TRUE);
}

DWORD SignAttached(BYTE* message, DWORD cbmess_len,
    BYTE* signMessage, DWORD* sm_len, BYTE* thumbprint, LPWSTR errMessage, DWORD em_len)
{
    return _Sign(message, cbmess_len, signMessage, sm_len, thumbprint, errMessage, em_len, FALSE);
}
#include "pch.h"
#include <iostream>
#include <Windows.h>
#include <io.h>
#include <fcntl.h>
#include "..\..\bs\DetachedSignLib\DetachedSignLib.h"

#define MY_MSG "CryptoAPI is a good way to handle security"

namespace DetachedSignLinNS
{
	TEST(DetachedSign, Sign) {
		DWORD result = 0;
		BYTE* pbMessage = (BYTE*)MY_MSG;
		DWORD cbMessage = (strlen((CHAR*)pbMessage) + 1);
		BYTE* signMessage = nullptr;
		DWORD sm_len;
		WCHAR errMessage[1024];
		if (result = SignDetached(pbMessage, cbMessage, signMessage, &sm_len, L"ŒŒŒ \"’”— ¬¿–Õ¿\"", errMessage, 1024) == 0)
		{
			signMessage = new BYTE[sm_len];
			if (result = SignDetached(pbMessage, cbMessage, signMessage, &sm_len, L"ŒŒŒ \"’”— ¬¿–Õ¿\"", errMessage, 1024) != 1)
			{
				FAIL();
			}
			else
			{
				SUCCEED();
			}
			delete[] signMessage;
		}
		else
		{
			FAIL();
		}
		//EXPECT_EQ(result, 0);
	}
}
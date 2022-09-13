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
		// 1a30ea2c9853f1195005a88f0aa32ce62c7de9c5
		BYTE thumbprint[20]{0x1a, 0x30, 0xea, 0x2c, 0x98, 0x53, 0xf1, 0x19, 0x50, 0x05, 0xa8, 0x8f, 0x0a, 0xa3, 0x2c, 0xe6, 0x2c, 0x7d, 0xe9, 0xc5};
		if (result = SignDetached(pbMessage, cbMessage, signMessage, &sm_len, thumbprint, errMessage, 1024) == 0)
		{
			signMessage = new BYTE[sm_len];
			if (result = SignDetached(pbMessage, cbMessage, signMessage, &sm_len, thumbprint, errMessage, 1024) != 1)
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
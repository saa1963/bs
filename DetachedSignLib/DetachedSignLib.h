#pragma once
#include <Windows.h>

#ifdef DETACHEDSIGNLIB_EXPORTS
#define DETACHEDSIGNLIB_API __declspec(dllexport)
#else
#define DETACHEDSIGNLIB_API __declspec(dllimport)
#endif

extern "C" DETACHEDSIGNLIB_API
DWORD SignDetached(BYTE * message, DWORD cbmess_len,
    BYTE * signMessage, DWORD * sm_len, LPCWSTR cert, LPWSTR errMessage, DWORD em_len);

extern "C" DETACHEDSIGNLIB_API
DWORD SignAttached(BYTE * message, DWORD cbmess_len,
    BYTE * signMessage, DWORD * sm_len, LPCWSTR cert, LPWSTR errMessage, DWORD em_len);

// Pos.h : Declaration of the CPos

#pragma once
#include "resource.h"       // main symbols
#include <iostream>
#include <windows.h>
#include <string>
#include <thread>
#include <chrono>
#include <comdef.h>
#include "KZIngenicoX_i.h"



#if defined(_WIN32_WCE) && !defined(_CE_DCOM) && !defined(_CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA)
#error "Single-threaded COM objects are not properly supported on Windows CE platform, such as the Windows Mobile platforms that do not include full DCOM support. Define _CE_ALLOW_SINGLE_THREADED_OBJECTS_IN_MTA to force ATL to support creating single-thread COM object's and allow use of it's single-threaded COM object implementations. The threading model in your rgs file was set to 'Free' as that is the only threading model supported in non DCOM Windows CE platforms."
#endif

using namespace ATL;
using namespace std;

typedef struct CurrencyParams
{
	char CurAlpha[3]; // [in] символьный код валюты.
	unsigned char nDecPoint; // [in] позиция десятичной точки при отображении валюты.
};


typedef struct ScreenParams
{
	long len; // [in]
	long screenID; // [in]
	long maxInp; // [in]
	long minInp; // [in]
	unsigned long format; // [in]
	LPCSTR pTitle; // [in]
	LPCSTR pStr[10]; // [in]
	LPCSTR pInitStr; // [in]
	LPCSTR pButton0; // [in]
	LPCSTR pButton1; // [in]
	CurrencyParams CurParam; // [in]
	long eventKey; // [out]
	char* pBuf; // [in]
};
typedef struct Response
{
	string AUTHNR;
	string EXP;
	string PAN;
	string INVOICENR;
	string DATE;
	string TIME;
	string ISSUER;
	string RRN;
	string VISUALHOSTRESPONCE;
	string APPROVE;
	string RESPONSECODE;
};

typedef int (*ScreenShow)(ScreenParams* pScreenParams);
typedef void (*ScreenClose)(void);
typedef int (*Init)(const char*, ScreenShow, ScreenClose);
typedef int (*Proc)(const char* in_params, int* len_out_params, int* len_receipt);
typedef int (*GetRsp)(char* out_params, char* receipt);
typedef int (*Close)(void);

ScreenShow TRGUI_ScreenShow;
ScreenClose TRGUI_ScreenClose;

typedef struct State
{
	int stateNr;
	string stateMessage;
	State() : stateNr(0), stateMessage("") {};
};



string receipt;
string slip;
static Response response;
string _LastStatMsgDescription;
string _LastErrorDescription;
int _LastError;
int _LastStatMsgCode;
string outStr;



int nr;
int error;
Init TRPOSX_Init;
Proc TRPOSX_Proc;
GetRsp TRPOSX_GetResp;
Close TRPOSX_Close;
int Stop();
static HINSTANCE Connect(LPCWSTR, string);
static void Purchase(HINSTANCE, string, string, int);
static int Getresponce(HINSTANCE);
string bstr_to_str(BSTR source);
BSTR str_to_bstr(string source);



static int _ScreenShow(ScreenParams* pScreenParams);
static void _ScreenClose();

HINSTANCE posTerminal;

class ATL_NO_VTABLE CPos :
	public CComObjectRootEx<CComSingleThreadModel>,
	public CComCoClass<CPos, &CLSID_Pos>,
	public IDispatchImpl<IPos, &IID_IPos, &LIBID_KZIngenicoXLib, /*wMajor =*/ 1, /*wMinor =*/ 0>
	
	
{
public:
	CPos()
	{
		setlocale(LC_ALL, "Russian");
	}

DECLARE_REGISTRY_RESOURCEID(106)


BEGIN_COM_MAP(CPos)
	COM_INTERFACE_ENTRY(IPos)
	COM_INTERFACE_ENTRY(IDispatch)
END_COM_MAP()



	DECLARE_PROTECT_FINAL_CONSTRUCT()

	HRESULT FinalConstruct()
	{
		return S_OK;
	}

	void FinalRelease()
	{
	}
	
	static HINSTANCE Connect(LPCWSTR dllPath, string setupPath);
	static void Purchase(HINSTANCE instance, string ECRNumber, string transactionNumber, int amount);
public:
	
	/*STDMETHOD(Add)(DOUBLE a, DOUBLE b, DOUBLE * res);
	STDMETHOD(Connect1)(BYTE* result);*/
	STDMETHOD(Initialize)(BSTR dllPath, BSTR setupPath, BYTE* result);
	STDMETHOD(StartPurchase)(DOUBLE amount, BSTR ECRNr, BSTR TRNr, byte* result);
	STDMETHOD(GetResponce)();
	STDMETHOD(get_lastError)(LONG* pVal);
	STDMETHOD(get_LastErrorDescription)(BSTR* pVal);
	STDMETHOD(get_LastStatMsgCode)(LONG* pVal);
	STDMETHOD(get_LastStatMsgDescription)(BSTR* pVal);
	STDMETHOD(get_LastErrorCode)(LONG* pVal);
	STDMETHOD(get_ResponseCode)(LONG* pVal);
	STDMETHOD(get_PAN)(BSTR* pVal);
	STDMETHOD(get_ExpDate)(BSTR* pVal);
	STDMETHOD(get_IssuerName)(BSTR* pVal);
	STDMETHOD(get_AuthCode)(BSTR* pVal);
	STDMETHOD(get_RRN)(BSTR* pVal);
	STDMETHOD(get_Receipt)(BSTR* pVal);
	STDMETHOD(get_Amount)(LONG* pVal);
	
	STDMETHOD(get_outString)(BSTR* pVal);
};

OBJECT_ENTRY_AUTO(__uuidof(Pos), CPos)

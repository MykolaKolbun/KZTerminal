// KZ_Console_App.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <windows.h>
#include <string>
#include <thread>
#include <chrono>

using namespace std;
HINSTANCE  trposx;
HINSTANCE  trgui;

typedef struct State
{
    int stateNr;
    string stateMessage;
    State() : stateNr(0), stateMessage("") {};
};

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
    const char* pTitle; // [in]
    const char* pStr[10]; // [in]
    const char* pInitStr; // [in]
    const char* pButton0; // [in]
    const char* pButton1; // [in]
    CurrencyParams CurParam; // [in]
    long eventKey; // [out]
    char* pBuf; // [in]
};

typedef struct Responce
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
    string RESPONCECODE;
};

typedef int (*ScreenShow)(ScreenParams* pScreenParams);
typedef void (*ScreenClose)(void);
typedef int (*Init)(const char*, ScreenShow, ScreenClose);
typedef int (*Proc)(const char* in_params, int* len_out_params, int* len_receipt);
typedef int (*GetRsp)(char* out_params, char* receipt);
typedef int (*Close)(void);

ScreenShow TRGUI_ScreenShow;
ScreenClose TRGUI_ScreenClose;

static State state;

Init TRPOSX_Init;
Proc TRPOSX_Proc;
GetRsp TRPOSX_GetResp;
Close TRPOSX_Close;
int Stop();
static HINSTANCE Connect(LPCWSTR, string);
static void Purchase(HINSTANCE, string, string, int, int&, string&);

static int _ScreenShow(ScreenParams* pScreenParams)
{
    state.stateMessage = "";
    for (int i = 0; i < 10; i++)
    {
        if (pScreenParams->pStr[i] != NULL)
        {
            /* this_thread::sleep_for(std::chrono::milliseconds(100));
             state.stateMessage = to_string(i)+state.stateMessage + pScreenParams->pStr[i];*/
            this_thread::sleep_for(std::chrono::milliseconds(100));
            state.stateMessage = pScreenParams->pStr[i];
            state.stateMessage += "\n";
            //cout << state.stateMessage;
            state.stateNr++;
        }
    }
    if (pScreenParams->screenID == 3)
    {
        pScreenParams->eventKey = 0x31;
    }
    return 0;
}

static void _ScreenClose()
{
    ScreenClose();
}

int main()
{
    HINSTANCE posTerminal = Connect(L"C:\\My Programming\\.Net\\KZ_Ingenico_EPI\\Supply folder\\Trposx.dll", "c:\\My Programming\\.Net\\KZ_Ingenico_EPI\\Supply folder\\setup.txt");
    int nr = 1000011112;
    state = State();
    if (posTerminal == 0)
    {
        std::cout << "Library not loaded";
    }
    else
    {
        while (true)
        {
            int a;
            std::cout << "To start purchase enter amount:";
            std::cin >> a;
            if (a == 0)
            {
                FreeLibrary(posTerminal);
                return 0;
            }
            else
            {

                if (posTerminal != NULL)
                {
                    int error = 2;
                    string respString;
                    std::thread thr(Purchase, posTerminal, "01", to_string(nr), a, ref(error), ref(respString));
                    thr.detach();
                    int temp = state.stateNr;
                    while (error == 2)
                    {
                        if (state.stateNr != temp)
                        {
                            std::cout << state.stateMessage;
                            temp = state.stateNr;
                        }
                    }
                    nr++;
                }
            }
        }
    }
}

static HINSTANCE Connect(LPCWSTR dllPath, string setupPath)
{
    HINSTANCE pos = LoadLibrary(L"Trposx.dll");
    if (pos != 0)
    {
        Init TRPOSX_Init = (Init)GetProcAddress(pos, R"(TRPOSX_Init)");
        int error = TRPOSX_Init(setupPath.c_str(), _ScreenShow, _ScreenClose);

        if (error != 0)
        {
            return nullptr;
        }
        else
        {
            return pos;
        }
    }
    else
    {
        int er = GetLastError();
        return nullptr;
    }
}

static void Purchase(HINSTANCE instance, string ECRNumber, string transactionNumber, int amount, int& error, string& outMessage)
{
    string inParamsStr;
    string _1 = "MessageID=PUR";
    string _2 = "\nECRnumber=";
    string _3 = ECRNumber;
    string _4 = "\nECRReceiptNumber=";
    string _5 = transactionNumber;
    string _6 = "\nTransactionAmount=";
    string _7 = to_string(amount * 100);
    string _8 = "\n";
    inParamsStr = _1 + _2 + _3 + _4 + _5 + _6 + _7 + _8;
    int lenOut, lenReceipt = 0;
    int* _lenOut = &lenOut;
    int* _lenReceipt = &lenReceipt;
    Proc TRPOSX_Proc = (Proc)GetProcAddress(instance, "TRPOSX_Proc");
    error = TRPOSX_Proc(inParamsStr.c_str(), _lenOut, _lenReceipt);
}

int Settlement(HINSTANCE instance, string ECRNumber, string transactionNumber)
{
    string inParamsStr;
    string _1 = "MessageID=STL";
    string _2 = "\nECRnumber=";
    string _3 = ECRNumber;
    string _4 = "\nECRReceiptNumber=";
    string _5 = transactionNumber;
    string _8 = "\n";
    inParamsStr = _1 + _2 + _3 + _4 + _5 + _8;
    int lenOut, lenReceipt = 0;
    int* _lenOut = &lenOut;
    int* _lenReceipt = &lenReceipt;
    Proc TRPOSX_Proc = (Proc)GetProcAddress(instance, "TRPOSX_Proc");
    return TRPOSX_Proc(inParamsStr.c_str(), _lenOut, _lenReceipt);
}

int Ping(HINSTANCE instance, string ECRNumber)
{
    string inParamsStr;
    string _1 = "MessageID=PNG";
    string _2 = "\nECRnumber=";
    string _3 = ECRNumber;
    string _8 = "\n";
    inParamsStr = _1 + _2 + _3 + _8;
    int lenOut, lenReceipt = 0;
    int* _lenOut = &lenOut;
    int* _lenReceipt = &lenReceipt;
    Proc TRPOSX_Proc = (Proc)GetProcAddress(instance, "TRPOSX_Proc");
    return TRPOSX_Proc(inParamsStr.c_str(), _lenOut, _lenReceipt);
}

int Stop()
{
    return 0;
}

// Pos.cpp : Implementation of CPos

#include "pch.h"
#include "Pos.h"
#include <vector>
// CPos

//
//
//STDMETHODIMP CPos::Add(DOUBLE a, DOUBLE b, DOUBLE* res)
//{
//    //double _res = a + b;
//    //*res = _res;
//    /*posTerminal = Connect(L"C:\\My Programming\\.Net\\KZ_Ingenico_EPI\\Supply folder\\Trposx.dll", "c:\\My Programming\\.Net\\KZ_Ingenico_EPI\\Supply folder\\setup.txt");
//    nr = 1000011112;
//    state = State();
//    double _result = (BYTE)posTerminal;
//    *res = _result;*/
//    return S_OK;
//}
//
//
//STDMETHODIMP CPos::Connect1(BYTE* result)
//{
//    return S_OK;
//}

static int _ScreenShow(ScreenParams* pScreenParams)
{
    for (int i = 0; i < 10; i++)
    {
        if (pScreenParams->pStr[i] != NULL)
        {

            this_thread::sleep_for(std::chrono::milliseconds(150));
            _LastStatMsgDescription = pScreenParams->pStr[i];
            _LastStatMsgDescription += "\n";
            _LastStatMsgCode++;
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
    //ScreenClose();
}

static vector<string> ParseString(const string &inString, char delimiter)
{
    vector <string> tokens;
    string tag = "";
    for (auto it = inString.cbegin(); it != inString.cend(); ++it) {
        if (*it == delimiter)
        {
            tokens.push_back(tag);
            tag = "";
        }
        else
        {
            tag += *it;
        }
    }
    return tokens;
}

static vector<string> ParseStringA(const string& inString, char delimiter)
{
    vector <string> tokens;
    string tag = "";
    for (auto it = inString.cbegin(); it != inString.cend(); ++it) {
        if (*it == delimiter)
        {
            tokens.push_back(tag);
            tag = "";
        }
        else
        {
            if ((*it == '\r')||(*it == '\0'))
            {
                tokens.push_back(tag);
                tag = "";
            }
            tag += *it;
        }
    }
    return tokens;
}


static void SetResponse(string str)
{
    vector<string> vt = ParseStringA(str, '=');
    if (vt[0].compare("ResponseCode") != 0)
    {
        response.RESPONSECODE = vt[1];
    }
    if (vt[0].compare("Approve") != 0)
    {
        response.APPROVE = vt[1];
    }
    if (vt[0].compare("AUTHNR") != 0)
    {
        response.AUTHNR = vt[1];
    }
    if (vt[0].compare("DATE") != 0)
    {
        response.DATE = vt[1];
    }
    if (vt[0].compare("EXP") != 0)
    {
        response.EXP = vt[1];
    }
    if (vt[0].compare("INVOICENR") != 0)
    {
        response.INVOICENR = vt[1];
    }
    if (vt[0].compare("ISSUER") != 0)
    {
        response.ISSUER = vt[1];
    }
    if (vt[0].compare("PAN") != 0)
    {
        response.PAN = vt[1];
    }
    if (vt[0].compare("RRN") != 0)
    {
        response.RRN = vt[1];
    }
    if (vt[0].compare("TIME") != 0)
    {
        response.TIME = vt[1];
    }
    if (vt[0].compare("VisualHostResponse") != 0)
    {
        response.VISUALHOSTRESPONCE = vt[1];
    }
}

HINSTANCE CPos::Connect(LPCWSTR dllPath, string setupPath)
{
    string temp = "trposx.dll";
    HINSTANCE pos = LoadLibraryEx(L"c:\\My Programming\\.Net\\KZ_Ingenico_EPI\\Supply folder\\Trposx.dll", NULL, LOAD_WITH_ALTERED_SEARCH_PATH);

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
        return (HINSTANCE)GetLastError();
}

void CPos::Purchase(HINSTANCE instance, string ECRNumber, string transactionNumber, int amount)
{
    string inParamsStr;
    char outParamsCh[1000];
    char receiptCh[2000];
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
    _LastError = 2;
    int internalError = TRPOSX_Proc(inParamsStr.c_str(), _lenOut, _lenReceipt);
    int tempLen = 0;
    string tempStrinArray[40];
    if (internalError == 0)
    {
        if (_lenOut > 0)
        {
            GetRsp TRPOSX_GetResp = (GetRsp)GetProcAddress(instance, "TRPOSX_GetRsp");
            internalError = TRPOSX_GetResp(outParamsCh, receiptCh);
            if (internalError == 0)
            {
                outStr = string(outParamsCh, lenOut);
                receipt = string(receiptCh, lenReceipt);
                vector<string> vt = ParseString(outStr, '\n');
                copy(vt.begin(), vt.end(), tempStrinArray);
                for (int j = 0; j < vt.size()-1; j++)
                {
                    SetResponse(vt[j]);
                }
                _LastError = 0;
            }
        }
    }
    else
    {
        _LastError = internalError;
    }

}


STDMETHODIMP CPos::Initialize(BSTR dllPath, BSTR setupPath, BYTE* result)
{
    posTerminal = Connect(dllPath, bstr_to_str(setupPath));
    BYTE _result = (BYTE)posTerminal;
    *result = _result;
    return S_OK;
}

string bstr_to_str(BSTR source) 
{
    return _com_util::ConvertBSTRToString(source);
}

BSTR str_to_bstr(string source)
{
    return _com_util::ConvertStringToBSTR(source.c_str());
}


STDMETHODIMP CPos::StartPurchase(DOUBLE amount, BSTR ECRNr, BSTR TRNr, byte *result)
{
    _LastStatMsgCode = 0;
    _LastError = 2;
    std::thread thr(Purchase, posTerminal, bstr_to_str(ECRNr), bstr_to_str(TRNr), (int)amount);
    thr.detach();
    return S_OK;
}

STDMETHODIMP CPos::get_lastError(LONG* pVal)
{
    *pVal = (LONG)_LastError;
    return S_OK;
}

STDMETHODIMP CPos::get_LastErrorDescription(BSTR* pVal)
{
    *pVal = str_to_bstr(_LastErrorDescription);
    return S_OK;
}

STDMETHODIMP CPos::get_LastStatMsgCode(LONG* pVal)
{
    *pVal = (long)_LastStatMsgCode;
    return S_OK;
}

STDMETHODIMP CPos::get_LastStatMsgDescription(BSTR* pVal)
{
    *pVal = str_to_bstr(_LastStatMsgDescription);
    return S_OK;
}

STDMETHODIMP CPos::get_ResponseCode(LONG* pVal)
{
    *pVal = (long)stoi(response.RESPONSECODE, 0, 16);
    return S_OK;
}

STDMETHODIMP CPos::get_LastErrorCode(LONG* pVal)
{
    // TODO: Add your implementation code here

    return S_OK;
}

STDMETHODIMP CPos::get_PAN(BSTR* pVal)
{
    *pVal = str_to_bstr(response.PAN);
    return S_OK;
}

STDMETHODIMP CPos::get_ExpDate(BSTR* pVal)
{
    *pVal = str_to_bstr(response.EXP);
    return S_OK;
}

STDMETHODIMP CPos::get_IssuerName(BSTR* pVal)
{
    *pVal = str_to_bstr(response.ISSUER);
    return S_OK;
}

STDMETHODIMP CPos::get_AuthCode(BSTR* pVal)
{
    *pVal = str_to_bstr(response.AUTHNR);
    return S_OK;
}

STDMETHODIMP CPos::get_RRN(BSTR* pVal)
{
    *pVal = str_to_bstr(response.RRN);
    return S_OK;
}

STDMETHODIMP CPos::get_Receipt(BSTR* pVal)
{
    *pVal = str_to_bstr(receipt);
    return S_OK;
}

STDMETHODIMP CPos::get_Amount(LONG* pVal)
{
    *pVal = 0;
    return S_OK;
}

STDMETHODIMP CPos::GetResponce()
{
    // TODO: Add your implementation code here

    return S_OK;
}

STDMETHODIMP CPos::get_outString(BSTR* pVal)
{
    *pVal = str_to_bstr(outStr);
    return S_OK;
}

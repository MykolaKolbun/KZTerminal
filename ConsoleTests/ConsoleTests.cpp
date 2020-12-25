// ConsoleTests.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include <iostream>
#include <string>
#include <vector>

using namespace std;

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

Response response;

static vector<string> ParseString(const string& inString, char delimiter)
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
            if ((*it == '\r') || (*it == '\0'))
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
    if (vt[0] == "ResponseCode")
    {
        response.RESPONSECODE = vt[1];
    }
    if (vt[0] == "Approve")
    {
        response.APPROVE = vt[1];
    }
    if (vt[0] == "AUTHNR")
    {
        response.AUTHNR = vt[1];
    }
    if (vt[0] == "DATE")
    {
        response.DATE = vt[1];
    }
    if (vt[0] == "EXP")
    {
        response.EXP = vt[1];
    }
    if (vt[0] == "INVOICENR")
    {
        response.INVOICENR = vt[1];
    }
    if (vt[0] == "ISSUER")
    {
        response.ISSUER = vt[1];
    }
    if (vt[0] == "PAN")
    {
        response.PAN = vt[1];
    }
    if (vt[0]=="RRN")
    {
        response.RRN = vt[1];
    }
    if (vt[0] == "TIME")
    {
        response.TIME = vt[1];
    }
    if (vt[0] == "VisualHostResponse")
    {
        response.VISUALHOSTRESPONCE = vt[1];
    }
}


int main()
{
    string str = "MessageID=PUR\r\nECRNumber=01\r\nResponseCode=B5\r\n";
    vector<string> vt = ParseString(str, '\n');
    
    for (int i = 0; i < vt.size(); i++)
    {
        SetResponse(vt[i]);
        //std::cout <<i<<" " << vt[i]<<endl;
    }
    cout << response.RESPONSECODE<< endl;


}



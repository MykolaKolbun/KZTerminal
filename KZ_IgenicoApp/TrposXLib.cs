using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TRPOSXLib;

namespace KZ_IgenicoApp
{
    


    public delegate int TRGUI_ScreenShowDelegate(IntPtr pScreenParams);
    public delegate void TRGUI_ScreenCloseDelegate();

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class ScreenParams
    {
        public Int32 len; // [in]
        public Int32 screenID; // [in]
        public Int32 maxInp; // [in]
        public Int32 minInp; // [in]
        public UInt32 format; // [in]
        public IntPtr pTitle; // [in]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public IntPtr[] pStr; // [in]
        public IntPtr pInitStr; // [in]
        public IntPtr pButton0; // [in]
        public IntPtr pButton1; // [in]
        public CurrencyParams CurParam; // [in]
        public Int32 eventKey; // [out]
        public IntPtr pBuf; // [in]
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CurrencyParams
    {
        public IntPtr CurAlpha; // [in] символьный код валюты.
        public byte nDecPoint; // [in] позиция десятичной точки при отображении валюты.
    };

    public struct InputParams
    {
        /// <summary>
        /// Идентификатор операции, lenght 3.
        /// </summary>
        public string MessageID;

        /// <summary>
        /// Номер ККМ.
        /// </summary>
        public string ECRnumber;

        /// <summary>
        /// Номер операции ККМ. Хранится в журнале POS-терминала для возможности поиска транзакции по данному ключу. Обязательное поле для финансовых операций и сверки, необязательное – для сервисной.
        /// </summary>
        public int ECRReceiptNumber;

        /// <summary>
        /// Итоговая сумма транзакции. Десятичная точка не передается, а только подразумевается.
        /// </summary>
        public int TransactionAmount;

        public byte SRVSubfunction;

        /// <summary>
        /// Байт 0, представлен первой парой шестнадцатиричных цифр, биты:
        /// 0 – chained transactions(сцепленные/последовательные транзакции). Используется в операции VER
        /// 1 – режим EMV Fallback
        /// 2 – игнорировать приложение лояльности
        /// 3 – последовательное чтение журнала
        /// 4 – чек не требуется
        /// 5 – режим SOF
        /// 6 – операция инициирована POS auditor
        /// 7 – должен быть установлен
        /// Байт 1. Вторая пара шестнадцатиричных цифр, биты:
        /// 0 – 1 – RFU
        /// 2 – не запрашивать карту для Void(игнорировать Void Enter Card из профиля терминала)
        /// 3 – 6 – RFU
        /// 7 – должен быть установлен
        /// Байт 2 зарезервирован.Третья пара шестнадцатиричных цифр должна быть установлена в «20»
        /// </summary>
        public byte[] Flags;
    }
    public static class TRposXNative
    {
        
        public delegate int TRGUI_ScreenShowDelegate_My(ref ScreenParams pScreenParams);
        public delegate int TRGUI_ScreenCloseDelegate_My();
        #region Export 

        [DllImport("user32.dll")]
        static public extern bool OemToCharA(char[] lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport("user32.dll")]
        static public extern bool OemToChar(IntPtr lpszSrc, [Out] StringBuilder lpszDst);


        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Init(string pathToConfig, [In, Out] TRGUI_ScreenShowDelegate_My TRGUI_ScreenShow, [In, Out] TRGUI_ScreenCloseDelegate_My TRGUI_ScreenClose);

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Proc(string inParams, out int lenOutParams, out int lenReceipt);

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int TRPOSX_GetRsp([In, Out]  IntPtr out_params, [In, Out]  IntPtr receipt);

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Close();
        #endregion
    }

    class TrposiXLib
    {
        #region Fields
        Response resp;
        #endregion


        //CustomerDialog cd;
        //ScreenParams screen;
        //string responseString = "";
        //string receiptString = "";
        public int TRGUI_ScreenShow(ref ScreenParams _screen)
        {
            return 0;
        }

        public int TRGUI_ScreenClose()
        {
            return 0;
        }

        int ECRReceipt = 1000000070;

        public int Init()
        {
            string configPath = @"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\setup.txt";
            return TRposXNative.TRPOSX_Init(configPath, TRGUI_ScreenShow, TRGUI_ScreenClose);
        }

        public int Close()
        {
            return TRposXNative.TRPOSX_Close();
        }

        public int Purchase(int amount)
        {
            string inParamsStr;
            InputParams inParams = new InputParams();
            inParams.ECRnumber = "01";
            inParams.MessageID = "PUR";
            inParams.TransactionAmount = amount;
            inParams.ECRReceiptNumber = ECRReceipt;
            //inParamsStr = "MessageID="+inParams.MessageID + '\n' + "ECRnumber="+inParams.ECRnumber + '\n' + "ECRReceiptNumber="+inParams.ECRReceiptNumber.ToString() + '\n' + "TransactionAmount="+inParams.TransactionAmount.ToString()+ '\n';
            inParamsStr = $"MessageID={inParams.MessageID}{Environment.NewLine}ECRnumber={inParams.ECRnumber}{Environment.NewLine}ECRReceiptNumber={inParams.ECRReceiptNumber.ToString()}{Environment.NewLine}TransactionAmount={inParams.TransactionAmount.ToString()}{Environment.NewLine}";
            int error = TRposXNative.TRPOSX_Proc(inParamsStr, out int outLen, out int lenReceipt);
            if(error==0)
            {
                IntPtr bufferResponse = Marshal.AllocHGlobal(outLen);
                IntPtr bufferReceipt = Marshal.AllocHGlobal(lenReceipt);
                error = TRposXNative.TRPOSX_GetRsp(bufferResponse, bufferReceipt);
                string outputString = Marshal.PtrToStringAnsi(bufferResponse, outLen);
                string receiptString = Marshal.PtrToStringAnsi(bufferReceipt, lenReceipt);
                if (error == 0)
                {
                    resp = new Response(outputString);
                    MessageBox.Show(resp.VisualHostResponse);
                }
            }
            ECRReceipt++;
            return error;
        }

        public int Settlement()
        {
            string inParamsStr;
            InputParams inParams = new InputParams();
            inParams.ECRnumber = "01";
            inParams.MessageID = "STL";
            inParams.SRVSubfunction = 2;
            inParamsStr = $"MessageID={inParams.MessageID}{Environment.NewLine}ECRnumber={inParams.ECRnumber}{Environment.NewLine}";
            int error = TRposXNative.TRPOSX_Proc(inParamsStr, out int outLen, out int lenReceipt);
            return error;
        }

        public int Service()
        {
            string inParamsStr;
            InputParams inParams = new InputParams();
            inParams.ECRnumber = "01";
            inParams.MessageID = "SRV";
            inParams.SRVSubfunction = 2;
            inParamsStr = $"MessageID={inParams.MessageID}{Environment.NewLine}ECRnumber={inParams.ECRnumber}{Environment.NewLine}SRVsubfunction={inParams.SRVSubfunction}{Environment.NewLine}";
            int error = TRposXNative.TRPOSX_Proc(inParamsStr, out int outLen, out int lenReceipt);
            return error;
        }

        public int GetResponse()
        {
            return 0;
        }

        private static void ResponseStringToRespStruct(string responseString)
        {

        }
    }
}

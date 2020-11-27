using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace KZ_Ingenico_EPI
{
    public delegate int ScreenShowDelegate(ScreenParams pScreenParams);
    //public delegate int ScreenShowDelegate(IntPtr pScreenParams);
    public delegate void ScreenCloseDelegate();

    [StructLayout(LayoutKind.Sequential)]
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

        public void Print()
        {
            LoggerPermanent log = new LoggerPermanent("ScreenShow", "01");
            log.Write($"New---------------");
            StringBuilder title = new StringBuilder();
            if (this.pTitle.ToInt32() != 0)
            {
                TRposXNative.OemToChar(this.pTitle, title);
            }
            List<string> str = new List<string>();
            if (this.pStr != null)
            {
                foreach (IntPtr s in this.pStr)
                {
                    if (s.ToInt32()!=0)
                    {
                        log.Write($"IntPtr: {s.ToInt32()}");
                        StringBuilder strOem = new StringBuilder();
                        TRposXNative.OemToChar(s, strOem);
                        str.Add(strOem.ToString());
                    }
                }
            }
            log.Write($"ScreenID: {this.screenID}");
            log.Write($"Format: {this.format}");
            if (!String.IsNullOrEmpty(title.ToString()))
            {
                log.Write($"Title:{title.ToString()}");
            }
            foreach (string s in str)
            {
                if (!String.IsNullOrEmpty(s))
                {
                    log.Write($"String:{s}");
                }
            }
            log.Close();
        }
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
        public string ECRReceiptNumber;

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
        #region Export 

        [DllImport("user32.dll")]
        static public extern bool OemToCharA(char[] lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport("user32.dll")]
        static public extern bool OemToChar(IntPtr lpszSrc, [Out] StringBuilder lpszDst);


        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Init(string pathToConfig, ScreenShowDelegate TRGUI_ScreenShow, ScreenCloseDelegate TRGUI_ScreenClose);

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Proc(string inParams, out int lenOutParams, out int lenReceipt);

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int TRPOSX_GetRsp([In, Out]  IntPtr out_params, [In, Out]  IntPtr receipt);

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Close();

        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\trgui.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenShow([In, Out] IntPtr pScreenParams);
        [DllImport(@"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\trgui.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int ScreenClose();
        #endregion
    }

    class TrposXLib
    {
        #region Fields
        public Response resp;
        private static bool screenShow=false;

        #endregion

        public int ScreenShow(ScreenParams _screen)
        {
            IntPtr sPar = Marshal.AllocHGlobal(92);
            _screen.Print();
            Marshal.StructureToPtr(_screen, sPar, false);
            return TRposXNative.ScreenShow(sPar);
        }

        public int ScreenShow(IntPtr _screen)
        {
            screenShow = false;
            ScreenParams screenParams = new ScreenParams();
            Marshal.PtrToStructure(_screen, screenParams);
            screenParams.Print();
            switch (screenParams.screenID)
            {
                case 0:
                    StringBuilder title = new StringBuilder();
                    if (screenParams.pTitle.ToInt32() != 0)
                    {
                        TRposXNative.OemToChar(screenParams.pTitle, title);
                    }
                    string message = "";
                    if (screenParams.pStr != null)
                    {
                        foreach (IntPtr s in screenParams.pStr)
                        {
                            if (s.ToInt32() != 0)
                            {
                                StringBuilder strOem = new StringBuilder();
                                TRposXNative.OemToChar(s, strOem);
                                message += strOem.ToString();
                                message += Environment.NewLine;
                            }
                        }
                    }
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
            while(screenShow)
            { }
            if(screenParams.screenID==0)
            {
                screenParams.eventKey = 0;
            }
            IntPtr sPar = Marshal.AllocHGlobal(92);
            Marshal.StructureToPtr(screenParams, _screen, false);
            
            return 0;
        }

        public void ScreenClose()
        {
            screenShow = true;
        }

        int ECRReceipt = 1000000070;

        public int Init()
        {
            //ScreenParams sp = new ScreenParams();
            ScreenShowDelegate ssd = new ScreenShowDelegate(ScreenShow);
            ScreenCloseDelegate scd = new ScreenCloseDelegate(ScreenClose);
            string configPath = @"c:\My Programming\.Net\KZ_Ingenico_EPI\Supply folder\setup.txt";
            return TRposXNative.TRPOSX_Init(configPath, ssd, scd);
        }

        public int Close()
        {
            return TRposXNative.TRPOSX_Close();
        }

        public int Purchase(int amount, string RefferenseID, string terminalID)
        {
            string inParamsStr;
            InputParams inParams = new InputParams();
            inParams.ECRnumber = terminalID;
            inParams.MessageID = "PUR";
            inParams.TransactionAmount = amount;
            inParams.ECRReceiptNumber = RefferenseID;
            //inParamsStr = "MessageID="+inParams.MessageID + '\n' + "ECRnumber="+inParams.ECRnumber + '\n' + "ECRReceiptNumber="+inParams.ECRReceiptNumber.ToString() + '\n' + "TransactionAmount="+inParams.TransactionAmount.ToString()+ '\n';
            inParamsStr = $"MessageID={inParams.MessageID}{Environment.NewLine}ECRnumber={inParams.ECRnumber}{Environment.NewLine}ECRReceiptNumber={inParams.ECRReceiptNumber.ToString()}{Environment.NewLine}TransactionAmount={inParams.TransactionAmount.ToString()}{Environment.NewLine}";
            int error = TRposXNative.TRPOSX_Proc(inParamsStr, out int outLen, out int lenReceipt);
            if (error == 0)
            {
                IntPtr bufferResponse = Marshal.AllocHGlobal(outLen);
                IntPtr bufferReceipt = Marshal.AllocHGlobal(lenReceipt);
                error = TRposXNative.TRPOSX_GetRsp(bufferResponse, bufferReceipt);
                string outputString = Marshal.PtrToStringAnsi(bufferResponse, outLen);
                string receiptString = Marshal.PtrToStringAnsi(bufferReceipt, lenReceipt);
                if (error == 0)
                {
                    if (receiptString.Length > 0)
                    {
                        string[] receipts = receiptString.Split('\u0001');
                        Array.Resize(ref receipts, receipts.Length - 1);
                        resp = new Response(outputString);
                        if (receipts.Length > 1)
                        {
                            resp.Receipt = receipts[0];
                            resp.Slip = receipts[1];
                        }
                        else
                        {
                            resp.Slip = receipts[0];
                        }
                    }
                    else
                    {
                        resp = new Response(outputString);
                    }
                }
            }
            ECRReceipt++;
            return error;
        }

        public int Cancel(string RefferenseID, string terminalID)
        {
            string inParamsStr;
            InputParams inParams = new InputParams();
            inParams.ECRnumber = terminalID;
            inParams.MessageID = "VOI";
            inParams.ECRReceiptNumber = RefferenseID;
            //inParamsStr = "MessageID="+inParams.MessageID + '\n' + "ECRnumber="+inParams.ECRnumber + '\n' + "ECRReceiptNumber="+inParams.ECRReceiptNumber.ToString() + '\n' + "TransactionAmount="+inParams.TransactionAmount.ToString()+ '\n';
            inParamsStr = $"MessageID={inParams.MessageID}{Environment.NewLine}ECRnumber={inParams.ECRnumber}{Environment.NewLine}ECRReceiptNumber={inParams.ECRReceiptNumber.ToString()}{Environment.NewLine}TransactionAmount={inParams.TransactionAmount.ToString()}{Environment.NewLine}";
            int error = TRposXNative.TRPOSX_Proc(inParamsStr, out int outLen, out int lenReceipt);
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
    }
}

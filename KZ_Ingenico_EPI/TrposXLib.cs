using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace KZ_Ingenico_EPI
{
    public delegate int ScreenShowDelegate(IntPtr pScreenParams);
    public delegate void ScreenCloseDelegate();

    [StructLayout(LayoutKind.Sequential)]
    public struct ScreenParams
    {
        public uint len; // [in]
        public uint screenID; // [in]
        public uint maxInp; // [in]
        public uint minInp; // [in]
        public uint format; // [in]
        public IntPtr pTitle; // [in]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public IntPtr[] pStr; // [in]
        public IntPtr pInitStr; // [in]
        public IntPtr pButton0; // [in]
        public IntPtr pButton1; // [in]
        public IntPtr CurParam; // [in]
        public uint eventKey; // [out]
        public IntPtr pBuf;
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct CurrencyParams
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public char[] CurAlpha; // [in] символьный код валюты.
        public byte nDecPoint; // [in] позиция десятичной точки при отображении валюты.
    };

    public static class TRposXNative
    {
        #region Import 

        [DllImport("user32.dll")]
        static public extern bool OemToCharA(char[] lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport("user32.dll")]
        static public extern bool OemToChar(IntPtr lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport(@"c:\Alternatiview\EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Init(string pathToConfig, ScreenShowDelegate TRGUI_ScreenShow, ScreenCloseDelegate TRGUI_ScreenClose);

        [DllImport(@"c:\Alternatiview\EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Proc(string inParams, out int lenOutParams, out int lenReceipt);

        [DllImport(@"c:\Alternatiview\EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        public static extern int TRPOSX_GetRsp([In, Out]  IntPtr out_params, [In, Out]  IntPtr receipt);

        [DllImport(@"c:\Alternatiview\EPI\Supply folder\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Close();
        #endregion
    }

    public class Results
        {
            public int error;
            public int messageID;
            public string message;
        }

    class TrposXLib
    {
        public delegate void PosScreenMessage(string message);
        public static event PosScreenMessage Notify;
        #region Fields
        private static bool screenShow=false;
        public static bool isCanceled;
        ScreenShowDelegate ssd = new ScreenShowDelegate(ScreenShow);
        ScreenCloseDelegate scd = new ScreenCloseDelegate(ScreenClose);
        public Response resp;
        #endregion
        static IntPtr tempPtr;
        static ScreenParams tempScreen;
        public static void OnNotify(string str)
        {
            if (Notify != null)
                Notify(str);
        }

        public static int ScreenShow(IntPtr _screen)
        {
            tempPtr = _screen;
            string notifyMessagee = "";
            screenShow = true;
            //ScreenParams screenParams = new ScreenParams();
            //screenParams = (ScreenParams)Marshal.PtrToStructure(_screen, typeof(ScreenParams));
            tempScreen = (ScreenParams)Marshal.PtrToStructure(_screen, typeof(ScreenParams));
            List<string> str = new List<string>();
            if (tempScreen.pStr != null)
            {
                foreach (IntPtr s in tempScreen.pStr)
                {
                    if (s.ToInt32() != 0)
                    {
                        StringBuilder strOem = new StringBuilder();
                        TRposXNative.OemToChar(s, strOem);
                        str.Add(strOem.ToString());
                    }
                }
            }
            foreach (string s in str)
            {
                if (!String.IsNullOrEmpty(s))
                {
                    notifyMessagee += s;
                    notifyMessagee += Environment.NewLine;
                }
            }
            OnNotify(notifyMessagee);
            if (tempScreen.screenID == 3)
            {
                tempScreen.eventKey = 0x31;
            }
            Marshal.StructureToPtr(tempScreen, tempPtr, true);
            while(screenShow)
            { }
            return 0;
        }

        public static void ScreenClose()
        {
            tempScreen.eventKey = 0x22;
            Marshal.StructureToPtr(tempScreen, tempPtr, true);
            //screenShow = false;
        }

        public int Init()
        {
            string configPath = @"c:\Alternatiview\EPI\Supply folder\setup.txt";
            return TRposXNative.TRPOSX_Init(configPath, ssd, scd);
        }

        public int Close()
        {
            int error = 0;
            error = TRposXNative.TRPOSX_Close();
            return error;
        }

        //public static void PrintStruct(object _sp)
        //{
        //    result.messageID++;
        //    ScreenParams sp = (ScreenParams)_sp;
        //    LoggerPermanent log = new LoggerPermanent("ScreenShow", "01");
        //    StringBuilder title = new StringBuilder();
        //    if (sp.pTitle.ToInt32() != 0)
        //    {
        //        TRposXNative.OemToChar(sp.pTitle, title);
        //    }
        //    List<string> str = new List<string>();
        //    if (sp.pStr != null)
        //    {
        //        foreach (IntPtr s in sp.pStr)
        //        {
        //            if (s.ToInt32() != 0)
        //            {
        //                StringBuilder strOem = new StringBuilder();
        //                TRposXNative.OemToChar(s, strOem);
        //                str.Add(strOem.ToString());
        //            }
        //        }
        //    }
        //    log.Write($"EventKey: {sp.eventKey}");
        //    log.Write($"ScreenID: {sp.screenID}");
        //    log.Write($"Format: {sp.format}");
        //    if (!String.IsNullOrEmpty(title.ToString()))
        //    {
        //        log.Write($"Title:{title.ToString()}");
        //    }
        //    foreach (string s in str)
        //    {
        //        if (!String.IsNullOrEmpty(s))
        //        {
        //            log.Write($"String:{s}");
        //            result.message += s;
        //            result.message += Environment.NewLine;
        //        }
        //    }
        //    log.Close();
        //}

        public int Purchase(int amount, string RefferenseID, string terminalID)
        {
                string inParamsStr;
                isCanceled = false;
                inParamsStr = $"MessageID=PUR{Environment.NewLine}ECRnumber={terminalID}{Environment.NewLine}ECRReceiptNumber={RefferenseID}{Environment.NewLine}TransactionAmount={amount.ToString()}{Environment.NewLine}";
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
            return error;
        }

        public int Cancel()
        {
            isCanceled = true;
            return 0;
        }

        public int Settlement()
        {
            string inParamsStr;
            inParamsStr = $"MessageID=STL{Environment.NewLine}ECRnumber=01{Environment.NewLine}";
            int error = TRposXNative.TRPOSX_Proc(inParamsStr, out int outLen, out int lenReceipt);
            return error;
        }
    }
}

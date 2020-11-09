using System;
using System.Runtime.InteropServices;

namespace KZ_IgenicoApp
{
    public delegate int TRGUI_ScreenShow(ScreenParams pScreenParams);
    public delegate int TRGUI_ScreenClose();

    public struct ScreenParams
    {
        long len; // [in]
        long screenID; // [in]
        long maxInp; // [in]
        long minInp; // [in]
        ulong format; // [in]
        string pTitle; // [in]
        string[] pStr; // [in]
        string pInitStr; // [in]
        string pButton0; // [in]
        string pButton1; // [in]
        CurrencyParams CurParam; // [in]
        long eventKey; // [out]
        char[] pBuf; // [in]
    }

    public struct CurrencyParams
    {
        char[] CurAlpha; // [in] символьный код валюты.[3]
        char nDecPoint; // [in] позиция десятичной точки при отображении валюты.
    }

    public struct Tag
    {
        string Key;
        string Value;
    }

    public struct InputParams
    {
        /// <summary>
        /// Идентификатор операции, lenght 3.
        /// </summary>
        string MessageID;

        /// <summary>
        /// Номер ККМ.
        /// </summary>
        int ECRnumber;

        /// <summary>
        /// Номер операции ККМ. Хранится в журнале POS-терминала для возможности поиска транзакции по данному ключу. Обязательное поле для финансовых операций и сверки, необязательное – для сервисной.
        /// </summary>
        int ECRReceiptNumber;

        /// <summary>
        /// Итоговая сумма транзакции. Десятичная точка не передается, а только подразумевается.
        /// </summary>
        int TransactionAmount;

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
        byte[] Flags;
    }

    public struct OutputParams
    {
        /// <summary>
        /// Идентификатор операции
        /// </summary>
        string MessageID;

        /// <summary>
        /// Признак одобренности транзакции. Если параметр равен “Y”, то транзакция одобрена. Если параметр равен “N”, то отклонена
        /// </summary>
        string Approve;

        /// <summary>
        /// Номер ККМ (эхо значения в запросе)
        /// </summary>
        string ECRNumber;

        /// <summary>
        /// Номер операции ККМ (эхо значения в запросе)
        /// </summary>
        string ECRReceiptNumber;

        /// <summary>
        /// Код Ответа
        /// </summary>
        string ResponseCode;

        /// <summary>
        /// Сумма транзакции
        /// </summary>
        string TransactionAmount;

        /// <summary>
        /// Номер карты (Primary Account Number)
        /// </summary>
        string PAN;

        /// <summary>
        /// Срок действия карты в формате MMYY
        /// </summary>
        string ExpDate;

        /// <summary>
        /// Номер чека транзакции. Назначается POS-терминалом для каждой транзакции. Последовательно увеличивающийся номер.
        /// </summary>
        string InvoiceNumber;

        /// <summary>
        /// Кода Авторизации (назначается хостом, для каждой одобреной транзакции).
        /// </summary>
        string AuthorizationID;

        /// <summary>
        /// Дата проведения транзакции в формате DDMM
        /// </summary>
        string Date;

        /// <summary>
        /// Время проведения транзакции в формате hhmm
        /// </summary>
        string Time;

        /// <summary>
        /// Имя эмитента карты. Назначается в конфигурационных параметрах POS-терминала
        /// </summary>
        string IssuerName;

        /// <summary>
        /// Идентификатор организации. Назначается в конфигурационных параметрах POS-терминала.
        /// </summary>
        string MerchantNo;

        /// <summary>
        /// Processing Code (Поле 3 в хостовом сообщении ISO)
        /// </summary>
        string ProcessingCode;

        /// <summary>
        /// Используется для идентификации способа ввода карты: ручной ввод, прокатывание карты через считыватель или чипкарта (Поле 22 в ISO)
        /// </summary>
        string POSEntryMode;

        /// <summary>
        /// Идентифицирует условия, при которых была проведена транзакция. Например, транзакция без участия карты или покупателя, транзакция с помощью ККМ и т.д. (Поле 25 в ISO)
        /// </summary>
        string POSConditionCode;

        /// <summary>
        /// “P” – PIN-код проверялся,“S” – Необходима проверка подписи,“C” – проверялся pin-код и требуется проверка подписи,“ “ – параметр отсутствует, проверка не требуется,
        /// /// </summary>
        string CardholderVerificationCharacter;

        /// <summary>
        /// Retrieval Reference Number (назначается хостом, для каждой одобреной транзакции)
        /// </summary>
        string RRN;

        /// <summary>
        /// Идентификатор приложения чиповой карты
        /// </summary>
        string ApplicationID;

        /// <summary>
        /// Transaction Certificate (для EMV-приложений)
        /// </summary>
        string TC;

        /// <summary>
        /// Terminal Verification Result (Тег 95 )
        /// </summary>
        string TVR;

        /// <summary>
        /// Идентификатор терминала (Поле 41 в ISO). Назначается в конфигурационных параметрах POS-терминала.
        /// </summary>
        string TerminalID;

        /// <summary>
        /// Номер текущего журнала терминала
        /// </summary>
        string BatchNo;

        /// <summary>
        /// Название приложения для чип-карты
        /// </summary>
        string ApplicationLabel;

        /// <summary>
        /// Текстовая интерпретация кода ответа хоста
        /// </summary>
        string VisualHostResponse;

        /// <summary>
        /// Код валюты согласно стандарту ISO 4217.
        /// </summary>
        string Currency;

        /// <summary>
        /// Шифрованные данные карты
        /// </summary>
        string CardDataEnc;

        /// <summary>
        /// Информация о финансовых эквайерах. Одному эквайеру соответствует ровно один параметр, состоящий из слова Acquirer и следующим за ним двузначным номером от 01 до 99. Значение каждого параметра представляет собой структуру вида NMITR, где:
        /// N – Имя эквайера(10 байт, значение поля “Acquirer Name”);
        /// M – идентификатор торговца(15 байт, значение поля “Card Acceptor Merchant”);
        /// I – идентификатор эквайера(1 байт);
        /// T – идентификатор терминала(8 байт, значение поля “Card Acceptor Terminal”);
        /// R – зарезервировано(2 байта).
        /// </summary>
        string Acquirer01;

        /// <summary>
        /// Байт 0, представлен первой парой шестнадцатиричных цифр, биты:
        /// 0 – chained transactions(сцепленные/последовательные транзакции). Используется для операции VER.
        /// 1 – режим EMV Fallback.Установить, если требуется обработать чипкарту по магнитной полосе.
        /// 2 – игнорировать приложение лояльности
        /// 3 – последовательное чтение журнала 1
        /// 4 – чек не требуется
        /// 5– признак SOF 2
        /// 6 – запрос от POS Auditor
        /// 7 – должен быть установлен
        /// Байт 1
        /// Биты:
        /// 0 – 1 – RFU
        /// 2 – не запрашивать карту для Void (игнорировать Void Enter Card из профиля терминала)
        /// 3 – 6 – RFU
        /// 7 – должен быть установлен
        /// Байт 2 зарезервирован.Должен быть установлен в 0x20
        /// </summary>
        string Flags;

        /// <summary>
        /// Дополнительные данные для систем лояльности.
        /// </summary>
        string ExtData;

        /// <summary>
        /// Результат диалога для операции DLG
        /// </summary>
        string DialogResult;
    }
    class TrposXLib
    {
        #region Export 
        [DllImport("Supply folder\\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Init(string pathToConfig, TRGUI_ScreenShow ScreenShow, TRGUI_ScreenClose ScreenClose);

        [DllImport("Supply folder\\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Proc(string inParams, out int lenOutParams, out int lenReceipt);

        [DllImport("Supply folder\\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_GetRsp(string outParams, out string receipt);

        [DllImport("Supply folder\\TrposX.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int TRPOSX_Close();
        #endregion
    }
}

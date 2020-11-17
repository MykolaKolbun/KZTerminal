using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZ_IgenicoApp
{
    class Response
    {
        /// <summary>
        /// Идентификатор операции
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// Признак одобренности транзакции. Если параметр равен “Y”, то транзакция одобрена. Если параметр равен “N”, то отклонена
        /// </summary>
        public string Approve { get; set; }

        /// <summary>
        /// Номер ККМ (эхо значения в запросе)
        /// </summary>
        public string ECRNumber { get; set; }

        /// <summary>
        /// Номер операции ККМ (эхо значения в запросе)
        /// </summary>
        public string ECRReceiptNumber { get; set; }

        /// <summary>
        /// Код Ответа
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Сумма транзакции
        /// </summary>
        public string TransactionAmount { get; set; }

        /// <summary>
        /// Номер карты (Primary Account Number)
        /// </summary>
        public string PAN { get; set; }

        /// <summary>
        /// Срок действия карты в формате MMYY
        /// </summary>
        public string ExpDate { get; set; }

        /// <summary>
        /// Номер чека транзакции. Назначается POS-терминалом для каждой транзакции. Последовательно увеличивающийся номер.
        /// </summary>
        public string InvoiceNumber { get; set; }

        /// <summary>
        /// Кода Авторизации (назначается хостом, для каждой одобреной транзакции).
        /// </summary>
        public string AuthorizationID { get; set; }

        /// <summary>
        /// Дата проведения транзакции в формате DDMM
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Время проведения транзакции в формате hhmm
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Имя эмитента карты. Назначается в конфигурационных параметрах POS-терминала
        /// </summary>
        public string IssuerName { get; set; }

        /// <summary>
        /// Идентификатор организации. Назначается в конфигурационных параметрах POS-терминала.
        /// </summary>
        public string MerchantNo { get; set; }

        /// <summary>
        /// Processing Code (Поле 3 в хостовом сообщении ISO)
        /// </summary>
        public string ProcessingCode { get; set; }

        /// <summary>
        /// Используется для идентификации способа ввода карты: ручной ввод, прокатывание карты через считыватель или чипкарта (Поле 22 в ISO)
        /// </summary>
        public string POSEntryMode { get; set; }

        /// <summary>
        /// Идентифицирует условия, при которых была проведена транзакция. Например, транзакция без участия карты или покупателя, транзакция с помощью ККМ и т.д. (Поле 25 в ISO)
        /// </summary>
        public string POSConditionCode { get; set; }


        /// <summary>
        /// “P” – PIN-код проверялся,“S” – Необходима проверка подписи,“C” – проверялся pin-код и требуется проверка подписи,“ “ – параметр отсутствует, проверка не требуется,
        /// /// </summary>
        public string CardholderVerificationCharacter { get; set; }

        /// <summary>
        /// Retrieval Reference Number (назначается хостом, для каждой одобреной транзакции)
        /// </summary>
        public string RRN { get; set; }

        /// <summary>
        /// Идентификатор приложения чиповой карты
        /// </summary>
        public string ApplicationID { get; set; }

        /// <summary>
        /// Transaction Certificate (для EMV-приложений)
        /// </summary>
        public string TC { get; set; }

        /// <summary>
        /// Terminal Verification Result (Тег 95 )
        /// </summary>
        public string TVR { get; set; }

        /// <summary>
        /// Идентификатор терминала (Поле 41 в ISO). Назначается в конфигурационных параметрах POS-терминала.
        /// </summary>
        public string TerminalID { get; set; }

        /// <summary>
        /// Номер текущего журнала терминала
        /// </summary>
        public string BatchNo { get; set; }

        /// <summary>
        /// Название приложения для чип-карты
        /// </summary>
        public string ApplicationLabel { get; set; }

        /// <summary>
        /// Текстовая интерпретация кода ответа хоста
        /// </summary>
        public string VisualHostResponse { get; set; }

        /// <summary>
        /// Код валюты согласно стандарту ISO 4217.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Шифрованные данные карты
        /// </summary>
        public string CardDataEnc { get; set; }

        /// <summary>
        /// Информация о финансовых эквайерах. Одному эквайеру соответствует ровно один параметр, состоящий из слова Acquirer и следующим за ним двузначным номером от 01 до 99. Значение каждого параметра представляет собой структуру вида NMITR, где:
        /// N – Имя эквайера(10 байт, значение поля “Acquirer Name”);
        /// M – идентификатор торговца(15 байт, значение поля “Card Acceptor Merchant”);
        /// I – идентификатор эквайера(1 байт);
        /// T – идентификатор терминала(8 байт, значение поля “Card Acceptor Terminal”);
        /// R – зарезервировано(2 байта).
        /// </summary>
        public string Acquirer01 { get; set; }

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
        public string Flags { get; set; }

        /// <summary>
        /// Дополнительные данные для систем лояльности.
        /// </summary>
        public string ExtData { get; set; }

        /// <summary>
        /// Результат диалога для операции DLG
        /// </summary>
        public string DialogResult { get; set; }

        public Response()
        {
            this.Acquirer01 = String.Empty;
            this.ApplicationID = String.Empty;
            this.ApplicationLabel = String.Empty;
            this.Approve = String.Empty;
            this.AuthorizationID = String.Empty;
            this.BatchNo = String.Empty;
            this.CardDataEnc = String.Empty;
            this.CardholderVerificationCharacter = String.Empty;
            this.Currency = String.Empty;
            this.Date = String.Empty;
            this.DialogResult = String.Empty;
            this.ECRNumber = String.Empty;
            this.ECRReceiptNumber = String.Empty;
            this.ExpDate = String.Empty;
            this.ExtData = String.Empty;
            this.Flags = String.Empty;
            this.InvoiceNumber = String.Empty;
            this.IssuerName = String.Empty;
            this.MerchantNo = String.Empty;
            this.MessageID = String.Empty;
            this.PAN = String.Empty;
            this.POSConditionCode = String.Empty;
            this.POSEntryMode = String.Empty;
            this.ProcessingCode = String.Empty;
            this.ResponseCode = String.Empty;
            this.RRN = String.Empty;
            this.TC = String.Empty;
            this.TerminalID = String.Empty;
            this.Time = String.Empty;
            this.TransactionAmount = String.Empty;
            this.TVR = String.Empty;
            this.VisualHostResponse = String.Empty;
        }

        void CompliteTags(string inStr)
        {
            string testString = "MessageID=PUR\r\nResponseCode=00\r\nECRnumber=01\r\nECRReceiptNumber=1000000073\r\nTransactionAmount=000000001000\r\nTransactionAmount2=000000000000\r\nFlags=80C080\r\nPAN=5***********3529\r\nInvoiceNumber=990011\r\nAuthorizationID=XXXXXX\r\nDate=1711\r\nTime=2050\r\nIssuerName=ECMC    \r\nMerchantNo=398000002292675\r\nProcessingCode=000000\r\nPOSEntryMode=072\r\nPOSConditionCode=00\r\nCardholderVerificationCharacter= \r\nRRN=000000000000\r\nApplicationID=A0000000041010\r\nTC=2A1D584A5F4A658C\r\nTVR=0000008000\r\nTerminalID=02292675\r\nBatchNo=000003\r\nApplicationLabel=Debit MasterCard\r\nVisualHostResponse=ОДОБРЕНО XXXXXX\r\nApprove=Y\r\nCurrency=398\r\n\0";
            string[] line = testString.Split('\n');

            foreach (string mess in line)
            {
                string res = mess.Trim();
                string[] tags = res.Split('=');
                
            }

        }

        //private string[] CleanArray(string [] inArray)
        //{
        //    string[] outArray;
        //    for (int i=0; i< inArray.Length-1)
        //    return outArray;
        //}
    }
}

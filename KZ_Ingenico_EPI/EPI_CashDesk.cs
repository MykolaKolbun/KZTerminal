using SkiData.ElectronicPayment;
using SkiData.Parking.ElectronicPayment.Extensions;
using System;
using System.Globalization;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.IO;

namespace KZ_Ingenico_EPI
{
    public class EPI_CashDesk : ITerminal2
    {
        #region Fields
        private Settings _settings = new Settings();
        private TerminalConfiguration _termConfig;
        public static System.Timers.Timer timeCheckTimer = new System.Timers.Timer(60000);
        DeviceType deviceType;
        string deviceID;
        public static string machineID;
        bool settelmentDone = false;
        public TransactionResult lastTransaction;
        public TransactionData transactionDataContainer;
        private const byte merchID = 1;
        private string _userId = String.Empty;
        private string _userName = String.Empty;
        private string _shiftId = String.Empty;
        private bool isTerminalReady = false;
        private bool _activated = false;
        private bool _inTransaction = false;
        bool transactionCanceled = false;
        string terminalID = String.Empty;
        TimeSpan start = TimeSpan.Parse("23:57");
        TimeSpan end = TimeSpan.Parse("23:59");
        TimeSpan startShift = TimeSpan.Parse("00:01");
        TimeSpan endShift = TimeSpan.Parse("00:03");
        TrposXLib reader;
        #endregion

        #region Constructor
        public EPI_CashDesk()
        {
            this._settings.AllowsCancel = false;
            this._settings.AllowsCredit = true;
            this._settings.AllowsRepeatReceipt = false;
            this._settings.AllowsValidateCard = true;
            this._settings.AllowsVoid = true;
            this._settings.CanSetCardData = true;
            this._settings.HasCardReader = true;
            this._settings.IsContactless = false;
            this._settings.MayPrintReceiptOnRejection = false;
            this._settings.NeedsSkidataChipReader = false;
            this._settings.RequireReceiptPrinter = false;
            this._settings.PaymentCardMayDifferFromAccessCard = true;
        }
        #endregion

        #region IDisposable members

        private bool disposed = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //reader.Close();
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">if set to <see langword="true"/> the managed resources will be disposed.</param>
        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //if (_controlDialog != null)
                    //    _controlDialog.CloseForm();
                }
            }
            this.disposed = true;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Terminal"/> is reclaimed by garbage collection.
        /// </summary>
        ~EPI_CashDesk()
        {
            reader.Close();
            this.Dispose(false);
        }

        #endregion

        #region ITerminal2 members

        public string Name => "IngenicoKZ.Terminal";
        public string ShortName => "KZ_Terminal";

        Settings ITerminal.Settings
        {
            get { return this.TerminalSettings; }
        }

        private Settings TerminalSettings
        {
            get { return this._settings; }
        }

        public bool BeginInstall(TerminalConfiguration configuration)
        {
            _termConfig = configuration;
            bool done = false;
            reader = new TrposXLib();
            this.deviceType = configuration.DeviceType;
            this.deviceID = configuration.DeviceId;
            terminalID = configuration.CommunicationChannel;
            CreateFolders();
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("Begin Install");
            int error = reader.Init();
            log.Write($"Begin Install: Init result: {error}");
            if (error == 0)
            {
                done = true;
            }
            return done;
        }

        public void EndInstall()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            isTerminalReady = true;
            this.SetTimer();
            log.Write("EndInstall()");
        }

        public void SetDisplayLanguage(CultureInfo cultureInfo)
        { }

        public void SetParameter(Parameter parameter)
        { }

        public void AllowCards(CardIssuerCollection issuers)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            foreach (CardIssuer cardIssuer in issuers)
            {
                this.OnTrace(TraceLevel.Error,
                    "AVTerminal.AllowCards - CardIssuer: {0}", cardIssuer.Abbreviation);
                log.Write($"AllowCards() cardIssuer: {cardIssuer.Abbreviation}");
            }
        }

        public ValidationResult ValidateCard()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("ValidateCard()");
            return new ValidationResult();
        }

        public ValidationResult ValidateCard(Card card)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"ValidateCard(card) {card.Number}");
            return new ValidationResult();
        }

        public TransactionResult Debit(TransactionData debitData)
        {

            _inTransaction = true;
            transactionCanceled = false;
            transactionDataContainer = debitData;
            bool transactionResultDone = false;
            lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"Debit(Amount: {debitData.Amount}, RefID: {debitData.ReferenceId})");
            SQLConnect sql = new SQLConnect();
            Card card = Card.NoCard;
            if (isTerminalReady)
            {
                if (sql.IsSQLOnline())
                {
                    try
                    {
                        TrposXLib.Notify += TrposXLib_Notify;
                        int error = reader.Purchase((int)(debitData.Amount*100), debitData.ReferenceId, terminalID);
                        if (error == 0)
                        {
                            if (reader.resp.ResponseCode == 0 && reader.resp.ResponseCode != null)
                            {
                                OnAction(new ActionEventArgs(reader.resp.VisualHostResponse, ActionType.DisplayCustomerMessage));
                                log.Write($"Authorization Process: {reader.resp.VisualHostResponse}");
                                transactionResultDone = true;

                            }
                            else
                            {
                                OnAction(new ActionEventArgs(reader.resp.VisualHostResponse, ActionType.DisplayCustomerMessage));
                                log.Write($"Authorization Process: {reader.resp.VisualHostResponse}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        log.Write("Debit Exception: " + e.Message);
                        _inTransaction = false;
                        OnTrace(TraceLevel.Error, "EPI Exception:" + e.Message);
                        lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
                    }
                    finally
                    {
                        log.Write("Finaly");
                        log.Write($"Transaction Done: {transactionResultDone}");
                        TrposXLib.Notify -= TrposXLib_Notify;
                        if (transactionResultDone)
                        {
                            if (debitData is ParkingTransactionData parkingTransactionData)
                            {
                                card = new CreditCard(reader.resp.PAN, reader.resp.ExpDate, new CardIssuer(reader.resp.IssuerName));
                                log.Write($"Finaly RefID:{parkingTransactionData.ReferenceId}, Ticket: {parkingTransactionData.TicketId}, Amount: {(int)parkingTransactionData.Amount}, CardNR(PAN): {card.Number} \nReceipt: \n{reader.resp.Slip}");
                                sql.AddLinePurch(this.deviceID, parkingTransactionData.ReferenceId, parkingTransactionData.TicketId, reader.resp.Slip, (int)parkingTransactionData.Amount, card.Number);
                                TransactionDoneResult doneResult = new TransactionDoneResult(TransactionType.Debit, DateTime.Now);
                                doneResult.ReceiptPrintoutMandatory = false;
                                doneResult.Receipts = new Receipts(new Receipt(reader.resp.Receipt), new Receipt(reader.resp.Slip));
                                doneResult.Amount = (int)parkingTransactionData.Amount;
                                doneResult.Card = card;
                                doneResult.AuthorizationNumber = reader.resp.AuthorizationID;
                                doneResult.TransactionNumber = reader.resp.RRN;
                                doneResult.CustomerDisplayText = reader.resp.VisualHostResponse;
                                lastTransaction = doneResult;
                                OnAction(new ActionEventArgs(lastTransaction.CustomerDisplayText, ActionType.DisplayCustomerMessage));
                            }
                            else
                            {
                                card = new CreditCard(reader.resp.PAN, reader.resp.ExpDate, new CardIssuer(reader.resp.IssuerName));
                                log.Write($"Finaly RefID:{debitData.ReferenceId}, Amount: {(int)debitData.Amount}, CardNR(PAN): {card.Number} \n\tReceipt: \n\t{reader.resp.Slip}");
                                sql.AddLinePurch(this.deviceID, debitData.ReferenceId, "", reader.resp.Slip, (int)debitData.Amount, card.Number);
                                TransactionDoneResult doneResult = new TransactionDoneResult(TransactionType.Debit, DateTime.Now);
                                doneResult.ReceiptPrintoutMandatory = false;
                                doneResult.Receipts = new Receipts(new Receipt(reader.resp.Receipt), new Receipt(reader.resp.Slip));
                                doneResult.Amount = (int)debitData.Amount;
                                doneResult.Card = card;
                                doneResult.AuthorizationNumber = reader.resp.AuthorizationID;
                                doneResult.TransactionNumber = reader.resp.RRN;
                                doneResult.CustomerDisplayText = reader.resp.VisualHostResponse;
                                lastTransaction = doneResult;
                                OnAction(new ActionEventArgs(lastTransaction.CustomerDisplayText, ActionType.DisplayCustomerMessage));
                            }
                            CountTransaction counter = new CountTransaction(this.deviceID);
                            int tr = counter.Get();
                            if (tr < 5)
                            {
                                ManualSettelment();
                            }
                            else
                            {
                                counter.Send(tr--);
                            }
                        }
                        else
                        {
                            log.Write($"Finaly RefID:{debitData.ReferenceId}, \nReceipt: \n{reader.resp.Receipt}, \nAmount: {(int)debitData.Amount} ");
                            lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
                            lastTransaction.CustomerDisplayText = String.IsNullOrEmpty(reader.resp.VisualHostResponse) ? reader.resp.VisualHostResponse : "Ошибка!";
                            lastTransaction.Receipts = new Receipts(new Receipt(reader.resp.Receipt));
                            OnAction(new ActionEventArgs(lastTransaction.CustomerDisplayText, ActionType.DisplayCustomerMessage));
                        }
                    }
                }
                else
                {
                    log.Write($"No connection to SQL. RefID:{debitData.ReferenceId}");
                    lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
                    OnTrace(TraceLevel.Error, $"No connection to SQL. RefID:{debitData.ReferenceId}");
                }
            }
            else
            {
                log.Write($"Terminal not ready. RefID:{debitData.ReferenceId}");
                lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
                OnTrace(TraceLevel.Error, $"Terminal not ready. RefID:{debitData.ReferenceId}");
            }
            
            return lastTransaction;
        }

        private void TrposXLib_Notify(string message)
        {
            OnAction(new ActionEventArgs(message, ActionType.DisplayCustomerMessage));
        }

        public TransactionResult Debit(TransactionData debitData, Card card)
        {
            return Debit(debitData, Card.NoCard);
        }

        public TransactionResult Credit(TransactionData creditData)
        {
            return Debit(creditData);
        }

        public TransactionResult Credit(TransactionData creditData, Card card)
        {
            return Debit(creditData);
        }

        public TransactionResult Void(TransactionDoneResult debitResultData)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("Void");
            try
            {
                _inTransaction = true;
                this.OnTrace(TraceLevel.Verbose,
                    "TestTerminal.Void - should no longer be called");
            }
            finally
            {
                _inTransaction = false;
            }
            return new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
        }

        public void Cancel()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            reader.Cancel();
            log.Write("Cancel()");
        }

        public Receipts RepeatReceipt()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("RepeatReceipt");
            return new Receipts(new Receipt(reader.resp.Receipt, false), new Receipt(reader.resp.Slip, false));
        }

        public CommandDefinitionCollection GetCommands()
        {
            CommandDefinitionCollection commandDefinitionCollection = new CommandDefinitionCollection();
            CommandDefinition cardBtn = new CommandDefinition(1001, "Карта");
            commandDefinitionCollection.Add(cardBtn);
            return commandDefinitionCollection;
        }

        public void ExecuteCommand(int commandId)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"Execute Command({commandId.ToString()})");
            if (_activated == true)
            {
                switch (commandId)
                {
                    case 1001:
                        OnAction(new ActionEventArgs("Нет реализации", ActionType.DisplayOperatorMessage)); ;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                OnAction(new ActionEventArgs("Отмените операцию наличными! \n   (Нажмите отмена)", ActionType.DisplayCustomerMessage));
            }
        }

        public void ExecuteCommand(int commandId, object parameterValue)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"ExecuteCommand() command {commandId.ToString()}, parameterValue: {parameterValue.ToString()}");
        }

        public TransactionResult GetLastTransaction()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("GetLastTransaction()");
            if (lastTransaction == null)
                lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
            return lastTransaction;
        }

        public bool IsTerminalReady()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"IsTerminalReady({isTerminalReady.ToString()})");
            return isTerminalReady;
        }

        public bool SupportsCreditCards()
        {
            return false;
        }

        public bool SupportsDebitCards()
        {
            return true;
        }

        public bool SupportsElectronicPurseCards()
        {
            return false;
        }

        public bool SupportsCustomerCards()
        {
            return false;
        }

        public Card GetManualCard(Card card)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"GetManualCard() Card: {card.Number}");
            return GetManualCard(Card.NoCard);
        }

        public Card GetManualCard(Card card, string paymentType)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"GetManualCard() Card: {card.Number},PaymetType: {paymentType}");
            return GetManualCard(Card.NoCard);
        }

        public void Notify(int notificationId)
        { }

        public Card OpenInputDialog(IntPtr windowHandle, TransactionType transactionType, Card card)
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write($"OpenInputDialog()");
            return card;
        }
        #endregion

        #region Events

        public event ActionEventHandler Action;
        public event ConfirmationEventHandler Confirmation;
        public event ChoiceEventHandler Choice;
        public event DeliveryCheckEventHandler DeliveryCheck;
        public event ErrorOccurredEventHandler ErrorOccurred;
        public event ErrorClearedEventHandler ErrorCleared;
        public event IrregularityDetectedEventHandler IrregularityDetected;
        public event EventHandler TerminalReadyChanged;
        public event JournalizeEventHandler Journalize;
        public event TraceEventHandler Trace;

        private void OnAction(ActionEventArgs args)
        {
            if (Action != null)
                Action(this, args);
        }

        private void OnChoice(ChoiceEventArgs args)
        {
            if (Choice != null)
                Choice(this, args);
        }

        private void OnConfirmation(ConfirmationEventArgs args)
        {
            if (Confirmation != null)
                Confirmation(this, args);
        }

        private void OnDeliveryCheck(DeliveryCheckEventArgs args)
        {
            if (DeliveryCheck != null)
                DeliveryCheck(this, args);
        }

        private void OnErrorOccurred(ErrorOccurredEventArgs args)
        {
            if (ErrorOccurred != null)
                ErrorOccurred(this, args);
        }

        private void OnErrorCleared(ErrorClearedEventArgs args)
        {
            if (ErrorCleared != null)
                ErrorCleared(this, args);
        }

        private void OnIrregularityDetected(IrregularityDetectedEventArgs args)
        {
            if (IrregularityDetected != null)
                IrregularityDetected(this, args);
        }

        private void OnTerminalReadyChanged()
        {
            if (TerminalReadyChanged != null)
                TerminalReadyChanged(this, new EventArgs());
        }

        private void OnJournalize(JournalizeEventArgs args)
        {
            if (Journalize != null)
                Journalize(this, args);
        }

        private void OnTrace(TraceLevel level, string format, params object[] args)
        {
            if (Trace != null)
                Trace(this, new TraceEventArgs(String.Format(CultureInfo.InvariantCulture, format, args), level));
        }
        #endregion

        #region Timer
        private void SetTimer()
        {
            timeCheckTimer.Elapsed += OnTimedEvent;
            timeCheckTimer.AutoReset = true;
            timeCheckTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            TimeSpan now = DateTime.Now.TimeOfDay;
            if (!_inTransaction)
            {
                CountTransaction counter = new CountTransaction(deviceID);
                int transactionCountDown = counter.Get();
                if (transactionCountDown <= 3)
                {
                    this.ManualSettelment();
                }
                if (start <= end)
                {
                    if ((now >= start && now <= end) && (!settelmentDone))
                    {
                        this.ManualSettelment();
                        settelmentDone = true;
                    }
                }

                if (startShift <= endShift)
                {
                    if (now >= startShift && now <= endShift)
                    {
                        settelmentDone = false;
                    }
                }
            }
        }
        #endregion

        #region Custom methods

        void ManualSettelment()
        {
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("ManualSettelment()");
            _inTransaction = true;
            try
            {
                SQLConnect sql = new SQLConnect();
                int error = reader.Settlement();

                if (error == 1)
                {
                    log.Write("Settelment. No connection to terminal");
                }
                if (error == 0)
                {
                    CountTransaction counter = new CountTransaction(deviceID);
                    counter.Send(99);
                    log.Write($"SettlementReceipt: {reader.resp.Receipt}");
                    log.Write($"SettlementResult. Amount {reader.resp.Receipt}, Count {reader.resp.Receipt}");
                    sql.AddLineSettlement(this.deviceID, "settlement " + DateTime.Today.ToString("dd-MM-yyyy"), reader.resp.Receipt);
                }
                timeCheckTimer = new System.Timers.Timer(60000);
                _inTransaction = false;
            }
            catch (Exception e)
            {
                _inTransaction = false;
                log.Write($"ManualSettelmentException: {e.Message}");
                OnTrace(TraceLevel.Info, $"Ingenico: Exception: {e.Message}");
                timeCheckTimer = new System.Timers.Timer(60000);
            }
        }

        bool CreateFolders()
        {
            if (!Directory.Exists(StringValue.WorkingDirectory))
            {
                Directory.CreateDirectory(StringValue.WorkingDirectory);
            }
            if (!Directory.Exists($"{StringValue.WorkingDirectory}Log"))
            {
                Directory.CreateDirectory($"{StringValue.WorkingDirectory}Log");
            }
            if (!Directory.Exists($"{StringValue.WorkingDirectory}EPI"))
            {
                Directory.CreateDirectory($"{StringValue.WorkingDirectory}EPI");
            }
            return true;
        }
        #endregion
    }
}       

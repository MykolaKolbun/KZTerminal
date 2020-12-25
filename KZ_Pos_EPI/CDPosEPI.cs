using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiData.ElectronicPayment;
using SkiData.Parking.ElectronicPayment.Extensions;
using KZIngenicoXLib;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace KZ_Pos_EPI
{
    public class CDPosEPI : ITerminal2
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
        IPos reader;
        #endregion

        #region Constructor

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
        ~CDPosEPI()
        {
            this.Dispose(false);
        }

        #endregion

        #region ITerminal members

        public string Name => "IngenicoKZ.Terminal";
        
        Settings ITerminal.Settings 
        {
            get { return this._settings; }
        }
        
        public string ShortName => "KZ_Terminal";

        public bool BeginInstall(TerminalConfiguration configuration)
        {
            _termConfig = configuration;
            bool done = false;
            reader = new Pos();
            this.deviceType = configuration.DeviceType;
            this.deviceID = configuration.DeviceId;
            terminalID = configuration.CommunicationChannel;
            CreateFolders();
            Logger log = new Logger(this.ShortName, this.deviceID);
            log.Write("Begin Install");
            int error = reader.Initialize($@"{StringValue.WorkingDirectory}\EPI\Supply folder\trgui.dll", $@"{StringValue.WorkingDirectory}\EPI\Supply folder\setup.txt");
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
            log.Write("EndInstall()");
        }

        public void Notify(int notificationId)
        {
            
        }

        public Card OpenInputDialog(IntPtr windowHandle, TransactionType transactionType, Card card)
        {
            return card;
        }

        public Receipts RepeatReceipt()
        {
            return new Receipts(new Receipt(reader.Receipt));
        }

        public void SetDisplayLanguage(CultureInfo cultureInfo)
        {
        }

        public void SetParameter(Parameter parameter)
        {
        }

        public bool SupportsCreditCards()
        {
            return false;
        }

        public bool SupportsCustomerCards()
        {
            return false;
        }

        public bool SupportsDebitCards()
        {
            return false;
        }

        public bool SupportsElectronicPurseCards()
        {
            return false;
        }

        public void AllowCards(CardIssuerCollection issuers)
        {
            
        }

        public bool IsTerminalReady()
        {
            return isTerminalReady;
        }

        public void Cancel()
        {
            
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
                        //TODO Change referenceID to string ECRNumber
                        int error = reader.StartPurchase((int)(debitData.Amount * 100), terminalID, debitData.ReferenceId);
                        int temp = 0;
                        while (reader.lastError == 2)
                        {
                            if (temp != reader.LastStatMsgCode)
                            {
                                StringBuilder outStr = new StringBuilder();
                                OemToCharA((reader.LastStatMsgDescription + '\0').ToArray(), outStr);
                                OnAction(new ActionEventArgs(outStr.ToString(), ActionType.DisplayCustomerMessage));
                                temp = reader.LastStatMsgCode;
                            }
                        }
                        if (reader.lastError == 0)
                        {
                            if (reader.ResponseCode == 0)
                            {
                                transactionResultDone = true;
                            }
                            else
                            {
                                OnAction(new ActionEventArgs(reader.LastErrorDescription, ActionType.DisplayCustomerMessage));
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
                        if (transactionResultDone)
                        {
                            if (debitData is ParkingTransactionData parkingTransactionData)
                            {
                                card = new CreditCard(reader.PAN, reader.ExpDate, new CardIssuer(reader.IssuerName));
                                log.Write($"Finaly RefID:{parkingTransactionData.ReferenceId}, Ticket: {parkingTransactionData.TicketId}, Amount: {(int)parkingTransactionData.Amount}, CardNR(PAN): {card.Number} \nReceipt: \n{reader.Receipt}");
                                sql.AddLinePurch(this.deviceID, parkingTransactionData.ReferenceId, parkingTransactionData.TicketId, reader.Receipt, (int)parkingTransactionData.Amount, card.Number);
                                TransactionDoneResult doneResult = new TransactionDoneResult(TransactionType.Debit, DateTime.Now);
                                doneResult.ReceiptPrintoutMandatory = false;
                                doneResult.Receipts = new Receipts(new Receipt(reader.Receipt), new Receipt(reader.Receipt));
                                doneResult.Amount = (int)parkingTransactionData.Amount;
                                doneResult.Card = card;
                                doneResult.AuthorizationNumber = reader.AuthCode;
                                doneResult.TransactionNumber = reader.RRN;
                                doneResult.CustomerDisplayText = reader.LastErrorDescription;
                                lastTransaction = doneResult;
                                OnAction(new ActionEventArgs(lastTransaction.CustomerDisplayText, ActionType.DisplayCustomerMessage));
                            }
                            else
                            {
                                card = new CreditCard(reader.PAN, reader.ExpDate, new CardIssuer(reader.IssuerName));
                                log.Write($"Finaly RefID:{debitData.ReferenceId}, Amount: {(int)debitData.Amount}, CardNR(PAN): {card.Number} \n\tReceipt: \n\t{reader.Receipt}");
                                sql.AddLinePurch(this.deviceID, debitData.ReferenceId, "", reader.Receipt, (int)debitData.Amount, card.Number);
                                TransactionDoneResult doneResult = new TransactionDoneResult(TransactionType.Debit, DateTime.Now);
                                doneResult.ReceiptPrintoutMandatory = false;
                                doneResult.Receipts = new Receipts(new Receipt(reader.Receipt), new Receipt(reader.Receipt));
                                doneResult.Amount = (int)debitData.Amount;
                                doneResult.Card = card;
                                doneResult.AuthorizationNumber = reader.AuthCode;
                                doneResult.TransactionNumber = reader.RRN;
                                doneResult.CustomerDisplayText = reader.LastErrorDescription;
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
                            log.Write($"Finaly RefID:{debitData.ReferenceId}, \nReceipt: \n{reader.Receipt}, \nAmount: {(int)debitData.Amount} ");
                            lastTransaction = new TransactionFailedResult(TransactionType.Debit, DateTime.Now);
                            lastTransaction.CustomerDisplayText = String.IsNullOrEmpty(reader.LastErrorDescription) ? reader.LastErrorDescription : "Ошибка!";
                            lastTransaction.Receipts = new Receipts(new Receipt(reader.Receipt));
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

        #region Custom methods
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

        [DllImport("user32.dll")]
        static public extern bool OemToCharA(char[] lpszSrc, [Out] StringBuilder lpszDst);

        [DllImport("user32.dll")]
        static public extern bool OemToChar(IntPtr lpszSrc, [Out] StringBuilder lpszDst);

        void ManualSettelment()
        {

        }

        #endregion

        public TransactionResult Credit(TransactionData creditData)
        {
            throw new NotImplementedException();
        }

        public TransactionResult Credit(TransactionData creditData, Card card)
        {
            throw new NotImplementedException();
        }

        public TransactionResult Debit(TransactionData debitData, Card card)
        {
            throw new NotImplementedException();
        }

        public void ExecuteCommand(int commandId)
        {
            throw new NotImplementedException();
        }

        public void ExecuteCommand(int commandId, object parameterValue)
        {
            throw new NotImplementedException();
        }

        public CommandDefinitionCollection GetCommands()
        {
            throw new NotImplementedException();
        }

        public TransactionResult GetLastTransaction()
        {
            throw new NotImplementedException();
        }

        public Card GetManualCard(Card card)
        {
            throw new NotImplementedException();
        }

        public Card GetManualCard(Card card, string paymentType)
        {
            throw new NotImplementedException();
        }
        

        public ValidationResult ValidateCard()
        {
            throw new NotImplementedException();
        }

        public ValidationResult ValidateCard(Card card)
        {
            throw new NotImplementedException();
        }

        public TransactionResult Void(TransactionDoneResult debitResultData)
        {
            throw new NotImplementedException();
        }
    }
}

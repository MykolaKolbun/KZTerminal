using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace KZ_Ingenico_TransportLibApp
{
    class TrposX_TLV
    {
        Ethernet pos;
        static UInt16 sync = 0;
        public static Timer receivingTimer;
        public static byte ANS { get; set; }
        private bool received { get; set; }
        public static UInt32 lastError { get; set; }
        public static int lastStatus { get; set; }
        private bool timeElapsed { get; set; }

        IDictionary<byte, List<byte>> PurchResp;

        byte[] dataReceived;

        /// <summary>
        /// Инициализация соединения
        /// </summary>
        /// <param name="ip">IP адрес фискального принтера</param>
        /// <param name="port">Порт фискального принтера для отправки сообщений</param>
        /// <returns></returns>
        public uint Connect(string ip, int port)
        {
            return (uint)pos.Connect(ip, port);
        }

        /// <summary>
        /// Инициализация соединения
        /// </summary>
        /// <param name="connectionStringt"> строка подключения в формате IpAddress:port</param>
        /// <returns></returns>
        public uint Connect(string connectionStringt)
        {
            string[] str = connectionStringt.Split(':');
            string ip = str[0];
            int.TryParse(str[1], out int port);
            pos = new Ethernet();
            return (uint)pos.Connect(ip, port);
        }
        
        public bool FindPort(string ip, out int _port)
        {
            bool gotIt = false;
            _port = 0;
            for (int port = 30000; port <= 60000; port++)
            {
                pos = new Ethernet();
                if (pos.Connect(ip, port) == 0)
                {
                    _port = port;
                    gotIt = true;
                    return gotIt;
                }
            }
            return gotIt;
        }

        /// <summary>
        /// Подготовка данных для отправки на принтер и отправка
        /// </summary>
        /// <param name="amount">Сумма к оплате в копейках</param>
        public UInt32 Purchase(int amount)
        {
            PurchResp = new Dictionary<byte, List<byte>>();
            sync = 10;
            pos.ReceivedEvent += Pos_ReceivedEvent;
            received = false;
            timeElapsed = false;
            List<byte> mess = new List<byte>();

            mess.Add(0x01);
            mess.Add(0x03);
            mess.AddRange(Encoding.ASCII.GetBytes("PUR"));

            byte[] tempBytes = { 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30, 0x30 };
            byte[] amountBytes = BitConverter.GetBytes(amount);
            for (int i = 0; i< amountBytes.Length; i++)
            {
                tempBytes[i] = amountBytes[i];
            }
            mess.Add(0x04);
            mess.Add(0x07);
            mess.AddRange(tempBytes);

            mess.Add(0x03);
            mess.Add(0x0A);
            byte [] ERN = { 0x30, 0x30, 0x36, 0x36, 0x35, 0x35, 0x38, 0x38, 0x39, 0x39};
            mess.AddRange(ERN);

            mess.Add(0x02);
            mess.Add(0x02);
            byte[] ECRN = { 0x30, 0x31};
            mess.AddRange(ECRN);
            try
            {
                lastError = 0;
                pos.Send(mess.ToArray());
                SetTimer();
                while (!timeElapsed == !received)
                { }
                if (received)
                {
                    pos.ReceivedEvent -= Pos_ReceivedEvent;
                    receivingTimer.Elapsed -= ReceivingTimer_Elapsed;
                    receivingTimer.Stop();
                    receivingTimer.Dispose();
                }
                if (timeElapsed)
                {
                    receivingTimer.Elapsed -= ReceivingTimer_Elapsed;
                    pos.ReceivedEvent -= Pos_ReceivedEvent;
                    //log.Write("Timeout error");
                    lastError = 4;
                }
                return lastError;
            }
            catch (Exception e)
            {
                receivingTimer.Elapsed -= ReceivingTimer_Elapsed;
                pos.ReceivedEvent -= Pos_ReceivedEvent;
                return lastError = 3;
            }
        }

        //TODO NotImplemented
        private void Pos_ReceivedEvent(byte[] data)
        {
            int len = BitConverter.ToUInt16(data, 0);
            this.dataReceived = new byte[len];
            for (int k = 2; k < len; k++)
            {
                dataReceived[k-2] = data[k];
            }

            received = true;
        }

        /// <summary>
        /// Подготовка данных для отправки на принтер и отправка(команда без данных)
        /// </summary>
        /// <param name="command">Комманда</param>

        /// <summary>
        /// Таймер ожидания ответа от принтера
        /// </summary>
        private void SetTimer()
        {
            receivingTimer = new System.Timers.Timer(5000);
            receivingTimer.Elapsed += ReceivingTimer_Elapsed;
            receivingTimer.AutoReset = false;
            receivingTimer.Enabled = true;
        }

        /// <summary>
        /// Обработчик события превышения времени ожидания ответа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReceivingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timeElapsed = true;
        }

        public bool GetPurchAnswer(byte [] data)
        {
            List<byte> resp = new List<byte>();
            IDictionary<byte, List<byte>> Response = new Dictionary<byte, List<byte>>();
            resp.AddRange(data);
            if (TLVDecode.Analize(resp, out Response))
            {
                PurchResp = Response;
                return true;
            }
            else
                return false;
        }

    }
}

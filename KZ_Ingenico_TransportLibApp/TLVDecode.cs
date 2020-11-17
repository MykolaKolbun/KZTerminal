using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KZ_Ingenico_TransportLibApp
{
    class TLVDecode
    {
        static List<byte> Tags = new List<byte>(new byte[] {
              0x01  //Длинна 3,     Описание MessageID
            , 0x02  //Длинна 2,     Описание ECRnumber
            , 0x03  //Длинна 10,    Описание ERN
            , 0x04  //Длинна 12,    Описание TransactionAmount
            , 0x05  //Длинна …76,   Описание Track1Data
            , 0x06  //Длинна …37,   Описание Track2Data
            , 0x07  //Длинна …107,  Описание Track3Data
            , 0x08  //Длинна 3,     Описание Flags
            , 0x09  //Длинна …19,   Описание PAN
            , 0x0A  //Длинна 4,     Описание ExpDate
            , 0x0B  //Длинна 6,     Описание InvoiceNumber
            , 0x0C  //Длинна 6,     Описание AuthorizationID
            , 0x11  //Длинна 2,     Описание MerchanID
            , 0x18  //Длинна 12,    Описание RRN
            , 0x1A  //Длинна 1,     Описание SRVsubfunction
            , 0x1B  //Длинна 3,     Описание Currency
            , 0x50  //Длинна …32,   Описание ApplicationLabel
            , 0x81  //Длинна 3,     Описание MessageID
            , 0x82  //Длинна 2,     Описание ECRnumber
            , 0x83  //Длинна 10,    Описание ERN
            , 0x84  //Длинна 12,    Описание TransactionAmount
            , 0x88  //Длинна 3,     Описание Flags
            , 0x89  //Длинна …19,     Описание PAN
            , 0x8A  //Длинна 4,     Описание ExpDate
            , 0x8B  //Длинна 6,     Описание InvoiceNumber
            , 0x8C  //Длинна 6,     Описание AuthorizationID
            , 0x8D  //Длинна 4,     Описание Date
            , 0x8E  //Длинна 4,     Описание Time
            , 0x8F  //Длинна …8,    Описание IssuerName
            , 0x90  //Длинна 15,    Описание MerchantNo
            , 0x91  //Длинна 6,     Описание ProcessingCode
            , 0x92  //Длинна 3,     Описание POSEntryMode
            , 0x93  //Длинна 2,     Описание POSConditionCode
            , 0x94  //Длинна 1,     Описание CardholderVerificationCharacter
            , 0x95  //Длинна 10,    Описание TVR
            , 0x96  //Длинна 12,    Описание DateTime YYMMDDHHmmss
            , 0x97  //Длинна …40,   Описание TransactionID
            , 0x98  //Длинна 12,    Описание RRN
            , 0x99  //Длинна 6,     Описание BatchNo
            , 0x9A  //Длинна 12,    Описание TransactionAmount#2
            , 0x9B  //Длинна 2,     Описание Response Code
            , 0x9C  //Длинна 8,     Описание TerminalID
            , 0x9D  //Длинна …65535*, Описание Receipt
            , 0xA0  //Длинна …40,   Описание Visual Host Response
            , 0xA1  //Длинна 1,     Описание Approve
            , 0xC0  //Длинна …64,   Описание CardDataEnc
            , 0xC1  //Длинна …401,  Описание SelectionList. Список для выбора одного пункта
            , 0xC2  //Длинна …20,   Описание SelectionResult. Результат выбора
            , 0xE1  //Зарезервировано
        });

        public static bool Analize(List<byte> data, out IDictionary<byte, List<byte>> Response)
        {
            Response = new Dictionary<byte, List<byte>>();
            byte tag = ReadTag(ref data, true);
            if (Tags.Contains(tag))
            {
                int len = ReadLength(ref data);
                if (len > 0)
                {
                    Response.Add(tag, ReadData(ref data, len));
                }
            }
            return true;
        }

        private static int ReadLength( ref List<byte> data)
        {
            if ((data[0] & 0x80) == 0)
            {
                int ln = (int)data[0];
                data.RemoveAt(0);
                return ln; // length is in first byte
            }

            int length = 0;
            var lengthBytes = data[0] & 0x7f; // remove 0x80 bit
            data.RemoveAt(0);

            if (lengthBytes == 0)
                return 0; // indefinite form

            if (lengthBytes > 4)
                return 0; //throw new TlvException($"Unsupported length: {lengthBytes} bytes");

            for (var i = 0; i < lengthBytes; i++)
            {
                length <<= 8;
                length |= (int)data[0];
                data.RemoveAt(0);
            }
            return length;
        }

        private static List<byte> ReadTag(ref List<byte> data)
        {
            List<byte> tagID = new List<byte>();
            if ((data[0] & 0x80) == 0)
            {
                tagID.Add(data[0]);
                data.RemoveAt(0);
            }
            else
            {
                for(int i= 0; i<2; i++)
                {
                    tagID.Add(data[0]);
                    data.RemoveAt(0);
                }
            }
            return tagID;
        }

        private static byte ReadTag(ref List<byte> data, bool flag)
        {
            byte tagID = data[0];
            data.RemoveAt(0);
            return tagID;
        }

        private static List<byte> ReadData(ref List<byte> data, int len)
        {
            List<byte> outMessage= new List<byte>();
            for(int i = 0; i<len; i++)
            {
                outMessage.Add(data[0]);
                data.RemoveAt(0);
            }
            return outMessage; ;
        }
    }
}

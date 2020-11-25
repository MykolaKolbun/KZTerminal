using System;
using System.Runtime.Serialization.Formatters.Binary;


namespace KZ_Ingenico_EPI
{
    [Serializable]
    public class CountTransaction
    {
        string Path;// = string.Format(@"C:\FiscalFolder\TransactionCounts.dat");

        public CountTransaction(string machinID)
        {
            this.Path = $@"C:\FiscalFolder\TransactionCounts_{machinID}.dat";
        }
        public void Send(int count)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(Path, System.IO.FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, count);
            }
        }

        public int Get()
        {
            int count = 98;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                using (System.IO.FileStream fs = new System.IO.FileStream(Path, System.IO.FileMode.OpenOrCreate))
                {
                    count = (int)formatter.Deserialize(fs);
                }
            }
            catch
            {
                count = 98;
            }
            return count;
        }
    }
}

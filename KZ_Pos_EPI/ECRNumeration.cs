using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace KZ_Pos_EPI
{
    class ECRNumeration
    {
        string Path;
        public ECRNumeration(string machinID)
        {
            this.Path = $@"{StringValue.WorkingDirectory}EPI\ECRNumber_{machinID}.dat";
        }
        public void Add()
        {
            int ECRNR = Get();
            ECRNR++;
            BinaryFormatter formatter = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(Path, System.IO.FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, ECRNR);
            }
        }

        public int Get()
        {
            int count = 10000;
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
                count = 10000;
            }
            return count;
        }

        public void Resert()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (System.IO.FileStream fs = new System.IO.FileStream(Path, System.IO.FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, 10000);
            }
        }
    }
}

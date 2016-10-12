using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFrame.LoadBin
{
    public class data_xx
    {

    }

    public class sLoadBin_xx
    {
        public Dictionary<string, data_xx> data = new Dictionary<string, data_xx>();

        public void load(string name)
        {
            FileStream fs = new FileStream(name, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            int num = br.ReadInt32();
            for (int i = 0; i < num; ++i)
            {
            }
            br.Close();
            fs.Close();
        }
    }
}

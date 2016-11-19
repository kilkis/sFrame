using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_test1
	{
		public int a;
		public List<int> b = new List<int>();
		public int e;
	}

	public class sLoadBin_test1
	{
		public Dictionary<int ,data_test1> data = new Dictionary<int ,data_test1>();
		public static sLoadBin_test1 instance = new sLoadBin_test1();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_test1 tmp = new data_test1();
				tmp.a = br.ReadInt32();
				string tmp = br.ReadString();
				string[] tmps1 = tmp.Split(',');
				for (int j = 0; j < tmps1.Length; ++j)
				{
					b.Add(tmps1[j]);
				}
				tmp.e = br.ReadInt32();
				data.Add(tmp.a ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

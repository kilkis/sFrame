using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_language
	{
		public int id;
		public string language;
	}

	public class sLoadBin_language
	{
		public Dictionary<int ,data_language> data = new Dictionary<int ,data_language>();
		public static sLoadBin_language instance = new sLoadBin_language();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_language tmp = new data_language();
				tmp.id = br.ReadInt32();
				tmp.language = br.ReadString();
				data.Add(tmp.id ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

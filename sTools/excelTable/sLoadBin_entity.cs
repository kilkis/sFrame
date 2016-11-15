using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_entity
	{
		public int id;
		public int NameID;
		public string ModelName;
		public string Type;
		public int MoveSpeed;
	}

	public class sLoadBin_entity
	{
		public Dictionary<int ,data_entity> data = new Dictionary<int ,data_entity>();
		public static sLoadBin_entity instance = new sLoadBin_entity();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_entity tmp = new data_entity();
				tmp.id = br.ReadInt32();
				tmp.NameID = br.ReadInt32();
				tmp.ModelName = br.ReadString();
				tmp.Type = br.ReadString();
				tmp.MoveSpeed = br.ReadInt32();
				data.Add(tmp.id ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

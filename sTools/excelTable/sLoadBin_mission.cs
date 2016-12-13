using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_mission
	{
		public int id;
		public int type;
		public int needMid;
		public int needLvl;
		public int needOther;
		public int reGet;
		public List<int> reward = new List<int>();
		public int map;
		public string info;
	}

	public class sLoadBin_mission
	{
		public Dictionary<int ,data_mission> data = new Dictionary<int ,data_mission>();
		public static sLoadBin_mission instance = new sLoadBin_mission();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_mission tmp = new data_mission();
				tmp.id = br.ReadInt32();
				tmp.type = br.ReadInt32();
				tmp.needMid = br.ReadInt32();
				tmp.needLvl = br.ReadInt32();
				tmp.needOther = br.ReadInt32();
				tmp.reGet = br.ReadInt32();
				string tmp = br.ReadString();
				string[] tmps1 = tmp.Split(',');
				for (int j = 0; j < tmps1.Length; ++j)
				{
					reward.Add(tmps1[j]);
				}
				tmp.map = br.ReadInt32();
				tmp.info = br.ReadString();
				data.Add(tmp.id ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_item
	{
		public int id;
		public string name;
		public string info;
		public int useNeedItem;
		public int useNeedNum;
		public int maxnum;
		public int life;
		public int career;
		public int hpAdd;
		public int mpAdd;
		public int goldAdd;
		public int gemAdd;
		public int limitCD;
	}

	public class sLoadBin_item
	{
		public Dictionary<int ,data_item> data = new Dictionary<int ,data_item>();
		public static sLoadBin_item instance = new sLoadBin_item();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_item tmp = new data_item();
				tmp.id = br.ReadInt32();
				tmp.name = br.ReadString();
				tmp.info = br.ReadString();
				tmp.useNeedItem = br.ReadInt32();
				tmp.useNeedNum = br.ReadInt32();
				tmp.maxnum = br.ReadInt32();
				tmp.life = br.ReadInt32();
				tmp.career = br.ReadInt32();
				tmp.hpAdd = br.ReadInt32();
				tmp.mpAdd = br.ReadInt32();
				tmp.goldAdd = br.ReadInt32();
				tmp.gemAdd = br.ReadInt32();
				tmp.limitCD = br.ReadInt32();
				data.Add(tmp.id ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

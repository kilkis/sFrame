using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFrame.LoadBin
{
	public class data_mapinfo
	{
		public int mapID;
		public string mapName;
	}

	public class sLoadBin_mapinfo
	{
		public Dictionary<int ,data_mapinfo> data = new Dictionary<int ,data_mapinfo>();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_mapinfo tmp = new data_mapinfo();
				tmp.mapID = br.ReadInt32();
				tmp.mapName = br.ReadString();
				data.Add(tmp.mapID ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

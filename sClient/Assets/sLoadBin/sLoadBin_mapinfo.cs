using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_mapinfo
	{
		public int mapID;
		public string mapName;
		public string navMesh;
		public int type;
		public Vector3 position;
	}

	public class sLoadBin_mapinfo
	{
		public Dictionary<int ,data_mapinfo> data = new Dictionary<int ,data_mapinfo>();
        public static sLoadBin_mapinfo instance = new sLoadBin_mapinfo();
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
				tmp.navMesh = br.ReadString();
				tmp.type = br.ReadInt32();
				{
					float x = br.ReadSingle();
					float y = br.ReadSingle();
					float z = br.ReadSingle();
					tmp.position = new Vector3(x, y, z);
				}
				data.Add(tmp.mapID ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

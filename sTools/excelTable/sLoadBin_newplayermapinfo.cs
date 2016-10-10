using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFrame.LoadBin
{
	public class data_newplayermapinfo
	{
		public int race;
		public int mapID;
		public Vector3 position;
		public Vector3 rotation;
	}

	public class sLoadBin_newplayermapinfo
	{
		public List<data_newplayermapinfo> data = new List<data_newplayermapinfo>();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_newplayermapinfo tmp = new data_newplayermapinfo();
				tmp.race = br.ReadInt32();
				tmp.mapID = br.ReadInt32();
				{
					float x = br.ReadSingle();
					float y = br.ReadSingle();
					float z = br.ReadSingle();
					tmp.position = new Vector3(x, y, z);
				}
				{
					float x = br.ReadSingle();
					float y = br.ReadSingle();
					float z = br.ReadSingle();
					tmp.rotation = new Vector3(x, y, z);
				}
				data.Add(tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

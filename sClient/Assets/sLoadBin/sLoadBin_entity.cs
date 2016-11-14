using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_entity
	{
		public int id;
		public string NameID;
		public string Type;
		public int MaxHp;
		public int MaxMp;
		public int MoveSpeed;
		public int Strength;
		public int SkillID;
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
				tmp.NameID = br.ReadString();
				tmp.Type = br.ReadString();
				tmp.MaxHp = br.ReadInt32();
				tmp.MaxMp = br.ReadInt32();
				tmp.MoveSpeed = br.ReadInt32();
				tmp.Strength = br.ReadInt32();
				tmp.SkillID = br.ReadInt32();
				data.Add(tmp.id ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

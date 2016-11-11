using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_monster
	{
		public int id;
		public string Name;
		public int MaxHp;
		public int MaxMp;
		public int MoveSpeed;
		public int Strength;
		public int SkillID;
		public int ViewRange;
		public int BackRange;
	}

	public class sLoadBin_monster
	{
		public Dictionary<int ,data_monster> data = new Dictionary<int ,data_monster>();
		public static sLoadBin_monster instance = new sLoadBin_monster();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_monster tmp = new data_monster();
				tmp.id = br.ReadInt32();
				tmp.Name = br.ReadString();
				tmp.MaxHp = br.ReadInt32();
				tmp.MaxMp = br.ReadInt32();
				tmp.MoveSpeed = br.ReadInt32();
				tmp.Strength = br.ReadInt32();
				tmp.SkillID = br.ReadInt32();
				tmp.ViewRange = br.ReadInt32();
				tmp.BackRange = br.ReadInt32();
				data.Add(tmp.id ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

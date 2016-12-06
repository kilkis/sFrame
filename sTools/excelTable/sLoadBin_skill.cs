using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace sFramework.LoadBin
{
	public class data_skill
	{
		public int skillID;
		public int skillNameID;
		public int career;
		public int talent;
		public float coolDown;
		public int target;
		public object distance（F）;
		public int passitive;
		public float castTime;
		public int enableMove;
		public int cost;
		public int skillType;
		public int selfSkillEffect;
		public List<int> h = new List<int>();
	}

	public class sLoadBin_skill
	{
		public Dictionary<int ,data_skill> data = new Dictionary<int ,data_skill>();
		public static sLoadBin_skill instance = new sLoadBin_skill();

		public void load(string name)
		{
			FileStream fs = new FileStream(name, FileMode.Open);
			BinaryReader br = new BinaryReader(fs);
			int num = br.ReadInt32();
			for (int i = 0; i < num; ++i)
			{
				data_skill tmp = new data_skill();
				tmp.skillID = br.ReadInt32();
				tmp.skillNameID = br.ReadInt32();
				tmp.career = br.ReadInt32();
				tmp.talent = br.ReadInt32();
				tmp.coolDown = br.ReadSingle();
				tmp.target = br.ReadInt32();
				tmp.distance（F） = br.Read();
				tmp.passitive = br.ReadInt32();
				tmp.castTime = br.ReadSingle();
				tmp.enableMove = br.ReadInt32();
				tmp.cost = br.ReadInt32();
				tmp.skillType = br.ReadInt32();
				tmp.selfSkillEffect = br.ReadInt32();
				string tmp = br.ReadString();
				string[] tmps1 = tmp.Split(',');
				for (int j = 0; j < tmps1.Length; ++j)
				{
					h.Add(tmps1[j]);
				}
				data.Add(tmp.skillID ,tmp);
			}
			br.Close();
			fs.Close();
		}
	}
}

using UnityEngine;
using System.Collections;

public enum sEntityAttrType
{
	Name,
	GuildName,
	Vip,
	Lvl,
	Exp,
	Hp,
	HpMax,
	Mp,
	MpMax,
	Strength,
    Dexterity,
    Intelligence,
    Stamina,
    Equiplvl,
    DamageMin,
    DamageMax,
    Defence,
}

public class sEntityAttr
{
	//这里的坐标和方向用于保存在pc未创建时候服务器下发的消息
	public Vector3 position = Vector3.zero;
	public Vector3 direction = Vector3.zero;

    public string name;
    public string guildname;
    public int vip;

	public ushort level;
    public ulong exp;

    public int hp;
    public int hp_max;
    public int mp;
    public int mp_max;

    public int strength;
    public int dexterity;
    public int intelligence;
    public int stamina;
    public int equiplvl;

    public int damageMin;
    public int damageMax;
    public int defence;
}

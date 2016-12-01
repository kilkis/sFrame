using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sFramework;

public class sEntityInfo
{
	public string type;
    public long uid;
    public sEntityModel pm;//模型层
    public sEntityControl pc;//控制层
    public float delTime;

	public sEntityAttr attr = new sEntityAttr();
    
    public GameObject playerCC;//dummy层
}

/// <summary>
/// entity管理器
/// 负责管理所有服务器下发玩家,怪物，npc的信息，包括模型的脚本管理
/// todo:范围内模型读取与显示，显示上限处理，动态变化
/// </summary>
public class sEntityManager : sSingleton<sEntityManager>
{
    //最多显示的玩家数量
    public int maxShowPlayers = 30;
    //显示玩家的范围，这个考虑用服务器的AOI来处理
    public float showPlayersRange = 30.0f;
    //是否显示玩家
    public bool showPlayer = true;

    public sEntityInfo selfPlayer = new sEntityInfo();
    //服务器下发到客户端的entity
    public Dictionary<long, sEntityInfo> s2cPlayers = new Dictionary<long, sEntityInfo>();
    //预备删除，记录用数据
    public Dictionary<long, sEntityInfo> ready2delPlayers = new Dictionary<long, sEntityInfo>();
    //预备删除，遍历用数据
    public List<sEntityInfo> r2dPlayers = new List<sEntityInfo>();
    
    public bool isSelfCreated()
    {
        return selfPlayer.playerCC == null ? false : true;
    }

    bool isSelf(long uid)
    {
        return selfPlayer.uid == uid ? true : false;
    }

    //创建自己
    public void createSelf(long uid, Vector3 startpos)
    {
        if (selfPlayer.playerCC != null)
            return;
        Debug.Log("self id:" + uid);
        selfPlayer.playerCC = GameObject.Instantiate(sULoading.instance.playerCC, startpos, Quaternion.LookRotation(new Vector3(1, 0, 0))) as GameObject;
        selfPlayer.playerCC.SetActive(true);
        selfPlayer.playerCC.name = sStringBuilder.combine(selfPlayer.playerCC.name, "_", uid);
        selfPlayer.playerCC.layer = LayerMask.NameToLayer(sConst.playerLayer);
        selfPlayer.pc = selfPlayer.playerCC.GetComponent<sEntityControl>();
        selfPlayer.pc.enableControl(true);

        sULoading.instance.enableCamera();
        sCamera.instance.setFollower(selfPlayer.playerCC.transform);

        selfPlayer.uid = uid;
        selfPlayer.pm = selfPlayer.playerCC.GetComponent<sEntityModel>();
        selfPlayer.pm.playerUID = uid;
        selfPlayer.pm.bone = "FS_bone";
        selfPlayer.pm.chestName = "FS_chest_000";
        selfPlayer.pm.footName = "FS_foot_000";
        selfPlayer.pm.handName = "FS_hand_000";
        selfPlayer.pm.headName = "FS_head_000";
        selfPlayer.pm.legName = "FS_leg_000";
		selfPlayer.pm.createAvatar(selfPlayer.playerCC);
		//selfPlayer.pm.createModel("cube", selfPlayer.playerCC);

        selfPlayer.delTime = 0;
        //selfPlayer.name = name;
        //selfPlayer.guildname = guildname;
        //selfPlayer.vip = vip;
    }

    public bool isPlayerCreated(long uid)
    {
        if (!s2cPlayers.ContainsKey(uid))
            return false;
        if (s2cPlayers[uid].playerCC == null)
            return false;
        return true;
    }

    public sEntityInfo getOrCreatePlayer(long uid)
    {
        if (s2cPlayers.ContainsKey(uid))
            return s2cPlayers[uid];
        sEntityInfo tmp = new sEntityInfo();
        tmp.uid = uid;
        s2cPlayers.Add(uid, tmp);
        return tmp;
    }
	public void pushEntity(long uid, Vector3 startpos, string etype)
    {
        if (isSelf(uid))
            return;

        Debug.Log("push pid:" + uid);
        sEntityInfo tmp = getOrCreatePlayer(uid);
		tmp.playerCC = GameObject.Instantiate(sULoading.instance.playerCC, tmp.attr.position == Vector3.zero?startpos:tmp.attr.position, Quaternion.LookRotation(tmp.attr.direction)) as GameObject;
        tmp.playerCC.SetActive(true);
        tmp.playerCC.name = sStringBuilder.combine(tmp.playerCC.name, "_", uid);
        tmp.playerCC.layer = LayerMask.NameToLayer(sConst.othersLayer);
        tmp.pc = tmp.playerCC.GetComponent<sEntityControl>();
        tmp.uid = uid;
        tmp.pm = tmp.playerCC.GetComponent<sEntityModel>();
        tmp.pm.playerUID = uid;
        tmp.pm.bone = "FS_bone";
        tmp.pm.chestName = "FS_chest_000";
        tmp.pm.footName = "FS_foot_000";
        tmp.pm.handName = "FS_hand_000";
        tmp.pm.headName = "FS_head_000";
        tmp.pm.legName = "FS_leg_000";
		tmp.pm.createAvatar(tmp.playerCC);

        tmp.delTime = 0;
        
    }

	public void pushEntity(long uid, Vector3 startpos, string modelName, string etype)
	{
		if (isSelf (uid))
			return;
		Debug.Log ("push pid2:" + uid);

		sEntityInfo tmp = getOrCreatePlayer(uid);
        tmp.playerCC = GameObject.Instantiate(sULoading.instance.playerCC, tmp.attr.position == Vector3.zero ? startpos : tmp.attr.position, tmp.attr.direction == Vector3.zero?Quaternion.identity:Quaternion.LookRotation(tmp.attr.direction)) as GameObject;
		tmp.playerCC.SetActive(true);
        tmp.playerCC.name = sStringBuilder.combine(tmp.playerCC.name, "_", uid);
        tmp.playerCC.layer = LayerMask.NameToLayer(sConst.othersLayer);
		tmp.pc = tmp.playerCC.GetComponent<sEntityControl>();
		tmp.uid = uid;
		tmp.pm = tmp.playerCC.GetComponent<sEntityModel>();
		tmp.pm.playerUID = uid;
		tmp.pm.createModel (modelName, tmp.playerCC);
		tmp.delTime = 0;
	}

    public void popPlayer(long uid)
    {
        if (isSelf(uid))
            return;
        if ( s2cPlayers.ContainsKey(uid))
        {
            s2cPlayers[uid].pm.destroyModel();
            //todo:删除后续内容
        }
    }

    public void logicUpdate(float deltaTime)
    {
        for( int i= 0;i < r2dPlayers.Count; ++i )
        {

        }
    }

    public void renderUpdate()
    {

    }

    //强制设定坐标
    public void setPosition(long uid, Vector3 pos)
    {
        Debug.Log("set position:" + uid + " : " + pos);
        //强制设定坐标的情况是会包含自身的
        if( isSelf(uid))
        {
			if (selfPlayer.pc != null)
				selfPlayer.pc.setPosition (pos);
			else
				selfPlayer.attr.position = pos;
        }
        else
        {
			sEntityInfo tmp = getOrCreatePlayer(uid);
			if (tmp.pc != null)
				tmp.pc.setPosition (pos);
			else
				tmp.attr.position = pos;
        }
    }

    //移动到某个坐标
    public void moveToPosition(long uid, Vector3 pos)
    {
        Debug.Log("move position:" + uid + " : " + pos);
        //自身是不允许服务器传递移动消息的
        if ( isSelf(uid))
        {
            Debug.LogError("move position can't be self");
            return;
        }
    
		sEntityInfo tmp = getOrCreatePlayer(uid);
		if (tmp.pc != null)
			tmp.pc.moveToPosition (pos);
		else
			tmp.attr.position = pos;
    }

    public void setDirection(long uid, Vector3 dir)
    {
		if ( isSelf(uid))
		{
			Debug.LogError("move position can't be self");
			return;
		}

		sEntityInfo tmp = getOrCreatePlayer(uid);
		if (tmp.pc != null)
			tmp.pc.setDirection (dir);
		else
			tmp.attr.direction = dir;
    }

    //set attr
	public void setAttr(long uid, sEntityAttrType paType, object value)
	{
		Debug.Log ("set attr:" + uid + " - " + paType + " - " + value);
		sEntityInfo tmp = getOrCreatePlayer(uid);
		switch (paType) {
		    case sEntityAttrType.Name:
			tmp.attr.name = (string)value;
			break;
		    case sEntityAttrType.GuildName:
			tmp.attr.guildname = (string)value;
			break;
		    case sEntityAttrType.Vip:
			tmp.attr.vip = (int)value;
			break;
		    case sEntityAttrType.Lvl:
			tmp.attr.level = (ushort)value;
			break;
		    case sEntityAttrType.Exp:
            tmp.attr.exp = (ulong)value;
			break;
		    case sEntityAttrType.Hp:
			tmp.attr.hp = (int)value;
			break;
		    case sEntityAttrType.HpMax:
			tmp.attr.hp_max = (int)value;
			break;
		    case sEntityAttrType.Mp:
			tmp.attr.mp = (int)value;
			break;
		    case sEntityAttrType.MpMax:
			tmp.attr.mp_max = (int)value;
			break;
		    case sEntityAttrType.Strength:
			tmp.attr.strength = (int)value;
			break;
            case sEntityAttrType.Dexterity:
            tmp.attr.dexterity = (int)value;
            break;
            case sEntityAttrType.Intelligence:
            tmp.attr.intelligence = (int)value;
            break;
            case sEntityAttrType.Stamina:
            tmp.attr.stamina = (int)value;
            break;
            case sEntityAttrType.Equiplvl:
            tmp.attr.equiplvl = (int)value;
            break;
            case sEntityAttrType.DamageMin:
            tmp.attr.damageMin = (int)value;
            break;
            case sEntityAttrType.DamageMax:
            tmp.attr.damageMax = (int)value;
            break;
            case sEntityAttrType.Defence:
            tmp.attr.defence = (int)value;
            break;
		}
	}
   
    //...
}

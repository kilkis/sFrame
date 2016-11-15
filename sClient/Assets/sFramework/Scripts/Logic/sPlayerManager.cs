using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sFramework;

public class sPlayerInfo
{
    public long uid;
    public sPlayerModel pm;
    public sPlayerControl pc;
    public float delTime;

    public sPlayerAttr attr = new sPlayerAttr();
    
    public GameObject playerCC;
}

/// <summary>
/// 玩家管理器
/// 负责管理所有服务器下发玩家的信息，包括玩家模型的脚本管理
/// todo:范围内玩家模型读取与显示，显示上限处理，动态变化
/// </summary>
public class sPlayerManager : sSingleton<sPlayerManager>
{
    //最多显示的玩家数量
    public int maxShowPlayers = 30;
    //显示玩家的范围，这个考虑用服务器的AOI来处理
    public float showPlayersRange = 30.0f;
    //是否显示玩家
    public bool showPlayer = true;

    public sPlayerInfo selfPlayer = new sPlayerInfo();
    //服务器下发到客户端的玩家
    public Dictionary<long, sPlayerInfo> s2cPlayers = new Dictionary<long, sPlayerInfo>();
    //预备删除，记录用数据
    public Dictionary<long, sPlayerInfo> ready2delPlayers = new Dictionary<long, sPlayerInfo>();
    //预备删除，遍历用数据
    public List<sPlayerInfo> r2dPlayers = new List<sPlayerInfo>();
    
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
        selfPlayer.pc = selfPlayer.playerCC.GetComponent<sPlayerControl>();
        selfPlayer.pc.enableControl(true);

        sULoading.instance.enableCamera();
        sCamera.instance.setFollower(selfPlayer.playerCC.transform);

        selfPlayer.uid = uid;
        selfPlayer.pm = selfPlayer.playerCC.GetComponent<sPlayerModel>();
        selfPlayer.pm.playerUID = uid;
        selfPlayer.pm.bone = "FS_bone";
        selfPlayer.pm.chestName = "FS_chest_000";
        selfPlayer.pm.footName = "FS_foot_000";
        selfPlayer.pm.handName = "FS_hand_000";
        selfPlayer.pm.headName = "FS_head_000";
        selfPlayer.pm.legName = "FS_leg_000";
		//selfPlayer.pm.createAvatar(selfPlayer.playerCC);
		selfPlayer.pm.createModel(selfPlayer.playerCC);

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

    public sPlayerInfo getOrCreatePlayer(long uid)
    {
        if (s2cPlayers.ContainsKey(uid))
            return s2cPlayers[uid];
        sPlayerInfo tmp = new sPlayerInfo();
        tmp.uid = uid;
        s2cPlayers.Add(uid, tmp);
        return tmp;
    }
    public void pushPlayer(long uid, Vector3 startpos)
    {
        if (isSelf(uid))
            return;

        Debug.Log("push pid:" + uid);
        sPlayerInfo tmp = getOrCreatePlayer(uid);
		tmp.playerCC = GameObject.Instantiate(sULoading.instance.playerCC, tmp.attr.position == Vector3.zero?startpos:tmp.attr.position, Quaternion.LookRotation(tmp.attr.direction)) as GameObject;
        tmp.playerCC.SetActive(true);
        tmp.pc = tmp.playerCC.GetComponent<sPlayerControl>();
        tmp.uid = uid;
        tmp.pm = tmp.playerCC.GetComponent<sPlayerModel>();
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
			sPlayerInfo tmp = getOrCreatePlayer(uid);
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
    
		sPlayerInfo tmp = getOrCreatePlayer(uid);
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

		sPlayerInfo tmp = getOrCreatePlayer(uid);
		if (tmp.pc != null)
			tmp.pc.setDirection (dir);
		else
			tmp.attr.direction = dir;
    }

    //set attr
	public void setAttr(long uid, sPlayerAttrType paType, object value)
	{
		Debug.Log ("set attr:" + uid + " - " + paType + " - " + value);
		sPlayerInfo tmp = getOrCreatePlayer(uid);
		switch (paType) {
		case sPlayerAttrType.Name:
			tmp.attr.name = (string)value;
			break;
		case sPlayerAttrType.GuildName:
			tmp.attr.guildname = (string)value;
			break;
		case sPlayerAttrType.Vip:
			tmp.attr.vip = (int)value;
			break;
		case sPlayerAttrType.Lvl:
			tmp.attr.level = (ushort)value;
			break;
		case sPlayerAttrType.Exp:
			tmp.attr.exp = (int)value;
			break;
		case sPlayerAttrType.Hp:
			tmp.attr.hp = (int)value;
			break;
		case sPlayerAttrType.HpMax:
			tmp.attr.hp_max = (int)value;
			break;
		case sPlayerAttrType.Mp:
			tmp.attr.mp = (int)value;
			break;
		case sPlayerAttrType.MpMax:
			tmp.attr.mp_max = (int)value;
			break;
		case sPlayerAttrType.Strength:
			tmp.attr.strength = (int)value;
			break;
		}
	}
   
    //...
}

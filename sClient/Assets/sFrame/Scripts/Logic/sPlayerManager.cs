using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using sFrame;

public class sPlayerInfo
{
    public long uid;
    public sPlayerModel pm;
    public sPlayerControl pc;
    public float delTime;

    public string name;
    public string guildname;
    public int vip;

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


    GameObject selfplayer;

    public bool isSelfCreated()
    {
        return selfPlayer.playerCC == null ? false : true;
    }
    //创建自己
    public void createSelf(long uid, string name, string guildname, int vip, Vector3 startpos)
    {
        if (selfPlayer.playerCC != null)
            return;
        Debug.Log("self id:" + uid);
        selfPlayer.playerCC = GameObject.Instantiate(sULoading.instance.playerCC, startpos, Quaternion.LookRotation(new Vector3(1, 0, 0))) as GameObject;
        selfPlayer.playerCC.SetActive(true);
        selfPlayer.pc = selfPlayer.playerCC.GetComponent<sPlayerControl>();

        sULoading.instance.enableCamera();
        sCamera.instance.setFollower(selfPlayer.playerCC.transform);

        selfPlayer.uid = uid;
        selfPlayer.pm = new sPlayerModel();
        selfPlayer.pm.playerUID = uid;
        selfPlayer.pm.bone = "FS_bone";
        selfPlayer.pm.chestName = "FS_chest_000";
        selfPlayer.pm.footName = "FS_foot_000";
        selfPlayer.pm.handName = "FS_hand_000";
        selfPlayer.pm.headName = "FS_head_000";
        selfPlayer.pm.legName = "FS_leg_000";
        selfPlayer.pm.createFromName(selfPlayer.playerCC);

        selfPlayer.delTime = 0;
        selfPlayer.name = name;
        selfPlayer.guildname = guildname;
        selfPlayer.vip = vip;
    }

    public bool isPlayerCreated(long uid)
    {
        if (!s2cPlayers.ContainsKey(uid))
            return false;
        if (s2cPlayers[uid].playerCC == null)
            return false;
        return true;
    }
    public void pushPlayer(long uid, string name, string guildname, int vip, Vector3 startpos)
    {
        if( !s2cPlayers.ContainsKey(uid))
        {
            Debug.Log("pid:" + uid);
            sPlayerInfo tmp = new sPlayerInfo();
            tmp.playerCC = GameObject.Instantiate(sULoading.instance.playerCC, startpos, Quaternion.LookRotation(new Vector3(1, 0, 0))) as GameObject;
            tmp.playerCC.SetActive(true);
            
            tmp.uid = uid;
            tmp.pm = new sPlayerModel();
            tmp.pm.playerUID = uid;
            tmp.pm.bone = "FS_bone";
            tmp.pm.chestName = "FS_chest_000";
            tmp.pm.footName = "FS_foot_000";
            tmp.pm.handName = "FS_hand_000";
            tmp.pm.headName = "FS_head_000";
            tmp.pm.legName = "FS_leg_000";
            tmp.pm.createFromName(tmp.playerCC);

            tmp.delTime = 0;
            tmp.name = name;
            tmp.guildname = guildname;
            tmp.vip = vip;
        }
    }

    public void popPlayer(long uid)
    {
        if( s2cPlayers.ContainsKey(uid))
        {
            s2cPlayers[uid].pm.destroyModel();
            //todo:删除后续内容
        }
    }

    public void logicUpdate()
    {
        for( int i= 0;i < r2dPlayers.Count; ++i )
        {

        }
    }
}

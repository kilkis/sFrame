using UnityEngine;
using System;
using System.Collections.Generic;

namespace sFramework
{
    public class sNetworkInterface : MonoBehaviour
    {

        public static sNetworkInterface instance;

        public UInt64 selAvatarDBID = 0;

        void Awake()
        {
            instance = this;
        }

        public void createAccount(string usrName, string passWord, string cbFile, string cbMethod)
        {
            sNetworkOutOfWorld.inst.pushLuaCB("createAccount", cbFile, cbMethod);
            KBEngine.Event.fireIn("createAccount", usrName, passWord, System.Text.Encoding.UTF8.GetBytes("sFrame MMO"));
        }
        public void login(string usrName, string passWord, string cbFile, string cbMethod)
        {
            sNetworkOutOfWorld.inst.pushLuaCB("login", cbFile, cbMethod);
            KBEngine.Event.fireIn("login", usrName, passWord, System.Text.Encoding.UTF8.GetBytes("sFrame MMO"));
        }

        public void useTargetSkill(int sid)
        {
            KBEngine.Event.fireIn("useTargetSkill", sid, (int)sEntityManager.GetInstance().selfPlayer.pc.focusEntityID);	
        }

        public void sendMsg2Server(string cmdName, params object[] args )
        {
            Debug.Log(args[0].GetType() + " .. " + args[0].ToString());
            KBEngine.Event.fireIn(cmdName, args);
        }

        public void selAvatar(UInt64 dbid, string cbFile, string cbMethod)
        {
            //UInt64 _dbid = UInt64.Parse(dbid.ToString());
            sNetworkOutOfWorld.inst.pushLuaCB("selAvatar", cbFile, cbMethod);
            KBEngine.Event.fireIn("selectAvatarGame", dbid);
        }

        public void createAvatar(byte roleType, string name, string cbFile, string cbMethod)
        {
            sNetworkOutOfWorld.inst.pushLuaCB("createAvatar", cbFile, cbMethod);
            KBEngine.Event.fireIn("reqCreateAvatar", roleType, name);
        }

        public void delAvatar(string name, string cbFile, string cbMethod)
        {
            Debug.Log("remove avatar:" + name);
            sNetworkOutOfWorld.inst.pushLuaCB("delAvatar", cbFile, cbMethod);
            KBEngine.Event.fireIn("reqRemoveAvatar", name);
        }

        public Dictionary<UInt64, sAvatarList> getAvatarList()
        {
            return sNetworkOutOfWorld.inst.getAvatarList();
        }
        
    }
}

using KBEngine;
using UnityEngine;
using System; 
using System.Collections.Generic;
using sFramework;

public class sAvatarList
{
    public UInt64 dbid;
    public string name;

    public sAvatarList(UInt64 dbid, string name)
    {
        this.dbid = dbid;
        this.name = name;
    }
}

public class sLuaCallback
{
    public string cbFile;
    public string cbMethod;

    public sLuaCallback(string file, string method)
    {
        cbFile = file;
        cbMethod = method;
    }
}

public class sNetworkOutOfWorld : MonoBehaviour 
{
	public static sNetworkOutOfWorld inst;

    private Dictionary<string, sLuaCallback> luaCBs = new Dictionary<string, sLuaCallback>();
	
	public int ui_state = 0;
	private string stringAccount = "";
	private string stringPasswd = "";
	private string labelMsg = "";
	private Color labelColor = Color.green;
	
	private Dictionary<UInt64, sAvatarList> ui_avatarList = null;
	
	private string stringAvatarName = "";
	private bool startCreateAvatar = false;

	private UInt64 selAvatarDBID = 0;
    //public bool showReliveGUI = false;

    public bool isDebug = true;
	
	void Awake() 
	 {
		inst = this;
		//DontDestroyOnLoad(transform.gameObject);
	 }
	 
	// Use this for initialization
	void Start () 
	{
		installEvents();
		//Application.LoadLevel("login");
	}

    public void pushLuaCB(string msg, string file, string method)
    {
        luaCBs.Add(msg, new sLuaCallback(file, method));
    }

    public void callAndRemoveLua(string msg)
    {
        if (luaCBs.ContainsKey(msg))
        {
            LuaFramework.Util.CallMethod(luaCBs[msg].cbFile, luaCBs[msg].cbMethod);
            luaCBs.Remove(msg);
        }
    }

	void installEvents()
	{
		// common
		KBEngine.Event.registerOut("onKicked", this, "onKicked");
		KBEngine.Event.registerOut("onDisableConnect", this, "onDisableConnect");
		KBEngine.Event.registerOut("onConnectStatus", this, "onConnectStatus");
		
		// login
		KBEngine.Event.registerOut("onCreateAccountResult", this, "onCreateAccountResult");
		KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
		KBEngine.Event.registerOut("onVersionNotMatch", this, "onVersionNotMatch");
		KBEngine.Event.registerOut("onScriptVersionNotMatch", this, "onScriptVersionNotMatch");
		KBEngine.Event.registerOut("onLoginBaseappFailed", this, "onLoginBaseappFailed");
		KBEngine.Event.registerOut("onLoginSuccessfully", this, "onLoginSuccessfully");
		KBEngine.Event.registerOut("onLoginBaseapp", this, "onLoginBaseapp");
		KBEngine.Event.registerOut("Loginapp_importClientMessages", this, "Loginapp_importClientMessages");
		KBEngine.Event.registerOut("Baseapp_importClientMessages", this, "Baseapp_importClientMessages");
		KBEngine.Event.registerOut("Baseapp_importClientEntityDef", this, "Baseapp_importClientEntityDef");
		
		// select-avatars
		KBEngine.Event.registerOut("onReqAvatarList", this, "onReqAvatarList");
		KBEngine.Event.registerOut("onCreateAvatarResult", this, "onCreateAvatarResult");
		KBEngine.Event.registerOut("onRemoveAvatar", this, "onRemoveAvatar");
	}

	void OnDestroy()
	{
		KBEngine.Event.deregisterOut(this);
	}
	
	void onSelAvatarUI()
	{
		if (startCreateAvatar == false && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 40, 200, 30), "RemoveAvatar(删除角色)"))    
        {
			if(sNetworkInterface.instance.selAvatarDBID == 0)
			{
				err("Please select a Avatar!(请选择角色!)");
			}
			else
			{
				info("Please wait...(请稍后...)");
				
				if(ui_avatarList != null && ui_avatarList.Count > 0)
				{
					//Dictionary<string, object> avatarinfo = ui_avatarList[selAvatarDBID];
					KBEngine.Event.fireIn("reqRemoveAvatar", ui_avatarList[sNetworkInterface.instance.selAvatarDBID]);
				}
			}
        }

		if (startCreateAvatar == false && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 75, 200, 30), "CreateAvatar(创建角色)"))    
		{
			startCreateAvatar = !startCreateAvatar;
		}

        if (startCreateAvatar == false && GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 110, 200, 30), "EnterGame(进入游戏)"))    
        {
        	if(sNetworkInterface.instance.selAvatarDBID == 0)
        	{
        		err("Please select a Avatar!(请选择角色!)");
        	}
        	else
        	{
        		info("Please wait...(请稍后...)");
        		
				KBEngine.Event.fireIn("selectAvatarGame", sNetworkInterface.instance.selAvatarDBID);
				//Application.LoadLevel("world");
				ui_state = 2;
			}
        }
		
		if(startCreateAvatar)
		{
	        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 40, 200, 30), "CreateAvatar-OK(创建完成)"))    
	        {
	        	if(stringAvatarName.Length > 1)
	        	{
		        	startCreateAvatar = !startCreateAvatar;
					KBEngine.Event.fireIn("reqCreateAvatar", (Byte)1, stringAvatarName);
				}
				else
				{
					err("avatar name is null(角色名称为空)!");
				}
	        }
	        
	        stringAvatarName = GUI.TextField(new Rect(Screen.width / 2 - 100, Screen.height - 75, 200, 30), stringAvatarName, 20);
		}
		
		if(ui_avatarList != null && ui_avatarList.Count > 0)
		{
			int idx = 0;
			foreach(UInt64 dbid in ui_avatarList.Keys)
			{
				//Dictionary<string, object> info = ui_avatarList[dbid];
			//	Byte roleType = (Byte)info["roleType"];
				string name = ui_avatarList[dbid].name;
			//	UInt16 level = (UInt16)info["level"];
				UInt64 idbid = ui_avatarList[dbid].dbid;

                idx++;
				
				Color color = GUI.contentColor;
				if(sNetworkInterface.instance.selAvatarDBID == idbid)
				{
					GUI.contentColor = Color.red;
				}
				
				if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 120 - 35 * idx, 200, 30), name))    
				{
					Debug.Log("selAvatar:" + name);
                    sNetworkInterface.instance.selAvatarDBID = idbid;
				}
				
				GUI.contentColor = color;
			}
		}
		else
		{
			if(KBEngineApp.app.entity_type == "Account")
			{
				KBEngine.Account account = (KBEngine.Account)KBEngineApp.app.player();
                if (account != null)
                    ui_avatarList = new Dictionary<UInt64, sAvatarList>();
                foreach(UInt64 dbid in account.avatars.Keys)
                {
                    ui_avatarList.Add(dbid, new sAvatarList(dbid, (string)account.avatars[dbid]["name"]));
                }
                
			}
		}
	}
	
	void onLoginUI()
	{
		if(GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 30, 200, 30), "Login(登陆)"))  
        {  
        	Debug.Log("stringAccount:" + stringAccount);
        	Debug.Log("stringPasswd:" + stringPasswd);
        	
			if(stringAccount.Length > 0 && stringPasswd.Length > 5)
			{
				login();
			}
			else
			{
				err("account or password is error, length < 6!(账号或者密码错误，长度必须大于5!)");
			}
        }

        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 + 70, 200, 30), "CreateAccount(注册账号)"))  
        {  
			Debug.Log("stringAccount:" + stringAccount);
			Debug.Log("stringPasswd:" + stringPasswd);

			if(stringAccount.Length > 0 && stringPasswd.Length > 5)
			{
				createAccount();
			}
			else
			{
				err("account or password is error, length < 6!(账号或者密码错误，长度必须大于5!)");
			}
        }
        
		stringAccount = GUI.TextField(new Rect (Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 30), stringAccount, 20);
		stringPasswd = GUI.PasswordField(new Rect (Screen.width / 2 - 100, Screen.height / 2 - 10, 200, 30), stringPasswd, '*');
	}

    /*void onWorldUI()
	{
		if(showReliveGUI)
		{
			if(GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 30), "Relive(复活)"))  
			{
				KBEngine.Event.fireIn("relive", (Byte)1);		        	
			}
		}
		
		UnityEngine.GameObject obj = UnityEngine.GameObject.Find("player(Clone)");
		if(obj != null)
		{
			GUI.Label(new Rect((Screen.width / 2) - 100, 20, 400, 100), "position=" + obj.transform.position.ToString()); 
		}
	}*/

    //建议在发布之前，这个函数完整的屏蔽掉
    void OnGUI()
    {
        /*if (ui_state == 1)
        {
            onSelAvatarUI();
        }
        else if (ui_state == 2)
        {
            //onWorldUI();
        }
        else
        {
            onLoginUI();
        }*/
        if (isDebug)
        {
            if (KBEngineApp.app != null && KBEngineApp.app.serverVersion != ""
                && KBEngineApp.app.serverVersion != KBEngineApp.app.clientVersion)
            {
                labelColor = Color.red;
                labelMsg = "version not match(curr=" + KBEngineApp.app.clientVersion + ", srv=" + KBEngineApp.app.serverVersion + " )(版本不匹配)";
            }
            else if (KBEngineApp.app != null && KBEngineApp.app.serverScriptVersion != ""
                && KBEngineApp.app.serverScriptVersion != KBEngineApp.app.clientScriptVersion)
            {
                labelColor = Color.red;
                labelMsg = "scriptVersion not match(curr=" + KBEngineApp.app.clientScriptVersion + ", srv=" + KBEngineApp.app.serverScriptVersion + " )(脚本版本不匹配)";
            }

            GUI.contentColor = labelColor;
            GUI.Label(new Rect((Screen.width / 2) - 100, 40, 400, 100), labelMsg);

            GUI.Label(new Rect(0, 5, 400, 100), "client version: " + KBEngine.KBEngineApp.app.clientVersion);
            GUI.Label(new Rect(0, 20, 400, 100), "client script version: " + KBEngine.KBEngineApp.app.clientScriptVersion);
            GUI.Label(new Rect(0, 35, 400, 100), "server version: " + KBEngine.KBEngineApp.app.serverVersion);
            GUI.Label(new Rect(0, 50, 400, 100), "server script version: " + KBEngine.KBEngineApp.app.serverScriptVersion);
        }
	}  
	
	public void err(string s)
	{
		labelColor = Color.red;
		labelMsg = s;
	}
	
	public void info(string s)
	{
		labelColor = Color.green;
		labelMsg = s;
	}
	
	public void login()
	{
		info("connect to server...(连接到服务端...)");
		KBEngine.Event.fireIn("login", stringAccount, stringPasswd, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
	}
	
	public void createAccount()
	{
		info("connect to server...(连接到服务端...)");
		
		KBEngine.Event.fireIn("createAccount", stringAccount, stringPasswd, System.Text.Encoding.UTF8.GetBytes("kbengine_unity3d_demo"));
	}
	
	public void onCreateAccountResult(UInt16 retcode, byte[] datas)
	{
		if(retcode != 0)
		{
			err("createAccount is error(注册账号错误)! err=" + KBEngineApp.app.serverErr(retcode));
			return;
		}
		
		if(KBEngineApp.validEmail(stringAccount))
		{
			info("createAccount is successfully, Please activate your Email!(注册账号成功，请激活Email!)");
		}
		else
		{
			info("createAccount is successfully!(注册账号成功!)");
		}
	}
	
	public void onConnectStatus(bool success)
	{
		if(!success)
			err("connect(" + KBEngineApp.app.getInitArgs().ip + ":" + KBEngineApp.app.getInitArgs().port + ") is error! (连接错误)");
		else
			info("connect successfully, please wait...(连接成功，请等候...)");
	}
	
	public void onLoginFailed(UInt16 failedcode)
	{
		if(failedcode == 20)
		{
			err("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode) + ", " + System.Text.Encoding.ASCII.GetString(KBEngineApp.app.serverdatas()));
		}
		else
		{
			err("login is failed(登陆失败), err=" + KBEngineApp.app.serverErr(failedcode));
		}
	}
	
	public void onVersionNotMatch(string verInfo, string serVerInfo)
	{
		err("");
	}

	public void onScriptVersionNotMatch(string verInfo, string serVerInfo)
	{
		err("");
	}
	
	public void onLoginBaseappFailed(UInt16 failedcode)
	{
		err("loginBaseapp is failed(登陆网关失败), err=" + KBEngineApp.app.serverErr(failedcode));
	}
	
	public void onLoginBaseapp()
	{
		info("connect to loginBaseapp, please wait...(连接到网关， 请稍后...)");
	}

	public void onLoginSuccessfully(UInt64 rndUUID, Int32 eid, Account accountEntity)
	{
		info("login is successfully!(登陆成功!)");
		ui_state = 1;
        
        //Application.LoadLevel("selavatars");
    }

	public void onKicked(UInt16 failedcode)
	{
		err("kick, disconnect!, reason=" + KBEngineApp.app.serverErr(failedcode));
		//Application.LoadLevel("login");
		ui_state = 0;
	}

	public void Loginapp_importClientMessages()
	{
		info("Loginapp_importClientMessages ...");
	}

	public void Baseapp_importClientMessages()
	{
		info("Baseapp_importClientMessages ...");
	}
	
	public void Baseapp_importClientEntityDef()
	{
		info("importClientEntityDef ...");
	}
	
	public void onReqAvatarList(Dictionary<UInt64, Dictionary<string, object>> avatarList)
	{
        info("on req avatar list");
        if( ui_avatarList == null )
            ui_avatarList = new Dictionary<UInt64, sAvatarList>();
        ui_avatarList.Clear();
        foreach (UInt64 dbid in avatarList.Keys)
        {
            ui_avatarList.Add(dbid, new sAvatarList(dbid, (string)avatarList[dbid]["name"]));
        }

        callAndRemoveLua("login");
    }

    public Dictionary<UInt64, sAvatarList> getAvatarList()
    {
        return ui_avatarList;
    }

    public void onCreateAvatarResult(Byte retcode, object info, Dictionary<UInt64, Dictionary<string, object>> avatarList)
	{
		if(retcode != 0)
		{
			err("Error creating avatar, errcode=" + retcode);
			return;
		}
		
		onReqAvatarList(avatarList);
	}
	
	public void onRemoveAvatar(UInt64 dbid, Dictionary<UInt64, Dictionary<string, object>> avatarList)
	{
		if(dbid == 0)
		{
			err("Delete the avatar error!(删除角色错误!)");
			return;
		}
		
		onReqAvatarList(avatarList);
	}
	
	public void onDisableConnect()
	{
	}
}

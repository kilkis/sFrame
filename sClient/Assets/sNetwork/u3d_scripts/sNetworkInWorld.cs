using KBEngine;
using UnityEngine;
using System;
using sFramework;
using sFramework.LoadBin;

public class sNetworkInWorld : MonoBehaviour 
{
	private UnityEngine.GameObject terrain = null;
	public UnityEngine.GameObject terrainPerfab;
	
	private UnityEngine.GameObject player = null;
	public UnityEngine.GameObject entityPerfab;
	public UnityEngine.GameObject avatarPerfab;
	
	void Awake() 
	{
		//DontDestroyOnLoad(transform.gameObject);
	}
	 
	// Use this for initialization
	void Start () 
	{
		installEvents();
	}

	void installEvents()
	{
        KBEngine.Event.registerOut("onConnectStatus", this, "onConnectStatus");
        // in world
        KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
		KBEngine.Event.registerOut("onAvatarEnterWorld", this, "onAvatarEnterWorld");
		KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
		KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
		KBEngine.Event.registerOut("set_position", this, "set_position");
		KBEngine.Event.registerOut("set_direction", this, "set_direction");
		KBEngine.Event.registerOut("updatePosition", this, "updatePosition");
		KBEngine.Event.registerOut("onControlled", this, "onControlled");
		KBEngine.Event.registerOut("set_HP", this, "set_HP");
		KBEngine.Event.registerOut("set_MP", this, "set_MP");
		KBEngine.Event.registerOut("set_HP_Max", this, "set_HP_Max");
		KBEngine.Event.registerOut("set_MP_Max", this, "set_MP_Max");
		KBEngine.Event.registerOut("set_level", this, "set_level");
		KBEngine.Event.registerOut("set_name", this, "set_entityName");
        KBEngine.Event.registerOut("set_strength", this, "set_strength");
        KBEngine.Event.registerOut("set_state", this, "set_state");
		KBEngine.Event.registerOut("set_moveSpeed", this, "set_moveSpeed");
		KBEngine.Event.registerOut("set_modelScale", this, "set_modelScale");
		KBEngine.Event.registerOut("set_modelID", this, "set_modelID");
		KBEngine.Event.registerOut("recvDamage", this, "recvDamage");
		KBEngine.Event.registerOut("otherAvatarOnJump", this, "otherAvatarOnJump");
		KBEngine.Event.registerOut("onAddSkill", this, "onAddSkill");

        KBEngine.Event.registerOut("onSetSpaceData", this, "onSetSpaceData");
        KBEngine.Event.registerOut("onEnterSpace", this, "onEnterSpace");
    }

    public void onConnectStatus(bool success)
    {
        if (!success)
            Debug.LogError("base connect(" + KBEngineApp.app.getInitArgs().ip + ":" + KBEngineApp.app.getInitArgs().port + ") is error! (连接错误)");
        else
            Debug.Log("base connect successfully, please wait...(连接成功，请等候...)");
    }


    void OnDestroy()
	{
		KBEngine.Event.deregisterOut(this);
	}
	
	// Update is called once per frame
	void Update () 
	{
		createPlayer();
        /*if (Input.GetKeyUp(KeyCode.Space))
        {
			Debug.Log("KeyCode.Space");
			KBEngine.Event.fireIn("jump");
        }
		else if (Input.GetMouseButton (0))     
		{   
			// 射线选择，攻击
			if(Camera.main)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   
				RaycastHit hit;   

				if (Physics.Raycast(ray, out hit))     
				{   
					Debug.DrawLine (ray.origin, hit.point); 
					UnityEngine.GameObject gameObj = hit.collider.gameObject;
					if(gameObj.name.IndexOf("terrain") == -1)
					{
						string[] s = gameObj.name.Split(new char[]{'_' });
						
						if(s.Length > 0)
						{
							int targetEntityID = Convert.ToInt32(s[s.Length - 1]);
							KBEngine.Event.fireIn("useTargetSkill", (Int32)1, (Int32)targetEntityID);	
						}	
					}
				}  
			}
		} */   
	}
	
    public void onSetSpaceData(UInt32 spaceID, string key, string value)
    {
        if( sGameFlow.instance.getCurState() == sGameState.Create )
        {
            Debug.Log("xxxxxxxxxxxxxxxxxx");
            //去除选择界面的相关内容，前提是这个流程在正常顺序内只进入一次
            sNetworkOutOfWorld.inst.callAndRemoveLua("selAvatar", new object[]{});
        }
        //这里会获得当前角色所在的地图的信息，主要只使用spaceID
        Debug.Log("onSetSpaceData:" + spaceID + "," + key + "," + value);
        sMapManager.GetInstance().loadMap((int)spaceID);
    }

    public void onEnterSpace(KBEngine.Entity entity)
    {
        //通常这个函数不处理
        Debug.Log("onEnterSpace:" + entity.id);
    }

    public void addSpaceGeometryMapping(string respath)
	{
        return;
        //这个函数是服务器加载地图的情况，服务器几张图，客户端会收到多少，所以不能在这里加载地形
        //Debug.Log("loading scene(" + respath + ")...");
        //sNetworkOutOfWorld.inst.info("scene(" + respath + "), spaceID=" + KBEngineApp.app.spaceID);
		//if(terrain == null)
        //	terrain = Instantiate(terrainPerfab) as UnityEngine.GameObject;

		//if(player)
			//player.GetComponent<GameEntity>().entityEnable();

    }	
	
	public void onAvatarEnterWorld(UInt64 rndUUID, Int32 eid, KBEngine.Avatar avatar)
	{
        Debug.Log("onAvatarEnterWorld");
		if(!avatar.isPlayer())
		{
			return;
		}

        sNetworkOutOfWorld.inst.info("loading scene...(加载场景中...)");
		Debug.Log("loading scene...");
	}

	public void createPlayer()
	{
        //if (player != null)
        //	return;
        if (sEntityManager.GetInstance().isSelfCreated())
            return;

		if (KBEngineApp.app.entity_type != "Avatar") {
			return;
		}

		KBEngine.Avatar avatar = (KBEngine.Avatar)KBEngineApp.app.player();
		if(avatar == null)
		{
			Debug.Log("wait create(palyer)!");
			return;
		}

        //float y = avatar.position.y;
        //if(avatar.isOnGround)
        //	y = 1.3f;
        Debug.Log("server pos:"+avatar.position);
        Vector3 startpos = new Vector3(0.0f, 0.0f, 0.0f);
        sEntityManager.GetInstance().createSelf(avatar.id, avatar.position);
        //player = Instantiate(avatarPerfab, new Vector3(avatar.position.x, y, avatar.position.z), 
		  //                   Quaternion.Euler(new Vector3(avatar.direction.y, avatar.direction.z, avatar.direction.x))) as UnityEngine.GameObject;

		//player.GetComponent<GameEntity>().entityDisable();
		//avatar.renderObj = player;
		//((UnityEngine.GameObject)avatar.renderObj).GetComponent<GameEntity>().isPlayer = true;
	}

	public void onAddSkill(KBEngine.Entity entity)
	{
		Debug.Log("onAddSkill");
	}
	
	public void onEnterWorld(KBEngine.Entity entity)
	{
        Debug.Log("onEnterWorld");
        if (entity.isPlayer())
			return;
		Debug.Log("entity id:" + entity.className+" - "+entity.id);
		//Debug.Log ();
        //float y = entity.position.y;
        //if(entity.isOnGround)
        //y = 1.3f;

        //entity.renderObj = Instantiate(entityPerfab, new Vector3(entity.position.x, y, entity.position.z), 
        //Quaternion.Euler(new Vector3(entity.direction.y, entity.direction.z, entity.direction.x))) as UnityEngine.GameObject;

        //((UnityEngine.GameObject)entity.renderObj).name = entity.className + "_" + entity.id;
		switch (entity.className) {
		case "Avatar":
			sEntityManager.GetInstance ().pushEntity (entity.id, entity.position, entity.className);
			break;
		case "Monster":
		case "NPC":
			{
				int tid = int.Parse(entity.getDefinedProperty ("uid").ToString());
				data_entity tmp = null;
				if (sLoadBin_entity.instance.data.TryGetValue (tid, out tmp)) {
					sEntityManager.GetInstance ().pushEntity (entity.id, entity.position, tmp.ModelName, entity.className);
				}
			}
			break;
		}
			
		
	}
	
	public void onLeaveWorld(KBEngine.Entity entity)
	{
        Debug.LogError("onLeaveWorld");
        //if(entity.renderObj == null)
        //	return;

        //UnityEngine.GameObject.Destroy((UnityEngine.GameObject)entity.renderObj);
        //entity.renderObj = null;
        sEntityManager.GetInstance().popPlayer(entity.id);
    }

	public void set_position(KBEngine.Entity entity)
	{
        sEntityManager.GetInstance().setPosition(entity.id, entity.position);
        
		//if(entity.renderObj == null)
			//return;

		//((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().destPosition = entity.position;
		//((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().position = entity.position;
	}

	public void updatePosition(KBEngine.Entity entity)
	{
        sEntityManager.GetInstance().moveToPosition(entity.id, entity.position);
        
        //if (entity.renderObj == null)
			//return;
		
		//GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		//gameEntity.destPosition = entity.position;
		//gameEntity.isOnGround = entity.isOnGround;
	}
	
	public void onControlled(KBEngine.Entity entity, bool isControlled)
	{
		if(entity.renderObj == null)
			return;
		
		GameEntity gameEntity = ((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>();
		gameEntity.isControlled = isControlled;
	}
	
	public void set_direction(KBEngine.Entity entity)
	{
		if(entity.renderObj == null)
			return;
		
		((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().destDirection = 
			new Vector3(entity.direction.y, entity.direction.z, entity.direction.x); 
	}

	public void set_HP(KBEngine.Entity entity, object v)
	{
		sEntityManager.GetInstance ().setAttr (entity.id, sEntityAttrType.Hp, v);
	}
	
	public void set_MP(KBEngine.Entity entity, object v)
	{
		sEntityManager.GetInstance ().setAttr (entity.id, sEntityAttrType.Mp, v);
    }
	
	public void set_HP_Max(KBEngine.Entity entity, object v)
	{
		sEntityManager.GetInstance ().setAttr (entity.id, sEntityAttrType.HpMax, v);
	}
	
	public void set_MP_Max(KBEngine.Entity entity, object v)
	{
		sEntityManager.GetInstance ().setAttr (entity.id, sEntityAttrType.MpMax, v);
    }
	
	public void set_level(KBEngine.Entity entity, object v)
	{
		sEntityManager.GetInstance ().setAttr (entity.id, sEntityAttrType.Lvl, v);
    }

    public void set_strength(KBEngine.Entity entity, object v)
    {
		sEntityManager.GetInstance ().setAttr (entity.id, sEntityAttrType.Strength, v);
    }
    
    public void set_entityName(KBEngine.Entity entity, object v)
	{
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().entity_name = (string)v;
		}
	}
	
	public void set_state(KBEngine.Entity entity, object v)
	{
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().set_state((SByte)v);
		}
		
		if(entity.isPlayer())
		{
			Debug.Log("player->set_state: " + v);
			
			/*if(((SByte)v) == 1)
                sNetworkOutOfWorld.inst.showReliveGUI = true;
			else
                sNetworkOutOfWorld.inst.showReliveGUI = false;
			*/
			return;
		}
	}

	public void set_moveSpeed(KBEngine.Entity entity, object v)
	{
		float fspeed = ((float)(Byte)v) / 10f;
		
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().speed = fspeed;
		}
	}
	
	public void set_modelScale(KBEngine.Entity entity, object v)
	{
	}
	
	public void set_modelID(KBEngine.Entity entity, object v)
	{
	}
	
	public void recvDamage(KBEngine.Entity entity, KBEngine.Entity attacker, Int32 skillID, Int32 damageType, Int32 damage)
	{
	}
	
	public void otherAvatarOnJump(KBEngine.Entity entity)
	{
		Debug.Log("otherAvatarOnJump: " + entity.id);
		if(entity.renderObj != null)
		{
			((UnityEngine.GameObject)entity.renderObj).GetComponent<GameEntity>().OnJump();
		}
	}
}

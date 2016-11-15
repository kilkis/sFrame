using UnityEngine;
using System.Collections;

public class sPlayerModel : MonoBehaviour {

    public long playerUID = -1;

    public string bone = "";
    public string chestName = "";
    public string footName = "";
    public string handName = "";
    public string headName = "";
    public string legName = "";

    public GameObject playerCC;

    Animation anim;
    sPlayerControl pc;

	sCacheUnit _model;

	// Use this for initialization
	void Start () {
        
    }

	public void createModel(GameObject sp)
	{
		playerCC = sp;
		sLoadingGame.GetInstance ().loadWeak ("cube", loadModelOK, false);
	}
    //调用创建avatar的接口
    public void createAvatar(GameObject sp)
    {
        if( string.IsNullOrEmpty(bone) || string.IsNullOrEmpty(chestName) || string.IsNullOrEmpty(footName) ||
            string.IsNullOrEmpty(handName) || string.IsNullOrEmpty(headName) || string.IsNullOrEmpty(legName))
        {
            Debug.LogError("sth. of player is null");
            return;
        }
        playerCC = sp;
        sAvatarMgr.GetInstance().createPlayer(playerUID, bone, "", new string[] { chestName, footName, handName, headName, legName }, loadAvatarOK);
    }

    public void destroyAvatar()
    {
        sAvatarMgr.GetInstance().deletePlayer(playerUID);
    }

	public void destroyModel()
	{
		sLoadingGame.GetInstance ().unloadWeak (_model);
		_model = null;
	}

	void loadModelOK(sCacheUnit scu)
	{
		
		scu.obj.transform.parent = playerCC.transform;

		float height = playerCC.GetComponent<CapsuleCollider>().height;
		scu.obj.transform.localPosition = new Vector3(0, -height/2, 0);
		scu.obj.transform.localRotation = Quaternion.identity;
		anim = scu.obj.GetComponent<Animation> ();
		pc = playerCC.GetComponent<sPlayerControl>();
		pc.setAnim(anim);

		_model = scu;
	}
    
    //avatar创建成功
    void loadAvatarOK()
    {
        sAvatarData ac = sAvatarMgr.GetInstance().getPlayer(playerUID);
        ac.controller.Instance.transform.parent = playerCC.transform;
        float height = playerCC.GetComponent<CapsuleCollider>().height;
        ac.controller.Instance.transform.localPosition = new Vector3(0, -height/2, 0);
        ac.controller.Instance.transform.localRotation = Quaternion.identity;

        anim = ac.controller.Instance.GetComponent<Animation>();

        pc = playerCC.GetComponent<sPlayerControl>();
        pc.setAnim(anim);
        //pc.enableControl(true);
    }
}

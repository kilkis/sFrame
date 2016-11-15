using UnityEngine;
using KBEngine;
using System.Collections;
using System.Collections.Generic;

public enum playerState
{
    idle,
    run,
}

public class sEntityControl : MonoBehaviour {

    public NavMeshAgent agent;
    //确认是否可以控制
    bool _enableControl = false;

    int layer = 0;
    Animation modelAnim;
    playerState curState;
    Vector3 focuspos;

    List<Vector3> _curPath = new List<Vector3>();

    Transform _trans;
    
    // Use this for initialization
    void Start () {
        layer = 1 << LayerMask.NameToLayer(sConst.terrainLayer);
        curState = playerState.idle;

        _trans = gameObject.transform;
    }
	
    void FixedUpdate()
    {
        if( _enableControl )
        {
            if (KBEngineApp.app == null)
                return;
           
            KBEngine.Event.fireIn("updatePlayer", _trans.position.x,
                _trans.position.y, _trans.position.z, _trans.rotation.eulerAngles.y);

        }
    }

	public void setDirection(Vector3 dir)
	{
		if (dir == Vector3.zero)
			return;
		if (_trans != null)
			_trans.rotation = Quaternion.Lerp (_trans.rotation, Quaternion.LookRotation (dir), 0.1f * Time.deltaTime);
	}

    public void moveToPosition(Vector3 pos)
    {
        changeAnimState(playerState.idle);
        _curPath.Clear();
        _curPath.Add(pos);
        changeAnimState(playerState.run);
    }

    public void setPosition(Vector3 pos)
    {
        //被服务器强制设定的坐标，状态切换回待机
        _curPath.Clear();
        changeAnimState(playerState.idle);

        if (_trans != null)
            _trans.position = pos;
    }

	// Update is called once per frame
	void Update () {
	    if(_enableControl)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonUp(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitinfo;
                if (Physics.Raycast(ray, out hitinfo, 999, layer))
                {
                    focuspos = hitinfo.point;
                    //只使用navmesh得到路线，路线跑动自己处理逻辑
                    //agent.ResetPath();
                    
                    agent.SetDestination(hitinfo.point);
                    //Debug.Log("hit:" + hitinfo.point + ", "+transform.position);
                    _curPath.Clear();
                    for(int i = 0; i < agent.path.corners.Length; ++i )
                    {
                        _curPath.Add(agent.path.corners[i]);
                        //Debug.Log("point:" + agent.path.corners[i]);
                    }
                    //Debug.Log("path len:" + _curPath.Count);
                    agent.Stop();
                    
                    changeAnimState(playerState.run);
                }
            }

#else
#endif
            
        }

        //todo:需要加入服务器通知移动控制
        if (curState == playerState.run)
        {
            //自行处理移动
            Vector3 op = _curPath[0] - transform.position;
            op.y = 0;
            float dis = Vector3.Distance(op, Vector3.zero);
            float rundis = 3 * Time.deltaTime;
            op.Normalize();
            Debug.Log("dis: " + dis + " : " + rundis);
            if (dis <= rundis)
            {
                transform.position += dis * op;
                _curPath.RemoveAt(0);
                if (_curPath.Count == 0)
                {
                    changeAnimState(playerState.idle);
                }
            }
            else
            {
                transform.position += rundis * op;
            }

            if (op != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(op), 0.5f);
            }
        }

    }

    public void enableControl(bool b)
    {
        _enableControl = b;
    }

    public void setAnim(Animation anim)
    {
        modelAnim = anim;
        changeAnimState(playerState.idle);
    }

    void changeAnimState(playerState ps)
    {
        if (curState == ps)
            return;
        curState = ps;
        if( modelAnim != null )
        { 
            switch (ps)
            {
                case playerState.idle:
                    modelAnim.CrossFade("Stand");
                    break;
                case playerState.run:
                    modelAnim.CrossFade("SRun");
                    break;
            }

        }
    }
}

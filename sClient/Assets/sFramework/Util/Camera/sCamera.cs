using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class sCamera : MonoBehaviour {

    public static sCamera instance;
    private Transform _follower;

    //camera目标点与跟随者坐标差值
    public Vector3 focusPosOff = new Vector3(0, 1, 0);

    public Camera camera;
    private Transform cameraTrans;
    //离目标点的当前距离
    private float curScrollDis = 5.0f;
    //离目标点最近距离
    public float minScrollDis = 2.0f;
    //离目标点最远距离
    public float maxScrollDis = 10.0f;
    //远近行为是否是直线
    public bool isScrollLine = true;
    //远近操作速度
    public float scrollSpeed = 10f;

    private Vector3 mouseClickPos = Vector3.zero;
    //水平角度
    private float horizontalAngle = 0;
    //垂直角度
    private float verticalAngle = 45;

    private float nHorizontalAngle = 0;
    private float nVerticalAngle = 45;

    private Vector3 cameraCalPos = Vector3.zero;
    private float touchDis = 0;
    //camera shake
    private float shakeTime = 0f;
    private float fps = 20f;
    private float frameTime = 0f;
    private float shakeDelta = 0.005f;
    private bool isShake = false;
    //shake end

    //camera遮挡检测
    private float _cameraCheckInv = 0;
    private Vector3 _checkPos;
    int checkLayer = 0;
    GameObject _col;
    MeshRenderer _mr;
    //end

    void Awake()
    {
        instance = this;
    }
	// Use this for initialization
	void Start () {
        _checkPos = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        checkLayer = 1 << LayerMask.NameToLayer(sConst.cameraColLayer);
    }
    
    void Update()
    {
        _cameraCheckInv += Time.deltaTime;
        if( _cameraCheckInv >= sConst.cameraCheckInv )
        {
            _cameraCheckInv = 0;
            _checkCameraCol();
        }
    }
    //检测场景遮挡
    void _checkCameraCol()
    {
        Ray ray = Camera.main.ScreenPointToRay(_checkPos);
        RaycastHit hitinfo;
        if( Physics.Raycast(ray, out hitinfo, 99, checkLayer))
        {
            _col = hitinfo.collider.gameObject;
            Debug.Log("col:" + _col);
            _mr = _col.GetComponent<MeshRenderer>();
            //_mr.material
        }
    }

    void FixedUpdate () {
        if (_follower == null)
            return;
#if UNITY_EDITOR
        bool scrollChange = false;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            curScrollDis -= scrollSpeed*Time.deltaTime;
            if (curScrollDis < minScrollDis)
                curScrollDis = minScrollDis;
            scrollChange = true;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            curScrollDis += scrollSpeed * Time.deltaTime;
            if (curScrollDis > maxScrollDis)
                curScrollDis = maxScrollDis;
            scrollChange = true;
        }
        
        //点击后转镜头
        if( Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            mouseClickPos = Input.mousePosition;
            nHorizontalAngle = horizontalAngle;
            nVerticalAngle = verticalAngle;
        }
        if ( Input.GetMouseButton(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            Vector3 offpos = Input.mousePosition - mouseClickPos;
            nHorizontalAngle = horizontalAngle + offpos.x/10.0f;
            nVerticalAngle = verticalAngle - offpos.y/10.0f;
            if (nVerticalAngle > 70)
                nVerticalAngle = 70;
            else if (nVerticalAngle < 10)
                nVerticalAngle = 10;
            scrollChange = true;
            
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            mouseClickPos = Vector3.zero;
            horizontalAngle = nHorizontalAngle;
            verticalAngle = nVerticalAngle;
        }
        if (scrollChange)
        {
            cameraCalPos.y = curScrollDis * Mathf.Sin(nVerticalAngle * Mathf.PI / 180.0f);
            float tmp = curScrollDis * Mathf.Cos(nVerticalAngle * Mathf.PI / 180.0f);
            cameraCalPos.x = tmp * Mathf.Sin(nHorizontalAngle * Mathf.PI / 180.0f);
            cameraCalPos.z = tmp * Mathf.Cos(nHorizontalAngle * Mathf.PI / 180.0f);
        }

        //考虑坐标跟随不以直接设定的方式处理，而是smoothlamp做渐变
        cameraTrans.position = _follower.position + focusPosOff + cameraCalPos;
        Vector3 dir = -focusPosOff - cameraCalPos;
        dir.Normalize();
        cameraTrans.rotation = Quaternion.LookRotation(dir);
#else
        bool scrollChange = false;
        if(Input.touchCount>1)  
        {
            if(Input.touches[0].phase == TouchPhase.Began && Input.touches[1].phase == TouchPhase.Began)
            {
                touchDis = (Input.touches[1].position-Input.touches[0].position).sqrMagnitude;
            }
            if(Input.touches[0].phase==TouchPhase.Moved || Input.touches[1].phase==TouchPhase.Moved)  
            {  
                Vector2 mDir=Input.touches[1].position-Input.touches[0].position;  
                //根据向量的大小判断当前手势是放大还是缩小  
                float dis = mDir.sqrMagnitude;
                if ( dis > touchDis )
                { 
                    curScrollDis -= scrollSpeed * Time.deltaTime;
                    if (curScrollDis < minScrollDis)
                        curScrollDis = minScrollDis;
                    scrollChange = true;
                }
                else if( dis < touchDis )
                {
                    curScrollDis += scrollSpeed * Time.deltaTime;
                    if (curScrollDis > maxScrollDis)
                        curScrollDis = maxScrollDis;
                    scrollChange = true;
                }
            }
        }
        if(Input.touchCount==1)  
        {
            //点击后转镜头
            if(Input.touches[0].phase==TouchPhase.Began)  
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return;
                }
                mouseClickPos = Input.mousePosition;
                nHorizontalAngle = horizontalAngle;
                nVerticalAngle = verticalAngle;
            }
            if(Input.touches[0].phase==TouchPhase.Moved)  
            {
                Vector3 offpos = Input.mousePosition - mouseClickPos;
                nHorizontalAngle = horizontalAngle + offpos.x/10.0f;
                nVerticalAngle = verticalAngle - offpos.y/10.0f;
                if (nVerticalAngle > 70)
                    nVerticalAngle = 70;
                else if (nVerticalAngle < 10)
                    nVerticalAngle = 10;
                scrollChange = true;
            
            }
            if(Input.touches[0].phase==TouchPhase.Ended)  
            {
                mouseClickPos = Vector3.zero;
                horizontalAngle = nHorizontalAngle;
                verticalAngle = nVerticalAngle;
            }
        }
        
        if (scrollChange)
        {
            cameraCalPos.y = curScrollDis * Mathf.Sin(nVerticalAngle * Mathf.PI / 180.0f);
            float tmp = curScrollDis * Mathf.Cos(nVerticalAngle * Mathf.PI / 180.0f);
            cameraCalPos.x = tmp * Mathf.Sin(nHorizontalAngle * Mathf.PI / 180.0f);
            cameraCalPos.z = tmp * Mathf.Cos(nHorizontalAngle * Mathf.PI / 180.0f);
        }
        
        //考虑坐标跟随不以直接设定的方式处理，而是smoothlamp做渐变
        cameraTrans.position = _follower.position + focusPosOff + cameraCalPos;
        Vector3 dir = -focusPosOff - cameraCalPos;
        dir.Normalize();
        cameraTrans.rotation = Quaternion.LookRotation(dir);
#endif

        if ( isShake )
        {
            if( shakeTime > 0 )
            {
                shakeTime -= Time.deltaTime;
                if( shakeTime <= 0 )
                {
                    camera.rect = new Rect(0, 0, 1, 1);
                    isShake = false;
                }
                else
                {
                    frameTime += Time.deltaTime;
                    if( frameTime > 1.0f / fps )
                    {
                        frameTime = 0;
                        camera.rect = new Rect(shakeDelta * (-1.0f + 2.0f * Random.value), shakeDelta * (-1.0f + 2.0f * Random.value), 1.0f, 1.0f);
                    }
                }
            }
        }
    }
    public void shake()
    {
        if (isShake)
            return;
        shakeTime = 0.5f;
        fps = 20f;
        frameTime = 0.03f;
        shakeDelta = 0.005f;
        isShake = true;
    }
    public void setFollower(Transform trans)
    {
        _follower = trans;
        initCamera();
    }

    public void clearFollower()
    {
        _follower = null;
    }

    void initCamera()
    {
        if (camera == null)
        {
            Debug.LogError("wrong!!! there is no camera!");
            return;
        }
        cameraTrans = camera.transform;
        //获得角色方向，用来和他统一方向
        Vector3 dir = _follower.forward;
        cameraCalPos.y = curScrollDis * Mathf.Sin(verticalAngle*Mathf.PI/180.0f);
        float tmp = curScrollDis * Mathf.Cos(verticalAngle * Mathf.PI / 180.0f);
        cameraCalPos.x = tmp * Mathf.Sin(horizontalAngle * Mathf.PI / 180.0f);
        cameraCalPos.z = tmp * Mathf.Cos(horizontalAngle * Mathf.PI / 180.0f);
        Debug.Log("cameracal:" + cameraCalPos);
    }
}

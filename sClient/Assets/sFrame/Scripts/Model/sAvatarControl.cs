using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class sAvatarControl 
{
    public GameObject Instance = null;
    public GameObject WeaponInstance = null;

    public string skeleton;
    public string[] equipments;
    public string weapon;

    private Animation animationController = null;

    /// <summary>
    /// 异步读取的数量
    /// </summary>
    private int loadbonenum = 0;
    private int loadequipnum = 0;
    private int loadweaponnum = 0;

    private bool isLoadWithoutWeaponOK = false;

    public bool allLoadOk = false;

    SkinnedMeshRenderer[] meshes;
    GameObject[] objects;

    loadAvatarCallback _cb;

    public sAvatarControl(string skeleton, string[] equips, string weapon, bool combine, loadAvatarCallback cb)
    {
        isLoadWithoutWeaponOK = false;
        loadbonenum = 0;
        loadequipnum = 0;
        loadweaponnum = 0;
        _cb = cb;
        //读取骨骼

        this.skeleton = skeleton;
        this.weapon = weapon;
        //读取装备
        this.equipments = new string[equips.Length];
        for (int i = 0; i < equips.Length; ++i )
        {
            this.equipments[i] = equips[i];
        }
        
        
    }

    public void startLoad()
    {
        sLoadingGame.GetInstance().loadAvatar(this.skeleton, _loadBoneCallback);

        // Create weapon
        sLoadingGame.GetInstance().loadAvatar(this.weapon, _loadWeaponCallback);
    }

    public void _loadBoneCallback(UnityEngine.Object obj)
    {
        this.Instance = GameObject.Instantiate(obj) as GameObject;

        //++loadbonenum;

        // Create and collect other parts SkinnedMeshRednerer
        meshes = new SkinnedMeshRenderer[this.equipments.Length];
        objects = new GameObject[this.equipments.Length];
        for (int i = 0; i < this.equipments.Length; i++)
        {
            sLoadingGame.GetInstance().loadAvatar(this.equipments[i], _loadEquipCallback, true);
        }
    }
    public void _loadEquipCallback(UnityEngine.Object obj)
    {
        objects[loadequipnum] = GameObject.Instantiate(obj) as GameObject;
        meshes[loadequipnum] = objects[loadequipnum].GetComponentInChildren<SkinnedMeshRenderer>();
        ++loadequipnum;
        if( loadequipnum == this.equipments.Length )
        {
            _allEquipLoadOK();
        }
    }

    public void _loadWeaponCallback(UnityEngine.Object obj)
    {
        WeaponInstance = GameObject.Instantiate(obj) as GameObject;
        ++loadweaponnum;
        if (isLoadWithoutWeaponOK)
        {
            _weaponLoadOK();
        }
    }

    private void _allEquipLoadOK()
    {
        //Debug.Log("_allEquipLoadOK");
        // Combine meshes
        sCombineSkinnedMesh.CombineObject(Instance, meshes, true);
        
        // Only for display
        animationController = Instance.GetComponent<Animation>();

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject.DestroyImmediate(objects[i].gameObject);
        }
        objects = null;
        loadequipnum = 0;

        sLoadAssetbundle.GetInstance().unloadAvatarTemp();
        meshes = null;
        if ( loadweaponnum > 0 )
        {
            _weaponLoadOK();
        }
        if (_cb != null)
            _cb();
        allLoadOk = true;
    }

    private void _weaponLoadOK()
    {
        Debug.Log("_weaponLoadOK");

        Transform[] transforms = Instance.GetComponentsInChildren<Transform>();
        foreach (Transform joint in transforms)
        {
            if (joint.name == "weapon_hand_r")
            {// find the joint (need the support of art designer)
                WeaponInstance.transform.parent = joint.gameObject.transform;
                break;
            }
        }

        // Init weapon relative informations
        WeaponInstance.transform.localScale = Vector3.one;
        WeaponInstance.transform.localPosition = Vector3.zero;
        WeaponInstance.transform.localRotation = Quaternion.identity;

        loadweaponnum = 0;
    }

    public void ChangeHeadEquipment(string equipment, bool combine = false)
    {
        ChangeEquipment(0, equipment, combine);
    }

    public void ChangeChestEquipment(string equipment, bool combine = false)
    {
        ChangeEquipment(1, equipment, combine);
    }

    public void ChangeHandEquipment(string equipment, bool combine = false)
    {
        ChangeEquipment(2, equipment, combine);
    }

    public void ChangeFeetEquipment(string equipment, bool combine = false)
    {
        ChangeEquipment(3, equipment, combine);
    }

    public void ChangeWeapon(string weapon)
    {
        Object res = Resources.Load("Prefab/" + weapon);
        GameObject oldWeapon = WeaponInstance;
        WeaponInstance = GameObject.Instantiate(res) as GameObject;
        WeaponInstance.transform.parent = oldWeapon.transform.parent;
        WeaponInstance.transform.localPosition = Vector3.zero;
        WeaponInstance.transform.localScale = Vector3.one;
        WeaponInstance.transform.localRotation = Quaternion.identity;

        GameObject.Destroy(oldWeapon);
    }

    public void ChangeEquipment(int index, string equipment, bool combine = false)
    {
        this.equipments[index] = equipment;
        
        Object res = null;
        SkinnedMeshRenderer[] meshes = new SkinnedMeshRenderer[4];
        GameObject[] objects = new GameObject[this.equipments.Length];
        for (int i = 0; i < this.equipments.Length; i++)
        {

            res = Resources.Load("Prefab/" + this.equipments[i]);
            objects[i] = GameObject.Instantiate(res) as GameObject;
            meshes[i] = objects[i].GetComponentInChildren<SkinnedMeshRenderer>();
        }

        sCombineSkinnedMesh.CombineObject(Instance, meshes, combine);

        for (int i = 0; i < objects.Length; i++)
        {

            GameObject.DestroyImmediate(objects[i].gameObject);
        }
    }
}

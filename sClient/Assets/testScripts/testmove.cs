using UnityEngine;
using System.Collections;

public class testmove : MonoBehaviour {
    public NavMeshAgent agent;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        return;
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitinfo;
            if (Physics.Raycast(ray, out hitinfo, 999))
            {
                agent.SetDestination(hitinfo.point);
            }
        }
	}
}

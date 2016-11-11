using UnityEngine;
using System.Collections;

public class sEditor_Monster : MonoBehaviour {

	public int monsterID;
	public float viewRange = 5.0f;
	public float backRange = 10.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawLine (transform.position, transform.position + 2.0f * transform.forward);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, viewRange);

		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, backRange);
	}
}

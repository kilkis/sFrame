using UnityEngine;
using System.Collections;

public class sFps : MonoBehaviour {
    public float f_UpdateInterval = 0.5F;

    private float f_LastInterval;

    private int i_Frames = 0;

    private float f_Fps;
    // Use this for initialization
    void Start () {
        f_LastInterval = Time.realtimeSinceStartup;

        i_Frames = 0;
    }
	
	// Update is called once per frame
	void Update () {
        // f_Fps.ToString("f2")
        ++i_Frames;

        if (Time.realtimeSinceStartup > f_LastInterval + f_UpdateInterval)
        {
            f_Fps = i_Frames / (Time.realtimeSinceStartup - f_LastInterval);

            i_Frames = 0;

            f_LastInterval = Time.realtimeSinceStartup;
        }
    }
}

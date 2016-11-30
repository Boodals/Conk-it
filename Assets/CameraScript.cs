using UnityEngine;
using System.Collections;
using System;
//using UnityStandardAssets;

public class CameraScript : MonoBehaviour {

    Vector3 standardPos;
    public static CameraScript singleton;

    //public UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration chromic;

    // Use this for initialization
    void Start () {
        singleton = this;
        standardPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, standardPos, 8 * Time.deltaTime);
       // chromic.chromaticAberration = Mathf.Lerp(chromic.chromicAberration, 1, 10 * Time.deltaTime);
    }

    public void Hit(Vector3 dir, float intensity)
    {
        transform.position += dir * intensity;
    }

    public void Goal()
    {
       // chromic.chromaticAberration = 10;
    }
}

using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets;

public class CameraScript : MonoBehaviour {

    Vector3 standardPos;
    public static CameraScript singleton;

    public UnityStandardAssets.ImageEffects.VignetteAndChromaticAberration chromic;

    Vector3 targetPoint;
    float moveTimer = 0;

    // Use this for initialization
    void Start () {
        singleton = this;
        standardPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 curTarget = standardPos;

        if (moveTimer > 0)
            curTarget = targetPoint;

        transform.position = Vector3.Lerp(transform.position, curTarget, 13 * Time.deltaTime);

        moveTimer -= Time.deltaTime * 5;
        chromic.chromaticAberration = Mathf.Lerp(chromic.chromaticAberration, 1, 10 * Time.deltaTime);
    }

    public void Hit(Vector3 dir, float intensity)
    {
        targetPoint = standardPos + dir * intensity;
        moveTimer = 0.8f;
    }

    public void Goal(Vector3 ballPos)
    {
       //Debug.Break();
       chromic.chromaticAberration = 15;
       Hit(ballPos.normalized, 0.9f);
    }
}

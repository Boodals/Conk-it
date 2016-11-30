using UnityEngine;
using System.Collections;

public class GoalManagerScript : MonoBehaviour
{
    public int myID;

    public int myScore;

	// Use this for initialization
	void Start ()
    {
        myScore = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}


    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Ball"))
        {
            myScore++;
            HUDScript.singleton.UpdateScore(myScore, myID);
            collider.gameObject.GetComponent<Ball>().Kill();

            CameraScript.singleton.Goal();
        }
    }
}

using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

    public static SpawnManager singleton;

    public GameObject player1;
    public GameObject player2;
    public Transform player1RespawnLocation;
    public Transform player2RespawnLocation;

    public float respawnTimer;

	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {

	}

    void respawnMe(int playerID)
    {
        if (playerID == 1)
        {
            player1.transform.position = player1RespawnLocation.position;
            
        }
        else
        {  
            player2.transform.position = player2RespawnLocation.position;
        }
    }
}

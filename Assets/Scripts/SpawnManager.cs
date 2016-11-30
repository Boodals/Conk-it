using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour {

    Transform respawnLocation;
    GameObject objectToRespawn;

    public float respawnTimer;
    float currentTimer;

    bool respawning;

	// Use this for initialization
	void Start () {
        respawning = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (respawning == true)
        {        
            objectToRespawn.SetActive(false);
            objectToRespawn.transform.position = respawnLocation.position;

            currentTimer += Time.deltaTime;

            if (currentTimer >= respawnTimer)
            {
                objectToRespawn.SetActive(true);

                respawning = false;

                currentTimer = 0.0f;
            }          
        }
	}

    void respawnObject(GameObject respawningObject, Transform respawningLocation)
    {
        respawning = true;

        objectToRespawn = respawningObject;
        respawnLocation = respawningLocation;      
    }
}

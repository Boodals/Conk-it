using UnityEngine;
using System.Collections;

public class BallManager : MonoBehaviour
{
    public float        m_minSpawnSpeed;
    public float        m_maxSpawnSpeed;
    public GameObject   m_prefab;
    public int          m_numBalls;


    private GameObjectPool m_balls;

    public void Kill(GameObject ball)
    {
        m_balls.dissable(ball);
        //for now spawn a new object when one leaves
        Spawn();
    }
    void Awake()
    {
        m_balls = new GameObjectPool(m_prefab);
        m_balls.resize(m_numBalls, false);

        foreach (GameObject b in m_balls.asArray())
        {
            b.GetComponent<Ball>().m_manager = this;
        }
    }
    // Use this for initialization
    void Start ()
    {

        //spawn a ball
        Spawn();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    //add more?
	}

    public void Spawn()
    {
        GameObject ball = null;
        if (m_balls.getFree(out ball))
        {
            Vector2 direction   = Random.insideUnitCircle;
            Vector2 spawnOffset = new Vector2(0, 1);
            direction += spawnOffset;
            direction.Normalize();
            float   speed       = Random.Range(m_minSpawnSpeed, m_maxSpawnSpeed);

            ball.GetComponent<Ball>().Spawn(transform.position, direction* speed);
        }

    }
}

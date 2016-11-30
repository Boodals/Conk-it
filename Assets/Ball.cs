using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{

    public float m_velocityHitScale;

    private Rigidbody2D m_rb;


    public void Spawn(Vector3 position, Vector3 velocity)
    {
        transform.position = position;
        m_rb.velocity = velocity;
    }
    public void Hit(Vector3 playerPosition)
    {
        Vector2 returnVector = transform.position - playerPosition;
        returnVector.Normalize();

        m_rb.velocity = returnVector * m_velocityHitScale * m_rb.velocity.magnitude;
    }


    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
    }

	// Use this for initialization
	void Start ()
    {
	   
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}


}

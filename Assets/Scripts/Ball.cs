using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public float m_initialPauseDuration;
    public float m_pauseVelocityTimeScale;

    public float m_hitSpeed;
    public float m_maxSpeed;

    private Rigidbody2D m_rb;
    private float m_pauseEndTime;
    private bool m_paused;
    private bool m_prevPaused;

    float m_curPower;
    public bool PausedThisFrame()
    {
        return m_paused && !m_prevPaused;
    }

    public bool UnpausedThisFrame()
    {
        return !m_paused && m_prevPaused;
    }
    public bool Paused()
    {
        return m_paused;
    }
    public void Spawn(Vector3 position, Vector3 velocity)
    {
        transform.position = position;
        m_rb.velocity = velocity;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="playerPosition"></param>
    /// <param name="power">normalised power scale</param>
    public void Hit(Vector3 playerPosition, float power)
    {
        Vector2 returnVector = transform.position - playerPosition;
        returnVector.Normalize();

        //float velocityScale = m_velocityHitScale;
        //if (m_rb.velocity.magnitude >= m_speedCap)
        //    velocityScale = 1;

        //m_rb.velocity = returnVector * velocityScale * m_rb.velocity.magnitude;


        float speed = m_hitSpeed + ((m_maxSpeed - m_hitSpeed) * power);
        m_rb.velocity = returnVector * speed;

        Pause();
    }

    private void Pause()
    {
        m_paused = true;

        /*start pause now*/
        float m_pauseStartTime = Time.realtimeSinceStartup;  

        /*scale the pause time by the velocity of the ball and some constant*/
        float durationScale = m_pauseVelocityTimeScale * m_rb.velocity.magnitude;
        m_pauseEndTime = m_pauseStartTime + (m_initialPauseDuration * durationScale);

        Debug.Log("current time: " + m_pauseStartTime  + "  end time: " + m_pauseEndTime);
        Time.timeScale = 0;
    }

    private void Unpause()
    {
        m_paused = false;
        Time.timeScale = 1;
    }

    void Awake()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_paused = false;
        m_prevPaused = false;
    }

	// Use this for initialization
	void Start ()
    {
	   
	}
	
	// Update is called once per frame
	void Update ()
    {
        m_prevPaused = m_paused;
        if (m_paused && Time.realtimeSinceStartup >= m_pauseEndTime)
        {
            Unpause();
        }


        ////testing
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    m_curPower += Time.deltaTime;
        //}
        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    Hit(new Vector3(1, 1, 0), m_curPower);
        //    m_curPower = 0;
        //}
 
    }


}

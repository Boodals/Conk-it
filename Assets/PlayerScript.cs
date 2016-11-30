using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour
{
    public enum PlayerStates { Normal, Charging, Attacking}
    public PlayerStates currentState = PlayerStates.Normal;

    float movementSpeed = 38.0f;

    public bool grounded = false;
    public float curCharge = 0;

    Rigidbody2D rb;
    GameObject myMesh, myHand;
    public CircleCollider2D attackCollider;

    public bool wallHanging = false;
    public Vector3 wallJumpPoint;
    float hangDelay = 0;

    public string hInput, vInput, jump, attack;

    float attackTime = 0;
    float deathTimer = 0;

    Vector3 scale;

    public int myPlayerID;



    public void Die()
    {
        transform.localScale = Vector3.zero;
        deathTimer = 5;
    }

    void HandleDeath()
    {
        transform.position = new Vector3(0, 15, 0);
        rb.velocity = Vector3.zero;

        deathTimer -= Time.deltaTime;

        if (deathTimer <= 0)
        {
            //spawn me back
            SpawnManager.singleton.respawnMe(myPlayerID);
        }
    }













    // Use this for initialization
    void Start()
    {
        myMesh = transform.FindChild("Box").gameObject;
        myHand = transform.FindChild("Hand").gameObject;

        rb = GetComponent<Rigidbody2D>();

        attackCollider.enabled = false;

        scale = transform.localScale;
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if(deathTimer>0)
        {
            HandleDeath();
            return;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, scale, 8 * Time.deltaTime);
        }

        if(transform.position.y < -12)
        {
            Die();
        }

        ///////////////////

        grounded = CheckGrounded();

        if (grounded || wallHanging)
        {
            if (Input.GetButtonDown(jump))
            {
                Jump();
            }
        }

        //Handling attacking
        if (currentState == PlayerStates.Normal)
        {
            if (Input.GetButton(attack))
            {
                curCharge += Time.deltaTime;
            }

            if(Input.GetButtonUp(attack) && !wallHanging)
            {
                Attack();
            }
        }

        curCharge = Mathf.Clamp(curCharge, 0, 1);
        myMesh.transform.localScale = Vector3.Lerp(myMesh.transform.localScale, Vector3.one * (curCharge+1), 8 * Time.deltaTime);

        //If I'm spinning right now...
        if(attackTime>0)
        {
            attackCollider.enabled = true;

            attackTime -= Time.deltaTime * 3;
            float rotation = Mathf.Lerp(356, 0, attackTime);

            myMesh.transform.localEulerAngles = new Vector3(0, 0, rotation);
        }
        else
        {
            attackCollider.enabled = false;
            myMesh.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    void Attack()
    {
        if (attackTime > 0)
            return;

        //Debug.Break();

        attackCollider.radius = 1f + curCharge*0.85f;

        myMesh.transform.localScale *= 2;
        attackTime = 0.5f + curCharge*0.5f;
        curCharge = 0;
    }

    void Jump()
    {
        float jumpHeight = 14;
        Vector3 dir = Vector3.up;

        if(wallHanging && hangDelay<=0)
        {
            wallHanging = false;
            hangDelay = 0.25f;

            dir = (transform.position - wallJumpPoint).normalized * 0.4f;
            dir.y = 0.5f;
            dir.Normalize();
            Debug.Log(dir);
        }

        rb.AddForce(dir * jumpHeight, ForceMode2D.Impulse);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject == gameObject)
            return;
        if (col.gameObject.CompareTag("Untagged"))
            return;

        Ball victimBall = col.gameObject.GetComponent<Ball>();
        if(victimBall)
        {
            victimBall.Hit(transform.position, 0.2f + curCharge * 0.8f);
        }

        Debug.Log(col.gameObject.name + " hit");
    }

    void FixedUpdate()
    {
        Vector3 movement = new Vector3(Input.GetAxisRaw(hInput), 0, 0);

        FindWall(movement);

        if (!wallHanging)
        {

            rb.AddForce(movement * (movementSpeed - curCharge*35) * Time.deltaTime, ForceMode2D.Impulse);
            ManageMaxSpeed();
        }
        else
        {
            rb.velocity = -Vector3.up * Time.deltaTime * 0.2f;
        }

        if (grounded && movement.magnitude<0.2f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, 8 * Time.deltaTime);
        }
    }

    void FindWall(Vector3 direction)
    {
        RaycastHit2D hit;

        hit = Physics2D.Raycast(transform.position, direction, 0.7f, LayerMask.GetMask("Default"));

        if(hangDelay>0)
        {
            hangDelay -= Time.deltaTime;
        }

        if(hit && hangDelay<=0)
        {
            wallHanging = true;
            wallJumpPoint = hit.point;
            myHand.transform.position = Vector3.Lerp(wallJumpPoint, transform.position, 0.25f);
            transform.position = Vector3.Lerp(transform.position, new Vector2(wallJumpPoint.x, wallJumpPoint.y) + new Vector2(hit.normal.x, hit.normal.y)*0.45f, 6 * Time.deltaTime);
        }
        else
        {
            wallHanging = false;
            wallJumpPoint = Vector3.zero;
            myHand.transform.position = transform.position;
        }
    }

    void ManageMaxSpeed()
    {
        float maxSpeed = 10.0f;

        if(Mathf.Abs(rb.velocity.x) > maxSpeed)
        {
            Vector3 velo = rb.velocity;

            velo.x = maxSpeed;

            if(rb.velocity.x < 0)
            {
                velo.x = -maxSpeed;
            }

            rb.velocity = velo;
        }
    }

    bool CheckGrounded()
    {
        bool cG = false;
        float distance = 0.08f;

        RaycastHit2D g = Physics2D.BoxCast(transform.position, transform.localScale*0.95f, 0, -Vector3.up, distance, LayerMask.GetMask("Default"));

        cG = g;

        if (cG)
        {
            //Debug.Log(g.collider.gameObject.name);
            Debug.DrawLine(transform.position, transform.position - Vector3.up * distance, Color.magenta);
        }

        return cG;
    }

    bool CheckObstacle(Vector3 dir)
    {
        dir.y = 0.1f;

        bool cannotMove = false;
        float distance = 0.08f;
        RaycastHit2D g = Physics2D.BoxCast(transform.position, transform.localScale * 0.95f, 0, dir, distance, LayerMask.GetMask("Default"));

        if(g)
        {
            cannotMove = true;
        }

        return cannotMove;
    }
}

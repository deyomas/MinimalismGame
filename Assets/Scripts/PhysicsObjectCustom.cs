using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObjectCustom : MonoBehaviour
{
    public float minGroundNormalY = .65f;
    public float gravityModifier = 1.0f;
    public float airDrag = 0.05f;

    protected Vector2 targetVelocity;
    protected float targetRotation;
    protected bool grounded;
    protected Vector2 groundNormal;

    protected Vector2 velocity;
    protected Rigidbody2D rb2d;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);

    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

	private void OnEnable()
	{
        rb2d = GetComponent<Rigidbody2D>();
	}

	// Start is called before the first frame update
	void Start()
    {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    // Update is called once per frame
    void Update()
    {
        //targetVelocity = Vector2.zero;
        //ComputeVelocity();
    }

    protected virtual void ComputeVelocity()
    {

    }


    protected virtual void FixedUpdate()
	{
        Vector2 deltaPosition = Vector2.zero;
        Vector2 moveAlongGround = Vector2.zero;
        Vector2 move = Vector2.zero;

        //Jumping
        if (true)
        {
            //How do I mix a custom jump velocity with calculated rigidbody2d velocity?
            //Do I modify position? Do I add to and monitor velocity?
            
            if (false && velocity.y > 0)
            {
                rb2d.velocity = new Vector2(rb2d.velocity.x, velocity.y);
                velocity.y = 0;
            }

            //velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
            //velocity += rb2d.gravityScale * Physics2D.gravity * Time.deltaTime;
            velocity = rb2d.velocity;

            //Vertical
            grounded = false;

            deltaPosition = velocity * Time.deltaTime;
            moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x); //Perpendicular to ground normal
            move = Vector2.up * deltaPosition.y;
            Movement(move, true);

            //rb2d.position += velocity * Time.deltaTime;


        }

        //Strafing
        if (true)
        {
            //Horizontal
            if (Mathf.Abs(targetVelocity.x) > 0.01f || (grounded && Mathf.Abs(velocity.x) > 0.01f)) //Has user input or has ground friction w/ motion
            {
                velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, .25f);
            }
            else if (!grounded) //Apply air friction w/o user input
            {
                velocity.x *= (1 - airDrag);
            }
            else //No input, grounded w/ minimal speed
            {
                velocity.x = 0;
            }
            //Update for horizontal calculation
            deltaPosition = velocity * Time.deltaTime;
            move = moveAlongGround * deltaPosition.x;
            //rb2d.velocity = new Vector2(move.x, rb2d.velocity.y);
            Movement(move, false);
        }


        //Twisting
        if (true)
        {
            //Debug.Log(rb2d.angularVelocity + ", " + grounded);
            //Rotation
            if (Mathf.Abs(targetRotation) > 0.01f)
            {
                rb2d.angularVelocity = Mathf.Lerp(rb2d.angularVelocity, targetRotation, grounded ? .5f : .35f);
                //rb2d.AddTorque(targetRotation);
            }
            else if (!grounded)
            {
                rb2d.angularVelocity *= (1 - rb2d.angularDrag);
            }
            else
            {

            }
            //Debug.Log(rb2d.velocity);
        }
    }

    void Movement(Vector2 move, bool yMovement)
	{
        float distance = move.magnitude;
        
        if (distance > minMoveDistance)
        {
            int count = rb2d.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for(int i = 0; i < count; i++)
            {
                hitBufferList.Add(hitBuffer[i]);
            }
            
            for(int i = 0; i < hitBufferList.Count; i++)
            {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY)
                {
                    grounded = true;
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }
                
                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0)
                {
                    velocity = velocity - projection * currentNormal;
                }
                
                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }

        if (!yMovement && velocity.x != 0) //zero check so we don't interfear with rb2d's normal calculations
        {
            rb2d.velocity = new Vector2(velocity.x, rb2d.velocity.y);
            //rb2d.velocity = rb2d.velocity + velocity;
            //rb2d.velocity += Vector2.right * velocity.x * Time.deltaTime;
            //rb2d.position = rb2d.position + move.normalized * distance;
        }
        return;

        if (yMovement)
            rb2d.position = rb2d.position + move.normalized * distance;
        //rb2d.velocity += (yMovement ? Vector2.up * velocity.y : Vector2.right * velocity.x * Time.deltaTime);
        //rb2d.velocity = (yMovement ? new Vector2(rb2d.velocity.x, velocity.y) : new Vector2(velocity.x, rb2d.velocity.y));
        //rb2d.velocity = (yMovement ? rb2d.velocity + Vector2.up * velocity.y : rb2d.velocity + velocity);
	}
}

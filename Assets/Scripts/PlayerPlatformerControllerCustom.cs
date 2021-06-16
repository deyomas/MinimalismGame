using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerPlatformerControllerCustom : PhysicsObjectCustom, IAttacker, IKillable
{
    public float maxSpeed = 7;
    public float maxRotationSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    public float torquePower = 7;
    public float attackBounceBack = 5;
    public float bounceInputDelay = .3f;

    BoxCollider2D box;
    Collider2D[] results;
    Vector2[] points;
    Vector2 hitNormal;
    float ignoreMoveUntil = 0;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        box = GetComponent<BoxCollider2D>();
        results = new Collider2D[16];
        points = new Vector2[]
        {
            box.size/2,
            -box.size/2,
            new Vector2(-box.size.x/2, box.size.y/2),
            new Vector2(box.size.x/2, -box.size.y/2)
        };
    }

    protected override void FixedUpdate()
    {
        Vector2 temp = targetVelocity;
        float tempAir = airDrag;
        if (ignoreMoveUntil > Time.time)
        {
            targetVelocity = Vector2.zero;
            airDrag = 0.05f;
        }
        base.FixedUpdate();
        targetVelocity = temp;
        airDrag = tempAir;
        CheckIfDamage();
    }

    Vector2 GetCollisionNormal(Collider2D collider)
    {
        int count = rb2d.Cast(rb2d.velocity, contactFilter, hitBuffer);
        hitBufferList.Clear();
        for (int i = 0; i < count; i++)
        {
            hitBufferList.Add(hitBuffer[i]);
        }

        for (int i = 0; i < hitBufferList.Count; i++)
        {
            if (hitBufferList[i].collider == collider)
                return hitBufferList[i].normal;

            Vector2 currentNormal = hitBufferList[i].normal;
            if (currentNormal.y > minGroundNormalY)
            {
                
            }
        }
        return Vector2.zero;
    }

    public LineRenderer line;

    bool DoesColliderHitCorner(Collider2D collision, out Vector2 normal)
    {
        normal = Vector2.zero;
        if (collision == null) return false;

        Vector3 localColliderCenter = new Vector3(-.037f, -.23f, 0);
        Vector3 colliderCenter = transform.position + (Vector3)RotateVectorBy(localColliderCenter, transform.rotation.eulerAngles.z);
        
        ContactPoint2D[] cPoints = new ContactPoint2D[10];
        rb2d.GetContacts(cPoints);
        ContactPoint2D cPoint = new ContactPoint2D();
        for(int i = 0; i < cPoints.Length; i++)
        {
            if (cPoints[i].collider == collision)
            {
                cPoint = cPoints[i];
                i = 100; //break loop lol
            }
        }
        if (cPoint.collider == null)
            return false;

        

        Vector2 hitClosestPoint = cPoint.point;
        Vector2 localHitDirection = RotateVectorBy(cPoint.point - (Vector2)colliderCenter, -transform.rotation.eulerAngles.z);
        //Vector2 localHitDirection = RotateVectorBy(selfClosestPoint - (Vector2)colliderCenter, -transform.rotation.eulerAngles.z);

        //For debugging
        line.SetPositions(new Vector3[]{
            localColliderCenter - Vector3.forward,
            (Vector3)localHitDirection + localColliderCenter - Vector3.forward //+ transform.position
        });
        
        /*
        foreach (Vector2 point in points)
        {
            if ((cPoint.point - (Vector2)(colliderCenter + (Vector3)RotateVectorBy(point, -transform.rotation.eulerAngles.z))).magnitude < .05f)
            {
                normal = GetCollisionNormal(collision);
                return true;
                float hi = 0f;
            }
        }
        */

        foreach (Vector2 point in points)
        {
            float angle = Vector2.Dot(point, localHitDirection) / (point.magnitude * localHitDirection.magnitude);
            angle = Mathf.Acos(angle);
            angle *= 180 / Mathf.PI;
            if (angle < 6)
            {
                normal = GetCollisionNormal(collision);
                return true;
            }
        }

        return false;

        return collision.OverlapPoint(box.bounds.min) ||
                collision.OverlapPoint(box.bounds.max) ||
                collision.OverlapPoint(new Vector2(box.bounds.min.x, box.bounds.max.y)) ||
                collision.OverlapPoint(new Vector2(box.bounds.max.x, box.bounds.min.y));
    }

    /// <summary>
    /// Rotates a local vector (centered at origin) around the origin by a specified angle
    /// </summary>
    /// <param name="input"></param>
    /// <param name="angle"></param>
    /// <returns></returns>
    Vector2 RotateVectorBy(Vector2 input, float angle)
    {
        angle *= Mathf.PI / 180; 
        Vector2 output;
        output.x = (input.x * Mathf.Cos(angle)) - (input.y * Mathf.Sin(angle));
        output.y = input.y * Mathf.Cos(angle) + input.x * Mathf.Sin(angle);
        return output;
    }

    //--------------IKillable interface

    public void TakeDamage()
    {
        Debug.Log("The player died!");
        //Destroy(gameObject);
        //Play effects and game over or respawn
    }

    //--------------End IKillable interface


    //--------------IAttacker interface
    public void CheckIfDamage()
    {
        results = new Collider2D[16];
        rb2d.OverlapCollider(contactFilter, results);
        foreach (Collider2D result in results)
        {
            if (result != null && DoesColliderHitCorner(result, out hitNormal))
            {
                ApplyDamage(result.gameObject);
            }
        }
    }

    public void ApplyDamage(GameObject obj)
    {
        IKillable enemy = obj.GetComponent<IKillable>();
        if (enemy != null)
        {
            enemy.TakeDamage();

            Vector2 normal = (transform.position - obj.transform.position).normalized;

            if (hitNormal != Vector2.zero)
                normal = hitNormal;


            normal = Mathf.Abs(normal.y) > Mathf.Abs(normal.x) ?
                        (Mathf.Abs(normal.y) / normal.y) * Vector2.up :
                        (Mathf.Abs(normal.x) / normal.x) * Vector2.right;



            if (normal.x == 0)
            {
                //Hit top or bottom
                if (normal.x < 0)
                {
                    //Hit the underside of an enemy
                    rb2d.velocity = -Vector2.up * attackBounceBack + rb2d.velocity;
                }
                else
                {
                    rb2d.velocity = rb2d.velocity * Vector2.right;
                    rb2d.AddForce(normal * attackBounceBack);
                }
            }
            else
            {
                //Hit sides
                //rb2d.velocity = (rb2d.velocity * Vector2.up) + (Vector2.up * jumpTakeOffSpeed);
                rb2d.velocity = (Vector2.up * jumpTakeOffSpeed / 2);
                rb2d.AddForce(normal * attackBounceBack * 1.5f);

                //normal = Vector2.up;
                /*
                rb2d.velocity = rb2d.velocity - 2 * (Vector2.Dot(rb2d.velocity, normal) * normal);
                rb2d.velocity = rb2d.velocity + Vector2.up * attackBounceBack;
                */
            }

            ignoreMoveUntil = Time.time + bounceInputDelay;

            return;


            float angle = Mathf.Atan(normal.y / normal.x) * 180 / Mathf.PI;
            angle = ((int)(angle - 45) / 90) * 90f;


            rb2d.velocity = rb2d.velocity.normalized * (Mathf.Max(rb2d.velocity.magnitude, 5)); //+ normal * attackBounceBack;
            rb2d.AddForce(normal * attackBounceBack);
            //
        }
    }

    //------------------End IAttacker interface



    #region Movement
    //---------------------Movement--------------------------

    /// <summary>
    /// Handles jumping based on whether the jump started or was canceled
    /// </summary>
    /// <param name="jumpStarted"></param>
    /// <param name="jumpCancled"></param>
    private void HandleJump(bool jumpStarted, bool jumpReleased)
    {
        if (jumpStarted)
        {
            //velocity.y = jumpTakeOffSpeed;
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpTakeOffSpeed);
        }
        else if (jumpReleased)
        {
            if (rb2d.velocity.y > 0)
            {
                //velocity.y = velocity.y * 0.5f;
                rb2d.velocity = new Vector2(rb2d.velocity.x, rb2d.velocity.y * 0.5f);
            }
        }
    }

    /// <summary>
    /// Handles moving the player based on a given input vector
    /// </summary>
    /// <param name="inputDirection">The input direction vector</param>
    private void HandleMove(Vector2 inputDirection)
    {
        Vector2 move = Vector2.zero;

        move.x = inputDirection.x;

        targetVelocity = move * maxSpeed;
    }


    private void HandleRotate(float rotateDirection)
    {
        targetRotation = 0;
        if (Mathf.Abs(rb2d.angularVelocity) < maxRotationSpeed)
        {
            targetRotation = -rotateDirection * maxRotationSpeed;
            if (Mathf.Abs(rotateDirection) > 0.01f)
            {
                rb2d.AddTorque(-rotateDirection * torquePower);
            }
        }
    }
    #endregion

    #region Input
    //---------------------Input-------------------------------

    /// <summary>
    /// Callback for the PlayerInput component
    /// </summary>
    /// <param name="context"></param>
    public void OnJump(InputAction.CallbackContext context)
    {
        bool hasJumped = context.started;
        bool jumpReleased = context.canceled;
        /* bool hasJumped = false;
         bool jumpReleased = false;
         if (context.started)
             hasJumped = true;
         if (context.canceled)
             jumpReleased = true;*/
        HandleJump(hasJumped, jumpReleased);
    }

    /// <summary>
    /// Callback for the PlayerInput component
    /// </summary>
    /// <param name="context"></param>
    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputDirection = context.ReadValue<Vector2>();
        HandleMove(inputDirection);
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        float rotateDirection = context.ReadValue<float>();
        HandleRotate(rotateDirection);
    }
    #endregion
}

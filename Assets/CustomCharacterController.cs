using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//UnityEngine.InputSystem.PlayerNotifications.InvokeUnityEvents

using UnityEngine.InputSystem;


    public class CustomCharacterController : MonoBehaviour
{
    const float BOX_WIDTH = 100;

    public float rotationForce = 1000; //No idea what a good default value is
    public float movementForce = 1000; //No idea what a good default value is
    public float jumpSpeed = 1000; //No idea what a good default value is

    [SerializeField]
    new Rigidbody2D rigidbody;

    [SerializeField]
    CircleCollider2D[] Corners;

    bool bIsInAir;

    private void Awake() 
    {
        GetComponent<RectTransform>().sizeDelta = new Vector2(BOX_WIDTH, BOX_WIDTH);
        rigidbody = GetComponent<Rigidbody2D>();
        if (Corners.Length == 0)
        {
            Corners = new CircleCollider2D[4];

            //Spawn corner colliders in each corner
            for (int i = 0; i <= 3; i++)
            {
                Corners[i] = SpawnCornerCollider(i / 2, i % 2);
            }
        }

        GetComponent<PlayerInput>().enabled = true;
    }

    /// <summary>
    /// Spawns a circle collider in the designated corner
    /// </summary>
    /// <param name="x">Index of the horizontal corner</param>
    /// <param name="y">Index of the vertical corner</param>
    private CircleCollider2D SpawnCornerCollider(int x, int y)
    {
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    int GetTouchingCorners()
	{
        int touching = 0;
        foreach (CircleCollider2D corner in Corners)
        {
            if (corner != null && corner.IsTouchingLayers())
                touching++;
        }
        return touching;
    }

    //Receives Broadcast() from PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ToString());
        //Check if the player can move

        //Check if at least 2 corners are touching something (Unsure if I want this)
        if (GetTouchingCorners() > 1)
        {
            //Apply movement
            rigidbody.AddForce(new Vector2(100, 0));
        }
    }

    //Receives Broadcast() from PlayerInput
    public void OnRotate(InputAction.CallbackContext context)
    {
        //Debug.Log(context.ToString());
        return;
        //Apply a rotational force based on the direction
        int direction = context.ReadValue<int>();
        rigidbody.AddTorque(direction * rotationForce);
    }


    //Receives Broadcast() from PlayerInput
    public void OnJump(InputAction.CallbackContext context)
    {
        Debug.Log("HelLo");

        //Check if we can jump
        if (context.started)
		{
            //Apply jump force
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpSpeed);
            rigidbody.gravityScale = .8f;
            
            Invoke("ResetGravity", .3f);
        } 
        else if (context.canceled && rigidbody.velocity.y > 0) //If we're still heading up
		{
            //Small initial downward force to kickstart fall
            rigidbody.AddForce(new Vector2(0, -100f));

            //temporarily increase gravity to speed fall as well.
            rigidbody.gravityScale = 1.5f;
            Invoke("ResetGravity", .5f);

        }
    }

    void ResetGravity()
	{
        rigidbody.gravityScale = 1f;
	}
}

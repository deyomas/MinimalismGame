using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CameraController : MonoBehaviour
{
	private static CameraController cameraInst;

	public static CameraController GetCameraController()
	{
		return cameraInst;
	}

	[SerializeField]
	private GameObject PlayerInstance;

	[SerializeField]
	private Camera PlayerCamera;

	private Vector2 PlayerLoc;

	public Vector2 PlayerBounds
	{
		get{ return GetPlayerBounds(); }
	}

	private void Awake()
	{
		if (cameraInst == null)
		{
			cameraInst = this;
		}

		if (PlayerInstance == null)
		{
			PlayerInstance = GameObject.Find("Player");
		}

		if (PlayerCamera == null)
		{
			PlayerCamera = FindObjectOfType<Camera>();
		}
	}

	private void Start()
	{
		
	}

	private void Update()
	{
		UpdatePlayerLoc();
		if (IsOutsidePlayerBounds(PlayerLoc))
		{
			Vector3 pos = transform.position;

			pos.y += (PlayerBounds.x - PlayerLoc.x);
			pos.z += (PlayerBounds.y - PlayerLoc.y);

			transform.position = pos;
		}
	}

	//Updates PlayerLoc
	private Vector2 UpdatePlayerLoc()
	{
		PlayerLoc = Vector2.zero;
		return Vector2.zero;
	}

	private bool IsOutsidePlayerBounds(Vector2 Pos)
	{

		return false;
	}

	//Converts screen size to world positions
	private Vector2 GetPlayerBounds()
	{
		float BufferPrecent = 0.6f;
		
		return new Vector2(
			PlayerCamera.pixelWidth * BufferPrecent + PlayerCamera.pixelWidth * (1-BufferPrecent)/2,
			PlayerCamera.pixelHeight * BufferPrecent + PlayerCamera.pixelHeight * (1-BufferPrecent)/2
		);
	}
}


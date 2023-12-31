using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
	public float Velocity;
	public bool active = true;
	public float OriginalVelocity;
	private bool isLockedOn = false;
	[Space]

	public float InputX;
	public float InputZ;
	public Vector3 desiredMoveDirection;
	public bool blockRotationPlayer;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public Camera cam;
	public CharacterController controller;
	public bool isGrounded;

	[Header("Animation Smoothing")]
	[Range(0, 1f)]
	public float HorizontalAnimSmoothTime = 0.2f;
	[Range(0, 1f)]
	public float VerticalAnimTime = 0.2f;
	[Range(0, 1f)]
	public float StartAnimTime = 0.3f;
	[Range(0, 1f)]
	public float StopAnimTime = 0.15f;

	public float gravity = 9.81f;
	public float verticalVel;
	private int jumpCount = 0;
	private bool canJump = true;
	public float jumpForce = 10f;

	private Vector3 moveVector;
	private TacticalMode gameScript;


	private bool isDashing = false;
	public float dashSpeed = 10f;
	public float dashDuration = 0.5f;
	public float dashCooldown = 2.0f;
	private float dashCooldownTimer = 0.0f;

	// Use this for initialization
	void Start()
	{
		OriginalVelocity = Velocity;
		anim = this.GetComponent<Animator>();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController>();
		gameScript = GetComponent<TacticalMode>();
	}


	// Update is called once per frame
	void Update()
	{
		if (gameScript.usingAbility || Input.GetKey(KeyCode.Q))
		{
			ApplyGravity(); // Apply gravity even when the "Q" key is held
			return;
		}
		InputMagnitude();

		// Check if the character is grounded using multiple methods
		isGrounded = IsGroundedMultipleChecks();

		if (isGrounded)
		{
			// Reset jump count and allow jumping when grounded
			jumpCount = 0;
			canJump = true;
			verticalVel = 0; // Reset vertical velocity when grounded
		}
		else
		{
			// Apply gravity when not grounded
			ApplyGravity();
		}

		// Handle jumping only when allowed and grounded
		if (canJump && isGrounded && Input.GetButtonDown("Jump"))
		{
			Jump();
		}

		// Apply vertical movement based on the calculated vertical velocity
		moveVector = new Vector3(0, verticalVel * 0.2f, 0) * Time.deltaTime;
		controller.Move(moveVector);
	}

	void Jump()
	{
		if (canJump)
		{
			if (isGrounded)
			{
				verticalVel = jumpForce; // Apply the jump force directly to the vertical velocity
				jumpCount++;
				anim.SetTrigger("JumpTrigger");
			}
			else if (!isGrounded && jumpCount == 1)
			{
				verticalVel = jumpForce; // Apply the jump force for the second jump
				jumpCount++;
				anim.SetTrigger("SecondJumpTrigger");
				canJump = false; // Disable jumping until grounded again
			}
		}
	}

	void ApplyGravity()
	{
		// Apply gravity to the character's vertical velocity
		verticalVel -= gravity * Time.deltaTime;
	}


	void PlayerMoveAndRotation(float InputX, float InputZ)
	{

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		forward.y = 0f;
		right.y = 0f;

		forward.Normalize();
		right.Normalize();

		desiredMoveDirection = forward * InputZ + right * InputX;

		if (blockRotationPlayer == false)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
			controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
		}
	}

	public void LookAt(Vector3 pos)
	{
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
	}

	public void RotateToCamera(Transform t)
	{

		var camera = Camera.main;
		var forward = cam.transform.forward;
		var right = cam.transform.right;

		desiredMoveDirection = forward;

		t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
	}

	void InputMagnitude()
	{
		// Calculate the character's speed
		Speed = new Vector2(InputX, InputZ).sqrMagnitude;

		// Update the "Speed" parameter in the Animator
		anim.SetFloat("Speed", Speed);
		//Calculate Input Vectors
		InputX = active ? Input.GetAxis("Horizontal") : 0;
		InputZ = active ? Input.GetAxis("Vertical") : 0;

		//anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
		//anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

		//Calculate the Input Magnitude
		Speed = new Vector2(InputX, InputZ).sqrMagnitude;

		//Physically move player

		if (Speed > allowPlayerRotation)
		{
			anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
			PlayerMoveAndRotation(InputX, InputZ);
		}
		else if (Speed < allowPlayerRotation)
		{
			anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
		}
	}

	IEnumerator Dash()
	{
		isDashing = true;
		float dashEndTime = Time.time + dashDuration;

		// Trigger the dash animation
		anim.SetTrigger("DashTrigger");

		// Calculate the dash direction based on player input
		Vector3 inputDirection = new Vector3(InputX, 0, InputZ).normalized;

		if (inputDirection != Vector3.zero)
		{
			// Calculate the movement relative to the camera
			Vector3 dashDirection = cam.transform.TransformDirection(inputDirection);

			while (Time.time < dashEndTime)
			{
				// Apply the dash using the calculated movement direction
				controller.Move(dashDirection * dashSpeed * Time.deltaTime);
				yield return null;
			}
		}

		// Stop the dash animation
		anim.ResetTrigger("DashTrigger");

		isDashing = false;
	}

    bool IsGroundedMultipleChecks()
    {
        bool grounded = controller.isGrounded;

        // Perform a raycast downward from the player's position
        float rayDistance = 0.3f; // Adjust this distance as needed
        grounded |= Physics.Raycast(transform.position, Vector3.down, rayDistance);

        // Perform a sphere cast to detect the ground
        float sphereRadius = 0.1f; // Adjust this radius as needed
        grounded |= Physics.SphereCast(transform.position, sphereRadius, Vector3.down, out _, rayDistance);

        return grounded;
    }

  

}
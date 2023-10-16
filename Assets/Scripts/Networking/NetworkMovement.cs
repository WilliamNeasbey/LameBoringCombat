using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkMovement : NetworkBehaviour
{
    [SyncVar]
    public float Velocity;

    public bool active = true;
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
    public float OriginalVelocity;
    public NetworkAnimator networkAnimator;
    // Remove the 'isGrounded' variable, as it will be handled differently with Mirror

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
    private float verticalVel;
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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // This code will only run on the local player's client.
        OriginalVelocity = Velocity;
        anim = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();
        gameScript = GetComponent<TacticalMode>();
        networkAnimator = GetComponent<NetworkAnimator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer || gameScript.usingAbility)
            return;

        // Check if the character is grounded
        bool isGrounded = controller.isGrounded;

        InputMagnitude();

        // Check for dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && Time.time >= dashCooldownTimer)
        {
            StartCoroutine(Dash());
            dashCooldownTimer = Time.time + dashCooldown;
        }

        if (isGrounded)
        {
            jumpCount = 0; // Reset jump count when grounded
            canJump = true; // Allow jumping when grounded
            verticalVel = 0; // Reset vertical velocity
        }
        else
        {
            // Apply gravity to verticalVel when not grounded
            verticalVel -= gravity * Time.deltaTime;
        }

        moveVector = new Vector3(0, verticalVel * 0.2f, 0) * Time.deltaTime;

        controller.Move(moveVector);

        // Update the "Velocity" parameter in the Animator
        //anim.SetFloat("Velocity", Velocity);

        // Handle jumping
        if (canJump && Input.GetButtonDown("Jump"))
        {
            Jump();
        }
    }


    void Jump()
    {
        if (jumpCount == 0)
        {
            anim.SetTrigger("JumpTrigger");
            networkAnimator.SetTrigger("JumpTrigger"); // Synchronize the jump trigger over the network
            verticalVel = jumpForce;
            jumpCount++;
        }
        else if (jumpCount == 1)
        {
            anim.SetTrigger("SecondJumpTrigger");
            networkAnimator.SetTrigger("SecondJumpTrigger"); // Synchronize the second jump trigger
            verticalVel = jumpForce;
            jumpCount++;
            canJump = false;
        }
    }


    bool IsGrounded()
    {
        return controller.isGrounded; // Use CharacterController's isGrounded property
    }

    [Command]
    void CmdDash(Vector3 dashDirection, float dashSpeed)
    {
        RpcDash(dashDirection, dashSpeed);
    }

    [ClientRpc]
    void RpcDash(Vector3 dashDirection, float dashSpeed)
    {
        StartCoroutine(ClientDash(dashDirection, dashSpeed));
    }

    IEnumerator ClientDash(Vector3 dashDirection, float dashSpeed)
    {
        float dashEndTime = Time.time + dashDuration;

        // Trigger the dash animation
        anim.SetTrigger("DashTrigger");

        while (Time.time < dashEndTime)
        {
            // Apply the dash using the calculated movement direction
            controller.Move(dashDirection * dashSpeed * Time.deltaTime);
            yield return null;
        }

        // Stop the dash animation
        anim.ResetTrigger("DashTrigger");

        isDashing = false;
    }

    void InputMagnitude()
    {
        // Calculate the character's speed
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        // Update the "Speed" parameter in the Animator
        anim.SetFloat("Speed", Speed);

        // Calculate Input Vectors
        InputX = active ? Input.GetAxis("Horizontal") : 0;
        InputZ = active ? Input.GetAxis("Vertical") : 0;

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

    IEnumerator Dash()
    {
        isDashing = true;

        // Calculate the dash direction based on player input
        Vector3 inputDirection = new Vector3(InputX, 0, InputZ).normalized;

        if (inputDirection != Vector3.zero)
        {
            // Calculate the movement relative to the camera
            Vector3 dashDirection = cam.transform.TransformDirection(inputDirection);

            CmdDash(dashDirection, dashSpeed);
        }

        // Make sure to add a return statement
        yield break;
    }
}

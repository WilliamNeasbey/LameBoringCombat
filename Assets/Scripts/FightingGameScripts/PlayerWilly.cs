using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWilly : MonoBehaviour
{
    public float Velocity;
    public float OriginalVelocity;
    public bool active = true;
    private bool isLockedOn = false;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;

    private Vector3 moveVector;

    private int punchComboCount = 0;
    private bool isPunching = false;
    public float punchCooldown = 0.5f; // Time between punch combos

    public Vector3 desiredMoveDirection;

    public int comboCount = 0; // Initialize combo count
    public float comboCooldown = 1.0f; // Cooldown between combos
    private float lastAttackTime = 0f; // Time of the last attack
    private bool canAttack = true; // Flag to control attack cooldown

    // Use this for initialization
    void Start()
    {
        OriginalVelocity = Velocity;
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        // Attack input
        if (canAttack && (Input.GetButtonDown("Punch") || Input.GetButtonDown("XButton")))
        {
            if (Time.time - lastAttackTime > comboCooldown)
            {
                // Reset combo count if it's been too long since the last attack
                comboCount = 0;
            }

            // Depending on the combo count, trigger the appropriate animation
            if (comboCount == 0)
            {
                // Trigger the first punch animation
                anim.SetTrigger("Punch");
            }
            else if (comboCount == 1)
            {
                // Trigger the second punch animation
                anim.SetTrigger("Punch2");
            }
            else if (comboCount == 2)
            {
                // Trigger the third punch animation
                anim.SetTrigger("Punch3");
            }

            comboCount = (comboCount + 1) % 3; // Increment combo count and wrap around

            lastAttackTime = Time.time;

            // Disable further attacks for a brief moment
            canAttack = false;
            StartCoroutine(ComboCooldown());
        }

        InputMagnitude();
        // Check if the character is grounded
        isGrounded = controller.isGrounded;
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

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
        controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
    }

    void InputMagnitude()
    {
        Speed = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).sqrMagnitude;

        anim.SetFloat("Speed", Speed);

        float InputX = active ? Input.GetAxis("Horizontal") : 0;
        float InputZ = active ? Input.GetAxis("Vertical") : 0;

        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, 0.3f, Time.deltaTime);
            PlayerMoveAndRotation(InputX, InputZ);
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, 0.15f, Time.deltaTime);
        }

        if (Input.GetButtonDown("Punch"))
        {
            Debug.Log("Punch button pressed"); // Add this line for debugging
            Punch();
        }
    }


    void Punch()
    {
        if (punchComboCount == 0)
        {
            anim.SetTrigger("Punch");
            punchComboCount++;
        }
        else if (punchComboCount == 1)
        {
            anim.SetTrigger("Punch2");
            punchComboCount++;
        }
        else if (punchComboCount == 2)
        {
            anim.SetTrigger("Airkick");
            punchComboCount = 0;
        }

        StartCoroutine(PunchCooldown());
    }

    // Coroutine to handle the combo cooldown
    IEnumerator ComboCooldown()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(comboCooldown);

        // Enable attacks again
        canAttack = true;
    }

    IEnumerator PunchCooldown()
    {
        isPunching = true;
        yield return new WaitForSeconds(punchCooldown);
        isPunching = false;
    }
}

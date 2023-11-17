using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using Cinemachine;
using UnityEngine.Rendering;
using DG.Tweening;

[System.Serializable] public class GameEvent : UnityEvent { }
[System.Serializable] public class TacticalModeEvent : UnityEvent<bool> { }

public class TacticalMode : MonoBehaviour
{
    [HideInInspector]
    public GameEvent OnAttack;
    [HideInInspector]
    public GameEvent OnModificationATB;
    [HideInInspector]
    public TacticalModeEvent OnTacticalTrigger;
    [HideInInspector]
    public TacticalModeEvent OnTargetSelectTrigger;

    public CharacterMovement characterMovement;
    private CharacterMovement movement;
    private Animator anim;
    public WeaponCollision weapon;
    public RightFistCollision sword;
    public KamehamehaCollision Kamehameha;
    public GameObject WeaponObject;
    public GameObject SwordCollision;
    public GameObject KamehamehaObject;
    public RightFistCollision rightFist;
    public GameObject RightFistObject;
    public LeftFistCollision leftFist;
    public GameObject LeftFistObject;
    public LeftLegCollision leftLeg;
    public GameObject LeftLegObject;
    public Transform playerCharacter;
    public Transform playerTransform;
    public float standAwayDistance = 1.0f; // Adjust as needed
    private Quaternion originalCharacterRotation;
    public float rotationSpeed = 5.0f; // Adjust the rotation speed as needed
    public float health = 100f;
    public GameObject projectilePrefab;
    public float atbRechargeRate = 50.0f; // Adjust the recharge rate as needed
    public GameObject barrierObject;
    public float barrierCost = 50f;
    public float barrierDrainRate = 50f; // Adjust as needed
    private bool isBarrierActive = false;
    private float defaultBarrierDrainRate;
    private float barrierActiveTime = 0f;
    private bool isChargingKi = false;
    public LockOnUI lockOnUITarget;


    [Header("Time Stats")]
    public float slowMotionTime = .005f;

    [Space]

    public bool tacticalMode;
    public bool isAiming;
    public bool usingAbility;
    public bool dashing;
    private bool isChargingKI = false; // Flag to track KI charging state
    
    //dashing crap
    private bool isDashing = false; private Vector3 dashStartPosition;
    private Vector3 dashEndPosition;
    private float dashSpeed = 10f; // Adjust the speed as needed
    private float dashDuration = .2f; // Adjust the duration as needed
    private float dashStartTime; // Variable to store the dash start time

    [Space]

    [Header("ATB Data")]
    public float atbSlider;
    public float filledAtbValue = 100;
    public int atbCount;

    [Space]

    [Header("Targets in radius")]
    public List<Transform> targets;
    public int targetIndex;
    public Transform aimObject;
    public int lockedTargetIndex = -1;
    public float maxDetectionRange = 10f;

    [Space]
    [Header("VFX")]
    public VisualEffect sparkVFX;
    public VisualEffect abilityVFX;
    public VisualEffect abilityHitVFX;
    public VisualEffect healVFX;
    public GameObject kiChargingParticle;
    public GameObject HealingParticle;

    [Space]
    [Header("Ligts")]
    public Light swordLight;
    public Light groundLight;

    [Header("Ligh Colors")]
    public Color sparkColor;
    public Color healColor;
    public Color abilityColot;

    [Space]
    [Header("Cameras")]
    public GameObject gameCam;
    public CinemachineVirtualCamera targetCam;
    public float zoomSpeed = 10f; // Adjust the zoom speed as needed
    private bool isLockedOn = false;

    [Space]

    public Volume slowMotionVolume;

    public float VFXDir = 5;

    private CinemachineImpulseSource camImpulseSource;

    [Space]
    [Header("combos")]
    private float lastAttackTime;
    private int comboCount = 0; // Initialize combo count
    private bool canAttack = true; // Added flag to control attack cooldown
    bool swordStyle = false;
    public GameObject Sword; 

    [Space]
    [Header("Sound")]
    public AudioSource KamehamehaSound;
    public AudioSource RoadHouseSound;
    public AudioSource KIchargingSound;


    [Space]
    [Header("ragdoll")]
    public GameObject ragdollPrefab;
    public float ragdollForce = 500f; // Adjust the force magnitude as needed
    public float forceRandomRange = 100f; // Adjust the random range as needed

    //Swords of sparta
    private bool isSwordsOfSpartaActive = false;
    private float swordsOfSpartaDuration = 5f; // Duration for Swords of Sparta effect

    public GameObject swordsOfSpartaObject; // Reference to the Swords of Sparta game object
    public AudioSource swordsOfSpartaActivationSound; // Sound to play on Swords of Sparta activation


    //Kikoken
    public GameObject KikokenPrefab;
    [SerializeField] private Transform projectilePosition; 
    public AudioSource KikokenSound;

    //teleport kick shadow nothing personal kid edgy kick from sonic06
    public float teleportDistance = 5f; // Adjust the teleport distance as needed
    public float teleportCooldown = 5f; // Cooldown duration for the teleport ability
    public LayerMask enemyLayer; // Specify the enemy layer in the Inspector

    private bool isTeleportOnCooldown = false;
    private float lastTeleportTime = 0f;

    private void Start()
    {
        originalCharacterRotation = playerCharacter.rotation;
        weapon.onHit.AddListener((target) => HitTarget(target));
        rightFist.onHit.AddListener((target) => HitTarget(target)); 
        leftFist.onHit.AddListener((target) => HitTarget(target));  
        leftLeg.onHit.AddListener((target) => HitTarget(target));  
        sword.onHit.AddListener((target) => HitTarget(target));  
        movement = GetComponent<CharacterMovement>();
        anim = GetComponent<Animator>();
        camImpulseSource = Camera.main.GetComponent<CinemachineImpulseSource>();
        characterMovement = GetComponent<CharacterMovement>();
        defaultBarrierDrainRate = barrierDrainRate;
    }

    void Update()
    {
        // Clear the targets list at the start of each frame
        targets.Clear();

        // Disable/Enable the CharacterMovement script based on tacticalMode
        if (tacticalMode)
        {
            // Disable the CharacterMovement script
            movement.enabled = false;
        }
        else
        {
            // Enable the CharacterMovement script
            movement.enabled = true;
        }

        // Check if the player's health is depleted
        if (health <= 0)
        {
            // Perform actions when health reaches zero or below (e.g., trigger death animation, game over logic)
            Die();
        }

        // Find all objects with the "Enemy" tag within the detection range
        Collider[] potentialTargets = Physics.OverlapSphere(playerTransform.position, maxDetectionRange);

        foreach (var potentialTargetCollider in potentialTargets)
        {
            Transform potentialTarget = potentialTargetCollider.transform;

            // Check if the potential target has the "Enemy" tag
            if (potentialTarget.CompareTag("Enemy"))
            {
                // Add the detected enemy to the targets list
                targets.Add(potentialTarget);
            }
        }
        if (isLockedOn)
        {
            if (targets.Count > 0 && !tacticalMode && !usingAbility)
            {
                targetIndex = NearestTargetToCenter();
                Transform lockedOnTarget = targets[targetIndex];
                Vector3 targetPosition = lockedOnTarget.position;
                targetPosition.y = transform.position.y;
                transform.LookAt(targetPosition);

                if (lockOnUITarget != null)
                {
                    lockOnUITarget.SetTarget(lockedOnTarget);
                }
            }
        }
        else
        {
            // ...
            if (lockOnUITarget != null)
            {
                lockOnUITarget.SetTarget(null);
            }
        }

        // Attack style switch logic
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            swordStyle = false; // Switch to default attack style
            Sword.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            swordStyle = true; // Switch to sword attack style
            Sword.SetActive(true);
        }


        // Attack
        if (canAttack && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Square")) && !tacticalMode && !usingAbility)
        {

            if (!swordStyle) // Default attack style
            {
                if (Time.time - lastAttackTime > 1f)
                {
                    // Reset combo count if it's been too long since the last attack
                    comboCount = 0;
                }

                if (comboCount == 0)
                {
                    anim.SetTrigger("Punch");
                }
                else if (comboCount == 1)
                {
                    anim.SetTrigger("Punch2"); // Trigger the second punch animation
                }
                else if (comboCount == 2)
                {
                    anim.SetTrigger("AirKick");
                }

                comboCount = (comboCount + 1) % 3; // Increment combo count and wrap around

                lastAttackTime = Time.time;

                // Disable further attacks for a brief moment
                canAttack = false;
                StartCoroutine(ComboCooldown());
            }
            else // Sword attack style
            {
                if (Time.time - lastAttackTime > 1f)
                {
                    // Reset combo count if it's been too long since the last attack
                    comboCount = 0;
                }

                if (comboCount == 0)
                {
                    anim.SetTrigger("Sword1");
                }
                else if (comboCount == 1)
                {
                    anim.SetTrigger("Sword2"); // Trigger the second punch animation
                }
                else if (comboCount == 2)
                {
                    anim.SetTrigger("Sword3");
                }

                comboCount = (comboCount + 1) % 3; // Increment combo count and wrap around

                lastAttackTime = Time.time;

                // Disable further attacks for a brief moment
                canAttack = false;
                StartCoroutine(ComboCooldown());
            }
            
        }

        // Attack
        if (canAttack && (Input.GetMouseButtonDown(1) || Input.GetButtonDown("Square")) && !tacticalMode && !usingAbility)
        {

            if (!swordStyle) // Default attack style
            {
                if (Time.time - lastAttackTime > 1f)
                {
                    // Reset combo count if it's been too long since the last attack
                    comboCount = 0;
                }

                if (comboCount == 0)
                {
                    anim.SetTrigger("LowKick");
                }
                else if (comboCount == 1)
                {
                    anim.SetTrigger("MidKick"); // Trigger the second punch animation
                }
                else if (comboCount == 2)
                {
                    anim.SetTrigger("HighKick");
                }

                comboCount = (comboCount + 1) % 3; // Increment combo count and wrap around

                lastAttackTime = Time.time;

                // Disable further attacks for a brief moment
                canAttack = false;
                StartCoroutine(ComboCooldown());
            }
            else // Sword attack style
            {
                if (Time.time - lastAttackTime > 1f)
                {
                    // Reset combo count if it's been too long since the last attack
                    comboCount = 0;
                }

                if (comboCount == 0)
                {
                    anim.SetTrigger("LowKick");
                }
                else if (comboCount == 1)
                {
                    anim.SetTrigger("MidKick"); // Trigger the second punch animation
                }
                else if (comboCount == 2)
                {
                    anim.SetTrigger("HighKick");
                }

                comboCount = (comboCount + 1) % 3; // Increment combo count and wrap around

                lastAttackTime = Time.time;

                // Disable further attacks for a brief moment
                canAttack = false;
                StartCoroutine(ComboCooldown());
            }
           
        }

        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetButtonDown("R3"))
        {
            ToggleLockOnTarget(true); // Cycle forward when Tab is pressed
        }

        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetButtonDown("R1")) && !usingAbility)
        {
            if (atbCount > 0 && !tacticalMode)
                SetTacticalMode(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelAction();
        }

        if (isLockedOn)
        {
            // Set the character's velocity to 2.5 when locked on
            //movement.Velocity = 2.5f;
            anim.SetBool("LockOnAnimation", true); // Set to true when locked on
        }
        else
        {
            // Reset the character's velocity to the original value when not locked on
            movement.Velocity = movement.OriginalVelocity;

            // Set the LockOnAnimation parameter to false
            anim.SetBool("LockOnAnimation", false);
        }

        // Check for mouse scroll delta input
        float scrollDelta = Input.mouseScrollDelta.y;
       // Debug.Log("Scroll Delta: " + scrollDelta); // Debug log to check the input

        // Adjust the camera's position based on scroll delta
        float zoomSpeed = 0.1f; // Adjust the speed as needed
        Vector3 newPosition = gameCam.transform.position + Vector3.forward * scrollDelta * zoomSpeed;
        gameCam.transform.position = newPosition;

        // Limit the camera's position to a reasonable range (e.g., a minimum and maximum distance from the player character)
        float minDistance = 5f; // Adjust as needed
        float maxDistance = 20f; // Adjust as needed
        gameCam.transform.position = new Vector3(
            gameCam.transform.position.x,
            gameCam.transform.position.y,
            Mathf.Clamp(gameCam.transform.position.z, -maxDistance, -minDistance)
        );

        // Check if the "q" key is held down

        // Ki charging logic
        if (Input.GetKey(KeyCode.Q))
        {
            ChargeKi();
            camImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = 0.3f; // Value of the camera shake
            camImpulseSource.GenerateImpulse();
            movement.enabled = false;
            canAttack = false;
        }
        else
        {
            StopKiCharge();
           // movement.enabled = true;
           // canAttack = true;
        }

        /*
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && !isDashing)
        {
            // Check if there is enough ATB to perform the dash
            if (atbSlider >= 20)
            {
                StartCoroutine(Dash());
            }
            else
            {
                Debug.Log("Not enough ATB for Dash.");
            }
        }
        */
        if (Input.GetKeyDown(KeyCode.V))
        {
            BreakItDown();
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Kikoken();
        }
        //Barrier
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleBarrier();
        }
        if (isBarrierActive)
        {
            // Increase the barrier active time
            barrierActiveTime += Time.deltaTime;

            // Adjust the barrier drain rate based on how long the barrier has been active
            barrierDrainRate = defaultBarrierDrainRate + (barrierActiveTime * 30.5f); // You can adjust the multiplier

            // Drain ATB based on the adjusted barrier drain rate
            ModifyATB(-barrierDrainRate * Time.deltaTime);
            if (atbSlider <= 0f)
            {
                DeactivateBarrier();
            }

        }
        else
        {
            // Reset barrier active time and drain rate when the barrier is deactivated
            barrierActiveTime = 0f;
            barrierDrainRate = defaultBarrierDrainRate;
        }

    }

    /*
    private void TeleportKick()
    {
        if (atbSlider >= 100 && !isTeleportOnCooldown)
        {
            // Deduct ATB points
            ModifyATB(-100);

            // Find the selected enemy (you can use your existing logic)

            // Calculate teleport destination position in front of the selected enemy
            Vector3 teleportDestination = CalculateTeleportDestination(selectedEnemy);

            // Play teleport sound
            PlayTeleportSound();

            // Teleport to the target's position
            MoveTowardsTarget(selectedEnemy);

            // Trigger the kick animation
            anim.SetTrigger("Kick");

            // Play the kick sound
            PlayKickSound();

            // Play voice line
            PlayVoiceLineForKick();

            // Set teleport cooldown
            isTeleportOnCooldown = true;
            lastTeleportTime = Time.time;

            // You may need to handle other game logic, such as damage to the enemy.
        }
        else
        {
            Debug.Log("Not enough ATB for teleport kick or still on cooldown.");
        }
    }
    */

    public void SwordsOfSparta()
    {
        if (atbSlider >= 200)
        {
            ModifyATB(-200);

            StartCoroutine(AbilityCooldown());

            SetTacticalMode(false);

                   isSwordsOfSpartaActive = true;
        swordsOfSpartaObject.SetActive(true);

        // Play the Swords of Sparta activation sound
        if (swordsOfSpartaActivationSound != null)
            swordsOfSpartaActivationSound.Play();

        // Disable the Swords of Sparta object after the specified duration
        StartCoroutine(DeactivateSwordsOfSparta());
        }
        else
        {
            Debug.Log("Not enough ATB for Hadouken.");
        }

    }


    private IEnumerator DeactivateSwordsOfSparta()
    {
        yield return new WaitForSeconds(swordsOfSpartaDuration);

        // Deactivate the Swords of Sparta object and reset the flag
        swordsOfSpartaObject.SetActive(false);
        isSwordsOfSpartaActive = false;
    }

    public void Kikoken()
    {
        if (atbSlider >= 100)
        {
            ModifyATB(-100);
            StartCoroutine(AbilityCooldown());
            SetTacticalMode(false);

            // Get the nearest enemy to the player
            int nearestTargetIndex = NearestTargetToCenter();
            if (nearestTargetIndex >= 0 && nearestTargetIndex < targets.Count)
            {
                Transform nearestEnemy = targets[nearestTargetIndex];

                // Calculate the direction from the player to the nearest enemy
                Vector3 lookDirection = nearestEnemy.position - playerCharacter.position;

                // Use Quaternion.LookRotation to smoothly rotate the player character's view
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

                // Apply the rotation to the player character's transform
                playerCharacter.rotation = lookRotation;
            }

            // Play the Kamehameha animation
            anim.SetTrigger("Kikoken");

            // Play the Kikoken sound
            if (KikokenSound != null)
                KikokenSound.Play();

            // Wait for a short delay to synchronize with the animation
            StartCoroutine(PerformKikokenWithDelay());
        }
        else
        {
            Debug.Log("Not enough ATB for Kikoken.");
        }
    }




    private IEnumerator PerformKikokenWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

        // Instantiate the kamehameha prefab at the kick position with the same rotation as the player
        GameObject kamehamehaObject = Instantiate(KikokenPrefab, projectilePosition.position, transform.rotation);
    }

    public void Hadouken()
    {
        //This is actually the kamehameha
        if (atbSlider >= 200)
        {
            ModifyATB(-200);

            StartCoroutine(AbilityCooldown());

            SetTacticalMode(false);

            // Create a new instance of the projectile
            GameObject newProjectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Get the projectile component and set its target
            Projectile projectile = newProjectile.GetComponent<Projectile>();
            if (projectile != null && isLockedOn && lockedTargetIndex >= 0 && lockedTargetIndex < targets.Count)
            {
                Transform lockedTarget = targets[lockedTargetIndex];
                projectile.SetTarget(lockedTarget);
            }

            // Animation
            anim.SetTrigger("Kamehameha");

            // Polish
            PlayVFX(abilityVFX, false);
            LightColor(groundLight, abilityColot, .3f);
            KamehamehaSound.Play();
        }
        else
        {
            Debug.Log("Not enough ATB for Hadouken.");
        }
    }

    public void LightningKicks()
    {
        if (atbSlider >= 200) // Check if the player has at least 200 ATB points
        {
            ModifyATB(-200); // Deduct 200 ATB points

            StartCoroutine(AbilityCooldown());

            SetTacticalMode(false);

            MoveTowardsTarget(targets[targetIndex]);

            // Animation
            anim.SetTrigger("LightningKicks");

            // Polish
            PlayVFX(abilityVFX, false);
            LightColor(groundLight, abilityColot, .3f);
        }
        else
        {
            // Display a message or perform some action to indicate that the player doesn't have enough ATB.
            Debug.Log("Not enough ATB for LightningKicks.");
        }
    }

    public void RoadHouse()
    {
        if (atbSlider >= 100) // Check if the player has at least 100 ATB points
        {
            ModifyATB(-100); // Deduct 100 ATB points

            StartCoroutine(AbilityCooldown());

            SetTacticalMode(false);

            MoveTowardsTarget(targets[targetIndex]);

            // Animation
            anim.SetTrigger("AirKick3");

            // Polish
            PlayVFX(abilityVFX, false);
            LightColor(groundLight, abilityColot, .3f);
            RoadHouseSound.Play();
        }
        else
        {
            // Display a message or perform some action to indicate that the player doesn't have enough ATB.
            Debug.Log("Not enough ATB for LightningKicks.");
        }
    }  
    
    public void BreakItDown()
    {
        if (atbSlider >= 100) // Check if the player has at least 100 ATB points
        {
            ModifyATB(-100); // Deduct 100 ATB points

            StartCoroutine(AbilityCooldown());

            SetTacticalMode(false);

            MoveTowardsTarget(targets[targetIndex]);

            // Animation
            anim.SetTrigger("BreakDance");

            // Polish
            PlayVFX(abilityVFX, false);
            LightColor(groundLight, abilityColot, .3f);
            RoadHouseSound.Play();
        }
        else
        {
            // Display a message or perform some action to indicate that the player doesn't have enough ATB.
            Debug.Log("Not enough ATB for LightningKicks.");
        }
    }

    public void Heal()
    {
        if (atbSlider >= 100)
        {
            // Deduct ATB points
            ModifyATB(-100);
            SetTacticalMode(false);
            // Calculate the amount to heal
            float healAmount = Mathf.Min(50, 100 - health); // Heal up to 50 or until reaching 100 health

            // Apply the healing
            health += healAmount;

            // Animation
            anim.SetTrigger("heal");

            // Activate the healing particles
            HealingParticle.SetActive(true);
            LightColor(groundLight, healColor, 0.5f);

            // Deactivate the particles after 2 seconds
            StartCoroutine(DeactivateHealingParticlesAfterDelay(2.0f));
        }
        else
        {
            Debug.Log("Not enough ATB for the Heal spell.");
        }
    }

    private void ToggleBarrier()
    {
        if (!isBarrierActive)
        {
            ActivateBarrier();
        }
        else
        {
            DeactivateBarrier();
        }
    }

    private void ActivateBarrier()
    {
        if (atbSlider >= barrierCost)
        {
            ModifyATB(-barrierCost);
            isBarrierActive = true;
            barrierObject.SetActive(true);

            // Disable player movement during barrier activation
            characterMovement.enabled = false;
        }
        else
        {
            Debug.Log("Not enough ATB for the Barrier.");
        }
    }

    private void DeactivateBarrier()
    {
        isBarrierActive = false;
        barrierObject.SetActive(false);

        // Enable player movement after barrier deactivation
        characterMovement.enabled = true;
    }

    private void ChargeKi()
    {
        if (!isChargingKi)
        {
            isChargingKi = true;
            anim.SetBool("chargingKI", true);
            KIchargingSound.Play();
            kiChargingParticle.SetActive(true);
            canAttack = false;
            characterMovement.enabled = false;
            Debug.Log("Movement script disabled.");
        }

       

        // Increase the ATB value while the button is held until it reaches the max
        if (atbSlider < filledAtbValue * 4 && !isBarrierActive)
        {
            ModifyATB(Time.deltaTime * atbRechargeRate); // Adjust atbRechargeRate as needed
        }
    }

    private void StopKiCharge()
    {
        if (isChargingKi)
        {
            isChargingKi = false;
            anim.SetBool("chargingKI", false); // Reset animation bool
            KIchargingSound.Stop(); // Disable the KI charging sound
            kiChargingParticle.SetActive(false);
            characterMovement.enabled = true;
            canAttack = true;
        }
    }


    private IEnumerator BarrierCooldown()
    {
        while (isBarrierActive)
        {
            yield return null;
        }

       
    }


    private IEnumerator DeactivateHealingParticlesAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HealingParticle.SetActive(false);
    }


    public void MoveTowardsTarget(Transform target)
{
    // Change the value of the last number one row below to change the automove distance
    if (Vector3.Distance(transform.position, target.position) > 1 && Vector3.Distance(transform.position, target.position) < 100)
    {
        StartCoroutine(DashCooldown());
        Vector3 newTargetPosition = TargetOffset(); // Calculate the target position to stand away from the target
        transform.DOMove(newTargetPosition, 0.5f); // Move the character to the new position
        transform.DOLookAt(targets[targetIndex].position, 0.2f); // Look at the target
    }
}



    IEnumerator AbilityCooldown()
    {
        usingAbility = true;
        yield return new WaitForSeconds(1f);
        usingAbility = false;
    }

    IEnumerator DashCooldown()
    {
        dashing = true;
        yield return new WaitForSeconds(1);
        dashing = false;
    }

    public Vector3 TargetOffset()
    {
        Vector3 position;
        position = targets[targetIndex].position;
        return Vector3.MoveTowards(position, transform.position, 1.2f);
    }

    public void HitTarget(Transform target)
    {
        OnModificationATB.Invoke();

        PlayVFX(sparkVFX, true);
        if (usingAbility)
            PlayVFX(abilityHitVFX, true, 4, 4, .3f);

        ModifyATB(25);

        LightColor(swordLight, sparkColor, .1f);

        if (target.GetComponent<EnemyScript>() != null)
        {
           // target.GetComponent<EnemyScript>().GetHit();

            // Debug logs to check health and removal
            Debug.Log("Enemy Health: " + target.GetComponent<EnemyScript>().health);

            // Check if the enemy is dead (health <= 0)
            if (target.GetComponent<EnemyScript>().health <= 0)
            {
                // Remove the enemy from the targets list
                if (targets.Contains(target))
                {
                    targets.Remove(target);
                    Debug.Log("Enemy Removed from Targets List");
                }

                // Destroy the enemy GameObject
                Destroy(target.gameObject);
                Debug.Log("Enemy Destroyed");
            }
        }
    }


    public void ModifyATB(float amount)
    {
        OnModificationATB.Invoke();

        atbSlider += amount;
        atbSlider = Mathf.Clamp(atbSlider, 0, (filledAtbValue * 4));

        if (amount > 0)
        {
            if (atbSlider >= filledAtbValue && atbCount == 0)
                atbCount = 1;
            if (atbSlider >= (filledAtbValue * 2) && atbCount == 1)
                atbCount = 2;
        }
        else
        {
            if (atbSlider <= filledAtbValue)
                atbCount = 0;
            if (atbSlider >= filledAtbValue && atbCount == 0)
                atbCount = 1;
        }

        OnModificationATB.Invoke();
    }

    public void ClearATB()
    {
        float value = (atbCount == 1) ? 0 : 1;
        atbSlider = value;
    }

    public void SetTacticalMode(bool on)
    {
        movement.desiredRotationSpeed = on ? 0 : .3f;
        movement.active = !on;

        tacticalMode = on;
        //movement.enabled = !on;

        if (!on)
        {
            SetAimCamera(false);
        }

        camImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = on ? 0 : 2;

        float time = on ? slowMotionTime : 1;
        Time.timeScale = time;

        //Polish
        DOVirtual.Float(on ? 0 : 1, on ? 1 : 0, .3f, SlowmotionPostProcessing).SetUpdate(true);

        OnTacticalTrigger.Invoke(on);
    }

    public void SelectTarget(int index)
    {
        if (index >= 0 && index < targets.Count)
        {
            Transform target = targets[index];

            // Check if the target is not null and not destroyed
            if (target != null)
            {
                targetIndex = index;
                aimObject.DOLookAt(target.position, 0.3f).SetUpdate(true);
            }
            else
            {
                // Handle the case where the target is null or destroyed
                // For example, you can print a debug message
                Debug.LogWarning("Target at index " + index + " is null or destroyed.");
            }
        }
        else
        {
            // Handle the case where the index is out of range
            // For example, you can print a debug message
            Debug.LogWarning("Invalid target index: " + index);
        }
    }


    public void SetAimCamera(bool on)
    {
        if (targets.Count < 1)
            return;

        OnTargetSelectTrigger.Invoke(on);

        targetCam.LookAt = on ? aimObject : null;
        targetCam.Follow = on ? aimObject : null;
        targetCam.gameObject.SetActive(on);
        isAiming = on;
    }

    IEnumerator RecenterCamera()
    {
        gameCam.GetComponent<CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = true;
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        gameCam.GetComponent<CinemachineFreeLook>().m_RecenterToTargetHeading.m_enabled = false;
    }

    public void PlayVFX(VisualEffect visualEffect, bool shakeCamera, float shakeAmplitude = 2, float shakeFrequency = 2, float shakeSustain = .2f)
    {
        if (visualEffect == abilityHitVFX)
            LightColor(groundLight, abilityColot, .2f);

        if (visualEffect == sparkVFX)
            visualEffect.SetFloat("PosX", VFXDir);
        visualEffect.SendEvent("OnPlay");

        camImpulseSource.m_ImpulseDefinition.m_AmplitudeGain = shakeAmplitude;
        camImpulseSource.m_ImpulseDefinition.m_FrequencyGain = shakeFrequency;
        camImpulseSource.m_ImpulseDefinition.m_TimeEnvelope.m_SustainTime = shakeSustain;

        if (shakeCamera)
            camImpulseSource.GenerateImpulse();
    }

    public void DirRight()
    {
        VFXDir = -5;
    }

    public void DirLeft()
    {
        VFXDir = 5;
    }
    public void HitEvent()
    {
        VFXDir = 5;
        WeaponObject.SetActive(true);
    }
    public void HitDisable()
    {
       
        WeaponObject.SetActive(false);
    }
    
    public void HitEventRightFist()
    {
        VFXDir = 5;
        RightFistObject.SetActive(true);
    }
    public void HitDisableRightFist()
    {

        RightFistObject.SetActive(false);
    }
    
    public void HitEventLeftFist()
    {
        VFXDir = 5;
        LeftFistObject.SetActive(true);
    }
    public void HitDisableLeftFist()
    {

        LeftFistObject.SetActive(false);
    }
    public void HitEventLeftLeg()
    {
        VFXDir = 5;
        LeftLegObject.SetActive(true);
    }
    public void HitDisableLeftLeg()
    {

        LeftLegObject.SetActive(false);
    }

    public void HitEventKamehameha()
    {
        VFXDir = 5;
        KamehamehaObject.SetActive(true);
    }
    public void HitDisableKamehameha()
    {

        KamehamehaObject.SetActive(false);
    }

    public void HitEventSword()
    {
        VFXDir = 5;
        SwordCollision.SetActive(true);
    }
    public void HitDisableSword()
    {

        SwordCollision.SetActive(false);
    }

    public void CancelAction()
    {
        if (!targetCam.gameObject.activeSelf && tacticalMode)
            SetTacticalMode(false);

        if (targetCam.gameObject.activeSelf)
            SetAimCamera(false);
    }

    int NearestTargetToCenter()
    {
        float[] distances = new float[targets.Count];
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        for (int i = 0; i < targets.Count; i++)
        {
            // Check if the target is null before accessing its position
            if (targets[i] != null)
            {
                distances[i] = Vector2.Distance(Camera.main.WorldToScreenPoint(targets[i].position), screenCenter);
            }
            else
            {
                // Handle the case where the target is null (destroyed)
                distances[i] = float.MaxValue; // Set a very large distance
            }
        }

        float minDistance = Mathf.Min(distances);
        int index = 0;

        for (int i = 0; i < distances.Length; i++)
        {
            if (minDistance == distances[i])
                index = i;
        }
        return index;
    }


    public void LightColor(Light l, Color x, float time)
    {
        l.DOColor(x, time).OnComplete(() => l.DOColor(Color.black, time));
    }
    public void SlowmotionPostProcessing(float x)
    {
        slowMotionVolume.weight = x;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Calculate the distance between the player and the potential target
            float distance = Vector3.Distance(playerTransform.position, other.transform.position);

            // Define your maximum detection range (adjust this value as needed)
            float maxDetectionRange = 10f;

            if (distance <= maxDetectionRange)
            {
                targets.Add(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (targets.Contains(other.transform))
            {
                targets.Remove(other.transform);
            }
        }
    }

    public void ToggleLockOnTarget(bool forward)
    {
        if (targets.Count == 0)
        {
            isLockedOn = false;
            anim.SetBool("LockOnAnimation", false);
            if (lockOnUITarget != null)
            {
                lockOnUITarget.SetTarget(null);
            }
            return;
        }

        bool previousLockState = isLockedOn; // Store the previous lock-on state

        isLockedOn = !isLockedOn;

        if (isLockedOn)
        {
            int newIndex = NearestTargetToCenter();
            if (newIndex != -1)
            {
                Transform lockedOnTarget = GetLockedOnTargetTransform(newIndex);
                targetIndex = newIndex;
                isLockedOn = true;

                SetCharacterViewToLockedTarget(targetIndex);

                if (lockOnUITarget != null)
                {
                    lockOnUITarget.SetTarget(lockedOnTarget);
                }
            }
            else
            {
                isLockedOn = false;
                anim.SetBool("LockOnAnimation", false);
            }
        }
        else
        {
            playerCharacter.rotation = originalCharacterRotation;

            if (lockOnUITarget != null)
            {
                lockOnUITarget.SetTarget(null);
            }
        }

        if (previousLockState != isLockedOn || !isLockedOn)
        {
            UpdateExistingTargetSystem(lockedTargetIndex);
        }

        UpdateLockOnUI();
    }


    private void UpdateLockOnUI()
    {
        if (isLockedOn)
        {
            Transform lockedOnTarget = GetLockedOnTargetTransform(targetIndex);

            if (lockedOnTarget != null && lockOnUITarget != null)
            {
                lockOnUITarget.SetTarget(lockedOnTarget);
            }
            else
            {
                // Handle when there is no locked-on target
                if (lockOnUITarget != null)
                {
                    lockOnUITarget.SetTarget(null);
                }
            }
        }
        else
        {
            if (lockOnUITarget != null)
            {
                lockOnUITarget.SetTarget(null);
            }
        }
    }



    private Transform GetLockedOnTargetTransform(int targetIndex)
    {
        if (targetIndex >= 0 && targetIndex < targets.Count)
        {
            return targets[targetIndex];
        }

        return null; // Return null if the targetIndex is out of bounds
    }




    private void SetCharacterViewToLockedTarget(int targetIndex)
    {
        if (targetIndex >= 0 && targetIndex < targets.Count)
        {
            Transform target = targets[targetIndex];

            if (target != null)
            {
                // Calculate the direction from the character to the target
                Vector3 lookDirection = target.position - playerCharacter.position;

                // Use Quaternion.LookRotation to smoothly rotate the character's view
                Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);

                // Apply the rotation to the character's transform or camera
                playerCharacter.rotation = lookRotation;
            }
        }
    }
    

    public void UpdateExistingTargetSystem(int lockedTargetIndex)
    {
        if (lockedTargetIndex >= 0 && lockedTargetIndex < targets.Count)
        {
            Transform lockedTarget = targets[lockedTargetIndex];

            if (lockedTarget != null && playerTransform != null)
            {
                if (Input.GetKey(KeyCode.Tab) || Input.GetButtonDown("R3")) // Keep looking at the target while Tab is pressed
                {
                    Vector3 lookDirection = lockedTarget.position - playerCharacter.position;
                    Quaternion lookRotation = Quaternion.LookRotation(lookDirection, Vector3.up);
                    playerCharacter.rotation = lookRotation;
                }
                else
                {
                    // Stop looking at the target when Tab is released
                    // You can optionally reset the player's rotation to its original state here
                    // playerCharacter.rotation = originalRotation;
                }
            }
        }
    }

    IEnumerator ComboCooldown()
    {
        // Wait for half a second before allowing the next attack
        yield return new WaitForSeconds(0.4f);
        canAttack = true; // Re-enable attacks
    }

    public void GetHit(float damage)
    {
        if (!isBarrierActive)
        {
            anim.SetTrigger("Hit");
            health -= 10;
            if (health <= 0f)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
        // Play death audio or perform other actions here

        // Spawn the ragdoll prefab at the same position as the enemy
        GameObject ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);

        // Calculate the force direction (opposite of the enemy's forward direction)
        Vector3 forceDirection = -transform.forward;

        // Apply a random force within the specified range
        float randomForceMagnitude = ragdollForce + Random.Range(-forceRandomRange, forceRandomRange);
        Vector3 appliedForce = forceDirection * randomForceMagnitude;

        // Get all rigidbodies in the ragdoll
        Rigidbody[] ragdollRigidbodies = ragdoll.GetComponentsInChildren<Rigidbody>();

        // Apply the force to each rigidbody in the ragdoll
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.AddForce(appliedForce);
        }
    }

}
    






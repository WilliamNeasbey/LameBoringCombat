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

    private CharacterMovement movement;
    private Animator anim;
    public WeaponCollision weapon;
    public GameObject WeaponObject;
    public Transform playerCharacter;
    public Transform playerTransform;
    private Quaternion originalCharacterRotation;
    public float rotationSpeed = 5.0f; // Adjust the rotation speed as needed


    [Header("Time Stats")]
    public float slowMotionTime = .005f;

    [Space]

    public bool tacticalMode;
    public bool isAiming;
    public bool usingAbility;
    public bool dashing;

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

    [Space]
    [Header("VFX")]
    public VisualEffect sparkVFX;
    public VisualEffect abilityVFX;
    public VisualEffect abilityHitVFX;
    public VisualEffect healVFX;
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
    private bool isLockedOn = false;

    [Space]

    public Volume slowMotionVolume;

    public float VFXDir = 5;

    private CinemachineImpulseSource camImpulseSource;

    private void Start()
    {
        originalCharacterRotation = playerCharacter.rotation;
        weapon.onHit.AddListener((target) => HitTarget(target));
        movement = GetComponent<CharacterMovement>();
        anim = GetComponent<Animator>();
        camImpulseSource = Camera.main.GetComponent<CinemachineImpulseSource>();
    }

    void Update()
    {
        if (isLockedOn)
        {
            if (targets.Count > 0 && !tacticalMode && !usingAbility)
            {
                targetIndex = NearestTargetToCenter();
                Vector3 targetPosition = targets[targetIndex].position;
                targetPosition.y = transform.position.y; // Keep the same height level
                transform.LookAt(targetPosition);
            }
        }

        //Attack
        if ((Input.GetMouseButtonDown(0)) && !tacticalMode && !usingAbility)
        {

            OnAttack.Invoke();

            if (!dashing)
                //MoveTowardsTarget(targets[targetIndex]);
                anim.SetTrigger("AirKick");
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleLockOnTarget(true); // Cycle forward when Tab is pressed
        }

        if (Input.GetMouseButtonDown(1) && !usingAbility)
        {
            if (atbCount > 0 && !tacticalMode)
                SetTacticalMode(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelAction();
        }
    }

    public void LightningKicks()
    {
        ModifyATB(-200);

        StartCoroutine(AbilityCooldown());

        SetTacticalMode(false);

        MoveTowardsTarget(targets[targetIndex]);

        //Animation
        anim.SetTrigger("LightningKicks");

        //Polish
        PlayVFX(abilityVFX, false);
        LightColor(groundLight, abilityColot, .3f);
    }

    public void Heal()
    {
        ModifyATB(-100);

        StartCoroutine(AbilityCooldown());

        SetTacticalMode(false);

        //Animation
        anim.SetTrigger("heal");

        //Polish
        PlayVFX(healVFX, false);
        LightColor(groundLight, healColor, .5f);
    }

    public void MoveTowardsTarget(Transform target)
    {
        if (Vector3.Distance(transform.position, target.position) > 1 && Vector3.Distance(transform.position, target.position) < 10)
        {
            StartCoroutine(DashCooldown());
            transform.DOMove(TargetOffset(), .5f);
            transform.DOLookAt(targets[targetIndex].position, .2f);
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
            target.GetComponent<EnemyScript>().GetHit();

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
        atbSlider = Mathf.Clamp(atbSlider, 0, (filledAtbValue * 2));

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
            targets.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (targets.Contains(other.transform))
                targets.Remove(other.transform);
        }
    }

    public void ToggleLockOnTarget(bool forward)
    {
        if (targets.Count == 0)
            return;

        isLockedOn = !isLockedOn; // Toggle lock-on state

        if (isLockedOn)
        {
            // If lock-on is enabled, set the character's rotation to face the locked target
            SetCharacterViewToLockedTarget(targetIndex);
        }
        else
        {
            // If lock-on is disabled, reset the character's rotation to its original direction
            playerCharacter.rotation = originalCharacterRotation;
        }

        // Call the method to update your existing target system with the new lockedTargetIndex
        UpdateExistingTargetSystem(lockedTargetIndex);
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
    private void UpdateLockOnUI(int targetIndex)
    {
        // Implement logic to update the UI here, e.g., highlight the locked target
        // You can access UI elements or call UI-related functions from here
    }

    public void UpdateExistingTargetSystem(int lockedTargetIndex)
    {
        if (lockedTargetIndex >= 0 && lockedTargetIndex < targets.Count)
        {
            Transform lockedTarget = targets[lockedTargetIndex];

            if (lockedTarget != null && playerTransform != null)
            {
                if (Input.GetKey(KeyCode.Tab)) // Keep looking at the target while Tab is pressed
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


}
    






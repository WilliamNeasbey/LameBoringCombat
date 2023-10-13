using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{

    private TacticalMode gameScript;
    public Image test;
    public CanvasGroup tacticalCanvas;
    public CanvasGroup attackCanvas;
    public CanvasGroup attackCanvas2;

    public Transform commandsGroup;
    public Transform targetGroup;

    public CanvasGroup aimCanvas;
    public bool aimAtTarget;

    public Slider atbSlider;
    public Image atbComplete1;
    public Image atbComplete2;
    public Image atbComplete3;
    public Image atbComplete4;
    
    void Start()
    {

        gameScript = FindObjectOfType<TacticalMode>();
        gameScript.OnAttack.AddListener(() => AttackAction());
        gameScript.OnModificationATB.AddListener(() => UpdateSlider());
        gameScript.OnTacticalTrigger.AddListener((x) => ShowTacticalMenu(x));
        gameScript.OnTargetSelectTrigger.AddListener((x) => ShowTargetOptions(x));
    }

    private void Update()
    {
        if (gameScript != null && gameScript.targets.Count > gameScript.targetIndex)
        {
            Transform target = gameScript.targets[gameScript.targetIndex];

            // Check if the target and its position are valid
            if (target != null)
            {
                Vector3 targetPosition = target.position + Vector3.up;
                aimCanvas.transform.position = Camera.main.WorldToScreenPoint(targetPosition);
            }
            else
            {
                // Handle the case where the target is null or destroyed
                // You might want to remove the target from the list or take other appropriate action
                aimCanvas.alpha = 0; // Hide the aim icon
            }
        }
    }



    public void AttackAction()
    {

    }

    public void UpdateSlider()
    {
        atbSlider.DOComplete();
        atbSlider.DOValue(gameScript.atbSlider, .15f);

        atbComplete1.DOFade(gameScript.atbSlider >= 100 ? 1 : 0, .2f);
        atbComplete2.DOFade(gameScript.atbSlider >= 200 ? 1 : 0, .2f);
        atbComplete3.DOFade(gameScript.atbSlider >= 300 ? 1 : 0, .2f);
        atbComplete4.DOFade(gameScript.atbSlider >= 400 ? 1 : 0, .2f);
    }

    public void ShowTacticalMenu(bool on)
    {
        tacticalCanvas.DOFade(on ? 1 : 0, .15f).SetUpdate(true);
        tacticalCanvas.interactable = on;
        attackCanvas.DOFade(on ? 0 : 1, .15f).SetUpdate(true);
        attackCanvas2.DOFade(on ? 0 : 1, .15f).SetUpdate(true);
        attackCanvas.interactable = !on;

        EventSystem.current.SetSelectedGameObject(null);

        if (on == true)
        {
            EventSystem.current.SetSelectedGameObject(tacticalCanvas.transform.GetChild(0).GetChild(0).gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(attackCanvas.transform.GetChild(0).gameObject);
            commandsGroup.gameObject.SetActive(!on);
            //targetGroup.gameObject.SetActive(on);
        }
    }

    public void ShowTargetOptions(bool on)
    {
        EventSystem.current.SetSelectedGameObject(null);

        aimAtTarget = on;
        aimCanvas.alpha = on ? 1 : 0;

        commandsGroup.gameObject.SetActive(!on);

        if (gameScript != null)
        {
            if (gameScript.lockedTargetIndex >= 0 && gameScript.lockedTargetIndex < gameScript.targets.Count)
            {
                Transform target = gameScript.targets[gameScript.lockedTargetIndex];

                if (target != null)
                {
                    // Update the aim icon's position to be above the locked target
                    Vector3 targetPosition = target.position + Vector3.up;
                    aimCanvas.transform.position = Camera.main.WorldToScreenPoint(targetPosition);
                }
            }
        }

        targetGroup.GetComponent<CanvasGroup>().DOFade(on ? 1 : 0, .1f).SetUpdate(true);
        targetGroup.GetComponent<CanvasGroup>().interactable = on;

        if (on)
        {
            for (int i = 0; i < targetGroup.childCount; i++)
            {
                if (gameScript != null && gameScript.targets.Count - 1 >= i)
                {
                    Transform target = gameScript.targets[i];
                    if (target != null)
                    {
                        targetGroup.GetChild(i).GetComponent<CanvasGroup>().alpha = 1;
                        targetGroup.GetChild(i).GetComponent<CanvasGroup>().interactable = true;
                        targetGroup.GetChild(i).GetComponentInChildren<TextMeshProUGUI>().text = target.name;
                    }
                    else
                    {
                        // Handle the case where the target is null or destroyed
                        // You might want to remove the target from the list or take other appropriate action
                        targetGroup.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
                        targetGroup.GetChild(i).GetComponent<CanvasGroup>().interactable = false;
                    }
                }
                else
                {
                    targetGroup.GetChild(i).GetComponent<CanvasGroup>().alpha = 0;
                    targetGroup.GetChild(i).GetComponent<CanvasGroup>().interactable = false;
                }
            }
        }
        EventSystem.current.SetSelectedGameObject(on ? targetGroup.GetChild(0).gameObject : commandsGroup.GetChild(0).gameObject);
    }




}
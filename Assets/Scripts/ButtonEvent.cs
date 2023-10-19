using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections;

public class ButtonEvent : MonoBehaviour, ISubmitHandler, ISelectHandler, IDeselectHandler
{

    public UnityEvent Confirm;
    public UnityEvent Select;
    public ButtonType buttonType = ButtonType.None;

    private Vector3 pos;

    public enum ButtonType
    {
        None,
        ButtonA,
        ButtonB,
        ButtonC,
        ButtonD,
        ButtonE,
        // Add more button types as needed
    }
    public UnityEvent ButtonAConfirmEvent;
    public UnityEvent ButtonBConfirmEvent;
    public UnityEvent ButtonCConfirmEvent;
    public UnityEvent ButtonDConfirmEvent;
    public UnityEvent ButtonEConfirmEvent;
    private void Start()
    {
        pos = transform.position;
        StartCoroutine(WaitForFrames());
    }
    
    public void SetA()
    {
        buttonType = ButtonType.ButtonA;
    }
    public void SetB()
    {
        buttonType = ButtonType.ButtonB;
    }

    public void SetC()
    {
        buttonType = ButtonType.ButtonC;
    }
    
    public void SetD()
    {
        buttonType = ButtonType.ButtonD;
    } 
    
    public void SetE()
    {
        buttonType = ButtonType.ButtonE;
    }
   
    IEnumerator WaitForFrames()
    {
        yield return new WaitForSecondsRealtime(.5f);
        pos = transform.position;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        transform.DOMove(pos, .2f).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        transform.DOMove(pos+ (Vector3.right*10), .2f).SetEase(Ease.InOutSine).SetUpdate(true);
        Select.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        transform.DOPunchPosition(Vector3.right, .2f, 10, 1).SetUpdate(true);

        // Check the button type and invoke the corresponding Confirm event
        switch (buttonType)
        {
            case ButtonType.ButtonA:
                ButtonAConfirmEvent.Invoke();
                break;

            case ButtonType.ButtonB:
                ButtonBConfirmEvent.Invoke();
                break;

            case ButtonType.ButtonC:
                ButtonCConfirmEvent.Invoke();
                break;
            
            case ButtonType.ButtonD:
                ButtonDConfirmEvent.Invoke();
                break;
            
            case ButtonType.ButtonE:
                ButtonEConfirmEvent.Invoke();
                break;

            // Add cases for other button types as needed

            default:
                // If no specific button type is set, invoke the default Confirm event
                Confirm.Invoke();
                break;
        }
    }


}

using UnityEngine;

public class ChangeAnimatorController : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component you want to control.

    public RuntimeAnimatorController newController; // Reference to the new Animator controller.

    // Function to change the Animator controller when the button is pressed.
    public void ChangeControllerOnClick()
    {
        if (animator != null && newController != null)
        {
            animator.runtimeAnimatorController = newController;
        }
        else
        {
            Debug.LogError("Animator or new Animator controller not assigned!");
        }
    }
}

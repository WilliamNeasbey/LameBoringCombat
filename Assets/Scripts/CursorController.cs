using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public bool isCursorLocked = true;

    // Call this method to lock and hide the cursor
    public void LockAndHideCursor()
    {
        isCursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Call this method to unlock and show the cursor
    public void UnlockAndShowCursor()
    {
        isCursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Toggle cursor state when this script is enabled
    void OnEnable()
    {
        if (isCursorLocked)
        {
            LockAndHideCursor();
        }
        else
        {
            UnlockAndShowCursor();
        }
    }
}

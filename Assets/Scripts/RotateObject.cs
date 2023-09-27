using UnityEngine;

public class DragRotateObject : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private bool isRotating = false;

    private void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
        isRotating = true;
    }

    private void OnMouseDrag()
    {
        if (!isRotating)
            return;

        Vector3 currentScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenPoint) + offset;

        // Calculate the difference in mouse position between frames
        Vector3 mouseDelta = currentPosition - transform.position;

        // Calculate the rotation amount based on mouse movement
        float rotationAmount = Mathf.Clamp(mouseDelta.x + mouseDelta.y, -0.3f, 0.3f);

        // Apply the rotation
        transform.Rotate(Vector3.up, rotationAmount, Space.World);
    }

    private void OnMouseUp()
    {
        isRotating = false;
    }
}

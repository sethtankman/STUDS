using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

/// <summary>
/// CameraMove made more like Fall Guys
/// </summary>
public class CameraMoveFG : MonoBehaviour
{
    public float rotationSpeedLateral, rotationSpeedVertical;
    public Transform target, player, tracker;
    float mouseX, mouseY;
    public float minY, maxY, playerRotation;
    private Vector2 direction;
    private bool obstructed;

    void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        float horizontalInput = direction.x;
        playerRotation += horizontalInput;
        Control(playerRotation);
        /*
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out hit, 3.5f))
        {
            if (hit.transform.tag != "Player")
            {
                tracker.gameObject.GetComponent<UCPTracker>().obstructed = true;
                Debug.Log("I am not pointed at the player");
                transform.Translate(target.position - transform.position);
            }
        }
        */
        //For editor only!
        /*if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }*/
    }

    void Control(float horizontalInput)
    {
        mouseX += direction.x * rotationSpeedLateral;
        mouseY -= direction.y * rotationSpeedVertical;
        mouseY = Mathf.Clamp(mouseY, minY, maxY);

        transform.LookAt(target);
        //Debug.Log("MouseX: " + mouseX + " MouseY: " + mouseY);
        target.rotation = Quaternion.Euler(mouseY, mouseX + horizontalInput, 0);
    }
    public void SetDirectionVector(Vector2 dir)
    {
        //Debug.Log("Direction Vector set to" + dir.x + " " + dir.y);
        direction = dir;
    }
}

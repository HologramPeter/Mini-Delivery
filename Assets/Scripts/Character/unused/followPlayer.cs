using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    //public Transform playerBody;
    float xRotation = 0f;

    public Transform player;

    public Vector3 offset;
    bool firstPersonMode = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    // Snap the camera in each frame
    void Update()
    {
        

        if (!firstPersonMode)
        {
            //Make the camera a little bit upper the player => tune offset in unity
            transform.position = player.position + offset;
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                firstPersonMode = true;
            }
        }
        else
        {
            //Cursor.lockState = CursorLockMode.Locked;
            transform.position = player.position; 
            float MouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float MouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= MouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            player.Rotate(Vector3.up * MouseX);
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                firstPersonMode = false;
            }
        }
    }
}




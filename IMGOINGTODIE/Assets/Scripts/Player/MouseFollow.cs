using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    public float mouseSens;

   public float xRotate;
    public float yRotate;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        //rotation on x and Y axis 

        xRotate -= mouseY;
        yRotate += mouseX;

        //Limiting Rotation 
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);
        yRotate = Mathf.Clamp(0, yRotate, 0);


        //apply rotations to the transform
        transform.localRotation = Quaternion.Euler(xRotate, yRotate, 0f);

    }
}

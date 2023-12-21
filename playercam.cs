using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playercam : MonoBehaviour
{
    public float xsen;
    public float ysen;

    public Transform camorient;

    float xrotate;
    float yrotate;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mousex = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xsen;
        float mousey = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ysen;

        yrotate += mousex;

        xrotate -= mousey;

        xrotate = Mathf.Clamp(xrotate, -90f, 90f);

        transform.rotation = Quaternion.Euler(xrotate, yrotate, 0);
        camorient.rotation = Quaternion.Euler(0 , yrotate, 0);
    }
}

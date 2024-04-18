using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    [SerializeField]
    private Joystick joystick;
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private GameObject parentCamera;

    private float rotHAmt;
    private float rotVAmt;
    public float rotationSpeed = 35f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        rotVAmt = joystick.Vertical;
        rotHAmt = joystick.Horizontal;

        parentCamera.transform.Rotate(0, rotHAmt * rotationSpeed * Time.deltaTime, 0);
        camera.transform.Rotate(-rotVAmt * rotationSpeed * Time.deltaTime, 0, 0);
    }
}

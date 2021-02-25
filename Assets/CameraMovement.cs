using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject mainCam;
    public float camSpeed = 10.0f, rotationSpeed = 10.0f;
    public BridgeCreator thisBridgeCreator;
    private bool rotationLocked, leftViewControls, topViewControls, bottomViewControls;
    
    // Start is called before the first frame update
    void Start()
    {
        rotationLocked = true;
        leftViewControls = false;
        topViewControls = false;
        bottomViewControls = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (thisBridgeCreator.ViewDirection == BridgeCreator.cameraDirection.Left)
            leftViewControls = true;
        else
            leftViewControls = false;
        if (thisBridgeCreator.ViewDirection == BridgeCreator.cameraDirection.Top)
            topViewControls = true;
        else
            topViewControls = false;
        if (thisBridgeCreator.ViewDirection == BridgeCreator.cameraDirection.Bottom)
            bottomViewControls = true;
        else
            bottomViewControls = false;

        if (Input.GetKeyDown(KeyCode.Space))
            rotationLocked = false;
        if (Input.GetKeyUp(KeyCode.Space))
            rotationLocked = true;

        if (Input.GetKey(KeyCode.D))
        {
            if (!rotationLocked)
                mainCam.transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0), Space.Self);
            else
            {
                if (leftViewControls == false)
                    mainCam.transform.position += (new Vector3(camSpeed * Time.deltaTime, 0, 0));
                else
                    mainCam.transform.position -= (new Vector3(camSpeed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (!rotationLocked)
                mainCam.transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0), Space.Self);

            else
            {
                if (leftViewControls == false)
                    mainCam.transform.position += (new Vector3(-camSpeed * Time.deltaTime, 0, 0));
                else
                    mainCam.transform.position -= (new Vector3(-camSpeed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (!rotationLocked)
                mainCam.transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0, 0), Space.Self);
            else
            {
                if (topViewControls == true)
                    mainCam.transform.position += (new Vector3(0, 0, -camSpeed * Time.deltaTime));
                else if (bottomViewControls == true)
                    mainCam.transform.position += (new Vector3(0, 0, camSpeed * Time.deltaTime));
                else
                    mainCam.transform.position += (new Vector3(0, -camSpeed * Time.deltaTime, 0));
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (!rotationLocked)
                mainCam.transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0, 0), Space.Self);
            else
            {
                if (topViewControls == true)
                    mainCam.transform.position += (new Vector3(0, 0, camSpeed * Time.deltaTime));
                else if (bottomViewControls == true)
                    mainCam.transform.position += (new Vector3(0, 0, -camSpeed * Time.deltaTime));
                else
                    mainCam.transform.position += (new Vector3(0, camSpeed * Time.deltaTime, 0));
            }
        }
    }
}

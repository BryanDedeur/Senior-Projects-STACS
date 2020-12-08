using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountCam : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject rightMount, leftMount, topMount, bottomMount;
    int camCount = 0;
    Quaternion target = Quaternion.Euler(0, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        mainCamera.transform.position = rightMount.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            camCount = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            camCount = 2;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            camCount = 3;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            camCount = 0;
        }
        UpdateCam(camCount);
    }

    void UpdateCam(int camNum)
    {
        if (camNum == 0)
        {
            mainCamera.transform.rotation = target;
            mainCamera.transform.rotation = rightMount.transform.rotation;
            mainCamera.transform.position = rightMount.transform.position;
        }
        if (camNum == 1)
        {
            mainCamera.transform.rotation = target;
            mainCamera.transform.rotation = topMount.transform.rotation;
            mainCamera.transform.position = topMount.transform.position;
        }
        if (camNum == 2)
        {
            mainCamera.transform.rotation = target;
            mainCamera.transform.rotation = bottomMount.transform.rotation;
            mainCamera.transform.position = bottomMount.transform.position;
        }
        if (camNum == 3)
        {
            mainCamera.transform.rotation = target;
            mainCamera.transform.rotation = leftMount.transform.rotation;
            mainCamera.transform.position = leftMount.transform.position;
        }
    }
}

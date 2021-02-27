using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bridge Creator Camera Controller
public class BCCameraController : MonoBehaviour
{
    public static BCCameraController instance;
    public enum CameraDirection
    {
        Left, Right, Top, Bottom
    }

    public bool testbool;
    public float deltaX = 1;
    public float deltaY = 1;
    public float camSpeed = 20.0f;
    public float rotationSpeed = 12.0f;

    private bool rotationLocked, leftViewControls, topViewControls, bottomViewControls;

    private float offset = 100;
    private CameraDirection viewDirection;

    private void Awake()
    {
        // this keeps instance a singlton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        RightView();
    }

    public void TopView()
    {
        viewDirection = CameraDirection.Top;
        transform.localPosition = new Vector3(0, 1, 0) * offset;
        transform.LookAt(Vector3.zero);

        //viewHighlight.transform.position = topButton.transform.position;
    }

    public void BottomView()
    {
        viewDirection = CameraDirection.Bottom;
        transform.localPosition = new Vector3(0, -1, 0) * offset;
        transform.LookAt(Vector3.zero);

        //viewHighlight.transform.position = bottomButton.transform.position;
    }

    public void LeftView()
    {
        viewDirection = CameraDirection.Left;
        transform.localPosition = new Vector3(.25f, .25f, 1) * offset;
        transform.LookAt(Vector3.zero);

        // viewHighlight.transform.position = leftButton.transform.position;
    }

    public void RightView()
    {
        viewDirection = CameraDirection.Right;
        transform.localPosition = new Vector3(.25f, .25f, -1) * offset;
        transform.LookAt(Vector3.zero);

        //viewHighlight.transform.position = rightButton.transform.position;
    }

    private void OnMouseDown()
    {
        if (testbool)
        {
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
        }
    }

    private void OnMouseDrag()
    {
        if (testbool)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x - deltaX, mousePosition.y - deltaY, mousePosition.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            testbool = true;
        }

        if (viewDirection == CameraDirection.Left)
            leftViewControls = true;
        else
            leftViewControls = false;
        if (viewDirection == CameraDirection.Top)
            topViewControls = true;
        else
            topViewControls = false;
        if (viewDirection == CameraDirection.Bottom)
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
                transform.Rotate(new Vector3(0, rotationSpeed * Time.deltaTime, 0), Space.Self);
            else
            {
                if (leftViewControls == false)
                    transform.position += (new Vector3(camSpeed * Time.deltaTime, 0, 0));
                else
                    transform.position -= (new Vector3(camSpeed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (!rotationLocked)
                transform.Rotate(new Vector3(0, -rotationSpeed * Time.deltaTime, 0), Space.Self);

            else
            {
                if (leftViewControls == false)
                    transform.position += (new Vector3(-camSpeed * Time.deltaTime, 0, 0));
                else
                    transform.position -= (new Vector3(-camSpeed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (!rotationLocked)
                transform.Rotate(new Vector3(rotationSpeed * Time.deltaTime, 0, 0), Space.Self);
            else
            {
                if (topViewControls == true)
                    transform.position += (new Vector3(0, 0, -camSpeed * Time.deltaTime));
                else if (bottomViewControls == true)
                    transform.position += (new Vector3(0, 0, camSpeed * Time.deltaTime));
                else
                    transform.position += (new Vector3(0, -camSpeed * Time.deltaTime, 0));
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (!rotationLocked)
                transform.Rotate(new Vector3(-rotationSpeed * Time.deltaTime, 0, 0), Space.Self);
            else
            {
                if (topViewControls == true)
                    transform.position += (new Vector3(0, 0, camSpeed * Time.deltaTime));
                else if (bottomViewControls == true)
                    transform.position += (new Vector3(0, 0, -camSpeed * Time.deltaTime));
                else
                    transform.position += (new Vector3(0, camSpeed * Time.deltaTime, 0));
            }
        }
    }
}

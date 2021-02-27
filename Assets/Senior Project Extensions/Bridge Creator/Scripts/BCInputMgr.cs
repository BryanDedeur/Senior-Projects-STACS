using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BCInputMgr : MonoBehaviour
{
    public static BCInputMgr instance;

    private void Awake()
    {
        // this keeps instance a singlton
        if (instance == null)
        {
            instance = this;
        } else if (instance != null)
        {
            Destroy(this);
        }
    }

    public Vector3 Snap(Vector3 p)
    {
        return new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), Mathf.Round(p.z));
    }

    public RaycastHit RaycastFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            // something was hit
            hit.point = Snap(hit.point);
        }
        return hit;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            RaycastHit hit = RaycastFromMouse();

            if (Input.GetKey(KeyCode.LeftControl))
            {
                BCSelectionMgr.instance.UnitSelect(hit);
            } else
            {
                BridgeCreator.instance.AttemptAddVertexAndConnectEdge(hit);
            }

        }

        if (Input.GetMouseButtonUp(0))
        {
            RaycastHit hit = RaycastFromMouse();

            //BridgeCreator.instance.AdjustDraggedVertex(hit);
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            RaycastHit hit = RaycastFromMouse();

            BridgeCreator.instance.AttemptRemoveEdgeOrVertex(hit);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) 
        {
            BCSelectionMgr.instance.EnableBoxSelection();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            BCSelectionMgr.instance.DisableBoxSelection();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            BridgeCreator.instance.EnableMirroring();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            BridgeCreator.instance.DisableMirroring();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RaycastHit hit = RaycastFromMouse();

            BridgeCreator.instance.MoveSelected(hit);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BridgeCreator.instance.MirrorSelectedObjects();
        }


    }

}

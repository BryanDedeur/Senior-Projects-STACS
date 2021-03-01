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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                BCSelectionMgr.instance.UnitSelect();
            } else
            {
                BridgeCreator.instance.AttemptAddVertexAndConnectEdge();
            }
        }

        if (Input.GetMouseButton(0)) // fires every frame
        {
            BridgeCreator.instance.AttemptMoveSelected();
        }

        if (Input.GetMouseButtonUp(0))
        {
            BridgeCreator.instance.EndMovingSelected();
            //BridgeCreator.instance.AdjustDraggedVertex(hit);
        }

        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            BridgeCreator.instance.AttemptRemoveEdgeOrVertex();
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

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BridgeCreator.instance.MirrorSelectedObjects();
        }


    }

}

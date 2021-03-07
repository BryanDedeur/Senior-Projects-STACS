using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class BCInputMgr : MonoBehaviour
{
    public static BCInputMgr instance;
    public bool leftClickState = false; //false to indicate normal left click true to indicate box slection

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
        if (!EventSystem.current.IsPointerOverGameObject())
        {

            if (Input.GetMouseButtonDown(0) && !leftClickState) // Left mouse button
            {
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    BCSelectionMgr.instance.UnitSelect();
                }
                else
                {
                    BridgeCreator.instance.AttemptAddVertexAndConnectEdge();
                }
            }

            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftShift) == false) // fires every frame
            {
                BridgeCreator.instance.AttemptMoveSelected();
                leftClickState = false;
            }

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
            leftClickState = true;
   
        }
        if(Input.GetMouseButtonDown(0) && leftClickState && Input.GetKey(KeyCode.LeftControl) == false)
        {
            BCSelectionMgr.instance.EnableBoxSelection();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            //leftClickState = false;
            BCSelectionMgr.instance.DisableHighLight();
          //  BCSelectionMgr.instance.DisableBoxSelection();
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
            BridgeCreator.instance.CopySelectedObjects();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            BridgeCreator.instance.MirrorSelectedObjects();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BCSelectionMgr.instance.DisableBoxSelection();
            BCSelectionMgr.instance.DeselectAll();

        }


    }
    public static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

}

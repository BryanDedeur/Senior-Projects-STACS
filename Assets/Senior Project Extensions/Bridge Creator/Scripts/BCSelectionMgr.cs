using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BCSelectionMgr : MonoBehaviour
{
    public static BCSelectionMgr instance;

    public float boxBorderThickness = 2;
    public Color boxBorderColor;
    public Color boxFillColor;

    public bool boxSelecting;

    private Vector2 startPos;
    private Vector2 endPos;
    public List<Transform> selectedObjects;

    private void Awake()
    {
        // this keeps instance a singlton
        if (instance == null)
        {
            instance = this;
            selectedObjects = new List<Transform>();
        }
        else if (instance != null)
        {
            Destroy(this);
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        var camera = Camera.main;
        var viewportBounds = Box.GetViewportBounds(camera, startPos, Input.mousePosition);
        if(gameObject.transform.position.z == -4)
        {
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }
        return false;
        
    }

    public bool IsWithinSelectionBoundsVec(Vector3 p)
    {
        var camera = Camera.main;
        var viewportBounds = Box.GetViewportBounds(camera, startPos, Input.mousePosition);
        if(p.z == -4)
        {
            return viewportBounds.Contains(camera.WorldToViewportPoint(p));
        }
        return false;
    }

    public void EnableBoxSelection()
    {
            startPos = Input.mousePosition;
            boxSelecting = true;
    }

    public void DisableBoxSelection()
    {
        boxSelecting = false;
        bool somethingSelected = false;
        foreach (var pair in Bridge.instance.vertices)
        {
            if (IsWithinSelectionBounds(pair.Value))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }

        foreach (var pair in Bridge.instance.edges)
        {
            if (IsWithinSelectionBounds(pair.Value.gameObject))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }

        if (!somethingSelected)
        {
            DeselectAll();
        }
    }

    public void DisableHighLight()
    {
        boxSelecting = false;
        bool somethingSelected = false;
        foreach (var pair in Bridge.instance.vertices)
        {
            if (IsWithinSelectionBounds(pair.Value))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }

        foreach (var pair in Bridge.instance.edges)
        {
            if (IsWithinSelectionBounds(pair.Value.gameObject))
            {
                somethingSelected = AdjustSelectedObjects(pair.Value.transform);
            }
        }
    }

    public bool AdjustSelectedObjects(Transform trans)
    {
        if (selectedObjects.Contains(trans))
        {
            selectedObjects.Remove(trans);
            Renderer rend = trans.GetComponent<Renderer>();
            rend.material.color = Color.white;
            return false;
        }
        else
        {
            selectedObjects.Add(trans);
            Renderer rend = trans.GetComponent<Renderer>();
            rend.material.color = Color.green;
            return true;
        }
        return false;
    }

    public void DeselectAll()
    {
        Renderer rend;
        for (; selectedObjects.Count > 0;)
        {
            rend = selectedObjects[0].GetComponent<Renderer>();
            rend.material.color = Color.white;

            selectedObjects.RemoveAt(0);
        }
    }

    private void SelectObjectsInRadius(Vector3 position) // instead of doing this I made the vertex colliders larger
    {
/*        ///loop through list of vertex assign radius 2 and see if the hit is within that radius 
        foreach (var v in Bridge.instance.vertexList)
        {
            //set z componet to the same as the vector
            if (Vector3.Distance(position, v) < 2)
            {
                bridge.SelectVertex(v);
                selectedVertex = v;
            }
        }*/
    }

    public void UnitSelect()
    {
        RaycastHit hit = BridgeCreator.instance.RaycastFromMouse();
        bool somethingSelected = false;
        if (hit.collider != null)
        {
            if (hit.transform.gameObject.layer == 9)
            {
                somethingSelected = AdjustSelectedObjects(hit.transform);
            } 
        }

        if (!somethingSelected)
        {
            DeselectAll();
        }
    }

    void OnGUI()
    {
        if (boxSelecting)
        {
            // Create a rect from both mouse positions
            var rect = Box.GetScreenRect(startPos, Input.mousePosition);
            Box.DrawScreenRect(rect, boxFillColor);
            Box.DrawScreenRectBorder(rect, boxBorderThickness, boxBorderColor);
        }

    }
}

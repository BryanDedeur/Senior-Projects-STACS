using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor.Experimental.UIElements.GraphView;
using System;

public class BridgeCreator : MonoBehaviour
{
    public LayerMask ignoreLayer;

    public GameObject bridgeContainer;
    public GameObject clickWall;
    public GameObject trussPrefab;
    public GameObject vertexPrefab;

    public Text vertexText;
    public Text edgeText;
    public GameObject viewHighlight;

/*    public Dictionary<Tuple<Vector3, Vector3>, GameObject> edgeDictionary;
    public Dictionary<Vector3, GameObject> vertexDictionary;*/

    public Transform draggedFocusObject;
    public Vector3 draggedStartPos;


    public bool mirroring;
    public bool mirrorAcrossX; // half of the length of the bridge will be cloned
    public bool mirrorAcrossY; // top to bottom of road platform
    public bool mirrorAcrossZ; // both sides of the road will be mirrored

    public bool mirrorBool;
    //public bool updatePosition;

/*    Vector3 mousePosition1;
*//*    List<Vector3> boxSelectedVertx = new List<Vector3>();
    List<GameObject> boxSelectedEdge = new List<GameObject>();*/
    //List<Vector3> vertexList = new List<Vector3>();
    public int numEdges;

    Vector3 screenPoint;
    Vector3 totalOffset;

    public GameObject[] trussPrefabs;
    public GameObject[] vertexPrefabs;
    public GameObject[] platformPrefabs;


    private Bridge bridge;

    private GameObject lastVertex;
    private GameObject lastVertexM;
    private GameObject edgeFixer;
/*    private Vector3 selectedVertex;
*/   
    private GameObject cursor;

    public static BridgeCreator instance;

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

    // Start is called before the first frame update
    void Start()
    {
        bridge = bridgeContainer.GetComponent<Bridge>();
        bridge.SetPlatform(platformPrefabs[0]);
        cursor = Instantiate(vertexPrefab.transform).gameObject;
        Destroy(cursor.GetComponent<Collider>());
    }

    public Vector3 Snap(Vector3 p)
    {
        return new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), Mathf.Round(p.z));
    }

    public RaycastHit RaycastFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, ~ignoreLayer)) // attempt to hit an existing bridge component
        {
            // something was hit
            hit.point = Snap(hit.point);
        } else
        {
            // nothing was hit
            if (Physics.Raycast(ray, out hit)) { // if no bridge component was hit try getting a point on the wall
                // something was hit
                hit.point = Snap(hit.point);
            }
        }
        return hit;
    }

    public void AttemptMoveSelected()
    {
/*        if (hit.collider != null)
        {
            foreach (var v in boxSelectedVertx)
            {
                bridge.CreateVertex(v + hit.point, vertexPrefab);
                //add location to list of vertex 
                //vertexList.Add(v + hit.point);
                edgeFixer = bridge.CreateVertex(v + hit.point, vertexPrefab);

                Vector3 newEdgeAnchor = bridge.UpdateEdge(v);
                while (newEdgeAnchor.x != 100000 && newEdgeAnchor.y != 100000 && newEdgeAnchor.z != 100000) //check for null vector
                {

                    bridge.CreateEdge(bridge.CreateVertex(newEdgeAnchor, vertexPrefab), edgeFixer, trussPrefab);
                    newEdgeAnchor = bridge.UpdateEdge(v);
                }
            }
        }

        foreach (var v in boxSelectedVertx)
        {
            bridge.UnselectVertex(v);
        }
        boxSelectedVertx.Clear();*/
        if (BCSelectionMgr.instance.selectedObjects.Count > 0)
        {
            RaycastHit hit = RaycastFromMouse();
            if (hit.collider != null)
            {
                if (draggedFocusObject == null)                {
                    if (hit.transform.gameObject.layer == 9) // make sure this is a bridge component
                    {
                        draggedFocusObject = hit.transform;
                        draggedStartPos = hit.transform.position;
                        totalOffset = Vector3.zero;
                    }
                }
                else
                {
                    Vector3 offset = hit.point - draggedStartPos;
                    totalOffset += offset;
                    foreach (Transform trans in BCSelectionMgr.instance.selectedObjects)
                    {
                        trans.position += offset;
                        if(trans.tag == "Vertex")
                        {

                        }
                        if(trans.tag == "Edge")
                        {

                        }

                    }

                    draggedStartPos = draggedFocusObject.transform.position;
                }
            }
        }
    }

    public void EndMovingSelected()
    {

        List<Transform> temp = BCSelectionMgr.instance.selectedObjects;

        foreach (Transform trans in temp)
        {
            if (trans.tag == "Vertex")
            {

                List<Edge> connected = bridge.GetAllEdgesContainingPosition(trans.position - totalOffset);
                foreach (Edge connectedEdge in connected)
                {
                    bridge.CreateEdge(connectedEdge.pos1 + totalOffset, connectedEdge.pos2 + totalOffset, connectedEdge.transform.gameObject);
                    bridge.RemoveEdge(connectedEdge);
                }

                bridge.RemoveVertex(trans.position - totalOffset);
                //bridge.RemoveVertex(trans.position);
                bridge.CreateVertex(trans.position, vertexPrefab);


            }
/*            if (trans.tag == "Edge")//need to update moved edges 
            {
                
            }*/
        }
        BCSelectionMgr.instance.DeselectAll();
        draggedFocusObject = null;
    }

    public void MirrorSelectedObjects()
    {
/*        //  Debug.Log("Count : " + boxSelectedVertx.Count);
        List<Vector3> temp = new List<Vector3>(boxSelectedVertx);
        foreach (var v in temp)
        {
            Vector3 newLocatation = v;
            newLocatation.z += 8;
            bridge.CreateVertex(newLocatation, vertexPrefab);

            Vector3 newEdgeAnchor = bridge.UpdateEdge(v);
            while (newEdgeAnchor.x != 100000 && newEdgeAnchor.y != 100000 && newEdgeAnchor.z != 100000) //check for null vector
            {
                newEdgeAnchor.z += 8;
                bridge.CreateEdge(newLocatation, newEdgeAnchor, trussPrefab);
                Vector3 newVec = newEdgeAnchor;
                newVec.z -= 8;
                bridge.CreateEdge(v, newVec, trussPrefab);

                newEdgeAnchor = bridge.UpdateEdge(v);
            }
            boxSelectedVertx.Remove(v);
        }*/
/*        List<GameObject> tempE = new List<GameObject>(boxSelectedEdge);
        foreach (var e in tempE)
        {
            bridge.UnSelectEdge(e);
            boxSelectedEdge.Remove(e);
        }*/
        
        foreach (Transform bridgeTrans in BCSelectionMgr.instance.selectedObjects)
        {
            Vector3 zOffSet = new Vector3(0.0f, 0.0f, -4.0f);
            
            zOffSet.z += bridge.GetWidth();
            zOffSet.z *= 2;

  
            Edge edge = bridgeTrans.GetComponent<Edge>();
            if (edge != null) // if edge
            {
                Vector3 newPos1 = edge.pos1 + zOffSet;
                Vector3 newPos2 = edge.pos2 + zOffSet;
                bridge.CreateEdge(newPos1, newPos2, trussPrefab);
            } else // if vertex
            {
                
               
                Vector3 newPos = bridgeTrans.position + zOffSet;
                
                bridge.CreateVertex(newPos, vertexPrefab);
               
            }
        }

    }

    public List<Transform> CopySelectedObjects()//
    {
        RaycastHit hit = RaycastFromMouse();
        List<Transform> newObjects = new List<Transform>();
        
        foreach (Transform bridgeTrans in BCSelectionMgr.instance.selectedObjects)
        {
            Edge edge = bridgeTrans.GetComponent<Edge>();
            if (edge != null) // if edge
            {
                Vector3 newPos1 = edge.pos1 + hit.point;
                Vector3 newPos2 = edge.pos2 + hit.point;
                newPos1.z = -4.0f;
                newPos2.z = -4.0f;
                newObjects.Add(bridge.CreateEdge(newPos1, newPos2, trussPrefab).transform);
               
               
            }
            else // if vertex
            {
                Vector3 temp = bridgeTrans.position + hit.point;
                temp.z = -4.0f;
                newObjects.Add(bridge.CreateVertex(temp, vertexPrefab).transform);
            }
        }
        print(newObjects.Count);
        return newObjects;

    }

    void UpdateCountText()
    {
        edgeText.text = bridge.GetEdges().ToString();
        vertexText.text = bridge.GetVertices().ToString();
    }

    // Adds a vertex and potentially creates and edge
    public void AddVertex(Vector3 position)
    {
        if (lastVertex == null)
        {
            lastVertex = bridge.CreateVertex(position, vertexPrefab);
        }
        else // create an edge
        {
            bridge.CreateEdge(lastVertex, bridge.CreateVertex(position, vertexPrefab), trussPrefab);
            numEdges++;

            // reset the tracker objects
            lastVertex = null;
        }

        //mirrioing logic 
        if (mirrorBool)
        {
            Vector3 temp = position;            
            temp.z = -temp.z;
            if (lastVertexM == null)
            {
                lastVertexM = bridge.CreateVertex(temp, vertexPrefab);
            }
            else
            {
                bridge.CreateEdge(lastVertexM, bridge.CreateVertex(temp, vertexPrefab), trussPrefab);
                lastVertexM = null;
                numEdges++;
            }
        }

        UpdateCountText();
    }

    public void AttemptAddVertexAndConnectEdge()
    {
        RaycastHit hit = RaycastFromMouse();

        if (hit.collider != null)
        {
            AddVertex(hit.point);
        }
    }

    public void AttemptRemoveEdgeOrVertex()
    {
        RaycastHit hit = RaycastFromMouse();

        if (hit.collider != null)
        {
            if (hit.transform.name == "Truss")
            {
                bridge.RemoveEdge(hit.transform.gameObject);
                numEdges--;
            }
            else if (hit.transform.name == "Vertex")
            {
                bridge.RemoveVertex(hit.transform.position);
                //vertexList.Remove(hit.transform.position);
            }
            else if (bridge.VertexExists(hit.point))
            {
                bridge.RemoveVertex(hit.point);
                //vertexList.Remove(hit.transform.position);
            }
        }
    }

    public void AdjustDraggedVertex()//
    {
/*        Vector3 edgeLocation;
        Vector3 newEdgeAnchor;

        edgeFixer = bridge.MoveVertex(selectedVertex, hit.point, vertexPrefab);
        //vertexList.Add(hit.point);
        newEdgeAnchor = bridge.UpdateEdge(selectedVertex);

        for (int i = 0; i < numEdges; i++)
        {
            if (newEdgeAnchor.x != 100000 && newEdgeAnchor.y != 100000 && newEdgeAnchor.z != 100000)
            {
                bridge.CreateEdge(bridge.CreateVertex(newEdgeAnchor, vertexPrefab), edgeFixer, trussPrefab);
                //vertexList.Add(newEdgeAnchor);
                newEdgeAnchor = bridge.UpdateEdge(selectedVertex);
            }
        }*/
    }

    public void EnableMirroring()
    {
        mirrorBool = true;
    }

    public void DisableMirroring()
    {
        mirrorBool = false;
    }

    private void UpdateCursor()
    {
        RaycastHit hit = RaycastFromMouse();
        if (hit.collider != null)
        {
            cursor.transform.localPosition = hit.point;
        }
    }

    // Update is called once per frame by unity
    void Update()
    {
        UpdateCursor();
    }
}

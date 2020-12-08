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
    // TODO make background scenery for the bridge creator

    public GameObject bridgeContainer;
    public GameObject clickWall;
    public GameObject trussPrefab;
    public GameObject vertexPrefab;
    public Button topButton, bottomButton, leftButton, rightButton;
    static int vertexCount;
    static int edgeCount;
    public Text vertexText;
    public Text edgeText;
    public Dictionary<Tuple<Vector3, Vector3>, GameObject> edgeDictionary;
    public Dictionary<Vector3, GameObject> vertexDictionary;

    public GameObject vertexSelectedPrefab;

    // TODO make a UI for these options
    // TODO make implementation for these options
    public bool mirrorAcrossX; // half of the length of the bridge will be cloned
    public bool mirrorAcrossY; // top to bottom of road platform
    public bool mirrorAcrossZ; // both sides of the road will be mirrored

    public bool selector;
    public bool boxSeletector;
    public bool updatePosition;
    bool isSelecting;
    Vector3 mousePosition1;
    List<Vector3> boxSelectedVertx = new List<Vector3>();
    List<Vector3> vertexList = new List<Vector3>();
    int numEdges;


    public enum cameraDirection
    {
        Left, Right, Top, Bottom
    }
    // TODO create UI interface to set this
    public cameraDirection ViewDirection = cameraDirection.Right;

    // TODO make truss prefab options
    public GameObject[] trussPrefabs;
    // TODO make vertex prefab options
    public GameObject[] vertexPrefabs;
    // TODO create platform options (road, road with sidewalk, train, sidewalk)
    public GameObject[] platformPrefabs;

    public bool mirroring;

    private Bridge bridge;

    // TODO make a selection indicator for the selected vertices and update when selected
    private GameObject firstSelection;
    private GameObject secondSelection;
    private GameObject edgeFixer;
    private Vector3 selectedVertex;
   

    private GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        isSelecting = false;

        bridge = bridgeContainer.GetComponent<Bridge>();

        // TODO setting the platform should be done by the UI
        bridge.SetPlatform(platformPrefabs[0]);

        cursor = Instantiate(vertexPrefab.transform).gameObject;
        Destroy(cursor.GetComponent<Collider>());
        vertexCount = 0;
        edgeCount = 0;
        edgeDictionary = bridge.edges;
        vertexDictionary = bridge.vertices;
    }

    Vector3 Snap(Vector3 p)
    {
        return new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), Mathf.Round(p.z));
    }

    // TODO add ability to click and drag select all vertices in a region

    // TODO add ability to click and drag a vertices from a position to another and update connected edges

    void MoveClickWall(Vector3 newPosition)
    {
        switch (ViewDirection)
        {
            case cameraDirection.Right:
                newPosition = new Vector3(0,0,-1) * bridge.GetWidth() / 2f;
                break;
            case cameraDirection.Left:
                newPosition = new Vector3(0, 0, 1) * bridge.GetWidth() / 2f;
                break;
            default:
                // TODO fix the click wall position when trying to click on a second vertex from top or bottom view
                newPosition = new Vector3(0, newPosition.y, 0);
                break;

        }

        clickWall.transform.position = newPosition;
        clickWall.transform.LookAt(Vector3.zero);
    }

    void CheckMouseState()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 edgeLocation;


        Vector3 newEdgeAnchor;
  
        // TODO use layers to ensure that nothing overlaps
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 snapped = Snap(hit.point);
            MoveClickWall(snapped);

            if (Input.GetMouseButtonDown(0) && !selector && !boxSeletector)
            {
                // TODO mirror accross X Y and Z axis if mirroring enabled
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    topButton.onClick.AddListener(UpdateTopView);
                    bottomButton.onClick.AddListener(UpdateBottomView);
                    rightButton.onClick.AddListener(UpdateLeftView);
                    leftButton.onClick.AddListener(UpdateRightView);
                    return;
                }
                if (firstSelection == null)
                {
                    firstSelection = bridge.CreateVertex(snapped, vertexPrefab);

                    //add location to list of vertex 
                    vertexList.Add(snapped);
                }
                else if (secondSelection == null)
                {
                    secondSelection = bridge.CreateVertex(snapped, vertexPrefab);
                    bridge.CreateEdge(firstSelection, secondSelection, trussPrefab);
                    numEdges++;
                    // reset the tracker objects
                    firstSelection = null;
                    secondSelection = null;

                    //add location to list of vertex
                    vertexList.Add(snapped);
                }
            }
            else
            {
                cursor.transform.localPosition = snapped;
            }



                if (Input.GetMouseButtonDown(1))
                  {

                      // TODO mirror accross X Y and Z axis if mirroring enabled

                      if (hit.transform.name == "Truss")
                      {
                          bridge.RemoveEdge(hit.transform.gameObject);
                    numEdges--;
                      }
                      else if (hit.transform.name == "Vertex")
                      {
                          bridge.RemoveVertex(hit.transform.position);
                       vertexList.Remove(hit.transform.position);
                      } else if (bridge.VertexExists(snapped))
                      {
                          bridge.RemoveVertex(snapped);
                         vertexList.Remove(hit.transform.position);
                      }
                  } 
           
            if(Input.GetKeyDown(KeyCode.LeftControl))
             {
                 selector = true;
                 Debug.Log(selector);
             }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                selector = false;

            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                boxSeletector = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
             {
           
                boxSeletector = false;

            }

            ///
            ///to enable single selection press space 
            ///
            ///to enable box select press right shift 
            ///
            ///to disable both press left shift 
            ///
            ///to move boxed vertx press back space // not working yet
            ///
            ///
            ///todo cnrtl click to select and shift click to drag 
            if (Input.GetMouseButtonDown(0) && selector == true)
            {
                ///loop through list of vertex assign radius 2 and see if the hit is within that radius 
               foreach(var v in vertexList)
                {
                    Debug.Log("Vertex");
                    Debug.Log(v);
                    //set z componet to the same as the vector
                    if (Vector3.Distance(snapped, v) < 2)
                    {
                        bridge.SelectVertex(v);
                        selectedVertex = v;

                    }
                }

                ///
               /* Debug.Log(selector);
                if (hit.transform.name == "Vertex")
                {
                    bridge.SelectVertex(snapped);
                    selectedVertex = snapped;
                }*/
            }
            if(Input.GetMouseButtonUp(0) && selector == true)
            {
                edgeFixer = bridge.MoveVertex(selectedVertex, snapped, vertexPrefab);
                vertexList.Add(snapped);
                newEdgeAnchor = bridge.UpdateEdge(selectedVertex);

                for(int i =0; i < numEdges; i++)
                {
                    if(newEdgeAnchor.x != 100000 && newEdgeAnchor.y != 100000 && newEdgeAnchor.z != 100000)
                    {
                        bridge.CreateEdge(bridge.CreateVertex(newEdgeAnchor, vertexPrefab), edgeFixer, trussPrefab);
                        vertexList.Add(newEdgeAnchor);
                        newEdgeAnchor = bridge.UpdateEdge(selectedVertex);
                    }
                    
                }



            }
            UpdateCountText();
        }
        //selecting single vertex moving and updating works 


        //Box selection logic 
        if (Input.GetMouseButtonDown(0) && boxSeletector == true)
        {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0) && boxSeletector == true)
        {
            Debug.Log("Up");
            isSelecting = false;
            updatePosition = true;
        }

        //highlight edges and add them to a list of selected edges
        if (isSelecting)
        {
            foreach (var selectableObject in FindObjectsOfType<GameObject>())
            {
                if (IsWithinSelectionBounds(selectableObject.gameObject) && selectableObject.name == "Vertex")
                {
                    bridge.SelectVertex(selectableObject.transform.position);
                    boxSelectedVertx.Add(selectableObject.transform.position);
                }
                if (IsWithinSelectionBounds(selectableObject.gameObject) && selectableObject.name == "Truss")
                {
                    bridge.SelectEdge(selectableObject);
                }

            }
        }

        //move the box selected vertices
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            mousePosition1 = Input.mousePosition;;
            foreach(var v in boxSelectedVertx)
            {
                edgeFixer = bridge.MoveVertex(v, mousePosition1, vertexPrefab);

                newEdgeAnchor = bridge.UpdateEdge(v);
                while (newEdgeAnchor.x != 100000 && newEdgeAnchor.y != 100000 && newEdgeAnchor.z != 100000) //check for null vector
                {

                    bridge.CreateEdge(bridge.CreateVertex(newEdgeAnchor, vertexPrefab), edgeFixer, trussPrefab);
                    newEdgeAnchor = bridge.UpdateEdge(v);
                }
            }
        }

    }

    void RepositionCameraAndClickWall()
    {
        if (Camera.current != null)
        {
            float offset = 100;
            switch(ViewDirection)
            {
                case cameraDirection.Right:
                    Camera.current.transform.localPosition = new Vector3(.25f,.25f,-1) * offset;
                    break;
                case cameraDirection.Left:
                    Camera.current.transform.localPosition = new Vector3(.25f, .25f, 1) * offset;
                    break;
                case cameraDirection.Top:
                    Camera.current.transform.localPosition = new Vector3(0, 1, 0) * offset;
                    break;
                case cameraDirection.Bottom:
                    Camera.current.transform.localPosition = new Vector3(0, -1, 0) * offset;
                    break;
            }

            Camera.current.transform.LookAt(Vector3.zero);
            // TODO make sure when camera changes that the click wall updates to face that new camera position
        }
    }
    void UpdateTopView()
    {
        ViewDirection = cameraDirection.Top;
    }
    void UpdateBottomView()
    {
        ViewDirection = cameraDirection.Bottom;
    }
    void UpdateLeftView()
    {
        ViewDirection = cameraDirection.Left;
    }
    void UpdateRightView()
    {
        ViewDirection = cameraDirection.Right;
    }
    
    void UpdateCountText()
    {
        edgeText.text = edgeDictionary.Count.ToString();
        vertexText.text = vertexDictionary.Count.ToString();
    }



    // Update is called once per frame
    void Update()
    {

        // TODO this should not happen every update... would be better if once the cameraDirection changes by the UI to call this function
        
        RepositionCameraAndClickWall();
        CheckMouseState();


    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            var rect = Box.GetScreenRect(mousePosition1, Input.mousePosition);
            Box.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Box.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
               var camera = Camera.main;
        var viewportBounds = Box.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    public bool IsWithinSelectionBoundsVec(Vector3 p)
    {
        var camera = Camera.main;
        var viewportBounds = Box.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(p));
    }

}

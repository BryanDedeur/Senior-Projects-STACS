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
    public Camera userCam;
    public GameObject bridgeContainer;
    public GameObject clickWall;
    public GameObject trussPrefab;
    public GameObject vertexPrefab;
    public GameObject buttonHighlight;

    // UI Stuff 
    public Button topButton, bottomButton, leftButton, rightButton;
    public Text vertexText;
    public Text edgeText;
    // TODO there may be a better way to do this
    public Dictionary<Tuple<Vector3, Vector3>, GameObject> edgeDictionary;
    public Dictionary<Vector3, GameObject> vertexDictionary;

    public GameObject vertexSelectedPrefab;

    // TODO make a UI for these options
    // TODO make implementation for these options
    public bool mirrorAcrossX; // half of the length of the bridge will be cloned
    public bool mirrorAcrossY; // top to bottom of road platform
    public bool mirrorAcrossZ; // both sides of the road will be mirrored

    public bool selector;
    public bool testbool; //remove after this seesion
    public bool mirrorBool;
    public bool boxSeletector;
    public bool updatePosition;
    bool isSelecting;
    Vector3 mousePosition1;
    List<Vector3> boxSelectedVertx = new List<Vector3>();
    List<GameObject> boxSelectedEdge = new List<GameObject>();
    List<Vector3> vertexList = new List<Vector3>();
    int numEdges;

    Vector3 screenPoint;
    Vector3 offset;



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
    public float deltaX, deltaY;


    private Bridge bridge;

    // TODO make a selection indicator for the selected vertices and update when selected
    private GameObject firstSelection;
    private GameObject secondSelection;
    private GameObject firstSelectionM;
    private GameObject secondSelectionM;
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
        UpdateRightView();
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
        //Vector3 edgeLocation;


        Vector3 newEdgeAnchor;
  
        // TODO use layers to ensure that nothing overlaps
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 snapped = Snap(hit.point);
            MoveClickWall(snapped);

            if (Input.GetMouseButtonDown(0) && !selector && !boxSeletector && !mirrorBool && !testbool)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    topButton.onClick.AddListener(UpdateTopView);
                    bottomButton.onClick.AddListener(UpdateBottomView);
                    rightButton.onClick.AddListener(UpdateRightView);
                    leftButton.onClick.AddListener(UpdateLeftView);
                    return;
                }
                // TODO mirror accross X Y and Z axis if mirroring enabled

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
            if (Input.GetKeyDown(KeyCode.M))
            {
                mirrorBool = true;
            }
            if (Input.GetKeyDown(KeyCode.N))
            {
                mirrorBool = false;
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("EATFOOD");
                testbool= true;
            }

            //mirrioing logic 
            if (Input.GetMouseButtonDown(0) && mirrorBool)
            {
                Vector3 temp = snapped;
                //value can be changed
                temp.z += 8;
                if (firstSelection == null)
                {
                    firstSelection = bridge.CreateVertex(snapped, vertexPrefab);
                    firstSelectionM = bridge.CreateVertex(temp, vertexPrefab);
                    //add location to list of vertex 
                    vertexList.Add(snapped);
                    vertexList.Add(temp);
                }
                else if (secondSelection == null)
                {
                    secondSelection = bridge.CreateVertex(snapped, vertexPrefab);
                    bridge.CreateEdge(firstSelection, secondSelection, trussPrefab);
                    numEdges++;


                    secondSelectionM = bridge.CreateVertex(temp, vertexPrefab);
                    bridge.CreateEdge(firstSelectionM, secondSelectionM, trussPrefab);
                    numEdges++;
                    // reset the tracker objects
                    firstSelection = null;
                    secondSelection = null;

                    //add location to list of vertex
                    vertexList.Add(snapped);
                    vertexList.Add(temp);
                }
            }


            ///
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


/*
            if(Input.GetMouseButtonDown(0) && selector == true)
            {
                Debug.Log("OnMOuseDOwn");
                screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
                offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
            }*/
            
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
        //todo: only select things on clickwall 
        if (isSelecting)
        {
            foreach (var selectableObject in FindObjectsOfType<GameObject>())
            {
                if (IsWithinSelectionBounds(selectableObject.gameObject) && selectableObject.name == "Vertex" && selectableObject.transform.position.z == -4)
                {
                    bridge.SelectVertex(selectableObject.transform.position);
                    boxSelectedVertx.Add(selectableObject.transform.position);
                }
                if (IsWithinSelectionBounds(selectableObject.gameObject) && selectableObject.name == "Truss" && selectableObject.transform.position.z == -4)
                {
                    bridge.SelectEdge(selectableObject);
                    boxSelectedEdge.Add(selectableObject);

                }

            }
        }

        //move the box selected vertices
        //fix the redrawing of edges 
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 mousePosition1 = Snap(hit.point);
                Debug.Log(mousePosition1);
                foreach (var v in boxSelectedVertx)
                {
                    bridge.CreateVertex(v+mousePosition1, vertexPrefab);
                    //add location to list of vertex 
                    vertexList.Add(v+mousePosition1);
                     edgeFixer = bridge.CreateVertex(v + mousePosition1, vertexPrefab);

                     newEdgeAnchor = bridge.UpdateEdge(v);
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
            boxSelectedVertx.Clear();
        }
        //mirror select objects
        if(Input.GetKeyDown(KeyCode.B))
        {
            //  Debug.Log("Count : " + boxSelectedVertx.Count);
            List<Vector3> temp = new List<Vector3>(boxSelectedVertx);
            foreach (var v in temp)
            {
                Vector3 newLocatation = v;
                newLocatation.z += 8;
                bridge.CreateVertex(newLocatation, vertexPrefab);

                newEdgeAnchor = bridge.UpdateEdge(v);
                while (newEdgeAnchor.x != 100000 && newEdgeAnchor.y != 100000 && newEdgeAnchor.z != 100000) //check for null vector
                {
                    newEdgeAnchor.z += 8;
                    bridge.AddEdge(newLocatation, newEdgeAnchor, trussPrefab);
                    Vector3 newVec = newEdgeAnchor;
                    newVec.z -= 8;
                    bridge.AddEdge(v, newVec, trussPrefab);

                    newEdgeAnchor = bridge.UpdateEdge(v);
                }
                bridge.UnselectVertex(v);
                boxSelectedVertx.Remove(v);

            }
            List<GameObject> tempE = new List<GameObject>(boxSelectedEdge);
            foreach (var e in tempE)
            {
                bridge.UnSelectEdge(e);
                boxSelectedEdge.Remove(e);
            }

           // foreach (var e in )
        }


        UpdateCountText();
    }


    void RepositionCameraAndClickWall()
    {
        //if (Camera.current != null)
        {
            float offset = 100;
            switch(ViewDirection)
            {
                case cameraDirection.Right:
                    userCam.transform.localPosition = new Vector3(.25f, 0,-1) * offset;
                    break;
                case cameraDirection.Left:
                    userCam.transform.localPosition = new Vector3(.25f, 0, 1) * offset;
                    break;
                case cameraDirection.Top:
                    userCam.transform.localPosition = new Vector3(0, 1, 0) * offset;
                    break;
                case cameraDirection.Bottom:
                    userCam.transform.localPosition = new Vector3(0, -1, 0) * offset;
                    break;
            }

            userCam.transform.LookAt(Vector3.zero);
            // TODO make sure when camera changes that the click wall updates to face that new camera position
        }
    }

    void UpdateTopView()
    {
        ViewDirection = cameraDirection.Top;
        buttonHighlight.transform.position = topButton.transform.position;
        RepositionCameraAndClickWall();
    }
    void UpdateBottomView()
    {
        ViewDirection = cameraDirection.Bottom;
        buttonHighlight.transform.position = bottomButton.transform.position;
        RepositionCameraAndClickWall();
    }
    void UpdateLeftView()
    {
        ViewDirection = cameraDirection.Left;
        buttonHighlight.transform.position = leftButton.transform.position;
        RepositionCameraAndClickWall();
    }
    void UpdateRightView()
    {
        ViewDirection = cameraDirection.Right;
        buttonHighlight.transform.position = rightButton.transform.position;
        RepositionCameraAndClickWall();
    }

    void UpdateCountText()
    {
        edgeText.text = bridge.GetEdges().ToString();
        vertexText.text = bridge.GetVertices().ToString();
    }



    // Update is called once per frame
    void Update()
    {

        // TODO this should not happen every update... would be better if once the cameraDirection changes by the UI to call this function
        

        CheckMouseState();
        //RepositionCameraAndClickWall();

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

    private void OnMouseDown()
    {
       if(testbool)
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
            Debug.Log("hehe");
        }
    }
}

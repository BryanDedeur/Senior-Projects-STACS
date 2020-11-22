using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreator : MonoBehaviour
{
    // TODO make background scenery for the bridge creator

    public GameObject bridgeContainer;
    public GameObject clickWall;
    public GameObject trussPrefab;
    public GameObject vertexPrefab;

    // TODO make a UI for these options
    // TODO make implementation for these options
    public bool mirrorAcrossX; // half of the length of the bridge will be cloned
    public bool mirrorAcrossY; // top to bottom of road platform
    public bool mirrorAcrossZ; // both sides of the road will be mirrored

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

    private GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        bridge = bridgeContainer.GetComponent<Bridge>();

        // TODO setting the platform should be done by the UI
        bridge.SetPlatform(platformPrefabs[0]);

        cursor = Instantiate(vertexPrefab.transform).gameObject;
        Destroy(cursor.GetComponent<Collider>());
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
        // TODO use layers to ensure that nothing overlaps
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 snapped = Snap(hit.point);
            MoveClickWall(snapped);

            if (Input.GetMouseButtonDown(0))
            {
                // TODO mirror accross X Y and Z axis if mirroring enabled

                if (firstSelection == null)
                {
                    firstSelection = bridge.CreateVertex(snapped, vertexPrefab);
                }
                else if (secondSelection == null)
                {
                    secondSelection = bridge.CreateVertex(snapped, vertexPrefab);
                    bridge.CreateEdge(firstSelection, secondSelection, trussPrefab);
                    // reset the tracker objects
                    firstSelection = null;
                    secondSelection = null;
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
                }
                else if (hit.transform.name == "Vertex")
                {
                    bridge.RemoveVertex(hit.transform.position);
                } else if (bridge.VertexExists(snapped))
                {
                    bridge.RemoveVertex(snapped);
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

    // Update is called once per frame
    void Update()
    {

        // TODO this should not happen every update... would be better if once the cameraDirection changes by the UI to call this function
        RepositionCameraAndClickWall();

        CheckMouseState();


    }
}

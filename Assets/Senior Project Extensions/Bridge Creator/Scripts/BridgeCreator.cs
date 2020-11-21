using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeCreator : MonoBehaviour
{
    public GameObject ClickWall;
    public GameObject Truss;
    public GameObject Vertex;

    public bool mirroring;
    public Vector3 viewAxis;

    private Dictionary<Vector3, GameObject> verticies;

    private GameObject object_v1;
    private GameObject object_v2;

    private GameObject cursor;

    // Start is called before the first frame update
    void Start()
    {
        verticies = new Dictionary<Vector3, GameObject>();

        cursor = Instantiate(Vertex.transform).gameObject;
        Destroy(cursor.GetComponent<Collider>());
    }

    Vector3 Snap(Vector3 p)
    {
        return new Vector3(Mathf.Round(p.x), Mathf.Round(p.y), Mathf.Round(p.z));
    }

    GameObject CreateVertex(Vector3 p)
    {
        p = Snap(p);

        if (verticies.ContainsKey(p))
        {
            return verticies[p];
        }

        Transform newVertex = Instantiate(Vertex.transform);
        newVertex.name = "Vertex";
        newVertex.localPosition = p;

        verticies.Add(p, newVertex.transform.gameObject);

        return newVertex.gameObject;
    }

    void ConnectPoints(GameObject v1, GameObject v2)
    {
        Transform newTruss = Instantiate(Truss.transform);
        newTruss.name = "Truss";

        Vector3 diff = v1.transform.localPosition - v2.transform.localPosition;
        float dist = diff.magnitude;
        newTruss.localScale = new Vector3(Truss.transform.localScale.x, Truss.transform.localScale.y, dist);
        newTruss.position = v2.transform.localPosition + (diff / 2.0f);
        newTruss.LookAt(v1.transform.localPosition);

        object_v1 = null;
        object_v2 = null;
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (object_v1 == null)
                {
                    object_v1 = CreateVertex(hit.point);
                } else if (object_v2 == null)
                {
                    object_v2 = CreateVertex(hit.point);
                    ConnectPoints(object_v1, object_v2);
                }
            } else
            {
                cursor.transform.localPosition = Snap(hit.point);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if (hit.transform.name == "Truss")
                {
                    Destroy(hit.transform.gameObject);
                } else if (verticies.ContainsKey(Snap(hit.point)))
                {
                    Vector3 snapped = Snap(hit.point);
                    Destroy(verticies[snapped]);
                    verticies.Remove(snapped);

                }
            }
        }

        if (Camera.current != null)
        {
            Camera.current.transform.localPosition = -viewAxis * 100;
            Camera.current.transform.LookAt(Vector3.zero);
        }
    }
}

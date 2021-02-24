using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



// This class is a container for holding bridge info and functions
public class Bridge : MonoBehaviour
{
    private GameObject container; // holds all the bridge components

    private float platformLength;
    private float platformWidth;

    private GameObject containerSide1;
    private GameObject containerSide2;
    private GameObject containerTop;
    private GameObject containerBottem;
    private GameObject platform; // the horizontal surface that vehicles move on

    private Dictionary<Vector3, GameObject> vertices;
    private Dictionary<Tuple<Vector3, Vector3>, GameObject> edges;
    private Dictionary<Vector3, GameObject> selectedVertices;

    //used for moving vertex
    private Vector3 mOffset;
    private float mZCoord;



    // Start is called before the first frame update
    void Start()
    {
        container = this.transform.gameObject;

        vertices = new Dictionary<Vector3, GameObject>();
        edges = new Dictionary<Tuple<Vector3, Vector3>, GameObject>();

    }

    public void SetPlatform(GameObject platformPrefab)
    {
        platform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, container.transform);
        SetWidth(platform.transform.localScale.z);
        SetLength(platform.transform.localScale.x);

    }

    public void SetLength(float newLength)
    {
        platformLength = newLength;
        platform.transform.localScale = new Vector3(platformLength, 1, platform.transform.localScale.z);
    }

    public float GetLength()
    {
        return platformLength;
    }

    public void SetWidth(float newWidth)
    {
        platformWidth = newWidth;
    }

    public float GetWidth()
    {
        return platformWidth;
    }

    public bool VertexExists(Vector3 position)
    {
        return vertices.ContainsKey(position);
    }

    public GameObject CreateVertex(Vector3 p, GameObject vertex)
    {
     
        if (vertices.ContainsKey(p))
        {
            return vertices[p];
        }

        Transform newVertex = Instantiate(vertex.transform);
        newVertex.name = "Vertex";
        newVertex.localPosition = p;

        vertices.Add(p, newVertex.transform.gameObject);

        return newVertex.gameObject;
    }

    public GameObject CreateEdge(GameObject v1, GameObject v2, GameObject truss)
    {
        Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(v1.transform.position, v2.transform.position);
        // check if edge already exsits
        if (edges.ContainsKey(edgeKey))
        {
            return edges[edgeKey];
        }

        // create a new truss
        Transform newTruss = Instantiate(truss.transform);
        newTruss.name = "Truss";

        Vector3 diff = v1.transform.localPosition - v2.transform.localPosition;
        float dist = diff.magnitude;
        newTruss.localScale = new Vector3(truss.transform.localScale.x, truss.transform.localScale.y, dist);
        newTruss.position = v2.transform.localPosition + (diff / 2.0f);
        newTruss.LookAt(v1.transform.localPosition);

        edges.Add(edgeKey, newTruss.gameObject);
        Debug.Log("edgekey");
        Debug.Log(edgeKey);
        return newTruss.gameObject;
    }

    //same as create edge but takes vector3s DOES NOT ADD TO EDGES LIST
     public GameObject AddEdge(Vector3 v1, Vector3 v2, GameObject truss)
    {
        Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(v1, v2);
        // check if edge already exsits
     

        // create a new truss
        Transform newTruss = Instantiate(truss.transform);
        newTruss.name = "Truss";

        Vector3 diff = v1 - v2;
        float dist = diff.magnitude;
        newTruss.localScale = new Vector3(truss.transform.localScale.x, truss.transform.localScale.y, dist);
        newTruss.position = v2 + (diff / 2.0f);
        newTruss.LookAt(v1);

        //edges.Add(edgeKey, newTruss.gameObject);
        Debug.Log("FakeKey");
        Debug.Log(edgeKey);
        return newTruss.gameObject;
    }

    public void RemoveVertex(Vector3 atPosition)
    {
        if (vertices.ContainsKey(atPosition))
        {
            Destroy(vertices[atPosition]);
            vertices.Remove(atPosition);
        }
    }

    public void RemoveEdge(GameObject edge)
    {
        foreach (var pair in edges)
        {
            if (pair.Value == edge)
            {
                Destroy(edge);
                edges.Remove(pair.Key);
                break;
            }
        }
    }

    public Vector3 UpdateEdge(Vector3 p)
    {
        //iterate through all possible pairs
        //check for edge connections with p, old vertex
        //return the first value that is a pair with p
       
        foreach(KeyValuePair<Vector3, GameObject> V in vertices)
        {
            Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(p, V.Key);
            Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(V.Key, p);
            
            if (edges.ContainsKey(edgeKey))
            {
                RemoveEdge(edges[edgeKey]);
                return V.Key;
            }

            if (edges.ContainsKey(edgeKey2))
            {
                RemoveEdge(edges[edgeKey2]);
                return V.Key;
            }
        }
        Vector3 nullVec;
        //"null vector"
        nullVec.x = 100000;
        nullVec.y = 100000;
        nullVec.z = 100000;
        return nullVec; 
    }

    public Vector3 GetEdge(Vector3 p)
    {
        //iterate through all possible pairs
        //check for edge connections with p, old vertex
        //return the first value that is a pair with p

        foreach (KeyValuePair<Vector3, GameObject> V in vertices)
        {
            Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(p, V.Key);
            Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(V.Key, p);

            if (edges.ContainsKey(edgeKey))
            {
                edges.Remove(edgeKey);
                return V.Key;
            }

            if (edges.ContainsKey(edgeKey2))
            {
                edges.Remove(edgeKey2);
                return V.Key;
            }
        }
        Vector3 nullVec;
        //"null vector"
        nullVec.x = 100000;
        nullVec.y = 100000;
        nullVec.z = 100000;
        return nullVec;
    }

    public GameObject SelectVertex(Vector3 p)
    {
        if (vertices.ContainsKey(p))
        {
            Renderer rend = vertices[p].GetComponent<Renderer>();
            rend.material.color = Color.green;
            return vertices[p];
        }
        return null;
       
    }

    public GameObject UnselectVertex(Vector3 p)
    {
        if (vertices.ContainsKey(p))
        {
            Renderer rend = vertices[p].GetComponent<Renderer>();
            rend.material.color = Color.white;
            return vertices[p];
        }
        return null;
    }

    public void SelectEdge(GameObject selectedEdge)
    {
        Renderer rend =  selectedEdge.GetComponent<Renderer>();
        rend.material.color = Color.green;
    }

    public void UnSelectEdge(GameObject selectedEdge)
    {
        Renderer rend = selectedEdge.GetComponent<Renderer>();
        rend.material.color = Color.white;
    }


    public GameObject MoveVertex(Vector3 oldloc, Vector3 newloc, GameObject fab)
    {
        RemoveVertex(oldloc);
        return CreateVertex(newloc, fab); 
    }

    public int GetVertices()
    {
        return vertices.Count;
    }

    public int GetEdges()
    {
        return edges.Count;
    }


   


}


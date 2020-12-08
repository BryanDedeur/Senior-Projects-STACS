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

    public Dictionary<Vector3, GameObject> vertices;
    public Dictionary<Tuple<Vector3, Vector3>, GameObject> edges;

    // TODO add defects Dictionary<edge, defect> (like vertices but are attached somehow along the edge)

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
        Setwidth(platform.transform.localScale.z);
        SetLength(platform.transform.localScale.x);

    }

    public void SetLength(float newLength)
    {
        // TODO the user needs a UI to set the length
    }

    public float GetLength()
    {
        return platformLength;
    }

    public void Setwidth(float newWidth)
    {
        // TODO the user needs a UI to set the width
        platformWidth = newWidth;
        // TODO update the edges and verticies as the width changes
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


        return newTruss.gameObject;
    }

    // TODO add function to create defects on the bridge


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

    // TODO add function to export decoded bridge to encoded files
        // file 1 should be a CSV containing the Vector3 coordinates to the vertices
        // file 2 should be a Adjacency matrix specifying the which edges are connected to which vertices
        // file 3 should be a file containing the defect locations

    // TODO add function to import encoded files into bridge class
        // reads in the three files mentioned above
}

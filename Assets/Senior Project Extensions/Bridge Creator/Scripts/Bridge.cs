using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// This class is a container for holding bridge info and functions
public class Bridge : MonoBehaviour
{

    public static Bridge instance;
    private GameObject container; // holds all the bridge components

    private float platformLength;
    private float platformWidth;

    private GameObject containerSide1;
    private GameObject containerSide2;
    private GameObject containerTop;
    private GameObject containerBottem;
    private GameObject platform; // the horizontal surface that vehicles move on

    public Dictionary<Vector3, GameObject> vertices;
    public Dictionary<Tuple<Vector3, Vector3>, Edge> edges;
    public Dictionary<int, Edge> edgesId;

    private int id;

    //used for moving vertex
    private Vector3 mOffset;
    private float mZCoord;

    public string bridgeName = "Default Bridge";
    public string bridgePath = "Bridges/";

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
        container = this.transform.gameObject;

        vertices = new Dictionary<Vector3, GameObject>();
        edges = new Dictionary<Tuple<Vector3, Vector3>, Edge>();
        edgesId = new Dictionary<int, Edge>();
        id = 0;

    }

    private float GetSumEdges()
    {
        float distance = 0;
        foreach (var edgeRef in edges)
        {
            distance += edgeRef.Value.transform.localScale.z;
        }
        return distance;
    }

    public int GetVertexId(Vector3 pos)
    {
        int index = 0;
        foreach (var vertex in vertices)
        {
            if (vertex.Key == pos)
            {
                return index;  // According to question, you are after the key 
            }
            index++;
        }
        return index;
    }

    private string FormatConnectionsToString() // connections between edges vertices
    {
        string output = "";
        output += vertices.Count.ToString() + "\n";
        output += edges.Count.ToString() + "\n";
        output += GetSumEdges().ToString() + "\n";
        foreach (var edgeRef in edges)
        {
            output += GetVertexId(edgeRef.Value.pos1).ToString() + " " + GetVertexId(edgeRef.Value.pos2).ToString() + " " + edgeRef.Value.transform.localScale.z.ToString() + "\n";
        }

        return output;
    }

    private string FormatVertexPositionsToString() // vertex positions
    {
        string output = "";
        foreach (var vert in vertices)
        {
            output += GetVertexId(vert.Key).ToString() + " " + vert.Key.x.ToString() + " " + vert.Key.y.ToString() + " " + vert.Key.z.ToString() + " " + "\n";
        }
        return output;
    }

    public void Save()
    {
        FileIO.instance.WriteToFile(bridgePath + bridgeName + ".txt", FormatConnectionsToString());
        FileIO.instance.WriteToFile(bridgePath + bridgeName + "-positions.txt", FormatVertexPositionsToString());
    }

    public void SetPlatform(GameObject platformPrefab)
    {
        platform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, container.transform);
        SetWidth(platform.transform.localScale.z);
        SetLength(platform.transform.localScale.x);

    }

    public int GetId()
    {
        return id;
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

    public Edge EdgeExists(Vector3 position1, Vector3 position2)
    {
        Tuple<Vector3, Vector3> edgeKey1 = new Tuple<Vector3, Vector3>(position1, position2);

        // check if edge already exsits
        if (edges.ContainsKey(edgeKey1))
        {
            return edges[edgeKey1];
        }

        Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(position2, position1);

        if (edges.ContainsKey(edgeKey2))
        {
            return edges[edgeKey2];
        }
        return null;
    }

    public GameObject CreateVertex(Vector3 p, GameObject vertex)
    {
        Debug.Log("create:" + p);
        if (vertices.ContainsKey(p))
        {
            return vertices[p];
        }

        Transform newVertex = Instantiate(vertex.transform);
        newVertex.name = "Vertex";
        newVertex.localPosition = p;
        newVertex.parent = transform;

        vertices.Add(p, newVertex.transform.gameObject);

        return newVertex.gameObject;
    }

    public Edge CreateEdge(Vector3 v1, Vector3 v2, GameObject truss)
    {
        Edge edge = EdgeExists(v1, v2);
        if (edge != null)
        {
            return edge;
        }

        // create a new truss
        Transform newTruss = Instantiate(truss.transform);
        newTruss.name = "Truss";

        Vector3 diff = v1 - v2;
        float dist = diff.magnitude;
        newTruss.localScale = new Vector3(truss.transform.localScale.x, truss.transform.localScale.y, dist);
        newTruss.position = v2 + (diff / 2.0f);
        newTruss.LookAt(v1);
        newTruss.parent = transform;

        edge = newTruss.gameObject.GetComponent<Edge>();
        if (edge == null)
        {
            edge = newTruss.gameObject.AddComponent<Edge>();
        }
        edge.pos1 = v1;
        edge.pos2 = v2;
        edge.id = id;

        edges.Add(new Tuple<Vector3, Vector3>(v1, v2), edge);
        edgesId.Add(id, edge);


        id++;
        return edge;
    }

    public Edge CreateEdge(GameObject v1, GameObject v2, GameObject truss)
    { 
        return CreateEdge(v1.transform.position, v2.transform.position, truss);
    }

    /*    //same as create edge but takes vector3s DOES NOT ADD TO EDGES LIST
         public GameObject AddEdge(Vector3 v1, Vector3 v2, GameObject truss)
        {
            Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(v1, v2);

            // create a new truss
            Transform newTruss = Instantiate(truss.transform);
            newTruss.name = "Truss";

            Vector3 diff = v1 - v2;
            float dist = diff.magnitude;
            newTruss.localScale = new Vector3(truss.transform.localScale.x, truss.transform.localScale.y, dist);
            newTruss.position = v2 + (diff / 2.0f);
            newTruss.LookAt(v1);
            newTruss.parent = transform;

            return newTruss.gameObject;
        }*/

    public void RemoveVertex(Vector3 atPosition)
    {
        // find all the edge keys that contain the position
        List<Tuple<Vector3, Vector3>> edgekeysWithPosition = new List<Tuple<Vector3, Vector3>>();

        foreach (var pair in edges)
        {
            if (pair.Key.Item1 == atPosition || pair.Key.Item2 == atPosition)
            {
                edgekeysWithPosition.Add(pair.Key);
            }
        }

        // remove edges with the keys that we stored
        for (; edgekeysWithPosition.Count > 0;)
        {
            RemoveEdge(edgekeysWithPosition[0].Item1, edgekeysWithPosition[0].Item2);
            edgekeysWithPosition.Remove(edgekeysWithPosition[0]);
        }


        if (vertices.ContainsKey(atPosition))
        {
            Destroy(vertices[atPosition]);
            vertices.Remove(atPosition);
        }
    }

    public void RemoveEdge(Vector3 p1, Vector3 p2)
    {
        Edge edge = null;
        Tuple<Vector3, Vector3> edgeKey1 = new Tuple<Vector3, Vector3>(p1, p2);

        // check if edge already exsits
        if (edges.ContainsKey(edgeKey1))
        {
            edge = edges[edgeKey1];
            edges.Remove(edgeKey1);
        }

        Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(p2, p1);

        if (edges.ContainsKey(edgeKey2))
        {
            edge = edges[edgeKey2];
            edges.Remove(edgeKey2);
        }

        Destroy(edge.gameObject);

    }

    public void RemoveEdge(Edge edge)
    {
        RemoveEdge(edge.pos1, edge.pos2);
    }

    public void RemoveEdge(GameObject edge)
    {
        Edge edgeComponent = edge.GetComponent<Edge>();
        if (edgeComponent != null)
        {
            RemoveEdge(edgeComponent);
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

    public List<Edge> GetAllEdgesContainingPosition(Vector3 pos)
    {
        List<Edge> edgesWithPos = new List<Edge>();

        foreach (var edgePair in edges)
        {
            if (edgePair.Value.pos1 == pos || edgePair.Value.pos2 == pos)
            {
                edgesWithPos.Add(edgePair.Value);
            }
        }

        return edgesWithPos;
    }

    public void MoveVertex(GameObject vertex, Vector3 newPos)
    {
        /*        RemoveVertex(oldloc);
                return CreateVertex(newloc, fab); */
        Debug.Log("New Vertex at:" + newPos);
        Vector3 currentPos = vertex.transform.position;
        // get all edges containing that current position
        List<Edge> edgesWithPos = GetAllEdgesContainingPosition(currentPos);

        foreach (Edge edge in edgesWithPos)
        {

            Vector3 otherPos = edge.pos1;
            if (edge.pos1 == currentPos)
            {
                otherPos = edge.pos2;
            }

            CreateEdge(newPos, otherPos, edge.transform.gameObject);
            RemoveEdge(edge);
        }

        // remove the vertex from the dictionary
        vertices.Remove(currentPos);

        // move and add to vertex dictionary
        vertex.transform.position = newPos;
        vertices.Add(newPos, vertex.gameObject);
 

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


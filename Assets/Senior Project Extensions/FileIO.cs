using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileIO : MonoBehaviour
{
    public static FileIO instance;

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
        //WriteToFile("Bridges/test.txt", "Hello world");   
    }
    public void WriteToFile(string fileName, string output, bool overwrite)
    {
        /*        if (File.Exists(fileName))
                {
                    Debug.Log(fileName + " already exists.");
                    return;
                }*/
        //add to the end of file
        print(fileName);
        
        if (overwrite)
        {
            StreamWriter sr = File.CreateText(fileName);
            sr.WriteLine(output);
            sr.Close();
        } else
        {
            TextWriter tw = new StreamWriter(fileName, true);
            tw.WriteLine(output);
            tw.Close();
        }
    }

    public string ReadFromFile(string fileName)
    {
        string fileInfo = "";
        StreamReader reader = new StreamReader(fileName);
        fileInfo = reader.ReadToEnd();
        reader.Close();

        return fileInfo;
    }

    public string[] ReadBridgeFile(string fileName)
    {
        string[] tempInfo;
        string vertexInfo = "";
        string connectionInfo = null;
        string[] fileInfo = new string[2];
        StreamReader reader = new StreamReader(fileName);
        //fileInfo = reader.ReadToEnd();
        while(!reader.EndOfStream)
        {
            tempInfo = reader.ReadLine().Split(' ');
            if (tempInfo.Length == 5)
            {
                vertexInfo += tempInfo[1] + " ";
                vertexInfo += tempInfo[2] + " ";
                vertexInfo += tempInfo[3] + " ";
            }
            if (tempInfo.Length == 3)
            {
                connectionInfo += tempInfo[0] + " ";
                connectionInfo += tempInfo[1] + " ";
            }
        }
        print("vertex info:" + vertexInfo + '\n');
        print("Connection info:" + connectionInfo + '\n');
        reader.Close();
        fileInfo[0] = vertexInfo;
        fileInfo[1] = connectionInfo;

        return fileInfo;
    }
}

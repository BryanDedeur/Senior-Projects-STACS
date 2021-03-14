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
    public void WriteToFile(string fileName, string output)
    {
        /*        if (File.Exists(fileName))
                {
                    Debug.Log(fileName + " already exists.");
                    return;
                }*/
        //add to the end of file
        print(fileName);
        
        var sr = File.CreateText(fileName);
        sr.WriteLine(output);
        sr.Close();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
}

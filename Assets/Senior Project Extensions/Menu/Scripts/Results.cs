﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Results : MonoBehaviour
{
    private string filePath = "Results/results.txt";

    public void DisplayUI(List<string[]> valuesParsed)
    {
        for (int i = 0; i < valuesParsed.Count; i++)
        { // line - copy the row UI
            string temp = "";
            for (int j = 0; j < valuesParsed[i].Length; j++)
            { // value - adjust the UI value in the row
                temp += valuesParsed[i][j] + " ";
            }
            print(temp);
        }

    }

    public void LoadResults()
    {
        string data = FileIO.instance.ReadFromFile(filePath);
        string[] linesParsed = data.Split('\n');
        List<string[]> valuesParsed = new List<string[]>();

        for (int i = 0; i < linesParsed.Length; i++)
        {
            valuesParsed.Add(linesParsed[i].Split('\t'));
        }

        DisplayUI(valuesParsed);
    }
}
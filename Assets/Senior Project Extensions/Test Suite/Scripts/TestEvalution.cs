using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvalution : MonoBehaviour
{
    public List<GameObject> trackedRobots;
    private List<Vector3> trackedRobotPreviousPos;

    //to cac total grade average grades from all secions
    public float totalRobotTravelDistance = 0;
    public float targetTotalRobotTravelDistance = 0;

    public float totalTime = 0;
    public float targetTime = 0;

    public float totalBatteryUsage = 0;
    public float targetTotalBatteryUsage = 0;
  
    public int totalDefectsFound = 0;
    public int actualDefects = 0;

    // TODO consider loading the targets

    public bool testMode;
    private bool testEnded = false;

    public string resultsPath = "Results/results.txt";

    private void Awake()
    {
        trackedRobotPreviousPos = new List<Vector3>();
        foreach (GameObject robot in trackedRobots)
        {
            trackedRobotPreviousPos.Add(robot.transform.position);
        }
    }
    public float DistanceError()//return grade for this section
    {
        float difference = totalRobotTravelDistance - targetTotalRobotTravelDistance; //amount over target
        if (difference > 0)//find percent greater than target
        {
            float percent = difference / targetTotalRobotTravelDistance;
            return 1.0f - percent;
        }
        else
        {
            return 1.0f;
        }
    }

    public float BatteryError()//return grade for this section
    {
        float difference = totalBatteryUsage - targetTotalBatteryUsage; //positive num if acutal batts used greater than target
        if (difference > 0)//find percent greater than target
        {
            float percent = difference / targetTotalBatteryUsage;
            return 1.0f - percent;
        }
        else
        {
            return 1.0f;
        }
    }

    public float DefectsError()
    {
        if (totalDefectsFound > 0)
        {
            return actualDefects / totalDefectsFound;
        }
        return 0;
    }

    public float FinalGrade()
    {
        float distance = DistanceError();
        float battery = BatteryError();
        float defects = DefectsError();
        float a = 1;
        float b = 1;
        float c = 1;
        //a, b, c, are weight that we may want to change


        float final = a * distance + b * battery + c * defects;
        final /= 3;
        return final;
    }

    public void PrintGrade(float grade)
    {
       Debug.Log("You scored a:" + grade);
        
    }

    public void UpdateRobotPositionTracking()
    {
        for (int i = 0; i < trackedRobots.Count; i++)
        {
            totalRobotTravelDistance += (trackedRobotPreviousPos[i] - trackedRobots[i].transform.position).magnitude;
            trackedRobotPreviousPos[i] = trackedRobots[i].transform.position;
        }
    }

    public void EndTest()
    {
        testEnded = true;
        WriteToFile();
    }

    public void WriteToFile()
    {
        string output = "";

        output += "NA" + "\t"; // TODO get bridge name
        output += "NA" + "\t"; // TODO get bridge diffculty
        output += testMode.ToString() + "\t";
        output += FinalGrade().ToString() + "\t";
        output += totalRobotTravelDistance.ToString() + "\t";
        output += totalTime.ToString() + "\t";
        output += totalBatteryUsage.ToString() + "\t";
        output += totalDefectsFound.ToString();

        FileIO.instance.WriteToFile(resultsPath, output, false);
    }

    private void Update()
    {
        UpdateRobotPositionTracking();
        totalTime += Time.deltaTime;



        if (totalTime > targetTime && testMode && !testEnded)
        {
            EndTest();
        }
    }

}


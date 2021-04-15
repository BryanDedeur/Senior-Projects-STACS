using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEvalution : MonoBehaviour
{
    public static TestEvalution inst;
    public List<StacsEntity> trackedRobots;
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
        inst = this;
        //this will start the robot moving 
        trackedRobotPreviousPos = new List<Vector3>();
        foreach (StacsEntity robot in trackedRobots)
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
        UIMgr.inst.PauseGame();
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

    //for easy test select robot then press o 
    //robot will start rount then after 15 second all commands will be cleared
    //test is complete when robot reached the way point
    IEnumerator easyError()
    {
        Debug.Log("Error");
        yield return new WaitForSeconds(15);
        AIMgr.inst.HandleClear(trackedRobots);
    }

    public void easyTest()
    {
        Vector3 testEasy = new Vector3(-19.2f, 18.3f, -16.0f);
        AIMgr.inst.HandleMove(trackedRobots, testEasy);

        StartCoroutine(easyError());    //wait 15 seconds then do it
    }

    bool checkEnd()
    {
        Debug.Log("Checking end");

        Vector3 targetPos = new Vector3(-19.3f, 18.3f, -15.25f); //position of pink way point 
        float distance;
        foreach (StacsEntity robot in trackedRobots)
        {
            distance = Vector3.Distance(robot.transform.position, targetPos);
            Debug.Log("Distance:" + distance);
            if (distance < 5.0f)
            {
                Debug.Log("test edndy");
                EndTest();
            }

        }

        return true;
    }


    private void Update()
    {
        UpdateRobotPositionTracking();
        totalTime += Time.deltaTime;

        if (Input.GetKey(KeyCode.O)) //start robot on path to point 5 but the robot will stop
        {
            easyTest();
        }
        checkEnd();

    }

}


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
    private bool errorOccured = false;

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
        /* float distance = DistanceError();
         float battery = BatteryError();
         float defects = DefectsError();
         float a = 1;
         float b = 1;
         float c = 1;
         //a, b, c, are weight that we may want to change


         float final = a * distance + b * battery + c * defects;
         final /= 3;
         return final;   */
        float grade = 0;
        if (UIMgr.inst.mTest == TestState.EasyTest1 && UIMgr.inst.mTestType == EGameType.Test) //easy test
        {
            if (totalTime < 35)
            {
                grade = 100;
            }
            else if (totalTime >= 35 && totalTime < 40)
            {
                grade = 90;
            }
            else if (totalTime >= 40 && totalTime < 45)
            {
                grade = 80;
            }
            else if (totalTime >= 45 && totalTime < 50)
            {
                grade = 70;
            }
            else if (totalTime >= 50)
            {
                grade = 0;
            }
        }
        else if (UIMgr.inst.mTest == TestState.EasyTest2 && UIMgr.inst.mTestType == EGameType.Test) //medium test
        {

        }
        else if (UIMgr.inst.mTest == TestState.MediumTest && UIMgr.inst.mTestType == EGameType.Test) //hard  test
        {

        }
        else if (UIMgr.inst.mTest == TestState.EasyTest1 && UIMgr.inst.mTestType == EGameType.Practice) //easy prac test
        {

        }
        else if (UIMgr.inst.mTest == TestState.EasyTest2 && UIMgr.inst.mTestType == EGameType.Practice) // medium prac test
        {

        }
        return grade;
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
        UIMgr.inst.resultsPagePanel.SetActive(true);
        UIMgr.inst.GradeText.text = FinalGrade().ToString("0");
    }

    public void FailTest()
    {
        Debug.Log("Fail Test");
        //gnna add a fail ui stuff
        testEnded = true;
        WriteToFileFail();
        UIMgr.inst.PauseGame();
        UIMgr.inst.resultsPagePanelFailed.SetActive(true);
    }


 


    public void WriteToFile()
    {
        string output = "";

        output += "Steel Truss Bridge" + "\t"; // TODO get bridge name
        if(UIMgr.inst.mTest == TestState.EasyTest1 || UIMgr.inst.mTest == TestState.EasyTest2)
        {
            output += "Easy" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.MediumTest)
        {
            output += "Medium" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.HardTest)
        {
            output += "Hard" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Test)
        {
            output += "Test" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Practice)
        {
            output += "Practice Test" + "\t";
        }
        //output += testMode.ToString() + "\t";
        output += FinalGrade().ToString() + "\t";
        output += totalRobotTravelDistance.ToString() + "\t";
        output += totalTime.ToString() + "\t";
        output += totalBatteryUsage.ToString() + "\t";
        output += totalDefectsFound.ToString();

        FileIO.instance.WriteToFile(resultsPath, output, false, false);
    }

    public void WriteToFileFail()
    {
        string output = "";

        output += "Steel Truss Bridge" + "\t"; // TODO get bridge name
        if (UIMgr.inst.mTest == TestState.EasyTest1 || UIMgr.inst.mTest == TestState.EasyTest2)
        {
            output += "Easy" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.MediumTest)
        {
            output += "Medium" + "\t";
        }
        if (UIMgr.inst.mTest == TestState.HardTest)
        {
            output += "Hard" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Test)
        {
            output += "Test" + "\t";
        }
        if (UIMgr.inst.mTestType == EGameType.Practice)
        {
            output += "Practice Test" + "\t";
        }
        //output += testMode.ToString() + "\t";
        output += "0" + "\t";
        output += totalRobotTravelDistance.ToString() + "\t";
        output += totalTime.ToString() + "\t";
        output += totalBatteryUsage.ToString() + "\t";
        output += totalDefectsFound.ToString();

        FileIO.instance.WriteToFile(resultsPath, output, false, false);
    }
    //for easy test select robot then press o 
    //robot will start rount then after 15 second all commands will be cleared
    //test is complete when robot reached the way point
    IEnumerator Error()
    {
        if (UIMgr.inst.mTest == TestState.EasyTest1)
        {
            Debug.Log("Error easy");
            yield return new WaitForSeconds(15);
            AIMgr.inst.HandleClear(trackedRobots);

           
        }
        else if(UIMgr.inst.mTest == TestState.MediumTest)
        {
            Debug.Log("Error for medium started");
            yield return new WaitForSeconds(15);
            AIMgr.inst.HandleClear(trackedRobots[0]);
        }
        errorOccured = true;
    }

      public void easyTest1()
    {
        Vector3 testEasy = new Vector3(-19.2f, 18.3f, -16.0f);
        AIMgr.inst.HandleMove(trackedRobots, testEasy);

        StartCoroutine(Error());    //wait 15 seconds then do it
    }

    public void easyTest2()
    {
        Debug.Log("easytest2");
        foreach(StacsEntity robot in trackedRobots)
        {
            robot.desiredSpeed = 4.0f; 
        }
        errorOccured = true;

    }

    public void mediumTest()
    {
        Debug.Log("Medium Test");
        Vector3 testEasy = new Vector3(0f, 31.11f, -15.25f);
        AIMgr.inst.HandleMove(trackedRobots[0], testEasy);
        trackedRobots[1].desiredSpeed = 4.0f;
        StartCoroutine(Error());
        


    }


    bool checkEnd()
    {
        int counter = 0;
        float distance;
        foreach (StacsEntity robot in trackedRobots)
        {

            if(UIMgr.inst.mTest == TestState.EasyTest1 || UIMgr.inst.mTest == TestState.EasyTest2)
            {
                Vector3 targetPos = new Vector3(-19.3f, 18.3f, -15.25f); //position of pink way point 
                distance = Vector3.Distance(robot.transform.position, targetPos);
                if(distance < 3.0f)
                {
                    EndTest();
                }
            }
            else if (UIMgr.inst.mTest == TestState.MediumTest)
            {
                Vector3 targetPos = new Vector3(0.0f, 31.11f, -15.25f); //position of pink way point 
                distance = Vector3.Distance(robot.transform.position, targetPos);
                Debug.Log(distance);
                if (distance < 4.0f)
                {
                    counter++;
                }
            }

        }
        if(counter == 2)
        {
           EndTest();
        }

        return true;
    }

    public void hint()
    {
        Debug.Log("hint");
        if (trackedRobots[0].speed == 0.0f && UIMgr.inst.mTest == TestState.EasyTest1)
        {

            UIMgr.inst.PauseGame();
            UIMgr.inst.hintPanel.SetActive(true);
            errorOccured = false;

        }
        if(UIMgr.inst.mTest == TestState.EasyTest2 && totalRobotTravelDistance > 0.7f)
        {
            UIMgr.inst.PauseGame();
            UIMgr.inst.hintPanel.SetActive(true);
            errorOccured = false;
        }

        
    }


    private void Update()
    {
        UpdateRobotPositionTracking();
        totalTime += Time.deltaTime;

        if(testEnded == false )
        {
            checkEnd();
        }
        if(errorOccured && UIMgr.inst.mTestType == EGameType.Practice )
        {
            Debug.Log("wow");
            hint();
        }
        

    }

}


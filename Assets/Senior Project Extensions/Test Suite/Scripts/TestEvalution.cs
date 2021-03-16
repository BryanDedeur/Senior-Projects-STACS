using UnityEngine;

public class TestEvalution : MonoBehaviour
{
    //to cac total grade average grades from all secions
    public float totalRobotTravelDistance = 0;
    public float targetTotalRobotTravelDistance = 0;
    float amountUnderTarget = 0;

    public float totalBatteryUsage = 0;
    public float targetTotalBatteryUsage = 0;
  
    public int totalDefectsFound = 0;
    public int actualDefects = 0;


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
        return actualDefects / totalDefectsFound;
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

}


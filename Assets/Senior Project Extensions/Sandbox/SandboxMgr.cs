using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SandboxMgr : MonoBehaviour
{
    public GameObject climbingRobot;
    public GameObject ParrotDrone;
    private GameObject robotFolder;
    public List<GameObject> robots;
    public GameObject player;
    public float spawnOffset = 1;
    public static SandboxMgr instance;
    public GameObject bridgeObject;
    
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
        robotFolder = new GameObject();
        robotFolder.name = "Robots";
        robots = new List<GameObject>();
    }
    public void SpawnClimbing()
    {
        robots.Add(Instantiate(climbingRobot, GetNextSpawnLocation(), Quaternion.identity, robotFolder.transform));
    }
    public void SpawnDrone()
    {
        robots.Add(Instantiate(ParrotDrone, GetNextSpawnLocation(), Quaternion.identity, robotFolder.transform));
    }
    public void DespawnRobot()
    {
        StacsEntity robot = SelectionMgr.inst.selectedEntity;
        //robots.Remove(robot);
        Destroy(robot.gameObject); 
    }
    private Vector3 GetNextSpawnLocation()
    {
        return player.transform.position + player.transform.forward * spawnOffset;  
    }
    
}

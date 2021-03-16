﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum EGameState
{
    None = 0,
    GameMenu,
    ShowHelp,
    Briefing,
    Monitoring,
    UINavigating,
    AfterActionReview,
}


public enum EGameType
{
    None = 0,
    Sandbox,
    Practice,
    Test
}

public class UIMgr : MonoBehaviour
{
    public static UIMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public StacsPanel BriefingPanel;
    public StacsPanel MenuPanel;
    public StacsPanel HelpPanel;

    public RectTransform TopLevelMenuPanel;

    //Entity state left panel update
    public Text EntityTypeText; public Text EntityNameText;
    public Text EntityBatteryText;
    public Text EntitySpeedText; public Text EntityDesiredSpeedText;
    public Text EntityHeadingText; public Text EntityDesiredHeadingText;
    public Text EntityAltitudeText; public Text EntityDesiredAltitudeText;

    public Text DataFeedText;

    //public GameObject myCanvas;

    public Button gameMenuButton;
    public Button briefingPanelOkButton;
    public Button menuHelpButton;
    public Button helpDoneButton;

    // Start is called before the first frame update
    void Start()
    {
        State = EGameState.Briefing;
        Type = EGameType.Sandbox;
        CameraViewPanels.Clear();
    }
    public bool show = false;
    // Update is called once per frame
    void Update()
    {
        //ProtoPanel.isValid = show;
        UpdateSelectedEntity();
        CheckForUINavigation();



    }

    public LineGraph Graph;
    public void UpdateDataFeed(StacsEntity ent)
    {
        Graph.ConnectToEntity(ent);
    }

    void CheckForUINavigation()
    {
        if (Input.GetKeyUp(KeyCode.Joystick1Button7))
        {
            State = EGameState.GameMenu;
        }

    }

    [ContextMenu("UpdateProto")] //For testing Stacs Panels
    public void UpdateProto()
    {
        BriefingPanel.isValid = show;
        SandboxPanel.isValid = show;
    }


    public void UpdateSelectedEntity()
    {
        if (SelectionMgr.inst.selectedEntity != null)
        {

            EntityTypeText.text = SelectionMgr.inst.selectedEntity.entityType.ToString();
            EntityNameText.text = SelectionMgr.inst.selectedEntity.name;
            EntityBatteryText.text = SelectionMgr.inst.selectedEntity.batteryState.ToString("F1") + "%";
            EntitySpeedText.text = SelectionMgr.inst.selectedEntity.speed.ToString("F1") + "m/s";
            EntityDesiredSpeedText.text = SelectionMgr.inst.selectedEntity.desiredSpeed.ToString("F1") + "m/s";
            EntityHeadingText.text = SelectionMgr.inst.selectedEntity.heading.ToString("F1") + "deg";
            EntityDesiredHeadingText.text = SelectionMgr.inst.selectedEntity.desiredHeading.ToString("F1") + "deg";
            EntityAltitudeText.text = SelectionMgr.inst.selectedEntity.altitude.ToString("F1") + "m";
            EntityDesiredAltitudeText.text = SelectionMgr.inst.selectedEntity.desiredAltitude.ToString("F1") + "m";
            if (SelectionMgr.inst.selectedEntity.entityType == EntityType.ClimbingRobot)
                DataFeedText.text = "Data: " + SelectionMgr.inst.selectedEntity.name;
        }
    }

    public EGameState priorState;
    public EGameState _state = EGameState.None;
    //[System.Serializable]
    public EGameState State
    {
        get { return _state; }
        set
        {
            priorState = _state;
            _state = value;

            BriefingPanel.isValid = (_state == EGameState.Briefing);
            HelpPanel.isValid = (_state == EGameState.ShowHelp);
            MenuPanel.isValid = (_state == EGameState.GameMenu);

            //Game Controller UI/Playing switch and Navigation
            switch (_state)
            {
                case EGameState.Briefing:
                    EventSystem.current.firstSelectedGameObject = briefingPanelOkButton.gameObject;
                    break;
                case EGameState.Monitoring:
                    EventSystem.current.firstSelectedGameObject = null;
                    briefingPanelOkButton.Select();
                    break;
                case EGameState.GameMenu:
                    EventSystem.current.firstSelectedGameObject = menuHelpButton.gameObject;
                    menuHelpButton.Select();
                    break;
                case EGameState.ShowHelp:
                    EventSystem.current.firstSelectedGameObject = helpDoneButton.gameObject;
                    helpDoneButton.Select();
                    break;
                default:
                    EventSystem.current.firstSelectedGameObject = null;
                    break;
            }


        }
    }

    //on briefing panel Ok button click
    public void StartGame()
    {
        //Debug.Log("GameStarting");
        State = EGameState.Monitoring;
    }

    public void HandleMenuHelp()
    {
        State = EGameState.ShowHelp;
    }

    public void HandleMenu()
    {
        State = EGameState.GameMenu;
    }

    public void HandleMenuBack()
    {
        State = priorState;
    }

    public void HandleMenuQuitTask()
    {
        SceneManager.LoadScene(0);
    }

    public void HandleMenuQuitToOS()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    public void HandleHelpDone()
    {
        State = EGameState.Monitoring;
    }
    //--------------------------------------------------------------------------------------------

    public RectTransform CameraViewPanelPrefab;
    public RectTransform CameraViewPanelsParent;
    public List<RectTransform> CameraViewPanels;

    public void MakeCameraView(RenderTexture tex)
    {
        RectTransform cameraViewPanel = Instantiate(CameraViewPanelPrefab, CameraViewPanelsParent);
        RawImage ri = cameraViewPanel.GetComponentInChildren<RawImage>();
        if (ri != null)
        {
            ri.texture = tex;
            CameraViewPanels.Add(cameraViewPanel);
        }

    }
    //-------------------------------------------------------------------------------------------------
    public void MirrorButtonOn()
    {
        Debug.Log("on");
        BridgeCreator.instance.EnableMirroring();
    }

    public void MirrorButtonOff()
    {
        Debug.Log("off");
        BridgeCreator.instance.DisableMirroring();
    }
    public void MirrorSelectedButton()
    {
        Debug.Log("Selected");
        BridgeCreator.instance.MirrorSelectedObjects();
    }
    public void DeselectButton()
    {
        BCSelectionMgr.instance.DisableBoxSelection();
        BCSelectionMgr.instance.DeselectAll();
    }
    public void RestartScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
  
    public void DiscardBridge()
    {
        RestartScene();
    }
    public StacsPanel SandboxPanel;
    public StacsPanel PracticePanel;
    public StacsPanel testPanel;


    public EGameType priorType;
    public EGameType _type = EGameType.None;
    //[System.Serializable]
    public EGameType Type
    {
        get { return _type; }
        set
        {
            priorType = _type;
            _type = value;

            SandboxPanel.isValid = (_type == EGameType.Sandbox);
            PracticePanel.isValid = (_type == EGameType.Practice);
            testPanel.isValid = (_type == EGameType.Test);

            //Game Controller UI/Playing switch and Navigation
            switch (_type)
            {
                case EGameType.Sandbox:
                    EventSystem.current.firstSelectedGameObject = MenuMgr.instance.SandboxButton.gameObject;
                    break;
               /* case EGameType.Practice:
                    EventSystem.current.firstSelectedGameObject = null;
                    briefingPanelOkButton.Select();
                    break;
              */
                case EGameType.Practice:
                    EventSystem.current.firstSelectedGameObject =MenuMgr.instance.PracticeButton.gameObject;
                    MenuMgr.instance.PracticeButton.Select();
                    break;
                case EGameType.Test:
                    EventSystem.current.firstSelectedGameObject = helpDoneButton.gameObject;
                    MenuMgr.instance.TestButton.Select();
                    break;
                default:
                    EventSystem.current.firstSelectedGameObject = null;
                    break;
            }


        }
    }
}

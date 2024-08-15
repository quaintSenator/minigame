using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StoryEventData : EventData
{
    public int stepIndex = 0;
    public bool doNextStep = false;
    public StoryEventData(int index, bool isDo = false)
    {
        stepIndex = index;
        doNextStep = isDo;
    }
}


public class StoryController : MonoBehaviour
{

    public Transform player;
    public Transform enemy;

    [SerializeField]
    private List<EventType> storySteps = new List<EventType>();
    /*
    Step 1：方块走路
    Step 2：三角形快速出现
    Step 3：三角形停下 说第一句话
    Step 4：方块停下
    Step 5：三角形说第二句话
    Step 6：三角形说第三句话
    Step 7：方块开始跑,三角形说第四句话
    Step 8：三角形开始追
    Step 9：浮现第一句背景台词
    */
    private List<Action> functions = new List<Action>();

    [SerializeField]
    private float cubeIdleSpeed = 4.0f;
    [SerializeField]
    private float cubeRunSpeed = 8.0f;
    [SerializeField]
    private float triangleRunSpeed = 6.0f;

    [SerializeField]
    private float defaultWaitTime = 1.0f;
    [SerializeField]
    private float cubeIdleTime = 2.0f;
    [SerializeField]
    private float triangleAppearTime = 0.5f;
    [SerializeField]
    private float cubeStopIdleTime = 0.5f;
    [SerializeField]
    private float cubeStartRunTime = 0.5f;
    [SerializeField]
    private float triangleStartRunTime = 0.5f;
    [SerializeField]
    private float endTime = 5.0f;
    [SerializeField]
    private float bgDialog1EndTime = 5.0f;

    [SerializeField]
    private bool autoPlay = true;

    private int stepIndex = 0;
    private bool canNextStep = false;

    private readonly StoryEventData mustDoStepData = new StoryEventData(-1, true);
    private string[] dialogs = {
        "我是一个正方形, 我生活的世界叫作平面国。",
    };

    private PlayerController playerController;
    private PlayerStoryController playerStoryController;
    private EnemyStoryController enemyStoryController;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        playerStoryController = player.GetComponent<PlayerStoryController>();
        enemyStoryController = enemy.GetComponent<EnemyStoryController>();
    }

    private void Start()
    {
        playerController.enabled = false;
        playerStoryController.enabled = true;
        functions.Add(Step_1);
        functions.Add(Step_2);
        functions.Add(Step_3);
        functions.Add(Step_4);
        functions.Add(Step_5);
        functions.Add(Step_6);
        functions.Add(Step_7);
        functions.Add(Step_8);
        functions.Add(Step_9);
        EventManager.InvokeEvent(EventType.StoryStartEvent, mustDoStepData);
    }

    private void OnEnable()
    {
        foreach (EventType eventType in storySteps)
        {
            EventManager.AddListener(eventType, DoStep);
        }
        EventManager.AddListener(EventType.StoryStartEvent, NextStep);
        EventManager.AddListener(EventType.NextStepEvent, NextStep);
    }

    private void OnDisable()
    {
        foreach(EventType eventType in storySteps)
        {
            EventManager.RemoveListener(eventType, DoStep);
        }
        EventManager.RemoveListener(EventType.NextStepEvent, NextStep);
        EventManager.AddListener(EventType.StoryStartEvent, NextStep);        
    }

    private void NextStep(EventData data = null)
    {
        StoryEventData storyData;
        if(data == null)
        {
            storyData = new StoryEventData(-1, false);
        }
        else{
            storyData = (StoryEventData)data;
        }

        if(storyData.doNextStep || canNextStep)
        {
            Debug.Log("NextStep"+stepIndex);
            EventManager.InvokeEvent(storySteps[stepIndex], new StoryEventData(stepIndex));
            stepIndex++;

        }
    }

    private void DoStep(EventData data = null)
    {
        if(data == null){
            return;
        }
        StoryEventData storyData = (StoryEventData)data;
        Debug.Log("step"+storyData.stepIndex);
        int index = storyData.stepIndex;
        functions[index].Invoke();
    }

    private void Step_1()
    {
        playerStoryController.SetSpeed(cubeIdleSpeed);

        canNextStep = false;
        CleverTimerManager.Ask4Timer(cubeIdleTime, NextStep, mustDoStepData);
    }

    private void Step_2()
    {
        enemyStoryController.SetSpeed(triangleRunSpeed);

        canNextStep = false;
        CleverTimerManager.Ask4Timer(triangleAppearTime, NextStep, mustDoStepData);
    }

    private void Step_3()
    {
        enemyStoryController.SetSpeed(0);
        enemyStoryController.StartSpeak();

        canNextStep = false;
        CleverTimerManager.Ask4Timer(cubeStopIdleTime, NextStep, mustDoStepData);
    }

    private void Step_4()
    {
        playerStoryController.SetSpeed(0);

        if(autoPlay){
            canNextStep = false;
            CleverTimerManager.Ask4Timer(defaultWaitTime, NextStep, mustDoStepData);
        }
        else{
            canNextStep = true;
        }

    }

    private void Step_5()
    {
        enemyStoryController.EndSpeak();
        enemyStoryController.StartSpeak();

        if(autoPlay){
            canNextStep = false;
            CleverTimerManager.Ask4Timer(defaultWaitTime, NextStep, mustDoStepData);
        }
        else{
            canNextStep = true;
        }
    }

    private void Step_6()
    {
        enemyStoryController.EndSpeak();
        enemyStoryController.StartSpeak();

        canNextStep = false;
        CleverTimerManager.Ask4Timer(cubeStartRunTime, NextStep, mustDoStepData);
    }

    private void Step_7()
    {
        playerStoryController.SetSpeed(cubeRunSpeed);
        enemyStoryController.EndSpeak();
        enemyStoryController.StartSpeak();

        canNextStep = false;
        CleverTimerManager.Ask4Timer(triangleStartRunTime, NextStep, mustDoStepData);
        
    }

    private void Step_8()
    {
        enemyStoryController.SetSpeed(triangleRunSpeed);
        enemyStoryController.EndSpeak();

        canNextStep = false;
        CleverTimerManager.Ask4Timer(endTime, NextStep, mustDoStepData);
    }

    private void Step_9()
    {
        EndStory();
    }

    private void EndStory()
    {
        EventManager.InvokeEvent(EventType.StoryEndEvent);
        playerController.enabled = true;
        playerStoryController.enabled = false;
        SceneManager.LoadScene("Level_1", LoadSceneMode.Single);
    }

}

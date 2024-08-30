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
    */
    private List<Action> functions = new List<Action>();

    [SerializeField]
    private float cubeIdleSpeed = 4.0f;
    [SerializeField]
    private float cubeRunSpeed = 8.0f;
    [SerializeField]
    private float triangleRunSpeed = 6.0f;

    [SerializeField]
    private List<float> stepTimes;

    [SerializeField]
    private bool autoPlay = true;

    private int stepIndex = 0;
    private bool canNextStep = false;

    private readonly StoryEventData mustDoStepData = new StoryEventData(-1, true);

    private PlayerController playerController;
    private PlayerStoryController playerStoryController;
    private EnemyStoryController enemyStoryController;
    private ParticleSystem frictionEffect;
	
	
	private bool ifEnd = false;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        playerStoryController = player.GetComponent<PlayerStoryController>();
        enemyStoryController = enemy.GetComponent<EnemyStoryController>();
        frictionEffect = player.Find("FrictionEffect").GetComponent<ParticleSystem>();

        functions.Add(Step_1);
        functions.Add(Step_2);
        functions.Add(Step_3);
        functions.Add(Step_4);
        functions.Add(Step_5);
        functions.Add(Step_6);
        functions.Add(Step_7);
        functions.Add(Step_8);
        functions.Add(Step_9);
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        foreach (EventType eventType in storySteps)
        {
            EventManager.AddListener(eventType, DoStep);
        }
        EventManager.AddListener(EventType.StartStoryEvent, OnStartStoryEvent);
        EventManager.AddListener(EventType.NextStepEvent, NextStep);
    }

    private void OnDisable()
    {
        foreach(EventType eventType in storySteps)
        {
            EventManager.RemoveListener(eventType, DoStep);
        }
        EventManager.RemoveListener(EventType.NextStepEvent, NextStep);
        EventManager.RemoveListener(EventType.StartStoryEvent, OnStartStoryEvent);        
    }

    private void OnStartStoryEvent(EventData data)
    {
        playerController.enabled = false;
        playerStoryController.enabled = true;
        frictionEffect.Stop();
        EventManager.InvokeEvent(EventType.NextStepEvent, mustDoStepData);
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
            EventManager.InvokeEvent(storySteps[stepIndex], new StoryEventData(stepIndex));
            stepIndex++;

        }
    }

    private void DoStep(EventData data = null)
    {
		if(ifEnd)
		{
			return;
		}
        if(data == null){
            return;
        }
        StoryEventData storyData = (StoryEventData)data;
        Debug.Log("step"+storyData.stepIndex);
        int index = storyData.stepIndex;
        functions[index].Invoke();
        if(index < functions.Count - 1){
            CleverTimerManager.Ask4Timer(stepTimes[index], NextStep, mustDoStepData);
        }

    }

    private void Step_1()
    {
        playerStoryController.SetSpeed(cubeIdleSpeed);

        canNextStep = false;
    }

    private void Step_2()
    {
        enemyStoryController.StartBgSpeak();

        canNextStep = false;
    }

    private void Step_3()
    {
        playerStoryController.SetSpeed(0);
        enemyStoryController.SetSpeed(triangleRunSpeed);    
        enemyStoryController.EndBgSpeak();

        canNextStep = false;
    }

    private void Step_4()
    {
        enemyStoryController.SetSpeed(0);

        if(autoPlay){
            canNextStep = false;
            //CleverTimerManager.Ask4Timer(defaultWaitTime, NextStep, mustDoStepData);
        }
        else{
            canNextStep = true;
        }

    }

    private void Step_5()
    {
        enemyStoryController.StartSpeak();

        if(autoPlay){
            canNextStep = false;
           // CleverTimerManager.Ask4Timer(defaultWaitTime, NextStep, mustDoStepData);
        }
        else{
            canNextStep = true;
        }
    }

    private void Step_6()
    {
        enemyStoryController.StartSpeak();

        canNextStep = false;
        //CleverTimerManager.Ask4Timer(cubeStartRunTime, NextStep, mustDoStepData);
    }

    private void Step_7()
    {
        playerStoryController.SetSpeed(cubeRunSpeed);
        frictionEffect.Play();

        canNextStep = false;
        //CleverTimerManager.Ask4Timer(triangleStartRunTime, NextStep, mustDoStepData);
        
    }

    private void Step_8()
    {
        enemyStoryController.StartSpeak();
        enemyStoryController.SetSpeed(triangleRunSpeed);

        canNextStep = false;
        //CleverTimerManager.Ask4Timer(endTime, NextStep, mustDoStepData);
    }

    private void Step_9()
    {
        EndStory();
    }

    private void EndStory()
    {
		ifEnd = true;
        playerController.enabled = true;
        enemyStoryController.gameObject.SetActive(false);
        playerStoryController.enabled = false;
        EventManager.InvokeEvent(EventType.EndStoryEvent);
    }

}

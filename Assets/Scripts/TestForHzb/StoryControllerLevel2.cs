using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class StoryControllerLevel2 : MonoBehaviour
{

    public Transform player;
    public Transform enemy;

    [SerializeField]
    private List<EventType> storySteps = new List<EventType>();
    /*
    Step 1：方块停在原地
    Step 2：方块说第一句话
    Step 3：说第二句话
    Step 4：旁白第一句
    Step 5：方块第三句
    Step 6：旁白第二句
    Step 7：旁白第三句
    Step 8：方块第四句
    Step 9：旁白第四句
    Step 10：正方形运动
    Step 11：旁白第五句
    */
    private List<Action> functions = new List<Action>();

    [SerializeField]
    private float cubeIdleSpeed = 0;
    [SerializeField]
    private float cubeRunSpeed = 8.0f;

    [SerializeField]
    private List<float> stepTimes;

    [SerializeField]
    private bool autoPlay = true;

    private int stepIndex = 0;
    private bool canNextStep = false;

    private readonly StoryEventData mustDoStepData = new StoryEventData(-1, true);

    private PlayerController playerController;
    private PlayerStoryController playerStoryController;
    private EnemyStoryController2 enemyStoryController;
    private ParticleSystem frictionEffect;
	
	
	private bool ifEnd = false;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        playerStoryController = player.GetComponent<PlayerStoryController>();
        enemyStoryController = enemy.GetComponent<EnemyStoryController2>();
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
        functions.Add(Step_10);
        functions.Add(Step_11);
        functions.Add(Step_12);
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
        canNextStep = false;
        if(index < functions.Count - 1){
            CleverTimerManager.Ask4Timer(stepTimes[index], NextStep, mustDoStepData);
        }
    }

    private void Step_1()
    {
        playerStoryController.SetSpeed(cubeIdleSpeed);

    }

    private void Step_2()
    {
        enemyStoryController.StartSpeak();

    }

    private void Step_3()
    {
        enemyStoryController.StartSpeak();
    }

    private void Step_4()
    {
        enemyStoryController.StartBgSpeak();

    }

    private void Step_5()
    {
        enemyStoryController.EndBgSpeak();
        enemyStoryController.StartSpeak();
    }

    private void Step_6()
    {
        enemyStoryController.StartBgSpeak();

    }

    private void Step_7()
    {
        enemyStoryController.EndBgSpeak();
        enemyStoryController.StartBgSpeak();
        
    }

    private void Step_8()
    {
        enemyStoryController.StartSpeak();
    }

    private void Step_9()
    {
        enemyStoryController.StartBgSpeak();
    }

    private void Step_10()
    {
        enemyStoryController.EndBgSpeak();
        playerStoryController.SetSpeed(cubeRunSpeed);
        frictionEffect.Play();
    }

    private void Step_11()
    {
        enemyStoryController.StartBgSpeak();
    }

    private void Step_12()
    {
        enemyStoryController.EndBgSpeak();
        EndStory();
    }

    private void EndStory()
    {
        Debug.Log("EndStory");
		ifEnd = true;
        playerController.enabled = true;
        enemyStoryController.gameObject.SetActive(false);
        playerStoryController.enabled = false;
        EventManager.InvokeEvent(EventType.EndStoryEvent);

    }

}

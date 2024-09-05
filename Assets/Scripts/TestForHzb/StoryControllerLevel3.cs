using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class StoryControllerLevel3 : MonoBehaviour
{

    public Transform player;
    public Transform boss;

    [SerializeField]
    private List<EventType> storySteps = new List<EventType>();
    /*
    Step 1：方块停在原地
    Step 2：方块说第一句话
    Step 3：boss出现
    Step 4：boss第一句
    Step 5：玩家第二句
    Step 6：boss第二句
    Step 7：玩家第三句
    Step 8：世界上不存在第三个维度。
    Step 9：玩家：书中记载，第三维度在最北方。球先生也请我到这里找他。
    Step 10：僧侣：哼，那个自称“球”的小人？
    Step 11：僧侣：它在雾里隐藏自己，能够变得忽大忽小
    Step 12：僧侣：但它是假的，孩子。
    Step 13：僧侣：没有第三个维度，也没有“球”。
    Step 14：僧侣：这里是最北方，平面国最高贵的地方
    Step 15：僧侣：你已经进化成了高贵的圆，为什么不停下来与我分享一切呢？
    Step 16：玩家：僧侣大人，对于你而言，说谎可能是轻而易举的事。
    Step 17：boss蓄力
    Step 18：玩家：可我就是相信着球先生和三次方的存在。
    Step 19：玩家：如果你坚信它们都是假的，就请连同我——
    Step 20：玩家：一起毁灭掉吧！
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
    private BossStoryController bossStoryController;
    private BossController bossController;
    private ParticleSystem frictionEffect;
	
	
	private bool ifEnd = false;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        playerStoryController = player.GetComponent<PlayerStoryController>();
        bossStoryController = boss.GetComponent<BossStoryController>();
        bossController = boss.GetComponent<BossController>();
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
        functions.Add(Step_13);
        functions.Add(Step_14);
        functions.Add(Step_15);
        functions.Add(Step_16);
        functions.Add(Step_17);
        functions.Add(Step_18);
        functions.Add(Step_19);
        functions.Add(Step_20);
        functions.Add(Step_21);
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
        bossStoryController.StartSpeak(1);

    }

    private void Step_3()
    {
        bossController.ShowUp();
        bossStoryController.StartBgSpeak();
    }

    private void Step_4()
    {
        bossStoryController.EndBgSpeak();

    }

    private void Step_5()
    {

        bossStoryController.StartSpeak(1);
    }

    private void Step_6()
    {
        bossStoryController.StartSpeak(2);

    }

    private void Step_7()
    {
        bossStoryController.StartSpeak(1);
        
    }

    private void Step_8()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_9()
    {
        bossStoryController.StartSpeak(1);
    }

    private void Step_10()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_11()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_12()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_13()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_14()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_15()
    {
        bossStoryController.StartSpeak(2);
    }

    private void Step_16()
    {
        bossStoryController.StartSpeak(1);
    }

    private void Step_17()
    {
        
    }

    private void Step_18()
    {
        bossStoryController.StartSpeak(1);
    }

    private void Step_19()
    {
        bossStoryController.StartSpeak(1);
    }

    private void Step_20()
    {
        bossStoryController.StartSpeak(1);
    }

    private void Step_21()
    {
        EndStory();
    }

    private void EndStory()
    {
        Debug.Log("EndStory");
		ifEnd = true;
        playerController.enabled = true;
        bossStoryController.gameObject.SetActive(false);
        playerStoryController.enabled = false;
        frictionEffect.Play();
        EventManager.InvokeEvent(EventType.EndStoryEvent);

    }

}

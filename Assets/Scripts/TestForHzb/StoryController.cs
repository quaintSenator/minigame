using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StoryEventData : EventData
{
    public int stepIndex = 0;
    public StoryEventData(int index)
    {
        stepIndex = index;
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
    Step 7：方块开始跑
    Step 8：三角形开始追
    */
    private List<Action> functions = new List<Action>{
        Step_1, Step_2, Step_3, Step_4
    };

    [SerializeField]
    readonly private float cubeIdleSpeed = 4.0f;
    [SerializeField]
    readonly private float cubeRunSpeed = 8.0f;
    [SerializeField]
    readonly private float triangleRunSpeed = 6.0f;

    private int stepIndex = 0;

    private PlayerController playerController;
    private PlayerStoryController playerStoryController;
    private EnemyStoryController enemyStoryController;

    private void Awake()
    {
        playerController = player.GetComponent<PlayerController>();
        playerStoryController = player.GetComponent<PlayerStoryController>();
        enemyStoryController = enemy.GetComponent<EnemyStoryController>();
    }

    private void OnEnable()
    {
        foreach (EventType eventType in storySteps)
        {
            EventManager.AddListener(eventType, DoStep);
        }
    }

    private void OnDisable()
    {
        foreach(EventType eventType in storySteps)
        {
            EventManager.RemoveListener(eventType, DoStep);
        }        
    }

    private void NextStep()
    {
        EventManager.InvokeEvent(storySteps[stepIndex], new StoryEventData(stepIndex));
    }

    private void DoStep(EventData data = null)
    {
        if(data == null){
            return;
        }

        StoryEventData storyData = (StoryEventData)data;
        int index = storyData.stepIndex;
        functions[index].Invoke();
    }

    private static void Step_1()
    {
        
    }
    private static void Step_2()
    {
        
    }
    private static void Step_3()
    {
        
    }
    private static void Step_4()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        playerController.enabled = false;
        playerStoryController.enabled = true;
    }

}

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
    private List<Action> functions = new List<Action>{
        Step_1, Step_2, Step_3, Step_4
    };

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

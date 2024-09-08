using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Sirenix.OdinInspector;
public class Epilogue : MonoBehaviour
{


    public bool isPlayingEpligue = false;
    public List<float> durationTime = new List<float>();
    public List<Action> step=new List<Action>();

    private float currentStepStartTime = -1;

    //Mask 相关
    public float durationMaskTime = 2;
    private bool isChangingToOpacity = false;
    private bool isChangingToTransparency = false;
    public Image mask = null;

    //Step 0
    public Camera mainCamera = null;
/*    public Color normalColor = Color.white;
    public Color promoteColor = Color.white;*/


    //0.68
    //1

    //Step1 

    GameObject background = null;

    int currentDialogIndex = 0;
    //public List<float> dialogStartDurationTime = new List<float>();
    public List<GameObject> dialogStart = new List<GameObject>();


    //public List<float> dialogEndDurationTime = new List<float>();
    public List<GameObject> dialogEnd = new List<GameObject>();

    int currentStepIndex = 0;


    private EndingAnimation animation;

    [Button]
    private void DebugEpilogue()
    {
        EventManager.InvokeEvent(EventType.StartEpilogueEvent);
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.StartEpilogueEvent, OnStartEpilogueEvent);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.StartEpilogueEvent, OnStartEpilogueEvent);
    }

    private void OnStartEpilogueEvent(EventData eventData )
    {
        currentStepStartTime = Time.time;
        isPlayingEpligue = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        step.Add(Step0_EnableMask);
        step.Add(Step1_ShowStartDialog1);
        step.Add(Step2_ShowStartDialog2);
        step.Add(Step3_ShowStartDialog3);
        step.Add(Step4_ShowStartDialog4);
        step.Add(Step5_ShowAnimation);
        step.Add(Step6_EnableMask);
        step.Add(Step7_ShowEndDialog1);
        step.Add(Step8_ShowEndDialog2);
        step.Add(Step9_ShowEndDialog3);
        step.Add(Step10_EndEpilogue);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPlayingEpligue)
        {
            return;
        }
        UpdateMask();

        if (currentStepIndex < step.Count 
            && Time.time > currentStepStartTime + durationTime[currentStepIndex])
        {


            step[currentStepIndex].Invoke();
            currentStepIndex++;
            currentStepStartTime = Time.time;

        }


    }



    void UpdateMask()
    {
        if(isChangingToOpacity)
        {
            float elapsedTime = Time.time - currentStepStartTime; // 已经经过的时间
            float t = Mathf.Clamp01(elapsedTime / durationMaskTime); // 计算插值因子，确保在0和1之间
            float newAlpha = Mathf.Lerp(0, 1, t);
            mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, newAlpha);

            if(t ==1)
            {
                isChangingToOpacity = false;
            }
        }
        if(isChangingToTransparency)
        {
            float elapsedTime = Time.time - currentStepStartTime; // 已经经过的时间
            float t = Mathf.Clamp01(elapsedTime / durationMaskTime); // 计算插值因子，确保在0和1之间
            float newAlpha = Mathf.Lerp(1, 0, t);
            mask.color = new Color(mask.color.r, mask.color.g, mask.color.b, newAlpha);

            if (t == 1)
            {
                isChangingToTransparency = false;
            }
        }
    }


    //index:0
    void Step0_EnableMask()
    {
        isChangingToOpacity = true;
/*        float elapsedTime = Time.time - currentStepStartTime; // 已经经过的时间
        float t = Mathf.Clamp01(elapsedTime / durationTime[currentStepIndex]); // 计算插值因子，确保在0和1之间

        //this.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

        if (t == 1)
        {
            //currentStepIndex++;
        }*/
    }


    void Step1_ShowStartDialog1()
    {
        //background.SetActive(true);
        dialogStart[currentDialogIndex].SetActive(true);
        currentDialogIndex++;
    }

    void Step2_ShowStartDialog2()
    {
        //background.SetActive(true);
        dialogStart[currentDialogIndex-1].SetActive(false);
        dialogStart[currentDialogIndex].SetActive(true);
        currentDialogIndex++;
    }

    void Step3_ShowStartDialog3()
    {
        //background.SetActive(true);
        dialogStart[currentDialogIndex - 1].SetActive(false);
        dialogStart[currentDialogIndex].SetActive(true);
        currentDialogIndex++;
    }

    void Step4_ShowStartDialog4()
    {
        //background.SetActive(true);
        dialogStart[currentDialogIndex - 1].SetActive(false);
        dialogStart[currentDialogIndex].SetActive(true);
        //currentDialogIndex=0;
    }


    void Step5_ShowAnimation()
    {
        dialogStart[currentDialogIndex].SetActive(false);
        animation = EndingAnimation.SpawnEnding(Camera.main.transform);
        isChangingToTransparency = true;
    }

    void Step6_EnableMask()
    {
        //isChangingToOpacity = true;
        isChangingToOpacity = true;
    }

    void Step7_ShowEndDialog1()
    {
        currentDialogIndex = 0;
        dialogEnd[currentDialogIndex].SetActive(true);
        currentDialogIndex++;
        //animation.StopMove();
        // animation.
    }

    void Step8_ShowEndDialog2()
    {
        dialogEnd[currentDialogIndex - 1].SetActive(false);
        dialogEnd[currentDialogIndex].SetActive(true);
        currentDialogIndex++;
        //currentDialogIndex = 0;
    }

    void Step9_ShowEndDialog3()
    {
        dialogEnd[currentDialogIndex - 1].SetActive(false);
        dialogEnd[currentDialogIndex].SetActive(true);
        //currentDialogIndex = 0;
    }

    void Step10_EndEpilogue()
    {
        dialogEnd[currentDialogIndex].SetActive(false);
        EventManager.InvokeEvent(EventType.EndEpilogueEvent);
    }

}

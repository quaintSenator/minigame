using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoStageController : BuildableBase
{

    public bool ifRotateSelf = true;
    public float selfRotateSpeed = 30.0f;
    public float selfRotateSpeedSpecial= 60.0f;
    public Material noBlendMat = null;
    public List<int> needSpacialProcessLevelIndexs = null;
    public bool debugReplaceMat = false;
    //private int Index = 0;

    public override void Init()
    {
        if(debugReplaceMat)
        {
            if (!noBlendMat)
            {
                return;
            }
            Renderer renderer = GetComponent<Renderer>();
            renderer.material = noBlendMat;
            return;
        }

        int currentLevelIndex = ProgressManager.Instance.GetCurrentLevelIndex();
        foreach(int needSpacialProcessLevelIndex in needSpacialProcessLevelIndexs)
        {
            if(needSpacialProcessLevelIndex == currentLevelIndex )
            {
                if(!noBlendMat)
                {
                    return;
                }
                Renderer renderer = GetComponent<Renderer>();
                renderer.material = noBlendMat;

                selfRotateSpeed = selfRotateSpeedSpecial;
                return;
            }
        }

    }

     private void SelfRotateSD()
    {
        transform.Rotate(Vector3.forward, selfRotateSpeed * Time.deltaTime);
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        player.SetIsGrounded(true);
    }

    protected override void TriggerOffThisBuildable(PlayerController player)
    {
        player.SetIsGrounded(false);
    }


    private void Update()
    {
        if (ifRotateSelf)
        {
            SelfRotateSD();
        }

    }

}
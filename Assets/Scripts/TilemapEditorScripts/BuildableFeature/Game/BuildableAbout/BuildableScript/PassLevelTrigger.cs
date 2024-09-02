using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 执行流程
/// 1、初始化向Player或者流程Manager动态注册重生点，之后再通过返回自身状态
/// 2、玩家经过检查点，向Player或者流程Manager，通知经过事件，永久在管理类中修改状态。
/// 
/// 关于重生点可能要考虑在地图中预先把index存一下
/// </summary>
public class PassLevelTrigger : BuildableBase
{

	public Renderer renderer=null;
	


    protected override void TriggerThisBuildable(PlayerController player)
    {

        EventManager.InvokeEvent(EventType.StartPassLevelEvent);
    }



}
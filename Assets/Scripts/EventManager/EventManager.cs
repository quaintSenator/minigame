using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace miniEvent
{
    public enum EventType
    {
        
    }

    public enum EventToWhom
    {
        ToView,
        ToModel,
        ToBoth
    }
    public class EventManager
    {
        public void onRegister()
        {
            
        }
        public void InvokeEvent(EventType eventType)
        {
            
        }

        public void registerEvent()
        {
            
        }
    }

/*
 * 使用EventManager的一个例子：
 * 当玩家撞到墙壁，玩家行为类持有collider并在collider碰撞时调用：
 * miniEvent.EventManager.registerEvent(miniEvent.EventType.PlayerCollideIntoWallEvent, )
 */
}
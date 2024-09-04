using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MusicSelectHander : MonoBehaviour
{
    public Dropdown dropdown;

    void Start()
    {
        // 添加监听器，以便在选项改变时调用 `OnDropdownValueChanged` 方法
        if (dropdown != null)
        {
            dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
    }

    void OnDropdownValueChanged(int value)
    {
        EventManager.InvokeEvent(EventType.SelectMusicEvent, new SelectMusicEventData(value));
        EventSystem.current.SetSelectedGameObject(null);
    }

    void OnDestroy()
    {
        // 移除监听器（防止内存泄漏）
        if (dropdown != null)
        {
            dropdown.onValueChanged.RemoveListener(OnDropdownValueChanged);
        }
    }
}

public class SelectMusicEventData: EventData
{
    public int index;
    public SelectMusicEventData(int index)
    {
        this.index = index;
    }
}

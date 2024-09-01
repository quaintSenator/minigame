using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int index;
    [SerializeField] private Image image;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color hoverColor;

    [SerializeField] private GameObject empty;
    [SerializeField] private GameObject saveItem;
    [SerializeField] private Text saveItemMusicName;
    [SerializeField] private Text saveItemTime;
    
    [SerializeField] private SaveAndLoadPage saveAndLoadPage;
    
    private MapData mapData;
    
    public void SetMapData(MapData mapData)
    {
        this.mapData = mapData;
        UpdateItem();
    }

    public void UpdateItem()
    {
        if (mapData.key == "")
        {
            empty.SetActive(true);
            saveItem.SetActive(false);
        }
        else
        {
            empty.SetActive(false);
            saveItemMusicName.text = mapData.musicName;
            saveItemTime.text = mapData.key;
            saveItem.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("OnPointerClick : " + index);
        // 如果是左键点击
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            saveAndLoadPage.LeftClickSaveItem(index);
        }
        else if (eventData.button == PointerEventData.InputButton.Right && mapData.key != "")
        {
            saveAndLoadPage.RightClickSaveItem(index);
        }
    }
}

public class SaveMapEventData : EventData
{
    public int index = 0;
    public SaveMapEventData(int index)
    {
        this.index = index;
    }
}

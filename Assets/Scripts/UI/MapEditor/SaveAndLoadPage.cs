using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadPage : MonoBehaviour
{
    [SerializeField] private Button bgMask;
    [SerializeField] private List<SaveItem> saveItems;
    
    private static List<MapData> mapDatas = new List<MapData>();
    
    private void Awake()
    {
        bgMask.onClick.AddListener(OnBgMaskClick);
        string data = PlayerPrefs.GetString(GameConsts.UGC_SAVE_DATA);
        if (data != "")
        {
            mapDatas = JsonUtility.FromJson<SerializeBridge<MapData>>(data).list;
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                mapDatas.Add(new MapData());
            }
        }

        for(int i = 0; i < saveItems.Count; i++)
        {
            saveItems[i].SetMapData(mapDatas[i]);
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener(EventType.UGCSaveMapDataEvent, OnSaveMapEvent);
    }
    
    private void OnDisable()
    {
        EventManager.RemoveListener(EventType.UGCSaveMapDataEvent, OnSaveMapEvent);
    }

    private void OnSaveMapEvent(EventData obj)
    {
        Debug.Log("OnSaveMapEvent");
        var data = obj as SaveMapEventData;
        saveData(BuildableCreator.Instance.GetCurrentMapData(), data.index);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBgMaskClick();
        }
    }

    private void OnBgMaskClick()
    {
        gameObject.SetActive(false);
        MapEditorSaveAndLoadUI.InSaveAndLoadUI = false;
    }
    
    public void saveData(MapData mapData, int index)
    {
        mapDatas[index] = mapData;
        PlayerPrefs.SetString(GameConsts.UGC_SAVE_DATA, JsonUtility.ToJson(new SerializeBridge<MapData>(mapDatas)));
        saveItems[index].SetMapData(mapData);
    }
}

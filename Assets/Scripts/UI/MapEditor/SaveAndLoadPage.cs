using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveAndLoadPage : SaveUIBase
{
    [SerializeField] private Button bgMask;
    [SerializeField] private List<SaveItem> saveItems;
    [SerializeField] private SavePopUp savePopUp;
    
    public List<MapData> mapDatas = new List<MapData>();
    
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

    public override void OnBgMaskClick()
    {
        gameObject.SetActive(false);
        MapEditorSaveAndLoadUI.InSaveAndLoadUI = false;
        MapEditorSaveAndLoadUI.CurrentPages.Remove(this);
    }
    
    public void saveData(MapData mapData, int index)
    {
        mapDatas[index] = mapData;
        PlayerPrefs.SetString(GameConsts.UGC_SAVE_DATA, JsonUtility.ToJson(new SerializeBridge<MapData>(mapDatas)));
        saveItems[index].SetMapData(mapData);
    }

    public void RightClickSaveItem(int index)
    {
        Action confirmFun = () =>
        {
            mapDatas[index] = new MapData();
            PlayerPrefs.SetString(GameConsts.UGC_SAVE_DATA, JsonUtility.ToJson(new SerializeBridge<MapData>(mapDatas)));
            saveItems[index].SetMapData(new MapData());
            Debug.Log("DeleteData : " + index);
        };
        
        savePopUp.OpenPopUp("Delete", confirmFun);
    }
    
    public void LeftClickSaveItem(int index)
    {
        if(mapDatas[index].key == "")
        {
            saveData(BuildableCreator.Instance.GetCurrentMapData(), index);
        }
        else
        {
            Action confirmFun = () =>
            {
                saveData(BuildableCreator.Instance.GetCurrentMapData(), index);
            };
            Action cancelFun = () =>
            {
                Debug.Log("LoadData : " + index);
                PlayerPrefs.SetString(GameConsts.UGC_SELECTED_MAPDATA, JsonUtility.ToJson(mapDatas[index]));
                Debug.Log("LoadData : " + PlayerPrefs.GetString(GameConsts.UGC_SELECTED_MAPDATA));
                SceneManager.LoadScene("LevelForMapEditor");
            };
        
            savePopUp.OpenPopUp("Cover Or Load?", confirmFun, cancelFun, "Cover", "Load");
        }
    }
}

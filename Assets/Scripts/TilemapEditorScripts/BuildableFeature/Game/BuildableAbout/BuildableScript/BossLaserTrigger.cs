using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class BossLaserTrigger : BuildableBase
{
    [SerializeField] private float holdTime = 0.3f;
    [SerializeField] private float continueTime = 0.5f;
    [SerializeField] private Transform previewObj;
    [SerializeField] private Transform triggerObj;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            previewObj.localPosition = new Vector3((-0.5f + continueTime * GameConsts.SPEED)/2, 0, 0);
            previewObj.localScale = new Vector3(continueTime * GameConsts.SPEED, 0.5f, 0);
            previewObj.gameObject.SetActive(true);
            triggerObj.gameObject.SetActive(true);
        }
        else
        {
            previewObj.gameObject.SetActive(false);
            triggerObj.gameObject.SetActive(false);
        }
    }
    protected override void TriggerThisBuildable(PlayerController player)
    {
        EventManager.InvokeEvent(EventType.ReleaseLaserEvent, new ReleaseLaserEventData(transform.position, holdTime, continueTime));
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            previewObj.localPosition = new Vector3((continueTime/2 + holdTime) * GameConsts.SPEED - 0.5f, 0, 0);
            previewObj.localScale = new Vector3(continueTime * GameConsts.SPEED, 0.5f, 0);
            previewObj.gameObject.SetActive(true);
        }
    }

    [Button]
    private void SaveTimeToPrefab()
    {
#if UNITY_EDITOR
        var soList = Resources.Load<BuildableList>("AllBuildableList");
        GameObject prefab = soList.GetPrefab(Type);

        if (prefab != null)
        {
            // 将当前游戏对象的修改应用到 Prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, AssetDatabase.GetAssetPath(prefab), InteractionMode.UserAction);
            PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.UserAction);
            Debug.Log("Prefab changes saved.");
        }
        else
        {
            Debug.LogWarning("This object is not a prefab instance.");
        }
#endif
    }
}

public class ReleaseLaserEventData : EventData
{
    public Vector3 position;
    public float holdTime;
    public float continueTime;
    public ReleaseLaserEventData(Vector3 position, float holdTime ,float continueTime)
    {
        this.position = position;
        this.continueTime = continueTime;
        this.holdTime = holdTime;
    }
}

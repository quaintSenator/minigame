using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class BossBulletTrigger : BuildableBase
{
    [FormerlySerializedAs("continueTime")]
    [InlineButton("SaveTimeToPrefab", "保存至预制体")]
    [SerializeField] private float meetTime = 0.5f;
    [SerializeField] private Transform previewObj;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            previewObj.localPosition = new Vector3((-0.5f + meetTime * GameConsts.SPEED)/2, 0, 0);
            previewObj.localScale = new Vector3(meetTime * GameConsts.SPEED, 0.5f, 0);
            previewObj.gameObject.SetActive(true);
        }
        else
        {
            previewObj.gameObject.SetActive(false);
        }
    }
    protected override void TriggerThisBuildable(PlayerController player)
    {
        EventManager.InvokeEvent(EventType.ReleaseBulletEvent, new ReleaseBulletEventData(transform.position, meetTime));
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            previewObj.localPosition = new Vector3((-0.5f + meetTime * GameConsts.SPEED)/2, 0, 0);
            previewObj.localScale = new Vector3(meetTime * GameConsts.SPEED, 0.5f, 0);
            previewObj.gameObject.SetActive(true);
        }
    }

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

public class ReleaseBulletEventData : EventData
{
    public Vector3 position;
    public float meetTime;
    public ReleaseBulletEventData(Vector3 position, float meetTime)
    {
        this.position = position;
        this.meetTime = meetTime;
    }
}

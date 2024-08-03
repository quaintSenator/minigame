using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmZoneVisual : MonoBehaviour
{
    private List<GameObject> visualList = new List<GameObject>();
    private float speed;
    
    public void Init(List<TimeVisualData> timeVisualDataList)
    {
        speed = GameConsts.SPEED;
        foreach (var visualDate in timeVisualDataList)
        {
            AddVisual(visualDate);
        }
    }
    
    public void UpdateVisual(List<TimeVisualData> timeVisualDataList)
    {
        for (int i = 0; i < timeVisualDataList.Count; i++)
        {
            if (i >= visualList.Count)
            {
                AddVisual(timeVisualDataList[i]);
                continue;
            }
            visualList[i].transform.localPosition = new Vector3((timeVisualDataList[i].startTime + timeVisualDataList[i].endTime) * speed / 2, 0, 0);
            visualList[i].transform.localScale = new Vector3(Mathf.Abs(timeVisualDataList[i].startTime) + Mathf.Abs(timeVisualDataList[i].endTime), 6, 1);
            visualList[i].GetComponent<SpriteRenderer>().color = timeVisualDataList[i].color;
        }
    }
    
    public void AddVisual(TimeVisualData visualDate)
    {
        GameObject go = new GameObject();
        go.transform.parent = transform;
        go.name = visualDate.name;
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = RhythmViewer.GetSprite();
        sr.color = visualDate.color;
        sr.sortingOrder = -2;
        go.transform.localPosition = new Vector3((visualDate.startTime + visualDate.endTime) * speed / 2, visualDate.height / 2, 0);
        go.transform.localScale = new Vector3(Mathf.Abs(visualDate.endTime - visualDate.startTime) * speed, visualDate.height, 1f);
        visualList.Add(go);
    }
}

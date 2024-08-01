using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum RhythmZoneType
{
    Perfect = 0,
}

public class RhythmViewer : MonoBehaviour
{
    [SerializeField] private RhythmDataFile rhythmDataFile;
    private List<Color> colorList = new List<Color>
    {
        new Color(1.0f, 0.0f, 0.0f, 0.3f),
        new Color(0.0f, 1.0f, 0.0f, 0.3f),
        new Color(0.0f, 0.0f, 1.0f, 0.3f),
        new Color(1.0f, 1.0f, 0.0f, 0.3f),
    };
    private Texture2D squareTexture;
    
    private void Start()
    {
        // 创建一个1x1纹理
        squareTexture = new Texture2D(1, 1);
        squareTexture.SetPixel(0, 0, Color.white); // 设置纹理的颜色
        squareTexture.Apply(); // 应用纹理的改变
        
        if (rhythmDataFile)
        {
            Debug.Log("RhythmDataFile is not null");
            foreach (var rhythmData in rhythmDataFile.rhythmDataList)
            {
                RhythmZoneType type = RhythmZoneType.Perfect;
                float startPos = rhythmData.perfectTime - 0.1f;
                float endPos = rhythmData.perfectTime + 0.1f;
                SpawnRhythmZoneVisual(startPos, endPos, type);
            }
        }
    }
    
    private void SpawnRhythmZoneVisual(float startPos, float endPos, RhythmZoneType type)
    {
        Debug.Log("SpawnRhythmZoneVisual : " + startPos + " " + endPos + " " + type);
        GameObject rhythmZoneVisual = new GameObject("RhythmZoneVisual");
        rhythmZoneVisual.transform.parent = transform;
        SpriteRenderer renderer = rhythmZoneVisual.AddComponent<SpriteRenderer>();
        //spite 设置为 2D Sprite中的Square
        renderer.sprite = Sprite.Create(squareTexture, new Rect(0.0f, 0.0f, 1.0f, 1.0f), new Vector2(0.5f, 0.5f), 1.0f);;
        renderer.color = colorList[(int)type];
        renderer.sortingOrder = -1;
        rhythmZoneVisual.transform.position = new Vector3((startPos + endPos) / 2 * 5.0f, 0.25f, 0);
        rhythmZoneVisual.transform.localScale = new Vector3(endPos - startPos, 6, 0);    
    }
}

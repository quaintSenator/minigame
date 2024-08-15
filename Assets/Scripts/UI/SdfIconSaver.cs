using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
using UnityEditor;
using UnityEngine;

public class SdfIconSaver : MonoBehaviour
{
#if UNITY_EDITOR
    public SdfIconType iconType;
#endif
    public Vector2Int size = new Vector2Int(120, 120);
    public Color color = Color.white;
    public string saveName = "icon";
    
    [Button]
    public void SavePng()
    {
#if UNITY_EDITOR
        string path = Path.Combine(Application.dataPath, "UI/SdfIcons");
        Texture2D _vloume;
        _vloume = SdfIcons.CreateTransparentIconTexture(iconType, color, size.x, size.y, 0);
        byte[] playIconData = _vloume.EncodeToPNG();
        File.WriteAllBytes(Path.Combine(path, saveName+".png"), playIconData);
        Debug.Log("SavePng: " + Path.Combine(path, saveName+".png"));
        AssetDatabase.Refresh();
#endif
    }
}

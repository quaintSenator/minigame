using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapEditorButton : MonoBehaviour
{
    //for test
    public void MapEditorButtonWasClicked()
    {
        SceneManager.LoadScene("TilemapEditorScene");
    }
}

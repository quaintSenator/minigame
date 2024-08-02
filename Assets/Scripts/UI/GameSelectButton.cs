using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSelectButton : MonoBehaviour
{
    public void OnClickGameSelect()
    {
        var btn2Create = Resources.Load("testPrefabButton") as GameObject;
        Instantiate(btn2Create);
    }
}

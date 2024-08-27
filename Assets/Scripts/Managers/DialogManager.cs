using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : Singleton<DialogManager>
{
    public List<List<string>> levelDialogs1 = new List<List<string>>{
        new List<string>{},
        new List<string>{},
        new List<string>{}
    };

    public Dictionary<int, List<List<string>>> dialogs = new Dictionary<int, List<List<string>>>();

}
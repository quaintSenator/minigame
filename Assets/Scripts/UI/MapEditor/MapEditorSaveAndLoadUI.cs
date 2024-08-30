using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorSaveAndLoadUI : MonoBehaviour
{
    [SerializeField] private Button saveAndLoadButton;
    [SerializeField] private GameObject saveAndLoadPage;


    public static bool InSaveAndLoadUI = false;
    private void Awake()
    {
        saveAndLoadButton.onClick.AddListener(OnSaveAndLoadButtonClick);
    }

    private void OnSaveAndLoadButtonClick()
    {
        saveAndLoadPage.SetActive(true);
        InSaveAndLoadUI = true;
    }
}

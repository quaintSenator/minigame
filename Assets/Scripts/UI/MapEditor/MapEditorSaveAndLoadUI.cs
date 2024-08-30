using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorSaveAndLoadUI : MonoBehaviour
{
    [SerializeField] private Button saveAndLoadButton;
    [SerializeField] private GameObject saveAndLoadPage;

    private void Awake()
    {
        saveAndLoadButton.onClick.AddListener(OnSaveAndLoadButtonClick);
        saveAndLoadPage.SetActive(false);
    }

    private void OnSaveAndLoadButtonClick()
    {
        saveAndLoadPage.SetActive(true);
    }
}

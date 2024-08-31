using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorSaveAndLoadUI : MonoBehaviour
{
    [SerializeField] private Button saveAndLoadButton;
    [SerializeField] private SaveUIBase saveAndLoadPage;


    public static List<SaveUIBase> CurrentPages = new List<SaveUIBase>();
    public static bool InSaveAndLoadUI = false;
    private void Awake()
    {
        saveAndLoadButton.onClick.AddListener(OnSaveAndLoadButtonClick);
    }

    private void Update()
    {
        if (InSaveAndLoadUI && Input.GetKeyDown(KeyCode.Escape))
        {
            SaveUIBase currentPage = CurrentPages[CurrentPages.Count - 1];
            currentPage.OnBgMaskClick();
        }
    }

    private void OnSaveAndLoadButtonClick()
    {
        saveAndLoadPage.gameObject.SetActive(true);
        CurrentPages.Add(saveAndLoadPage);
        InSaveAndLoadUI = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SavePopUp : SaveUIBase
{
    [SerializeField] private Text desc;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Text confirmButtonText;
    [SerializeField] private Text cancelButtonText;
    [SerializeField] private Button bgMask;

    private void Awake()
    {
        bgMask.onClick.AddListener(OnBgMaskClick);
    }

    public void OpenPopUp(string desc, System.Action confirmAction, System.Action cancelAction = null, string confirmText = "Confirm", string cancelText = "Cancel")
    {
        this.desc.text = desc;
        confirmButtonText.text = confirmText;
        cancelButtonText.text = cancelText;
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            confirmAction?.Invoke();
            OnBgMaskClick();
        });
        cancelButton.onClick.AddListener(() =>
        {
            cancelAction?.Invoke();
            OnBgMaskClick();
        });
        gameObject.SetActive(true);
        MapEditorSaveAndLoadUI.CurrentPages.Add(this);
    }

    public override void OnBgMaskClick()
    {
        gameObject.SetActive(false);
        MapEditorSaveAndLoadUI.CurrentPages.Remove(this);
    }
}

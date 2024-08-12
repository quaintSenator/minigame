using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildableSelectButton : MonoBehaviour
{
    [SerializeField] protected BuildableType buildableType;
    protected Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    protected virtual void OnClick()
    {
        Debug.Log("Click " + buildableType);
        BuildableCreator.Instance.SetSelectedObject(buildableType);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildableObjectButton : MonoBehaviour
{
    [SerializeField] BuildableType buildableType;
    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Debug.Log("Click " + buildableType);
        BuildableCreator.Instance.SetSelectedObject(buildableType);
    }
}

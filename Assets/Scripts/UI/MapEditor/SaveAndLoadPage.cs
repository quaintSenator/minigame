using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoadPage : MonoBehaviour
{
    [SerializeField] private Button bgMask;
    
    private void Awake()
    {
        bgMask.onClick.AddListener(OnBgMaskClick);
    }

    private void OnBgMaskClick()
    {
        gameObject.SetActive(false);
    }
}

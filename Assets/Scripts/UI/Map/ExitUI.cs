using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ExitUI : MonoBehaviour
{
    [SerializeField] private GameObject exitButton;

    private void Awake()
    {
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitButtonClick);
    }

    private void OnExitButtonClick()
    {
        Utils.AddMaskAndLoadScene(transform, "GUIScene");
    }
}

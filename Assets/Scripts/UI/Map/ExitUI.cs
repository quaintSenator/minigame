using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class ExitUI : MonoBehaviour
{
    [SerializeField] private GameObject exitButton;
    [SerializeField] private Image exitPage;

    private void Awake()
    {
        exitButton.GetComponent<Button>().onClick.AddListener(OnExitButtonClick);
    }

    private void OnExitButtonClick()
    {
        exitPage.gameObject.SetActive(true);
        exitPage.DOColor(new Color(0, 0, 0, 1f), 0.5f).onComplete = () =>
        {
            SceneManager.LoadScene("GUIScene");
        };
        
    }
}

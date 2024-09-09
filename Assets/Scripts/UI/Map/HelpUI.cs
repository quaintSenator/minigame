using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour
{
    [SerializeField] private GameObject helpPage;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button bgMask;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private List<Sprite> tutorialSprites;
    public int currentTutorialIndex = 0;
    
    public static bool InHelpUI = false;

    private void Awake()
    {
        helpButton.GetComponent<Button>().onClick.AddListener(OnHelpButtonClick);
        leftButton.GetComponent<Button>().onClick.AddListener(OnLeftButtonClick);
        rightButton.GetComponent<Button>().onClick.AddListener(OnRightButtonClick);
        bgMask.GetComponent<Button>().onClick.AddListener(OnBgMaskClick);
        
        tutorialImage.sprite = tutorialSprites[currentTutorialIndex];
        InHelpUI = false;
    }

    private void OnBgMaskClick()
    {
        gameObject.SetActive(true);
        helpPage.SetActive(false);
        InHelpUI = false;
    }

    private void OnRightButtonClick()
    {
        currentTutorialIndex++;
        if (currentTutorialIndex >= tutorialSprites.Count)
        {
            currentTutorialIndex = 0;
        }
        tutorialImage.sprite = tutorialSprites[currentTutorialIndex];
    }

    private void OnLeftButtonClick()
    {
        currentTutorialIndex--;
        if (currentTutorialIndex < 0)
        {
            currentTutorialIndex = tutorialSprites.Count - 1;
        }
        tutorialImage.sprite = tutorialSprites[currentTutorialIndex];
    }

    private void OnHelpButtonClick()
    {
        InHelpUI = true;
        helpPage.SetActive(true);
    }
}

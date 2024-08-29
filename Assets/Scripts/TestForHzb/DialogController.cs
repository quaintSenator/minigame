using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    private Text dialogText;

    [SerializeField]
    private float duringTime = 2f;
    [SerializeField]
    private float fadeTime = 1f;
    [SerializeField]
    private float waitTime = 0.7f;

    private float fadeTimer = 0f;
    private bool fading = false;
    
    private float deltaAlpha;

    private int dialogIndex = 0;
    private List<string> nowDialogList;

    private void Awake()
    {
        dialogText = GetComponent<Text>();
    }

    private void Update()
    {
        if(fading){
            fadeTimer += Time.deltaTime;

            Color dialogColor = dialogText.color;
            dialogColor.a -= deltaAlpha * Time.deltaTime;
            dialogText.color = dialogColor;
            
        }
        if(fadeTimer > fadeTime){
            fading = false;
            fadeTimer = 0;
            CleverTimerManager.Ask4Timer(waitTime, ShowOneDialog);
        }
    }

    public void ShowDialogs(List<string> dialogList)
    {
        dialogIndex = 0;
        nowDialogList = dialogList;

        ShowOneDialog();
    }

    private void ShowOneDialog(EventData data = null)
    {
        if(dialogIndex >= nowDialogList.Count)
            return;
            
        string dialog = GetDialog();
        dialogIndex++;
        ShowDialog(dialog);

        CleverTimerManager.Ask4Timer(duringTime, StartFade);
    }

    private void ShowDialog(string dialog)
    {
        Color dialogColor = dialogText.color;
        dialogColor.a = 255f;
        dialogText.color = dialogColor;
        dialogText.text = dialog;
    }

    private string GetDialog()
    {
        return nowDialogList[dialogIndex];
    }

    private void StartFade(EventData data = null)
    {
        fading = true;
        deltaAlpha = dialogText.color.a / fadeTime;
    }

}
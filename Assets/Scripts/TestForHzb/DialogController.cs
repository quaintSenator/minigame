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

    private void Awake()
    {
        dialogText = GetComponent<Text>();
    }

   /* private void Update()
    {
        if(fading){
            fadeTimer += Time.deltaTime;
        }
        if(fadeTimer > fadeTime){
            fading = false;
            fadeTimer = 0;
            CleverTimerManager.Ask4Timer(waitTime, ShowOneDialog);
        }
    }*/

    public void ShowDialog(string dialog)
    {
        dialogText.text = dialog;
        //CleverTimerManager.Ask4Timer(duringTime, StartFade);
    }

    private void StartFade(EventData data = null)
    {
        fading = true;
    }

}
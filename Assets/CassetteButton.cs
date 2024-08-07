using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CassetteButton : MonoBehaviour
{
    public void OnCassetteClick()
    {
        Debug.Log("OnCassetteClick");
        switch (gameObject.name)
        {
            case "cassette1":
                break;
            case "cassette2":
                break;
            case "cassette3":
                break;
        }

        EnterLevel(1);
    }

    public void EnterLevel(int i)
    {
        var sceneName = "NewPlayerScene";
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}

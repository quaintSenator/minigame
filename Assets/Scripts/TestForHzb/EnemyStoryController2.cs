using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStoryController2 : MonoBehaviour
{

    public RectTransform textRect;
    public RectTransform bubbleRect;
    public Transform player;

    [SerializeField]
    private float xOffset = 0;
    [SerializeField]
    private float yOffset = 0;

    [SerializeField]
    private float dialogDuringTime = 0;

    private float speed = 0;
    private int dialogIndex = 0;

    private Transform bubble;
    private Text dialog;
    private Text bgDialog;

    private string[] dialogs = {
        "……",
        "甩掉它了……",
        "真的甩掉了吗？",
        "谁在那里？",
        "我是球。",
        "等你穿过僧侣最北方的宫殿，变得更聪明就知道了。",
        "我为什么要去见你？",
        "你不是一直在好奇第三个维度吗？",
        "北方的路面有许多断层，先学会向北飞跃吧。"
    };

    private void Awake()
    {
        bubble = transform.Find("Canvas").Find("bubble");
        dialog = bubble.Find("Text").GetComponent<Text>();
        bgDialog = transform.Find("Canvas").Find("bg_dialog").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //ResetTextPos();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private void ResetTextPos()
    {
        bubble.gameObject.SetActive(true);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(player.position);
        //Debug.Log("screenPos"+screenPos);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(bubbleRect.parent.GetComponent<RectTransform>(), screenPos, null, out Vector2 localPos);
        bubbleRect.anchoredPosition = new Vector2(screenPos.x + xOffset, screenPos.y + yOffset);
    }

    private string GetDialog()
    {
        if(dialogIndex < 0){
            return "Error";
        }
        return dialogs[dialogIndex++];
    }

    public void StartSpeak()
    {
        ResetTextPos();
        dialog.text = GetDialog();
        CleverTimerManager.Ask4Timer(dialogDuringTime, EndSpeak);
    }

    public void StartBgSpeak()
    {
        bgDialog.text = GetDialog();
    }

    private void EndSpeak(EventData data = null)
    {
        bubble.gameObject.SetActive(false);
    }

    public void EndBgSpeak()
    {
        bgDialog.text = "";
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

}

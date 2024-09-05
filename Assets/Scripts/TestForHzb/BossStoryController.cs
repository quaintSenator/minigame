using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossStoryController : MonoBehaviour
{

    public RectTransform textRect;
    public RectTransform bubbleRect;
    public Transform boss;
    public Transform player;

    private float xOffset;
    private float yOffset;

    [SerializeField]
    private float playerXOffset = 0;
    [SerializeField]
    private float playerYOffset = 0;    
    [SerializeField]
    private float bossXOffset = 0;
    [SerializeField]
    private float bossYOffset = 0;


    [SerializeField]
    private float dialogDuringTime = 0;

    private float speed = 0;
    private int playerDialogIndex = 0;
    private int bossDialogIndex = 0;

    public Transform canvasTrans;
    private Transform bubble;
    private Text dialog;
    private Text bgDialog;

    private string[] playerDialogs = {
        "球先生！这就是最北方吗？为什么这么黑呢？",
        "僧侣大人……？",
        "但我不想要宫殿，我想见见第三个维度。",
        "书中记载，第三维度在最北方。球先生也请我到这里找他。",
        "僧侣大人，对于你而言，说谎可能是轻而易举的事。",
        "可我就是相信着球先生和三次方的存在。",
        "如果你坚信它们都是假的，就请连同我——",
        "一起毁灭掉吧！",
    };

    private string[] bossDialogs = {
        "很少有人能抵达这里。",
        "我将赐你贵族的身份，成为贵族，与我分享宫殿。",
        "世界上不存在第三个维度。",
        "哼，那个自称“球”的小人？",
        "它在雾里隐藏自己，能够变得忽大忽小。",
        "但它是假的，孩子。",
        "没有第三个维度，也没有“球”。",
        "这里是最北方，平面国最高贵的地方。",
        "你已经进化成了高贵的圆，为什么不停下来与我分享一切呢？",
    };

    private void Awake()
    {
        bubble = canvasTrans.Find("StoryCanvas").Find("bubble");
        dialog = bubble.Find("Text").GetComponent<Text>();
        bgDialog = canvasTrans.Find("StoryCanvas").Find("bg_dialog").GetComponent<Text>();
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

    private void ResetTextPos(int index)
    {
        Transform trans;
        if(index == 1)
        {
            trans = player;
            xOffset = playerXOffset;
            yOffset = playerYOffset;
        }
        else
        {
            trans = boss;
            xOffset = bossXOffset;
            yOffset = bossYOffset;
        }
        bubble.gameObject.SetActive(true);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(trans.position);
        //Debug.Log("screenPos"+screenPos);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(bubbleRect.parent.GetComponent<RectTransform>(), screenPos, null, out Vector2 localPos);
        bubbleRect.anchoredPosition = new Vector2(screenPos.x + xOffset, screenPos.y + yOffset);
    }

    private string GetDialog(int index = 1)
    {
        if(index == 1)  //player
        {
            return playerDialogs[playerDialogIndex++];
        }
        else
        {
            return bossDialogs[bossDialogIndex++];
        }
    }

    public void StartSpeak(int index)
    {
        ResetTextPos(index);
        dialog.text = GetDialog(index);
        CleverTimerManager.Ask4Timer(dialogDuringTime, EndSpeak);
    }

    private void EndSpeak(EventData data = null)
    {
        bubble.gameObject.SetActive(false);
    }

    public void StartBgSpeak()
    {
        bgDialog.text = GetDialog(2);
    }

    public void EndBgSpeak()
    {
        bgDialog.text = "";
    }


}

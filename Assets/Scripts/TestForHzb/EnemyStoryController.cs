using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStoryController : MonoBehaviour
{

    public RectTransform textRect;
    public RectTransform bubbleRect;

    [SerializeField]
    private float xOffset = 0;
    [SerializeField]
    private float yOffset = 0;

    private float speed = 6.0f;
    private int dialogIndex = 0;

    private Transform bubble;
    private Text dialog;

    private string[] dialogs = {
        "站住！",
        "你竟然在读三次方的教义！！！！",
        "这可是僧侣大人认定的禁书！",
        "别想跑！把书交出来！"
    };

    private void Awake()
    {
        bubble = transform.Find("Canvas").Find("sprite");
        dialog = bubble.Find("Text").GetComponent<Text>();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetTextPos();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime, Space.World);
    }

    private void ResetTextPos()
    {
        bubble.gameObject.SetActive(true);
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.Find("Sprite").position);
        //Debug.Log("screenPos"+screenPos);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(bubbleRect.parent.GetComponent<RectTransform>(), screenPos, null, out Vector2 localPos);
        bubbleRect.anchoredPosition = new Vector2(screenPos.x + xOffset, screenPos.y + yOffset);
    }

    private string GetDialog()
    {
        if(dialogIndex < 0 || dialogIndex > 3){
            return "Error";
        }
        return dialogs[dialogIndex++];
    }

    private void Speak()
    {
        ResetTextPos();
        dialog.text = GetDialog();
    }

    private void EndSpeak()
    {
        bubble.gameObject.SetActive(false);
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void Step_1()
    {
        
    }
}

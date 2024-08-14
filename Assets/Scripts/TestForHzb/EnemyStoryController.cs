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

    private string[] dialogs = {
        "站住！",
        "你竟然在读三次方的教义！！！！",
        "这可是僧侣大人认定的禁书！",
        "别想跑！把书交出来！"
    };

    // Start is called before the first frame update
    void Start()
    {
        ResetTextPos();
    }

    // Update is called once per frame
    void Update()
    {
        //ResetTextPos();
    }

    private void ResetTextPos()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.Find("Sprite").position);
        //Debug.Log("screenPos"+screenPos);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(bubbleRect.parent.GetComponent<RectTransform>(), screenPos, null, out Vector2 localPos);
        bubbleRect.anchoredPosition = new Vector2(screenPos.x + xOffset, screenPos.y + yOffset);
    }

    private void Step_1()
    {
        
    }
}

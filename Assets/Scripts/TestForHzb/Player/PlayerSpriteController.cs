using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpressionType
{
    IDLE = 0,
    CLOSE = 1,
    AFRAID = 2,
    ANGRY = 3,
}

public class ExpressionTypeData : EventData
{
    public ExpressionType type;
    public int priority;
    /* 
    眨眼切换：5
    普通起跳：10
    战斗：15
    弹簧起跳：20
    落地切换：99
    */
    public ExpressionTypeData(ExpressionType type, int priority)
    {
        this.type = type;
        this.priority = priority;
    }
}

public class PlayerSpriteController : MonoBehaviour
{

    public List<Sprite> sprites;    //方便定位用的，顺序定死

    private SpriteRenderer spriteRenderer;
    private Transform visual;

    private int selfPriority = 0;

    private float CLEAR_TIME = 0.2f;
    private float clearTimer = 0;
    private float ATTACK_DURING_TIME = 0.3f;
    private bool startclear = false;

    private void Awake()
    {
        spriteRenderer = transform.GetComponent<SpriteRenderer>();
        visual = transform.parent;
    }

    private void Update()
    {
        if(startclear)
        {
            clearTimer += Time.deltaTime;
        }
        if(clearTimer >= CLEAR_TIME)
        {
            clearTimer = 0;
            selfPriority = 0;
            startclear = false;
        }
    }

    public void SetSprite(EventData data = null)
    {
        if(data == null)
        {
            //spriteRenderer.sprite = sprites[(int)ExpressionType.IDLE];
            return;
        }
        var typeData = data as ExpressionTypeData;
        if(typeData.priority >= selfPriority)
        {
            spriteRenderer.sprite = sprites[(int)typeData.type];
            startclear = true;
            clearTimer = 0;
        }
    }

    public void SetCorrect()
    {
        visual.rotation = Quaternion.Euler(0, 0, 0);
        SetSprite(new ExpressionTypeData(ExpressionType.IDLE, 5));
    }

    public void SetAttackSprite()
    {
        SetSprite(new ExpressionTypeData(ExpressionType.ANGRY, 15));
        CleverTimerManager.Ask4Timer(ATTACK_DURING_TIME, SetSprite, new ExpressionTypeData(ExpressionType.IDLE, 15));
    }

}

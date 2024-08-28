using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPage : MonoBehaviour
{
    public Text m_text;
    private int m_id;

    void Start()
    {
        if (!m_text)
        {
            foreach (Transform child in transform)
            {
                if (child.name.Contains("tip_content"))
                {
                    m_text = child.gameObject.GetComponent<Text>();
                }
            }
        }

        m_id = -1;
    }
    public void SetText(string txt)
    {
        m_text.text = txt;
        ResizePageNText(txt.Length);
    }

    public void ResizePageNText(int len)
    {
        RectTransform pageRect = gameObject.GetComponent<RectTransform>();
        RectTransform textRect = m_text.gameObject.GetComponent<RectTransform>();
        pageRect.sizeDelta = new Vector2(60 * len, 90);
        textRect.sizeDelta = new Vector2(60 * len, 90);
    }

    public void SetID(int i)
    {
        m_id = i;
    }

    public int GetID()
    {
        return m_id;
    }
}

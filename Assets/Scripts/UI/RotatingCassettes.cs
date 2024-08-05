using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RotatingCassettes : MonoBehaviour
{
    private Transform c1;
    private Transform c2;
    private Transform c3;
    public Animator m_Animator;
    private int currentMiddleCassette;
    [SerializeField] private Button _rightScrollBtn;
    [SerializeField] private Button _leftScrollBtn;
    void Start()
    {
        currentMiddleCassette = 1;
        foreach (Transform childTransform in transform)
        {
            switch (childTransform.name)
            {
                case "cassette1":
                    c1 = childTransform;
                    break;
                case "cassette2":
                    c2 = childTransform;
                    break;
                case "cassette3":
                    c3 = childTransform;
                    break;
                default: break;
            }
        }
        c3.gameObject.SetActive(false);
        c2.gameObject.SetActive(false);
        
        m_Animator = gameObject.GetComponent<Animator>();
        m_Animator.enabled = false;
        
        
    }

    public void OnRightScroll()
    {
        _rightScrollBtn.enabled = false;
        if (!m_Animator.enabled)
        {
            m_Animator.enabled = true;
        }
        string anim2Play = "cassetteRotateAnimation";
        switch (currentMiddleCassette)
        {
            case 1:
                anim2Play += "3_1";
                c1.gameObject.SetActive(true);
                c3.gameObject.SetActive(true);
                currentMiddleCassette = 3;
                break;
            case 2:
                anim2Play += "1_2";
                c1.gameObject.SetActive(true);
                c2.gameObject.SetActive(true);
                currentMiddleCassette = 1;
                break;
            case 3:
                anim2Play += "2_3";
                c2.gameObject.SetActive(true);
                c3.gameObject.SetActive(true);
                currentMiddleCassette = 2;
                break;
            default: break;
        }
        m_Animator.Play(anim2Play);
    }
    public void after1_2()
    {
        Debug.LogWarning("after1_2 was called");
        _rightScrollBtn.enabled = true;
        c2.gameObject.SetActive(false);
    }
    public void after2_3()
    {
        Debug.LogWarning("after2_3 was called");
        _rightScrollBtn.enabled = true;
        c3.gameObject.SetActive(false);
    }
    public void after3_1()
    {
        Debug.LogWarning("after3_1 was called");
        _rightScrollBtn.enabled = true;
        c1.gameObject.SetActive(false);
    }
}

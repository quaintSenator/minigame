using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    float triggerTime = -1;

    public float duration = 15;

    public float startRotate = -8;
    public float endRotate = 8;
    // Start is called before the first frame update
    void Start()
    {
        triggerTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        ShakeSelf();
    }

    private void ShakeSelf()
    {
        float elapsedTime = Time.time - triggerTime; // �Ѿ�������ʱ��
        float t = Mathf.Clamp01(elapsedTime / duration); // �����ֵ���ӣ�ȷ����0��1֮��

        Quaternion quaternionStart = Quaternion.Euler(0, 0, startRotate);
        Quaternion quaternionEnd = Quaternion.Euler(0, 0, endRotate);

        this.transform.localRotation= Quaternion.Lerp(quaternionStart, quaternionEnd, t);

    }
}

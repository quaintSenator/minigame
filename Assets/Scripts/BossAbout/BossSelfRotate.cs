using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSelfRotate : MonoBehaviour
{

    public bool ifRotateSelf = true;
    public float circleRotateSpeed = 60.0f;
    public float triangleRotateSpeed = 180.0f;

    public Renderer circleRenderder = null;
    public Renderer triangleRenderder = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ifRotateSelf)
        {
            SelfRotateSD();
        }


    }

    private void SelfRotateSD()
    {
        if(circleRenderder !=null)
        {
            circleRenderder.transform.Rotate(Vector3.forward, circleRotateSpeed * Time.deltaTime);
        }
        if(triangleRenderder)
        {
            triangleRenderder.transform.Rotate(Vector3.forward, triangleRotateSpeed * Time.deltaTime);
        }


    }
}

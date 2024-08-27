using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPoint : MonoBehaviour
{
    private Transform startPoint;
    
    private void Start()
    {
        startPoint = Utils.GetStartPointPostion();
    }
    
    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, startPoint.position.y, 0);  
    }
}

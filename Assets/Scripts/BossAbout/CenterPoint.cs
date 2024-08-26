using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPoint : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, 0, 0);  
    }
}

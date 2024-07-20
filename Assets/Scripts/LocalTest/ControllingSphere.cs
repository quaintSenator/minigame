using System.Collections;
using System.Collections.Generic;
using miniEvent;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControllingSphere : MonoBehaviour
{
    [SerializeField] private Transform sphereTransform;
    [SerializeField] private float moveSpeed;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A))
        {
            move(MoveDirection.Left);
        }

        if (Input.GetKey(KeyCode.D))
        {
            move(MoveDirection.Right);
        }
        
    }

    void move(MoveDirection direction)
    {
        Vector3 newPosition = sphereTransform.position;
        if (direction == MoveDirection.Left)
        {
            newPosition.x -= moveSpeed * Time.deltaTime;
        }
        else
        {
            newPosition.x += moveSpeed * Time.deltaTime;
        }

        sphereTransform.position = newPosition;
    }
}

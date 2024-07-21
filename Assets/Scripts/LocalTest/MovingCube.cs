using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovingCube : MonoBehaviour
{
    enum CubeMoveDir
    {
        Right,
        Left
    }
    [SerializeField]
    private Transform cubeTransform;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float armLength;
    private CubeMoveDir moveDir;
    
    void Start()
    {
        if (cubeTransform != null) 
        {
            cubeTransform.position = new Vector3(0, 0, 0);
        }
        moveDir = CubeMoveDir.Right;
    }
    void FixedUpdate()
    {
        float x = cubeTransform.position.x;
        if (moveDir == CubeMoveDir.Right)
        {
            if (x + moveSpeed * Time.deltaTime > armLength)
            {
                moveDir = CubeMoveDir.Left;
            }
        }
        else
        {
            if (x - moveSpeed * Time.deltaTime < -armLength)
            {
                moveDir = CubeMoveDir.Right;
            }
        }
        move(moveDir);
    }

    void move(CubeMoveDir d)
    {
        float mult = (d == CubeMoveDir.Right ? 1.0f : -1.0f);
        float x = cubeTransform.position.x;
        x = mult * moveSpeed * Time.deltaTime + x;
        cubeTransform.position = new Vector3(x, 0, 0);
    }
}
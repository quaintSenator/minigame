using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChangeEventData : EventData
{
    private Vector2 afterGravityDir;
}
public class ForceManager : Singleton<ForceManager>
{
    private Vector2 gravityDir;
    private Boolean isheadingRight = true;
    void Start()
    {
        gravityDir = Vector2.down;
        isheadingRight = true;
    }

    public Vector2 getGravityDir()
    {
        return gravityDir;
    }
    public void switchGravityDir()
    {
        gravityDir = -gravityDir;
    }

    public float getFrictionThrowingRotationAngle(Vector2 v, Vector2 g, float realAngle)
    {
        if (v.x > 0)
        {
            if (g.y < 0)
            {
                return -realAngle;
            }
            else
            {
                return realAngle;
            }
        }
        else
        {
            if (g.y > 0)
            {
                return 180.0f - realAngle;
            }
            else
            {
                return -(90.0f + realAngle);
            }
        }
        
    }
}

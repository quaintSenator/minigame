using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOrbit : MonoBehaviour{

    [SerializeField] private Transform player;

    private LineRenderer line;
    private List<Vector3> points;

    private void Awake()
    {
        line = gameObject.GetComponent<LineRenderer>();
        points = new List<Vector3>{};
    }

    private void Update()
    {
        transform.position = player.position;
        AddPoints();
    }

    private void AddPoints()
    {
        Vector3 transPos=transform.position;
        if(transPos!=new Vector3(0,0,0))
            points.Add(transPos);
        
        line.positionCount = points.Count;
        if(points.Count>0)
        {
            line.SetPosition(points.Count-1,lastPoint);
        }
    }

    private Vector3 lastPoint{
        get{
            if(points==null){
                return Vector3.zero;
            }
            return points[points.Count-1];
        }
    }

}
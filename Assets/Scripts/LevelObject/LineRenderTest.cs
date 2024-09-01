using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderTest : MonoBehaviour
{
    // Start is called before the first frame update

        //private LineRenderer lineRenderer;

/*        void Start()
        {*/

        public Vector2 tilingFactor = new Vector2(1f, 1f); // 初始平铺块数

        private LineRenderer lineRenderer;

        private float length;

        private int tileCount;

        // Start is called before the first frame update
        void Start()
        {
            //给LineRenderer两个点
            lineRenderer = GetComponent<LineRenderer>();

            //lineRenderer.positionCount = 2;
            //Vector3 pos2 = transform.position + Vector3.forward * 10;
            //Vector3[] tempPoints = new Vector3[2] { transform.position, pos2 };
            //lineRenderer.SetPositions(tempPoints);

            Vector3 start = lineRenderer.GetPosition(0);

            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                length += Vector3.Distance(start, lineRenderer.GetPosition(i));
                start = lineRenderer.GetPosition(i);
            }

            tileCount = Mathf.FloorToInt(length);

            lineRenderer.material.mainTextureScale = new Vector2(tileCount, 1) * tilingFactor;

        }



        // Update is called once per frame
        void Update()
        {

        }


/*    public float linewidth;
    public LineRenderer line;
    public Vector3[] poslist = {};
    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();

        poslist.SetLength(line.positionCount);
        line.GetPositions(poslist);
        //line.GetPositions
        this.GetComponent<LineRenderer>().startWidth = linewidth;
        this.GetComponent<LineRenderer>().endWidth = linewidth;
        Vector3[] vec = AddLineRenderPos(poslist).ToArray();
        for (int i = 0; i < vec.Length; i++)
        {
            Debug.Log(vec[i]);
        }
        line.positionCount = vec.Length;
        line.SetPositions(vec);
    }
    // Update is called once per frame
    void Update()
    {
    }
    public static List<Vector3> AddLineRenderPos(Vector3[] posList, int addNum = 10)
    {
        List<Vector3> newPosList = new List<Vector3>();
        addNum /= 2;
        newPosList.Add(posList[0]);
        //为除首尾点之外的拐点附近，动态增加点位。
        for (int i = 1; i < posList.Length - 1; i++)
        {
            Vector3 start = posList[i - 1];
            Vector3 mid = posList[i];
            Vector3 end = posList[i + 1];
            for (int j = addNum; j > 0; j--)
            {
                newPosList.Add(Vector3.Lerp(mid, start, j * 0.01f));
            }
            newPosList.Add(mid);
            for (int j = 1; j <= addNum; j++)
            {
                newPosList.Add(Vector3.Lerp(mid, end, j * 0.01f));
            }
        }
        newPosList.Add(posList[posList.Length - 1]);
        return newPosList;
    }*/

}


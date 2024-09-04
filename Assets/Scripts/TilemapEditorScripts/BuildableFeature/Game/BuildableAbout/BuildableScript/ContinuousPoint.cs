using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ContinuousPoint : BuildableBase
{
    public ContinuousPoint LastPoint;
    public ContinuousPoint NextPoint;
    public Material lineMaterial;
    private LineRenderer lineRenderer;
    private float vVelocity;

    private float hVelocity = 0.0f;


    public override void Init()
    {
        ContinuousPointInit();
    }
    
    public override void Dispose()
    {
        UnLinkPoint();
        ContinuousPointDispose();
    }

    protected virtual void ContinuousPointInit()
    {
    }

    protected virtual void ContinuousPointDispose()
    {
        
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        //Debug.Log("vSpeed"+vVelocity);
        if(NextPoint != null)
        {
            Debug.Log("doSendVSpeed"+vVelocity);
            SendVelocity(player);
        }

        if(LastPoint == null)
        {
            player.SetCanFly(true);
        }
        else if(NextPoint == null)
        {
            player.SetFlyFinished(true);
        }
    }

    protected override void TriggerOffThisBuildable(PlayerController player)
    {
        if(LastPoint == null)
        {
            player.SetCanFly(false);
        }
        else if(NextPoint == null)
        {
            player.SetFlyFinished(false);
            //EventManager.InvokeEvent(EventType.SpacebarUpEvent);
            player.PassLastFlyPoint(transform.position.x);
        }
    }

    private float CalVerticalVelocity()
    {

		
        Vector3 startPos = transform.position;
        Vector3 endPos = NextPoint.transform.position;
        float hDistance = endPos.x - startPos.x;
		
		///Added by Yeniao start
		if(hDistance==0)
		{
			return GameConsts.SPEED;
		}
		///End
		
        float vDistance = endPos.y - startPos.y;
        float time = hDistance / GameConsts.SPEED;
        float vSpeed = vDistance / time;
        //Debug.Log("vSpeed"+vSpeed);
        return vSpeed;
    }

    private float CalHorizonVelocity()
    {


        Vector3 startPos = transform.position;
        Vector3 endPos = NextPoint.transform.position;
        float vDistance = endPos.y - startPos.y;
        //float hDistance = endPos.x - startPos.x;

        ///Added by Yeniao start
/*        if (hDistance == 0)
        {
            return GameConsts.SPEED;
        }*/
        ///End

        float hDistance =endPos.x - startPos.x;
        float time = vDistance / GameConsts.SPEED;
        float hSpeed = hDistance / time;
        //Debug.Log("vSpeed"+vSpeed);
        return hSpeed;
    }

    private void SendVelocity(PlayerController player)
    {
        player.SetVerticalVelocity(vVelocity);
        player.SetHorizonVelocity(hVelocity);
    }
    
    public void LinkPoint()
    {
        int groupIndex = GetBuildableGroupIndex(Index);
        LastPoint = GetLastBuildableInGroup(groupIndex, Index) as ContinuousPoint;
        NextPoint = GetNextBuildableInGroup(groupIndex, Index) as ContinuousPoint;
        if(LastPoint != null)
        {
            GetComponent<SpriteRenderer>().enabled = false;
            LastPoint.GetComponent<SpriteRenderer>().enabled = false;
            UpdateLineRenderer();
        }
        else
        {
            GetComponent<SpriteRenderer>().enabled = true;
            HideLineRenderer();
        }

        if(NextPoint != null)
        {
            vVelocity = CalVerticalVelocity();
            hVelocity = CalHorizonVelocity();
        }
    }
    
    private void UnLinkPoint()
    {
        if(LastPoint != null)
        {
            LastPoint.NextPoint = NextPoint;
        }
        if(NextPoint != null)
        {
            NextPoint.LastPoint = LastPoint;
        }
        LastPoint = null;
        NextPoint = null;
    }
    
    public void UpdateLineRenderer()
    {
        //生成LineRenderer,并设置起始点为LastPoint的RealPosition、终点为this的RealPosition
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, Utils.GetRealPostion(LastPoint.Position));
        lineRenderer.SetPosition(1, Utils.GetRealPostion(Position));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.sortingOrder = -1;
        lineRenderer.material = lineMaterial;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.enabled = true;
        CalLineRenderTiling();


    }
    
    public void HideLineRenderer()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    #region 计算轨迹长度动态修改材质Tiling scale相关 

    public float originTrackLength = 1.0f;

    public float scaler = 0;
    private void CalLineRenderTiling()
    {
        if(originTrackLength <= 0)
        {
            Debug.LogWarning("illgel originTrackLength");
            return;
        }
        //lineRenderer = GetComponent<LineRenderer>();
        Material MaterialInstance = lineRenderer.material;
        //lineRenderer.positionCount = 2;
        //Vector3 pos2 = transform.position + Vector3.forward * 10;
        //Vector3[] tempPoints = new Vector3[2] { transform.position, pos2 };
        //lineRenderer.SetPositions(tempPoints);
        if (lineRenderer.positionCount<2)
        {
            Debug.LogWarning("lineRender point count is less 2,please check");
            //do nothing
            return;
        }
        Vector3 startPosition = lineRenderer.GetPosition(0);
        Vector3 endPosition = lineRenderer.GetPosition(1);
        float length = Vector3.Distance(endPosition , startPosition);

        float tilingCount = Mathf.FloorToInt(length / originTrackLength);


        if (tilingCount == 0)
        {
            scaler= Mathf.Repeat(length, originTrackLength) / originTrackLength;
            scaler = 1 / scaler;
        }
        else
        {
            scaler = (Mathf.Repeat(length, originTrackLength) / tilingCount + originTrackLength) / originTrackLength;
            //scaler = scaler / tilingCount +1;
        }
        //MaterialInstance.
        MaterialInstance.mainTextureScale =new Vector2 (MaterialInstance.mainTextureScale.x / scaler, MaterialInstance.mainTextureScale.y);
        //lineRenderer.material.mainTextureScale = new Vector2(tileCount, 1) * tilingFactor;
    }


    #endregion
}

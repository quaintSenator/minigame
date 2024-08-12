using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinousSelectButton : BuildableSelectButton
{
    protected override void OnClick()
    {
        BuildableType buildableType = BuildableType.continous_start_point;
        var point = ContinousPoint.LastSpawnPoint;
        if (point.GetPointType() == ContinousPointType.Start)
        {
            buildableType = BuildableType.continous_middle_point;
        }
        else if (point.GetPointType() == ContinousPointType.Middle)
        {
            buildableType = BuildableType.continous_middle_point;
        }
        BuildableCreator.Instance.SetSelectedObject(buildableType);
    }
}

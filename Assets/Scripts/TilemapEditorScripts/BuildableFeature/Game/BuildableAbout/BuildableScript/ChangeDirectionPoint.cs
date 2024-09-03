using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeDirectionPoint : BuildableBase
{
    
    public override void Init()
    {
        if(Rotation == 0 || Rotation == 2)
        {
            GetComponent<BoxCollider2D>().offset = new Vector2(1f, 0);
        }
        else
        {
            GetComponent<BoxCollider2D>().offset = new Vector2(-1f, 0);
        }
        if(SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            Vector3 realPosition = Utils.GetRealPostion(Position);
            RhythmViewer.Instance.AddChangeDirectionPoint(realPosition);
        }
    }
    
    public override void Dispose()
    {
        if(SceneManager.GetActiveScene().name == "TilemapEditorScene")
        {
            Vector3 realPosition = Utils.GetRealPostion(Position);
            RhythmViewer.Instance.RemoveChangeDirectionPoint(realPosition);
        }
    }

    protected override void TriggerThisBuildable(PlayerController player)
    {
        EventManager.InvokeEvent(EventType.ChangeDirectionEvent);
    }
}

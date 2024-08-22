using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyChangeDirPoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent(out PlayerController player))
        {
            player.ChangeFlyDir();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExpressionPoint : MonoBehaviour
{
     //由于enemy会被摧毁，故只触发，所有执行挪到spriteController中进行

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out PlayerController player))
        {
            Transform cube = player.transform.Find("Visual").Find("cube");
            PlayerSpriteController spriteController = cube.GetComponent<PlayerSpriteController>();
            spriteController.SetAttackSprite();
        }
    }
}
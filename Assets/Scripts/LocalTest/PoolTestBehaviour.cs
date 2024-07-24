using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTestBehaviour : MonoBehaviour
{
    [SerializeField] private Transform MgrTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PoolManager.Instance().Spawn(PoolItemType.GameObject);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool hasLiveChild = false;
            foreach (Transform child in MgrTransform)
            {
                if (child.gameObject.activeSelf)
                {
                    PoolManager.Instance().ReturnToPool(child.gameObject);
                    hasLiveChild = true;
                    break;
                }
            }
            if (!hasLiveChild)
            {
                Debug.Log("No child to Return!");
            }
            
        }
    }
}

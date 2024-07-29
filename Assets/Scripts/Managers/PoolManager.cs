using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    GameObject,
    BoxMeFadingShadowCopy
}
public class PoolManager : MonoBehaviour
{
    private static PoolManager _Instance;
    private Dictionary<PoolItemType, Stack<GameObject>> _dict = new Dictionary<PoolItemType, Stack<GameObject>>();
    private Dictionary<PoolItemType, GameObject> _copy_dict = new Dictionary<PoolItemType, GameObject>();
    [SerializeField] public GameObject copy4PoolItemTypeGameObject;
    [SerializeField] public GameObject copy4PoolItemTypeBoxMeFadingShadowCopy;
    [SerializeField]public Transform poolManagerRootTransform;
    readonly int POOL_CAPACITY = 20;

    private void InitCopyDict()
    {
        var copyGameObject = Instantiate(copy4PoolItemTypeGameObject, this.transform);
        _copy_dict[PoolItemType.GameObject] = copyGameObject;
        copyGameObject.SetActive(false);
        
        var copyBoxMeFadingShadowCopy = Instantiate(copy4PoolItemTypeBoxMeFadingShadowCopy, this.transform);
        _copy_dict[PoolItemType.BoxMeFadingShadowCopy] = copyBoxMeFadingShadowCopy;
        copyBoxMeFadingShadowCopy.SetActive(false);
        //new PoolItemType must provide copy here.
        //Use Inspector GameObject Reference to pass value to _copy_dict
        emptyGameObject.name = "EmptyGameObject";
        emptyGameObject.transform.SetParent(transform);
        emptyGameObject.SetActive(false);
    }

    //扩展对象池
    public void ExpandPool(string key, GameObject prefab, int size = 5)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary.Add(key, new Queue<GameObject>());
        }

        for (int i = 0; i < size; i++)
        {
            GameObject obj;
            obj = Instantiate(prefab);
            obj.SetActive(false);
            poolDictionary[key].Enqueue(obj);
        }
    }
    
    //检查池子不会太大，并且prefab一致
    public void CheckPool(string key, GameObject prefab, int maxSize = 30)
    {
        if(poolDictionary.ContainsKey(key))
        {
            //检查池子是否与prefab一致
            if (poolDictionary[key].Count > 0)
            {
                if (poolDictionary[key].Peek().name != prefab.name)
                {
                    Debug.LogWarning("Pool with tag " + key + " doesn't match prefab.");
                    return;
                }
            }
            
            //检查池子不会太大
            while (poolDictionary[key].Count > maxSize)
            {
                GameObject obj = poolDictionary[key].Dequeue();
                Destroy(obj);
            }
        }
    }
    
    //是否有对象可用
    public bool HasObjectsAvailable(string key)
    {
        if (!poolDictionary.ContainsKey(key) || poolDictionary[key].Count == 0)
        {
            return false;
        }

        return true;
    }

    //从对象池中取出对象 (有给prefab的版本)
    public GameObject SpawnFromPool(string key, GameObject prefab, Transform parent = null)
    {
        if (!HasObjectsAvailable(key))
        {
            ExpandPool(key, prefab);
        }

        GameObject objectToSpawn = poolDictionary[key].Dequeue();
        objectToSpawn.SetActive(true);
        if (parent != null)
        {
            objectToSpawn.transform.SetParent(parent);
        }
        return objectToSpawn;
    }
    
    //从对象池中取出空物体对象 (无给prefab的版本)
    public GameObject SpawnFromPool(string key, Transform parent = null)
    {
        if (!HasObjectsAvailable(key))
        {
            ExpandPool(key, emptyGameObject);
        }

        GameObject objectToSpawn = poolDictionary[key].Dequeue();
        objectToSpawn.SetActive(true);
        if (parent != null)
        {
            objectToSpawn.transform.SetParent(parent);
        }
        return objectToSpawn;
    }

    //将对象放回对象池
    public void ReturnToPool(string key, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning("Pool with tag " + key + " doesn't exist.");
            return;
        }
        
        objectToReturn.SetActive(false);
        objectToReturn.transform.SetParent(transform);
        poolDictionary[key].Enqueue(objectToReturn);
        
        //暂时只想到通过检查名字来检查是否是同一个prefab
        //但是如果后面预制体的名字被改了，这个方法就会出问题
        //所以暂时先不检查是否时同一个prefab
        //CheckPool(key, objectToReturn);
    }
}

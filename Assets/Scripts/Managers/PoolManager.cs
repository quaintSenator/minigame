using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

public enum PoolItemType
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
    }
    private void Start()
    {
        InitCopyDict();
        if (_Instance == null)
        {
            _Instance = this;
        }
        GeneratePool(PoolItemType.GameObject, _copy_dict[PoolItemType.GameObject]);
        GeneratePool(PoolItemType.BoxMeFadingShadowCopy, _copy_dict[PoolItemType.BoxMeFadingShadowCopy]);
        if (_dict.Count < 1)
        {
            Debug.LogError("PoolGenerate Fail.");
        }
    }
    public static PoolManager Instance()
    {
        return _Instance;
    }
    private void GeneratePool(PoolItemType itemType, GameObject firstCopy)
    {
        if (!_dict.ContainsKey(itemType))
        {
            var stack2make = new Stack<GameObject>();
            stack2make.Push(firstCopy);
            _dict.Add(itemType, stack2make);
        }
    }
    public void ReturnToPool(GameObject go, PoolItemType itemType = PoolItemType.GameObject)
    {
        //回收时，transform树不剪
        if (_dict[itemType].Count >= POOL_CAPACITY - 1)
        {
            Debug.Log("pool overflow, Destroy.");
            Destroy(go);
        }
        else
        {
            _dict[itemType].Push(go);
            go.SetActive(false);
        }
    }
    public GameObject Spawn(PoolItemType objectType)
    {
        return Spawn(objectType, poolManagerRootTransform);
    }
    public GameObject Spawn(PoolItemType objectType, Transform parent)
    {
        foreach (var pair in _dict)
        {
            Debug.Log(pair.Key);
        }
        var pool = _dict[objectType];
        Debug.Log("beform Spawn, Pool Count = " + pool.Count.ToString());
        if (_dict.ContainsKey(objectType))
        {
            if (pool.Count > 1)
            {
                var top = pool.Peek();
                top.SetActive(true);
                top.transform.SetParent(parent);
                pool.Pop();
                return top;
            }
            else
            {
                if (pool.Count == 0)
                {
                    //pool count == 0 is not permitted
                    Debug.LogError("Fatal error, spawn fail due to empty pool");
                }
                else
                {
                    //pool count == 1
                    var leftObj = pool.Peek();//返回就地构造的一个新对象，保留栈顶元素
                    var newCopy = Instantiate(leftObj, parent);
                    newCopy.SetActive(true);
                    return newCopy;
                }
            }
        }
        return null;
    }
}
/*
 * 用池生成物体
 * PoolManager.Instance().Spawn(PoolItemType.GameObject);
 *
 * 用池回收物体
 * PoolManager.Instance().ReturnToPool(go);
 * 注意，回收后go成为一个临时值；仍然使用go将可能造成未定义错误
 *
 * 注册新池子（封装暂时较差，必须修改PoolManager部分代码，也可通知suifeng描述需求帮你改）
 * 1. 在PoolManager中新增成员：
 * [SerializeField] public GameObject copy4PoolItemTypeXXX;
   2. 在InitCopyDict部分增添代码：
   var copyGameObject = Instantiate(copy4PoolItemTypeXXX, this.transform);
        _copy_dict[PoolItemType.GameObject] = copyGameObject;
    3. 为PoolManager Inspector中拖引用，一看就知道拖什么
 */
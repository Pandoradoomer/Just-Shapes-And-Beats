using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    CIRCLE,
    PROJECTILE,
    SQUARE
}
public class ObjectPool : MonoBehaviour
{
    [SerializeField] int poolSize;
    [SerializeField] Transform prefab;
    private GameObject[] pool;
    LinkedList<GameObject> freeList;
    public PoolType poolType;
    // Start is called before the first frame update
    void Start()
    {
        pool = new GameObject[poolSize];
        freeList = new LinkedList<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(prefab).gameObject;
            pool[i].SetActive(false);
            freeList.AddFirst(pool[i]);
        }
    }

    public GameObject GetObject()
    {
        if(freeList.First != null)
        {
            GameObject first = freeList.First.Value;
            freeList.RemoveFirst();
            return first;
        }
        return null;
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        freeList.AddFirst(obj);
    }
}

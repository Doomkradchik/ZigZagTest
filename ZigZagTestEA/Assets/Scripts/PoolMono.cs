using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolMono<T> where T : MonoBehaviour
{
    public T Prefab { get; }
    public bool AutoExpand { get; set; }
    public Transform Root { get; } = null;
    private List<T> _pool;

    public T FreeElement
    {
        get
        {
            if (HasFreeElement(out T element))
                return element;

            if (AutoExpand)
                return CreateObject(true);

            throw new System.InvalidOperationException();
        }
    }

    public PoolMono(T prefab, int count)
    {
        Prefab = prefab;
        CreatePool(count);
    }

    public PoolMono(T prefab, int count, Transform root)
    {
        Prefab = prefab;
        Root = root;
        CreatePool(count);
    }

    private void CreatePool(int count)
    {
        _pool = new List<T>();

        for (int i = 0; i < count; i++)
        {
            CreateObject();
        }
    }

    private T CreateObject(bool isActiveByDefault = false)
    {
        var createdObject = Object.Instantiate(Prefab, Root);
        createdObject.gameObject.SetActive(isActiveByDefault);
        _pool.Add(createdObject);
        return createdObject;
    }

    public bool HasFreeElement(out T element)
    {
        foreach(var mono in _pool)
        {
            if (mono.gameObject.activeInHierarchy == false)
            {
                element = mono;
                element.gameObject.SetActive(true);
                element.enabled = true;
                return true;
            }
        }

        element = null;
        return false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectRuntimeSetAssigner : MonoBehaviour
{
    public GameObjectRuntimeSet[] sets;
    [SerializeField]
    private bool automaticOnEnable = true;
    [SerializeField]
    private bool automaticOnDisable = true;

    private void OnEnable()
    {
        if (automaticOnEnable)
        {
            AssignToAll();
        }
    }

    private void OnDisable()
    {
        if (automaticOnDisable)
        {
            UnassignToAll();
        }
    }

    private void AssignToAll()
    {
        foreach (GameObjectRuntimeSet i in sets)
        {
            i.Add(this.gameObject);
        }
    }

    private void UnassignToAll()
    {
        foreach (GameObjectRuntimeSet i in sets)
        {
            i.Remove(this.gameObject);
        }
    }
}

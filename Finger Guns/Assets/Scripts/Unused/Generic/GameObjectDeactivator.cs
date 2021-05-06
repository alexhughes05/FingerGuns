using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectDeactivator : MonoBehaviour
{
    [SerializeField]
    private bool deactivateOnAwake = false;
    [SerializeField]
    private bool deactivateOnEnable = false;
    [SerializeField]
    private bool deactivateOnStart = false;

    private void Awake()
    {
        if (deactivateOnAwake)
        {
            DeactivateGameObject();
        }
    }

    private void OnEnable()
    {
        if (deactivateOnEnable)
        {
            DeactivateGameObject();
        }
    }

    void Start()
    {
        if (deactivateOnStart)
        {
            DeactivateGameObject();
        }
    }

    public void DeactivateGameObject()
    {
        gameObject.SetActive(false);
    }
}

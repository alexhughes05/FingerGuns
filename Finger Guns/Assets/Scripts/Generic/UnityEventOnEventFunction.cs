using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventOnEventFunction : MonoBehaviour
{
    [SerializeField]
    private UnityEvent eventToExecute;
    [Header("When to Invoke")]
    [SerializeField]
    private bool onAwake = false;
    [SerializeField]
    private bool onEnable = false;
    [SerializeField]
    private bool onStart = false;
    [Space(6)]
    [SerializeField]
    private bool onDestroy = false;
    [SerializeField]
    private bool onDisable = false;

    public void InvokeUnityEvent()
    {
        eventToExecute.Invoke();
    }

    private void Awake()
    {
        if (onAwake)
        {
            InvokeUnityEvent();
        }
    }

    private void OnEnable()
    {
        if (onEnable)
        {
            InvokeUnityEvent();
        }
    }

    private void Start()
    {
        if (onStart)
        {
            InvokeUnityEvent();
        }
    }

    private void OnDestroy()
    {
        if (onDestroy)
        {
            InvokeUnityEvent();
        }
    }

    private void OnDisable()
    {
        if (onDisable)
        {
            InvokeUnityEvent();
        }
    }
}

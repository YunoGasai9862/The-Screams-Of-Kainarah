using System.Threading.Tasks;
using System;
using UnityEngine.Events;
using UnityEngine;
public class GameLoadPoolEvent : UnityEventWTAsync
{
    private UnityEvent m_gameLoadPoolEvent = new UnityEvent();
    public override UnityEvent GetInstance()
    {
        return m_gameLoadPoolEvent;
    }
    public override Task AddListener(UnityAction action)
    {
        try
        {
            Debug.Log($"Adding Listener {action.Method}");

            m_gameLoadPoolEvent.AddListener(action);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }

        return Task.CompletedTask;
    }
    public override Task Invoke()
    {
        try
        {
            m_gameLoadPoolEvent.Invoke();

            Debug.Log("Invoked");

        }catch(Exception ex)
        {
            Debug.LogException(ex);
        }


        return Task.CompletedTask;
    }
}
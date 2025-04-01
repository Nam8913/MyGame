using System;
using System.Collections.Generic;
using UnityEngine;

public static class EventCall
{
    public class EventTarget
    {
        public string nameEvt;
        public string typeEvt;
        public Action actionMethod;
        public Action exceptionMethod;

        public EventTarget(string name, string type, Action actionMethod, Action exceptionMethod = null)
        {
            this.nameEvt = name;
            this.typeEvt = type;
            this.actionMethod = actionMethod;
            this.exceptionMethod = exceptionMethod;
        }

        public EventTarget CreateNextRequest(string name, Action actionMethod, Action exceptionMethod = null)
        {
            EventTarget target = new EventTarget(name, this.typeEvt ,actionMethod, exceptionMethod);
            EventCall.Call(target);
            return target;
        }
         
    }
    public static EventTarget Call(string name, string type, Action action, Action exceptionMethod = null)
    {
        EventTarget rs = new EventTarget(name, type, action, exceptionMethod);
        EventQueue.Enqueue(rs);
        return rs;
    }
    public static EventTarget Call(EventTarget rs)
    {
        EventQueue.Enqueue(rs);
        return rs;
    }
    private static Queue<EventTarget> EventQueue = new Queue<EventTarget>();
    private static EventTarget currEvent = null;
    public static void UpdateEvent()
    {
        if (EventQueue.Count > 0)
        {
            currEvent = EventQueue.Dequeue();
            try
            {
                currEvent.actionMethod();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in event '{currEvent.nameEvt}': {ex.Message}");
                if (currEvent.exceptionMethod!= null)
                {
                    currEvent.exceptionMethod();
                }
            }
        }
        
    }
}

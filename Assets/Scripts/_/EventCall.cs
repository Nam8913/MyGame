using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public static class EventCall
{
    public class EventQueue
    {
        public string sceneOrderLoad;

        public string nameEvt;
        public string typeEvt;

        public bool doAsynchronously;

        public Action actionMethod;
        public IEnumerator actionMethodEnumerator;
        public Action<Exception> exceptionMethod;
    }

    public static void Add(string name, string type, Action action,  Action<Exception> exceptionMethod = null)
    {
        EventQueue rs = new EventQueue();
        rs.nameEvt = name;
        rs.typeEvt = type;
        rs.actionMethod = action;
        rs.exceptionMethod = exceptionMethod;
        eventQueue.Enqueue(rs);
    }
     public static void Add(string name, string type, IEnumerator action,  Action<Exception> exceptionMethod = null)
    {
        EventQueue rs = new EventQueue();
        rs.nameEvt = name;
        rs.typeEvt = type;
        rs.actionMethodEnumerator = action;
        rs.exceptionMethod = exceptionMethod;
        eventQueue.Enqueue(rs);
    }
    public static void OrderLoadScene(string sceneOrderLoad, string name, string type, Action action, Action<Exception> exceptionMethod = null)
    {
        EventQueue rs = new EventQueue();
        rs.nameEvt = name;
        rs.typeEvt = type;
        rs.actionMethod = action;
        rs.sceneOrderLoad = sceneOrderLoad;
        rs.exceptionMethod = exceptionMethod;
        eventQueue.Enqueue(rs);
    }
    public static void Add(EventQueue rs)
    {
        eventQueue.Enqueue(rs);
    }
    private static Queue<EventQueue> eventQueue = new Queue<EventQueue>();
    private static EventQueue currEvent = null;

    private static Thread eventThread = null;
    private static AsyncOperation asyncOperation = null;
    
    
    public static void UpdateEvent(out bool sceneChanged)
    {
        sceneChanged = false;
        if(currEvent != null)
        {
            if(currEvent.actionMethodEnumerator != null)
            {
                UpdateEnumratorEvent(currEvent);
            }else if(currEvent.doAsynchronously)
            {
                UpdateAsynchronous(currEvent);
            }else
            {
                UpdateSynchronous(currEvent, out sceneChanged);
            }
        }
        if (eventQueue.Count > 0 && currEvent == null)
        {
            currEvent = eventQueue.Dequeue();
        }
        
    }


    private static void UpdateSynchronous(EventQueue currEvent , out bool sceneChanged)
    {
        sceneChanged = false;
        if (currEvent != null)
        {
            try
            {
                if(currEvent.actionMethod != null)
                {
                    currEvent.actionMethod();
                }
                if(!string.IsNullOrEmpty(currEvent.sceneOrderLoad))
                {
                    sceneChanged = true;
                    CurrentGame.DoLoadScene(currEvent.sceneOrderLoad);
                }
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in event '{currEvent.nameEvt}': {ex.Message}");
                if (currEvent.exceptionMethod != null)
                {
                    currEvent.exceptionMethod(ex);
                }
            }finally
            {
                Reset();
            }
        }
    }
    private static void UpdateAsynchronous(EventQueue currEvent)
    {
        if (currEvent != null)
        {
            if(eventThread == null)
            {
                eventThread = new Thread(() => 
                {
                    try
                    {
                        if (currEvent != null)
                        {
                            currEvent.actionMethod();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception from asynchronous event: " + ex);
                        try
                        {
                            if (currEvent != null && currEvent.exceptionMethod != null)
                            {
                                currEvent.exceptionMethod(ex);
                            }
                        }
                        catch (Exception arg)
                        {
                            Debug.LogError("Exception was thrown while trying to handle exception. Exception: " + arg);
                        }
                    }
                    
                });
                eventThread.Start();
                return;
            }
            if(!eventThread.IsAlive)
            {
                try
                {
                    bool flag = false;
                    if(!string.IsNullOrEmpty(currEvent.sceneOrderLoad))
                    {
                        
                        if(asyncOperation == null)
                        {
                            asyncOperation = CurrentGame.DoLoadSceneAsync(currEvent.sceneOrderLoad);
                        }else
                        {
                            flag = true;
                            if(asyncOperation.isDone)
                            {
                                //asyncOperation.allowSceneActivation = true;
                                asyncOperation = null;
                            }
                        }
                        
                    }else
                    {
                        flag = true;
                    }

                    if(flag)
                    {
                        Reset();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error in event '{currEvent.nameEvt}': {ex.Message}");
                    if (currEvent.exceptionMethod!= null)
                    {
                        currEvent.exceptionMethod(ex);
                    }
                }
            }
            
            
        }
    }
    private static void UpdateEnumratorEvent(EventQueue currEvent)
    {
        if (currEvent != null)
        {
            try
            {
                float num = Time.realtimeSinceStartup + 0.1f;
                while (currEvent.actionMethodEnumerator.MoveNext())
                {
                    if(Time.realtimeSinceStartup >= num)
                    {
                        Debug.LogWarning("EventCall: Waiting for enumerator to finish. " + currEvent.nameEvt);
                        return;
                    }
                    // Do nothing, just wait for the enumerator to finish
                }
                IDisposable disposable = currEvent.actionMethodEnumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error in event '{currEvent.nameEvt}': {ex.Message}");
                IDisposable disposable = currEvent.actionMethodEnumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
                if (currEvent.exceptionMethod != null)
                {
                    currEvent.exceptionMethod(ex);
                }
            }finally
            {
                Reset();
            }
        }
    }

    private static void Reset()
    {
        currEvent = null;
        eventThread = null;
        asyncOperation = null;
    }
    public static void Clear()
    {
        eventQueue.Clear();
        Reset();
    }
}

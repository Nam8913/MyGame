using UnityEngine;

public abstract class NodeAbstract
{
    public enum State
    {
        Inactive,
        Running,
        Failure,
        Success
    }
    public State state = State.Inactive;
    protected virtual float GetPriority(){return 0f;}
    protected virtual float Evaluate(){return 0f;}
    

    public State GetUpdate()
    {
        if (!isStarted)
        {
            Enter();
            isStarted = true;
        }
        state = UpdateState();

        if (state == State.Failure || state == State.Success)
        {
            Exit();
            isStarted = false;
        }
        return state;
    }


    protected abstract void Enter();
    protected abstract void Exit();
    protected abstract State UpdateState();
    public virtual void OnTreeEnter(){}
    public virtual void OnTreeExit(){}
    protected virtual void OnSuccess(){}
    protected virtual void OnFailure(){}
    protected virtual void OnRunning(){}
    protected virtual void OnInactive(){}

    private bool isStarted = false;
    
}

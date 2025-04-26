using UnityEngine;

public sealed class RootNode : NodeAbstract
{
    public NodeAbstract children;
    protected override void Enter()
    {
    }

    protected override void Exit()
    {
    }

    protected override State UpdateState()
    {
        return children.GetUpdate();
    }
}

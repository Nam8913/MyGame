using UnityEngine;

public sealed class RootNode : NodeAbstract
{
    public NodeAbstract children;
    protected override void EnterNode()
    {
    }

    protected override void ExitNode()
    {
    }

    protected override State DoUpdateState()
    {
        return children.CallUpdate();
    }
}

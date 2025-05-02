using UnityEngine;

public class CreatureEntity : Entity
{
    [SerializeField]
    public BehaviorTree behaviorEntity;

    public override void Start()
    {
        //base.Start();
        behaviorEntity = new BehaviorTree(this.gameObject);
        
        ActionFleeNode fleeNode = new ActionFleeNode();

        behaviorEntity.CreateRootNode().CreateChildNode("FleeNode", fleeNode);
    }
    public override void Update()
    {
        base.Update();
        behaviorEntity.UpdateTreeBehavior();
    }
}

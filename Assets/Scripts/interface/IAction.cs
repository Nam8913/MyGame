using UnityEngine;

public interface IAction
{
    Entity entity { get;}
    string ActionName { get; set; }
    void Execute(GameObject user);
}

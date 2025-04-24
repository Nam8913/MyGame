using System.Collections.Generic;
using UnityEngine;

public class InteractDetector : MonoBehaviour
{
    public float rad = 3f;
    public LayerMask interactLayer;
    Player player;
    [SerializeField]
    GameObject target;

    void Start()
    {
        player = this.gameObject.GetComponent<Player>();
    }
    void FixedUpdate()
    {
        //Reset target
        this.target = null;

        Collider2D[] hit = hitFillter(Physics2D.OverlapCircleAll(this.gameObject.transform.position,rad,interactLayer));
        if(hit == null || hit.Length == 0) return;

        float closedTarget = Mathf.Infinity;
        Collider2D target = null;
        if(Vector2.Distance(player.transform.position,player.mousePlayerCtr.mousePos) < rad)
        {
            foreach (var item in hit)
            {
                float distToMouse = Vector2.Distance(player.mousePlayerCtr.mousePos,item.transform.position);
                if(distToMouse <= 1.0f && distToMouse < closedTarget)
                {
                    closedTarget = distToMouse;
                    target = item;
                }
            }
        }
        

        if(target == null)
        {
            foreach (var item in hit)
            {
                float distToTarget = Vector2.Distance(transform.position,item.transform.position);
                if(distToTarget < closedTarget)
                {
                    closedTarget = distToTarget;
                    target = item;
                }
            }
        }
        
        this.target = target != null ? target.gameObject : null;
    
    
        if(Input.GetKeyDown(KeyCode.E))
        {
            IAction action = this.target.GetComponent<IAction>();
            if(action != null)
            {
                action.Execute(player.gameObject);
            }
        }
    }
    void OnGUI()
    {
        if(target != null)
        {
            Vector2 posUI = Camera.main.WorldToScreenPoint(target.transform.position);
            if(GUI.Button(new Rect(posUI.x, (float)Screen.height - posUI.y, 100, 20), target.name))
            {
                //Debug.Log("Interact with " + target.name);
                IAction action = target.GetComponent<IAction>();
                if(action != null)
                {
                    action.Execute(player.gameObject);
                }
            }
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,rad);

        //draw distance from player to target
        if(target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(player.transform.position,target.transform.position);
        }
    }

    Collider2D[] hitFillter(Collider2D[] hits)
    {
        List<Collider2D> result = new List<Collider2D>();
        foreach (var item in hits)
        {
            item.TryGetComponent<Entity>(out Entity entity);
            if(entity == null) continue;
            if(entity.showHint)
            {
                result.Add(item);
            }
        }
        return result.ToArray();
    }
}

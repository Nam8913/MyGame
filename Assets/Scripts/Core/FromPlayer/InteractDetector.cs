using UnityEngine;

public class InteractDetector : MonoBehaviour
{
    public float rad = 3f;
    public LayerMask interactLayer;
    Player player;
    void Start()
    {
        player = this.gameObject.GetComponent<Player>();
    }
    void FixedUpdate()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(this.gameObject.transform.position,rad,interactLayer);
        if(hit == null || hit.Length == 0) return;

        float closedTarget = Mathf.Infinity;
        Collider2D target = null;
        foreach (var item in hit)
        {
            float distToMouse = Vector2.Distance(player.mousePlayerCtr.mousePos,item.transform.position);
            if(distToMouse <= 1.0f && distToMouse < closedTarget)
            {
                closedTarget = distToMouse;
                target = item;
            }
        }

        if(target == null)
        {
            closedTarget = Mathf.Infinity;
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
        Debug.Log(target.gameObject.name);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,rad);
    }
}

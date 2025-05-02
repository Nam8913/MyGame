using UnityEngine;

public class ProjectileComp : Entity
{
    public Entity owner;
    public float lifeTime = 5f; // Time in seconds before the projectile is destroyed
    // Update is called once per frame
    public override void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
        {
            Destroy(gameObject); // Destroy the projectile after its lifetime expires
        }   
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
       IDamageable damageable = collision.GetComponent<IDamageable>();
       if(collision.gameObject.GetComponent<Entity>() == owner) return;
       
        if (damageable != null)
        {
            Debug.Log("Projectile hit: " + collision.name);
            damageable.TakeDamage(this);
            Destroy(gameObject); // Destroy the projectile after hitting an object
        }
    }
}

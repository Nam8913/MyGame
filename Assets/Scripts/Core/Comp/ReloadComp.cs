using System.Collections;
using UnityEngine;

public class ReloadComp : MonoBehaviour,IAction
{
    public string ActionName { get; set; } = "Reload";
    
    public int currAmmoInMagazine = 0;
    public int magazineSize = 30;
    public float reloadTime = 2f;

    public Entity entity
    {
        get
        {
            return GetComponent<Entity>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // Check for reload input
        {
            Debug.Log("Reloading weapon...");
            StartCoroutine(ReloadCoroutine());
        }
    }

    IEnumerator ReloadCoroutine()
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        Execute(gameObject); // Call the Execute method after the reload time
        Debug.Log("Reload complete. Ammo in magazine: " + currAmmoInMagazine);
    }

    public void Execute(GameObject user)
    {
        // Implement the reload logic here
        Debug.Log("Reloading weapon...");
        currAmmoInMagazine = magazineSize;

        ShootComp shootComp = GetComponent<ShootComp>();

        if(shootComp == null)
        {
            Debug.LogError("ShootComp not found on entity " + gameObject.name);
            return;
        }else
        {
            shootComp.currentRecoil = 0f; // Reset recoil when reloading
            shootComp.timeSinceLastShot = 0f; // Reset time since last shot
        }
        
    }
}

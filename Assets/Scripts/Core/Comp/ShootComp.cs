using System;
using UnityEngine;

public class ShootComp : MonoBehaviour, IAction
{
    public string ActionName { get; set; } = "Shoot";

    public Entity entity => GetComponent<Entity>();

    [Header("Bullet Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public int bulletsPerShot = 1;              // Number of bullets per shot (e.g., 1 for pistol, 4 for shotgun)
    public float bulletSpeed = 30f;
    public float spreadAngle = 3f;              // Spread angle (higher for shotguns)

    [Header("Fire Rate Settings")]
    public float fireRateMin = 0.15f;           // Minimum time between shots
    public float fireRateMax = 0.5f;            // Maximum delay when spamming shots

    [Header("Recoil Settings")]
    public float recoilStrength = 1.5f;         // How quickly recoil decreases over time
    public float recoilIncreasePerShot = 0.5f;  // Recoil added per shot

    [Header("References")]
    public Transform dir;                       // Direction to shoot (optional)

    private ReloadComp reloadComp;
    public float timeSinceLastShot = 0f;
    public float currentRecoil = 0f;

    private void Start()
    {
        reloadComp = GetComponent<ReloadComp>();
    }

    private void Update()
    {
        HandleRecoil();
        HandleShootingInput();
    }

    private void HandleRecoil()
    {
        // Gradually reduce recoil over time
        if (currentRecoil > 0f)
        {
            currentRecoil -= Time.deltaTime * recoilStrength;
            currentRecoil = Mathf.Max(currentRecoil, 0f); // Ensure recoil doesn't go below 0
        }

        // Track time since the last shot
        timeSinceLastShot += Time.deltaTime;
    }

    private void HandleShootingInput()
    {
        // Check for shooting input (left mouse button)
        if (Input.GetMouseButton(0))
        {
            Execute(gameObject);
        }
    }

    public void Execute(GameObject user)
    {
        // Calculate fire delay based on current recoil
        float fireDelay = Mathf.Lerp(fireRateMin, fireRateMax, currentRecoil / 5f);
        if (timeSinceLastShot < fireDelay || reloadComp.currAmmoInMagazine <= 0) return;

        // Fire bullets
        for (int i = 0; i < bulletsPerShot; i++)
        {
            FireBullet();
        }

        // Update ammo, time, and recoil
        reloadComp.currAmmoInMagazine--;
        timeSinceLastShot = 0f;
        currentRecoil += recoilIncreasePerShot;
    }

    private void FireBullet()
    {
        // Calculate spread angle
        float spread = GenRandom.Range(-spreadAngle, spreadAngle);
        float zRotation = dir != null
            ? dir.transform.eulerAngles.z + spread - 90f
            : firePoint.eulerAngles.z + spread;

        // Create bullet with calculated rotation
        Quaternion rotation = Quaternion.Euler(0, 0, zRotation);
        Vector3 spawnPos = firePoint.position;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, rotation);
        bullet.GetComponent<ProjectileComp>().owner = entity;
        bullet.GetComponent<Rigidbody2D>().linearVelocity = rotation * Vector2.up * bulletSpeed;
    }
}
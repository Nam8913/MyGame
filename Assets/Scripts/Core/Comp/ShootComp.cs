using UnityEngine;

public class ShootComp : MonoBehaviour,IAction
{
    public string ActionName { get; set; } = "Shoot";

    public GameObject bulletPrefab;
    public Transform firePoint;

    public int bulletsPerShot = 1;              // Số đạn mỗi lần bắn (1 cho pistol, 4 cho shotgun)
    public float fireRateMin = 0.15f;           // Thời gian tối thiểu giữa các phát bắn
    public float fireRateMax = 0.5f;            // Nếu spam liên tục → tăng delay
    public float bulletSpeed = 30f;
    public float spreadAngle = 3f;              // Độ tỏa (shotgun cao hơn)
    public float recoilStrength = 1.5f;
    public float recoilIncreasePerShot = 0.5f;

   
    public float timeSinceLastShot = 0f;
    public float currentRecoil = 0f;

    ReloadComp reloadComp;

    public Entity entity
    {
        get
        {
            return GetComponent<Entity>();
        }
    }

    void Start()
    {
        reloadComp = GetComponent<ReloadComp>();
    }
    void Update()
    {
        timeSinceLastShot += Time.deltaTime;
        if (currentRecoil > 0f)
        {
            currentRecoil -= Time.deltaTime * recoilStrength; // Giảm độ giật theo thời gian
            if (currentRecoil < 0f) currentRecoil = 0f; // Đảm bảo không âm
        }
        if(Input.GetMouseButton(0))
        {
            Execute(this.gameObject);
        }
    }

    public void Execute(GameObject user)
    {
        float fireDelay = Mathf.Lerp(fireRateMin, fireRateMax, currentRecoil / 5f);
        if (timeSinceLastShot < fireDelay || reloadComp.currAmmoInMagazine <= 0) return;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float spread = Random.Range(-spreadAngle, spreadAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, firePoint.eulerAngles.z + spread);

            Vector3 spawnPos = firePoint.position;
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, rotation);
            bullet.GetComponent<Rigidbody2D>().linearVelocity = rotation * Vector2.up * bulletSpeed;
        }

        reloadComp.currAmmoInMagazine--;
        timeSinceLastShot = 0f;
        currentRecoil += recoilIncreasePerShot;
    }
}

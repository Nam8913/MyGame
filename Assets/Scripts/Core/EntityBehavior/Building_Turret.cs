using System.Collections;
using UnityEngine;

public class Building_Turret : Entity
{
    private ShootComp shootComp;
    private ReloadComp reloadComp;

    [Header("Recoil Settings")]
    public Vector3 recoilOffset = new Vector3(-0.05f, 0);
    public float recoilSpeed = 20f; // Speed when recoiling
    public float returnSpeed = 10f; // Speed when returning to the original position

    [Header("Turret Settings")]
    public float distance = 10f; // Detection range
    public float rotationSpeed = 160f; // Degrees per second
    public float fireAngleThreshold = 5f; // Allowed angle deviation to fire

    [Header("References")]
    public GameObject target;
    public GameObject barrelTransform; // The barrel that recoils
    public GameObject topTurret; // The rotating part of the turret

    private Vector3 originBarrelPos;
    private Vector3 targetBarrelPos;
    private bool isRecoiling = false;

    private float currAngle;

    public override void Start()
    {
        originBarrelPos = barrelTransform.transform.localPosition;
        targetBarrelPos = originBarrelPos;

        shootComp = GetComponent<ShootComp>();
        reloadComp = GetComponent<ReloadComp>();

        if (shootComp != null)
        {
            shootComp.dir = topTurret.transform;
        }
    }

    public override void Update()
    {
        HandleBarrelRecoil();
    }

    public override void FixedUpdate()
    {
        DetectTarget();
        RotateAndFireAtTarget();
    }

    private void HandleBarrelRecoil()
    {
        // Smoothly move the barrel to the target position
        barrelTransform.transform.localPosition = Vector3.Lerp(
            barrelTransform.transform.localPosition,
            targetBarrelPos,
            Time.deltaTime * (isRecoiling ? recoilSpeed : returnSpeed)
        );

        // Stop recoil if the barrel is close to the original position
        if (!isRecoiling && Vector3.Distance(barrelTransform.transform.localPosition, originBarrelPos) < 0.001f)
        {
            barrelTransform.transform.localPosition = originBarrelPos;
        }
    }

    private void DetectTarget()
    {
        if (target == null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, distance);
            foreach (var collider in colliders)
            {
                Entity entity = collider.GetComponent<Entity>();
                if (entity != null && entity != this)
                {
                    target = collider.gameObject;
                    break;
                }
            }
        }
    }

    private void RotateAndFireAtTarget()
    {
        if (target == null) return;

        Vector2 direction = (target.transform.position - transform.position).normalized;
        currAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Quaternion targetRotation = Quaternion.Euler(0, 0, currAngle);
        topTurret.transform.rotation = Quaternion.RotateTowards(
            topTurret.transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // Fire if the turret is aligned with the target
        if (Vector2.Angle(topTurret.transform.right, direction) < fireAngleThreshold &&
            shootComp != null && shootComp.timeSinceLastShot >= Time.deltaTime * recoilSpeed)
        {
            Fire();
        }
    }

    private void Fire()
    {
        shootComp?.Execute(gameObject);
        DoRecoil();
    }

    private void DoRecoil()
    {
        targetBarrelPos = originBarrelPos + recoilOffset;
        StopAllCoroutines();
        StartCoroutine(ReturnAfterDelay(0.05f)); // Small delay for quick recoil effect
        isRecoiling = true;
    }

    private IEnumerator ReturnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetBarrelPos = originBarrelPos;
        isRecoiling = false;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distance);

        // Draw line to the target
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
}
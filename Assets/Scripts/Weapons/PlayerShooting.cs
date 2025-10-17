using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Statistics))]
public class PlayerShooting : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform[] firePoints;
    [SerializeField] private GameObject blasterMissilePrefab;
    [SerializeField] private GameObject shotgunMissilePrefab;

    [Header("Fire Settings")]
    [SerializeField, Range(0.05f, 1f)] private float baseFireRate ;

    private float lastFireTime;
    private Keyboard keyboard;
    private Statistics stats;

    private int activeCannons = 1;

    private enum WeaponType { Blaster, Shotgun }
    private WeaponType currentWeapon = WeaponType.Blaster;

    private void Awake()
    {
        keyboard = Keyboard.current;
        stats = GetComponent<Statistics>();
        stats.Populate();
    }

    private void Update()
    {
        if (keyboard == null) return;

        HandleWeaponSwitch();
        HandleShooting();
    }

    private void HandleWeaponSwitch()
    {
        if (keyboard.digit1Key.wasPressedThisFrame)
            currentWeapon = WeaponType.Blaster;
        if (keyboard.digit2Key.wasPressedThisFrame)
            currentWeapon = WeaponType.Shotgun;
    }

    private void HandleShooting()
    {
        if (!keyboard.spaceKey.isPressed) return;

        float fireRate = Mathf.Max(0.05f, stats.GetStatistic(StatisticsType.FireRate));
        if (Time.time < lastFireTime + fireRate) return;

        Shoot();
        lastFireTime = Time.time;
    }

    private void Shoot()
    {
        switch (currentWeapon)
        {
            case WeaponType.Blaster:
                FireBlaster();
                break;
            case WeaponType.Shotgun:
                FireShotgun();
                break;
        }
    }

    private void FireBlaster()
    {
        if (firePoints == null || firePoints.Length == 0 || !blasterMissilePrefab) return;

        float damage = stats.GetStatistic(StatisticsType.Attack);

        for (int i = 0; i < activeCannons && i < firePoints.Length; i++)
        {
            var missile = Instantiate(blasterMissilePrefab, firePoints[i].position, Quaternion.Euler(90, 0, 0));
            if (missile.TryGetComponent(out Missile missileComponent))
                missileComponent.Damage = damage;
        }
    }

    private void FireShotgun()
    {
        if (firePoints == null || firePoints.Length == 0 || !shotgunMissilePrefab) return;

        float damage = stats.GetStatistic(StatisticsType.Attack) * 0.6f;
        float[] angles = { -30f, -15f, 0f, 15f, 30f };

        for (int i = 0; i < activeCannons && i < firePoints.Length; i++)
        {
            foreach (float angle in angles)
            {
                Quaternion rotation = Quaternion.Euler(90, angle, 0);
                var missile = Instantiate(shotgunMissilePrefab, firePoints[i].position, rotation);
                if (missile.TryGetComponent(out Missile missileComponent))
                {
                    missileComponent.Damage = damage;
                    missileComponent.CanBounce = true;
                }
            }
        }
    }

    public void EnableSecondCannon(bool enable)
    {
        if (enable && activeCannons < 2)
        {
            activeCannons = 2;
            Debug.Log("Double canon activé !");
        }
    }

    public void EnableThirdCannon(bool enable)
    {
        if (enable && activeCannons < 3)
        {
            activeCannons = 3;
            Debug.Log("Triple canon activé !");
        }
    }
}

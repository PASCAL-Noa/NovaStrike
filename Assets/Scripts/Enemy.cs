using UnityEngine;

[SerializeField, RequireComponent(typeof(LevelSystem), typeof(Statistics))]
public class Enemy : Entity
{
    [SerializeField] private int scoreValue = 5;
    [SerializeField] private int expValue = 2;
    [SerializeField] private float _mSpeed;
    public float Speed { get => _mSpeed; set => _mSpeed = value; }

    private void Start()
    {
        _mSpeed = statistics.GetStatistic(StatisticsType.Speed);
    }
    void Update()
    {
        transform.Translate(Vector3.back * (Speed * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Missile"))
        {
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("EndEnemy"))
        {
            Destroy(gameObject); // L’ennemi disparaît
        }
    }
    protected override void OnDeath()
    {
        // ajouter le score au joueur (si présent)
        if (GameManager.instance != null && GameManager.instance._mPlayer != null)
        {
            GameManager.instance._mPlayer.AddScore(scoreValue);
            GameManager.instance._mPlayer.Weapon.WeaponLevelSystem.AddXp(expValue);
        }

        Destroy(gameObject);
    }
}
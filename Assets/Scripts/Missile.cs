using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] public float _mDamage = 10;
    [SerializeField] public bool _mCanBounce = false;
    [SerializeField] private float _mSpeed = 15f;

    //properties
    public float Damage { get => _mDamage; set => _mDamage = value; }
    public bool CanBounce { get => _mCanBounce; set => _mCanBounce = value; }
    public float Speed { get => _mSpeed; set => _mSpeed = value; }


    void Update()
    {
        transform.Translate(Vector3.up * (Speed * Time.deltaTime));
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var entity = other.GetComponent<Entity>();
            if (entity != null)
                entity.TakeDamage(Damage);
            Destroy(gameObject);
        }
        else if (other.CompareTag("End"))
        {
            Destroy(gameObject);

        }
        else if (CanBounce && other.CompareTag("Wall"))
        {
            Vector3 normal = (transform.position - other.ClosestPoint(transform.position)).normalized;
            Vector3 reflectDir = Vector3.Reflect(transform.up, normal);
            transform.up = reflectDir;
        }
    }
}
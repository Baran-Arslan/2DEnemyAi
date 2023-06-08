using UnityEngine;

public class Arrow : MonoBehaviour
{
    [HideInInspector] public EnemyHandler EnemyHandler;
    [SerializeField] private float rotationSpeed = 5f;

    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        _rigidbody.AddForce(EnemyHandler.ShootPoint.right * EnemyHandler.ShootForce, ForceMode2D.Impulse);
    }

    private void Update()
    {
        float currentAngle = transform.eulerAngles.z;

        if (Mathf.Abs(currentAngle - -80) > 0.1f)
        {
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, -80, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, newAngle);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8 || collision.gameObject.CompareTag("Waypoint")) return;
        collision.transform.TryGetComponent<IDamagable>(out IDamagable damageable);
        damageable?.Damage(EnemyHandler.AttackDamage);
        Destroy(gameObject);
    }
}

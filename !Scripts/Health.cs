using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private float maxHealth = 100;
    private float currentHealth;
    private bool _isDead = false;

    private Animator _animator;

    private void Awake()
    {
    }
    private void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }
    public void Damage(float damageAmount)
    {
        if (_isDead) return;
        currentHealth -= damageAmount;
        _animator.CrossFadeInFixedTime("Hit", 0.1f);

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (TryGetComponent<EnemyHandler>(out EnemyHandler enemyHandler))
        {
            Destroy(enemyHandler);
        }
        _animator.CrossFadeInFixedTime("Death", 0.1f);
        _isDead = true;
        Invoke("Disableasc", 1);
    }
    private void Disableasc()
    {
        this.gameObject.SetActive(false);

    }
}

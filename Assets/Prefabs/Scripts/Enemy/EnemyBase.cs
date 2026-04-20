using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int damage = 10;
    public Animator animator;
    public HealthBase healthBase;
    public float timeToDestroy = 1f;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int KillHash = Animator.StringToHash("Kill");

    public void Damage(int amount)
    {
        if (healthBase != null) healthBase.Damage(amount);
    }

    private void Awake()
    {
        if (healthBase != null) healthBase.OnKill += OnEnemyKill;
    }

    private void OnDisable()
    {
        if (healthBase != null) healthBase.OnKill -= OnEnemyKill;
    }

    private void OnEnemyKill()
    {
        animator.SetTrigger(KillHash);
        Destroy(gameObject, timeToDestroy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var health = collision.gameObject.GetComponent<HealthBase>();
        if (health != null)
        {
            health.Damage(damage);
            animator.SetTrigger(AttackHash);
        }
    }
}
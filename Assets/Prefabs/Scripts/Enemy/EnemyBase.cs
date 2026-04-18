using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Settings")]
    public int damage = 10;

    [Header("References")]
    public Animator animator;
    public string triggerAttack = "Attack";

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug para ver no console com o que o inimigo colidiu
        Debug.Log("Colisão detectada com: " + collision.gameObject.name);

        // Verifica se o objeto colidido tem a Tag "Player" (opcional, mas recomendado)
        if (collision.gameObject.CompareTag("Player"))
        {
            var health = collision.gameObject.GetComponent<HealthBase>();

            if (health != null)
            {
                health.Damage(damage);
                PlayAttackAnimation();
            }
        }
    }

    private void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerAttack);
        }
        else
        {
            Debug.LogWarning("Animator não atribuído no objeto: " + gameObject.name);
        }
    }
}
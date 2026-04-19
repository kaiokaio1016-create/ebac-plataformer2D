using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public Vector3 direction;
    public float timeToDestroy = 2f;
    public float side = 1;
    public int damageAmount = 1;

    private bool _isDestroyed = false; // Trava de segurança

    private void Awake()
    {
        // Em vez de Destroy direto, usamos uma função para garantir a limpeza
        Invoke(nameof(DestroySelf), timeToDestroy);
    }

    private void Update()
    {
        // Se já colidiu ou foi marcado para destruir, não faz mais nada
        if (_isDestroyed) return;

        transform.Translate(direction * Time.deltaTime * side);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se já estiver em processo de destruição, ignora novas colisões
        if (_isDestroyed) return;

        var enemy = collision.transform.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            enemy.Damage(damageAmount);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        if (_isDestroyed) return;

        _isDestroyed = true;
        Destroy(gameObject);
    }
}
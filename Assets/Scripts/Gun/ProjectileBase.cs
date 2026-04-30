using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public Vector3 direction;
    public float timeToDestroy = 2f;
    public float side = 1;

    public int damageAmount = 1;

    private void Awake()
    {
        // Destrói o projétil depois de X segundos
        Destroy(gameObject, timeToDestroy);
    }

    private void Update()
    {
        // Move o projétil
        transform.Translate(direction * Time.deltaTime * side);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Procura o componente EnemyBase no objeto que colidiu
        var enemy = collision.transform.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            // Aplica o dano
            enemy.Damage(damageAmount);
        }

        // Destrói o projétil ao tocar em algo
        Destroy(gameObject);
    }
}
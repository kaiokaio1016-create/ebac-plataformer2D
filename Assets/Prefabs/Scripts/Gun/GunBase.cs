using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBase : MonoBehaviour
{
    [Header("Settings")]
    public ProjectileBase prefabProjectile;
    public Transform positionToShoot;
    public float timeBetweenShoot = .3f;
    public Transform playerSideReference;

    private Coroutine _currentCoroutine;

    void Update()
    {
        // Atira enquanto segurar o S
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (_currentCoroutine == null)
            {
                _currentCoroutine = StartCoroutine(StartShoot());
            }
        }
        // Para de atirar quando soltar o S
        else if (Input.GetKeyUp(KeyCode.F))
        {
            if (_currentCoroutine != null)
            {
                StopCoroutine(_currentCoroutine);
                _currentCoroutine = null; // Limpa a referência para poder começar de novo
            }
        }
    }

    IEnumerator StartShoot()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(timeBetweenShoot);
        }
    }

    public void Shoot()
    {
        // Proteção: Só tenta atirar se o prefab existir
        if (prefabProjectile == null)
        {
            Debug.LogError("ERRO: O Prefab do Projétil não foi arrastado para o GunBase!");
            return;
        }

        var projectile = Instantiate(prefabProjectile);

        // Configura posição
        if (positionToShoot != null)
            projectile.transform.position = positionToShoot.position;

        // Configura lado (direção)
        if (playerSideReference != null)
            projectile.side = playerSideReference.transform.localScale.x;
    }
}
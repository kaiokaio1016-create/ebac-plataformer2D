using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    [Header("Health Settings")]
    public int startLife = 10;
    public bool destroyOnKill = false;
    public float delayToKill = 0f;

    private int _currentLife;
    private bool _isDead = false;

    [SerializeField] private FlashColor _flashColor;

    private void Awake()
    {
        Init();

        // Busca o componente FlashColor caso não tenha sido arrastado no Inspector
        if (_flashColor == null)
        {
            _flashColor = GetComponent<FlashColor>();
        }
    }

    private void Init()
    {
        _isDead = false;
        _currentLife = startLife;
    }

    public void Damage(int damage)
    {
        if (_isDead) return;

        _currentLife -= damage;

        if (_currentLife <= 0)
        {
            Kill();
        }

        // Executa o efeito visual se o componente existir
        if (_flashColor != null)
        {
            _flashColor.Flash();
        }
    }

    private void Kill()
    {
        _isDead = true;

        if (destroyOnKill)
        {
            Destroy(gameObject, delayToKill);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    public Rigidbody2D myRigidbody;

    [Header("Speed setup")]
    public Vector2 friction = new Vector2(.1f, 0);
    public float speed;
    public float speedRun;
    public float forceJump = 2;

    [Header("Animation setup")]
    public float jumpScaleY = 1.5f;
    public float jumpScaleX = 0.7f;
    public float animationDuration = .3f;
    public Ease ease = Ease.OutBack;

    // --- ADIÇÕES PARA A QUEDA ---
    [Header("Fall Animation setup")]
    [Tooltip("Quanto ele estica para baixo na queda (Y menor que 1, X maior que 1)")]
    public float fallScaleY = 0.8f;
    public float fallScaleX = 1.2f;
    public float fallAnimationDuration = 0.2f;
    private bool _isFalling = false; // Flag para controlar se já estamos na animação de queda
    // ----------------------------

    private float _currentSpeed;
    private bool _isRunning = false;

    private void Update()
    {
        HandleJump();
        HandleMovinent();
        HandleFall(); // --- ADIÇÃO ---
    }

    private void HandleMovinent()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            _currentSpeed = speedRun;
        }
        else
        {
            _currentSpeed = speed;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            myRigidbody.velocity = new Vector2(-_currentSpeed, myRigidbody.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            myRigidbody.velocity = new Vector2(_currentSpeed, myRigidbody.velocity.y);
        }

        if (myRigidbody.velocity.x > 0)
        {
            myRigidbody.velocity += friction;
        }
        else if (myRigidbody.velocity.x < 0)
        {
            myRigidbody.velocity -= friction;
        }
    }

    private void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myRigidbody.velocity = Vector2.up * forceJump;

            // Resetamos o estado de queda e matamos tweens antigos
            _isFalling = false;
            DOTween.Kill(myRigidbody.transform);
            myRigidbody.transform.localScale = Vector2.one;

            HandleScaleJump();
        }
    }

    private void HandleScaleJump()
    {
        // Animação de pulo (Y estica, X comprime)
        myRigidbody.transform.DOScaleY(jumpScaleY, animationDuration).SetLoops(2, LoopType.Yoyo).SetEase(ease);
        myRigidbody.transform.DOScaleX(jumpScaleX, animationDuration).SetLoops(2, LoopType.Yoyo).SetEase(ease);
    }

    // --- NOVO MÉTODO PARA GERENCIAR A ANIMAÇÃO DE QUEDA ---
    private void HandleFall()
    {
        // Se a velocidade vertical for negativa e ainda não marcamos como caindo
        if (myRigidbody.velocity.y < -0.1f && !_isFalling)
        {
            _isFalling = true;

            // Para qualquer animação atual (como a do pulo) antes de começar a da queda
            DOTween.Kill(myRigidbody.transform);

            // Animação de queda: "achata" no Y e "alarga" no X
            myRigidbody.transform.DOScaleY(fallScaleY, fallAnimationDuration).SetEase(Ease.OutQuad);
            myRigidbody.transform.DOScaleX(fallScaleX, fallAnimationDuration).SetEase(Ease.OutQuad);
        }
    }

    // --- DETECÇÃO DE IMPACTO PARA VOLTAR AO TAMANHO NORMAL ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Quando colide com algo (chão), assumimos que parou de cair
        if (_isFalling)
        {
            _isFalling = false;

            // Mata o tween de queda instantaneamente
            DOTween.Kill(myRigidbody.transform);

            // Opcional: Efeito de "splat" rápido ao tocar no chão antes de voltar ao normal
            // Se preferir que volte direto, use apenas: myRigidbody.transform.localScale = Vector2.one;
            Sequence landSequence = DOTween.Sequence();
            landSequence.Append(myRigidbody.transform.DOScaleY(0.9f, 0.05f)); // Achata bem rápido
            landSequence.Append(myRigidbody.transform.DOScaleY(1f, 0.1f));   // Volta ao normal

            // Faz o mesmo para o X para manter volume
            myRigidbody.transform.DOScaleX(1.1f, 0.05f);
            myRigidbody.transform.DOScaleX(1f, 0.1f).SetDelay(0.05f);
        }
    }
}
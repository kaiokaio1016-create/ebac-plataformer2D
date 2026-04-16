using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D myRigidbody;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Setup de Velocidade")]
    public float speed = 5f;
    public float speedRun = 9f;
    public float forceJump = 10f;
    public float groundCheckRadius = 0.2f;

    [Header("Double Tap (Correr)")]
    public float doubleTapTime = 0.3f; // Tempo máximo entre cliques
    private float _lastTapTime;
    private KeyCode _lastKey;
    private bool _isRunning;

    [Header("Setup de Giro (Flip)")]
    public float playerSwipeDuration = .1f;

    [Header("Animação")]
    public string boolRun = "Run";
    public string triggerJump = "Jump";

    private float _currentSpeed;
    private bool _isGrounded;
    private Vector3 _originalScale;
    private float _moveInput;

    private void Start()
    {
        _originalScale = transform.localScale;
    }

    private void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        HandleRunInput();

        _moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Jump();
        }

        UpdateAnimations();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void HandleRunInput()
    {
        // Detecta clique duplo na Seta Direita ou Esquerda
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            KeyCode currentKey = Input.GetKeyDown(KeyCode.RightArrow) ? KeyCode.RightArrow : KeyCode.LeftArrow;

            if (currentKey == _lastKey && Time.time - _lastTapTime < doubleTapTime)
            {
                _isRunning = true;
            }

            _lastTapTime = Time.time;
            _lastKey = currentKey;
        }

        // Se soltar as teclas ou parar de mover, para de correr
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            _isRunning = false;
        }
    }

    private void ApplyMovement()
    {
        // 1. CAPTURAR A DIREÇÃO (Input)
        // Retorna -1 para esquerda, 1 para direita e 0 para parado
        float moveInput = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1;

        // 2. DEFINIR A VELOCIDADE (Se está correndo ou andando)
        bool isRunning = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift);
        _currentSpeed = isRunning ? speedRun : speed;
        animator.speed = isRunning ? 1.5f : 1f;

        // 3. APLICAR A VELOCIDADE NO RIGIDBODY
        // Aqui garantimos que a velocidade X seja a direção * velocidade
        myRigidbody.velocity = new Vector2(moveInput * _currentSpeed, myRigidbody.velocity.y);

        // 4. LÓGICA DE GIRO (FLIP) E ANIMAÇÃO
        if (moveInput != 0) // Se o player está se movendo
        {
            // Gira o personagem usando DOTween baseado no sinal do input
            float targetScaleX = moveInput * _originalScale.x;

            if (transform.localScale.x != targetScaleX)
            {
                transform.DOScaleX(targetScaleX, playerSwipeDuration);
            }

            // Ativa animação de correr apenas se estiver no chão
            animator.SetBool(boolRun, _isGrounded);
        }
        else // Se estiver parado
        {
            animator.SetBool(boolRun, false);
            if (_isGrounded)
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            }
        }

        // Garante que a animação de Run pare se ele estiver no ar
        if (!_isGrounded) animator.SetBool(boolRun, false);
    }

    private void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, forceJump);
        animator.SetTrigger(triggerJump);
    }

    private void UpdateAnimations()
    {
        // A animação só ativa se houver input REAL e estiver no chão
        // Se _moveInput for 0, o SetBool será FALSE imediatamente
        bool walking = Mathf.Abs(_moveInput) > 0.1f;
        animator.SetBool(boolRun, walking && _isGrounded);

        // Opcional: Aumenta a velocidade da animação se estiver correndo
        animator.speed = _isRunning ? 1.5f : 1.0f;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
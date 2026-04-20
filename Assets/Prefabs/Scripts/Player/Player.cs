using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D myRigidbody;
    public HealthBase healthBase;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Setup de Velocidade")]
    public float speed = 5f;
    public float speedRun = 9f;
    public float forceJump = 10f;
    public float groundCheckRadius = 0.2f;

    [Header("Double Tap (Correr)")]
    public float doubleTapTime = 0.3f;
    private float _lastTapTime;
    private KeyCode _lastKey;
    private bool _isDoubleTapping;

    [Header("Setup de Giro (Flip)")]
    public float playerSwipeDuration = .1f;

    // Usando Hash para performance (evita strings no Update)
    private static readonly int RunHash = Animator.StringToHash("Run");
    private static readonly int JumpHash = Animator.StringToHash("Jump");
    private static readonly int DeathHash = Animator.StringToHash("Death");

    private float _currentSpeed;
    private bool _isGrounded;
    private Vector3 _originalScale;
    private float _moveInput;

    private void Awake()
    {
        if (healthBase != null) healthBase.OnKill += OnPlayerKill;
    }

    private void OnDisable() // Evita memory leaks
    {
        if (healthBase != null) healthBase.OnKill -= OnPlayerKill;
    }

    private void Start() => _originalScale = transform.localScale;

    private void Update()
    {
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        HandleRunInput();
        _moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded) Jump();

        UpdateAnimations();
    }

    private void FixedUpdate() => ApplyMovement();

    private void HandleRunInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            KeyCode currentKey = Input.GetKeyDown(KeyCode.RightArrow) ? KeyCode.RightArrow : KeyCode.LeftArrow;

            if (currentKey == _lastKey && Time.time - _lastTapTime < doubleTapTime)
                _isDoubleTapping = true;

            _lastTapTime = Time.time;
            _lastKey = currentKey;
        }

        if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) < 0.1f)
            _isDoubleTapping = false;
    }

    private void ApplyMovement()
    {
        // Verifica Shift OU Double Tap
        bool isRunning = _isDoubleTapping || Input.GetKey(KeyCode.LeftShift);
        _currentSpeed = isRunning ? speedRun : speed;

        myRigidbody.velocity = new Vector2(_moveInput * _currentSpeed, myRigidbody.velocity.y);

        if (_moveInput != 0)
        {
            float targetScaleX = _moveInput * _originalScale.x;
            if (transform.localScale.x != targetScaleX)
                transform.DOScaleX(targetScaleX, playerSwipeDuration);
        }
    }

    private void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, forceJump);
        animator.SetTrigger(JumpHash);
    }

    private void UpdateAnimations()
    {
        bool isMoving = Mathf.Abs(_moveInput) > 0.1f;
        animator.SetBool(RunHash, isMoving && _isGrounded);
        animator.speed = (_isDoubleTapping || Input.GetKey(KeyCode.LeftShift)) && isMoving ? 1.5f : 1.0f;
    }

    private void OnPlayerKill() => animator.SetTrigger(DeathHash);

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius); // "D" maiúsculo aqui!
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
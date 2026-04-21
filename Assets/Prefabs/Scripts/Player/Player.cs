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
    private bool _isRunning;

    [Header("Setup de Giro (Flip)")]
    public float playerSwipeDuration = .1f;

    [Header("Animação")]
    public string boolRun = "Run";
    public string triggerJump = "Jump";
    public string triggerDeath = "Death";

    private float _currentSpeed;
    private bool _isGrounded;
    private bool _isDead = false;
    private Vector3 _originalScale;
    private float _moveInput;
   

    private void Awake()
    {
        if (healthBase != null)
        {
            healthBase.OnKill += OnPlayerKill;
        }
    }


    private void OnPlayerKill()
    {
        _isDead = true; // Adicione esta linha
        animator.SetTrigger(triggerDeath);
    }

    private void Start()
    {
        _originalScale = transform.localScale;
    }

    private void Update()
    {

        if (_isDead) return; 

        _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        _moveInput = Input.GetAxisRaw("Horizontal");


        HandleRunInput();
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftControl)) _isRunning = true;


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

        if (_moveInput == 0)
        {
            _isRunning = false;
        }
    }

    private void ApplyMovement()
    {
        float moveInput = 0;
        if (Input.GetKey(KeyCode.LeftArrow)) moveInput = -1;
        else if (Input.GetKey(KeyCode.RightArrow)) moveInput = 1;

        bool isRunning = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift);
        _currentSpeed = isRunning ? speedRun : speed;

        myRigidbody.velocity = new Vector2(moveInput * _currentSpeed, myRigidbody.velocity.y);

        if (moveInput != 0)
        {
            float targetScaleX = moveInput * _originalScale.x;
            if (transform.localScale.x != targetScaleX)
            {
                transform.DOScaleX(targetScaleX, playerSwipeDuration);
            }
        }
        else
        {
            if (_isGrounded)
            {
                myRigidbody.velocity = new Vector2(0, myRigidbody.velocity.y);
            }
        }
    }


    private void Jump()
    {
        myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, forceJump);
        animator.SetTrigger(triggerJump);
    }

    private void UpdateAnimations()
    {

        bool isWalking = Mathf.Abs(_moveInput) > 0.1f;


        animator.SetBool(boolRun, isWalking && _isGrounded);


        animator.speed = (_isRunning && isWalking) ? 1.5f : 1.0f;
    }


    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }


    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
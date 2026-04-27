using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    [Header("Componentes")]
    public Rigidbody2D myRigidbody;
    public HealthBase healthBase;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Setup com ScriptableObject")]
    public SOPlayerSetup playerSetup;
    public SOFloat sogroundCheckRadius;

    private Vector3 _originalScale;
    private bool _isDead = false;
    private bool _isGrounded;
    private float _moveInput;
    private GameObject _currentPlayerObj;
    private Animator _animator;

    [Header("Jump Collision Check")]
    public Collider2D collider2D;
    public float distToGround;
    public float spaceToGround = 0.1f;
    public ParticleSystem jumpVFX;

    private void Awake()
    {
        _originalScale = transform.localScale;

        if (healthBase != null) healthBase.OnKill += OnPlayerKill;

        if (playerSetup != null && playerSetup.player != null)
        {
            
            _currentPlayerObj = Instantiate(playerSetup.player, transform).gameObject;

            
            _animator = _currentPlayerObj.GetComponent<Animator>();
        }

        if (collider2D != null)
        {
            distToGround = collider2D.bounds.extents.y;
        }


    }

    private bool IsGrounded()
    {
       
        return Physics2D.Raycast(transform.position, -Vector2.up, distToGround + spaceToGround);
    }

    private void Update()
    {
        if (_isDead) return;

        CheckGround();

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

    private void CheckGround()
    {
        if (groundCheck != null && sogroundCheckRadius != null)
        {
            _isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                sogroundCheckRadius.value,
                groundLayer
            );
        }
    }

    private void ApplyMovement()
    {
        if (playerSetup == null) return;

        myRigidbody.velocity = new Vector2(
            _moveInput * playerSetup.speed,
            myRigidbody.velocity.y
        );

        if (_moveInput != 0)
        {
            float targetScaleX = (_moveInput > 0) ? _originalScale.x : -_originalScale.x;

            transform.DOScaleX(targetScaleX, playerSetup.playerSwipeDuration);
        }
    }

    private void Jump()
    {
        // Só executa o que está abaixo se estiver no chão
        if (IsGrounded())
        {
            myRigidbody.velocity = new Vector2(
                myRigidbody.velocity.x,
                playerSetup.forceJump
            );

            HandleScaleJump();
            PlayJumpVFX();
        }
    }

    private void PlayJumpVFX()
    {
        VFXManager.Instance.PlayVFXByType(VFXManager.VFXType.JUMP, transform.position);
        //if (jumpVFX != null) jumpVFX.Play();
    }

    private void HandleScaleJump()
    {
        transform.DOScaleY(playerSetup.jumpScaleY, playerSetup.animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(playerSetup.ease);

        transform.DOScaleX(playerSetup.jumpScaleX, playerSetup.animationDuration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(playerSetup.ease);
    }

    private void UpdateAnimations()
    {
        if (playerSetup == null || _animator == null) return;

        _animator.SetBool(
            playerSetup.boolRun,
            Mathf.Abs(_moveInput) > 0.1f && _isGrounded
        );
    }

    private void OnPlayerKill()
    {
        _isDead = true;

        if (_animator != null)
        {
            _animator.SetTrigger(playerSetup.triggerDeath);
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null && sogroundCheckRadius != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                groundCheck.position,
                sogroundCheckRadius.value
            );
        }
    }

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
}
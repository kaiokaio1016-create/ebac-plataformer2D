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

    
    [Header("Fall Animation setup")]
    [Tooltip("Quanto ele estica para baixo na queda (Y menor que 1, X maior que 1)")]
    public float fallScaleY = 0.8f;
    public float fallScaleX = 1.2f;
    public float fallAnimationDuration = 0.2f;
    private bool _isFalling = false; 
    

    private float _currentSpeed;
    private bool _isRunning = false;

    private void Update()
    {
        HandleJump();
        HandleMovinent();
        HandleFall(); 
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

            
            _isFalling = false;
            DOTween.Kill(myRigidbody.transform);
            myRigidbody.transform.localScale = Vector2.one;

            HandleScaleJump();
        }
    }

    private void HandleScaleJump()
    {
        
        myRigidbody.transform.DOScaleY(jumpScaleY, animationDuration).SetLoops(2, LoopType.Yoyo).SetEase(ease);
        myRigidbody.transform.DOScaleX(jumpScaleX, animationDuration).SetLoops(2, LoopType.Yoyo).SetEase(ease);
    }

    
    private void HandleFall()
    {
        
        if (myRigidbody.velocity.y < -0.1f && !_isFalling)
        {
            _isFalling = true;

            
            DOTween.Kill(myRigidbody.transform);

            
            myRigidbody.transform.DOScaleY(fallScaleY, fallAnimationDuration).SetEase(Ease.OutQuad);
            myRigidbody.transform.DOScaleX(fallScaleX, fallAnimationDuration).SetEase(Ease.OutQuad);
        }
    }

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (_isFalling)
        {
            _isFalling = false;

            
            DOTween.Kill(myRigidbody.transform);

            
            
            Sequence landSequence = DOTween.Sequence();
            landSequence.Append(myRigidbody.transform.DOScaleY(0.9f, 0.05f)); 
            landSequence.Append(myRigidbody.transform.DOScaleY(1f, 0.1f));   

            
            myRigidbody.transform.DOScaleX(1.1f, 0.05f);
            myRigidbody.transform.DOScaleX(1f, 0.1f).SetDelay(0.05f);
        }
    }
}
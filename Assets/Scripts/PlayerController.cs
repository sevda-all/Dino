using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float JumpForce = 1000;
    [SerializeField] private int LifeAmount = 3;
    
    public static event Action<int> PlayerScoreChanged;
    public static event Action<int> PlayerLifeAmountChanged;
    
    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;

    private static readonly int Jump = Animator.StringToHash("IsJumping");
    private static readonly int Idle = Animator.StringToHash("IsIdle");
    private bool _isOnGround;
    
    private int _scoore;
    private int _currentLifeAmount;
    private bool isImmortal;
    private int _highScore;

    private float _spriteBlinkingTotalTimer;
    private float _spriteBlinkingTimer;
    private const float ImmortalTime = 1.5f;

    private void Awake()
    {
        if(!gameObject.TryGetComponent(out _animator))
        {
            Debug.Log("Animator component is not attached to player game object");
        }

        if (!gameObject.TryGetComponent(out _rigidbody2D))
        {
            Debug.Log("Rigidbody2D component is not attached to player game object");
        }

        gameObject.TryGetComponent(out _spriteRenderer);
        
        _currentLifeAmount = LifeAmount;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            _highScore = PlayerPrefs.GetInt("HighScore");
        }
    }

    void Start()
    {
        _animator.SetBool(Idle, true);
        EventManager.Instance.AddListener(EventType.GameRestart, OnGameRestarted);
    }

    private void OnGameRestarted(EventType eventType, Component sender, object param)
    {
        _scoore = 0;
        _currentLifeAmount = LifeAmount;
        PlayerScoreChanged?.Invoke(_scoore);
        PlayerLifeAmountChanged?.Invoke(_currentLifeAmount);
        _animator.ResetTrigger(Idle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!_isOnGround) 
                return;
            _rigidbody2D.AddForce(Vector2.up*JumpForce);
            _animator.SetBool(Jump, true);
        }
        
        if(isImmortal)
            StartBlinkingEffect();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Ground")) 
            return;
        _animator.ResetTrigger(Jump);
        _isOnGround = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.gameObject.tag)
        {
            case "Obstacle":
                if (!isImmortal)
                {
                    _currentLifeAmount--;
                    PlayerLifeAmountChanged?.Invoke(_currentLifeAmount);
                    if (_currentLifeAmount == 0)
                    {
                        if (_scoore > _highScore)
                        {
                            _highScore = _scoore;
                        }
                        PlayerPrefs.SetInt("HighScore", _highScore);
                        EventManager.Instance.PostNotification(EventType.PlayerDied, this, _highScore);
                        _animator.SetBool(Idle, true);
                    }

                    isImmortal = true;
                    StartCoroutine(ImmortalState());
                }
                break;
            case "Bonus":
                if (other.gameObject.TryGetComponent<Bonus>(out var bonus))
                {
                    _scoore += bonus.BonusPoints;
                    PlayerScoreChanged?.Invoke(_scoore);
                    other.gameObject.SetActive(false);
                }
                break;
                
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            _isOnGround = false;
        }
    }

    private IEnumerator ImmortalState()
    {
        yield return new WaitForSeconds(ImmortalTime);
        isImmortal = false;
    }

    private void StartBlinkingEffect()
    {
        _spriteBlinkingTotalTimer += Time.deltaTime;
        if (_spriteBlinkingTotalTimer >= ImmortalTime)
        {
            _spriteBlinkingTotalTimer = 0.0f;
            _spriteRenderer.enabled = true;
            return;
        }

        _spriteBlinkingTimer += Time.deltaTime;
        if (!(_spriteBlinkingTimer >= 0.1f)) 
            return;
        _spriteBlinkingTimer = 0.0f;
        _spriteRenderer.enabled = !_spriteRenderer.enabled;
    }
}

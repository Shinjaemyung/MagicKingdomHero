using Cinemachine;
using System;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public static Hero Instance { get; private set; }

    [SerializeField, Tooltip("최대 체력")]
    private float maxHealth = 100f;

    public float Health { get; private set; }
    public float MaxHealth => maxHealth;

    /// <summary>체력 변경 시 발생. (currentHealth, maxHealth)</summary>
    public event Action<float, float> OnHealthChanged;

    /// <summary>사망 시 발생</summary>
    public event Action OnDied;

    /// <summary>부활 시 발생</summary>
    public event Action OnRevived;

    /// <summary>활성화 시 발생</summary>
    public event Action OnActivated;

    /// <summary>비활성화 시 발생</summary>
    public event Action OnDeactivated;

    [SerializeField, Tooltip("피격 시 효과음")]
    private AudioClip hitClip;

    private AudioSource _audioSource;
    private CinemachineImpulseSource _impulseSource;

    public bool isDead;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Health = maxHealth;
        _audioSource = gameObject.GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void UpdateHealth(float amount)
    {
        Health = Mathf.Clamp(Health + amount, 0f, maxHealth);
        if (amount < 0f)
        {
            _audioSource?.PlayOneShot(hitClip);
            _impulseSource.GenerateImpulseWithForce(0.5f);
        }

        OnHealthChanged?.Invoke(Health, maxHealth);

        if (Health <= 0f)
            Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        OnDied?.Invoke();
    }

    public void Revive()
    {
        if (!isDead) return;
        isDead = false;
        OnRevived?.Invoke();
    }

    public void OnHeroActivated()
    {
        gameObject.SetActive(true);
        OnActivated?.Invoke();
    }

    public void OnHeroDeactivated()
    {
        gameObject.SetActive(false);
        OnDeactivated?.Invoke();
    }
}

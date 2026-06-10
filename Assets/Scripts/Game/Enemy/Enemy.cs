using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(UI_EnemyHealthBar))]
[RequireComponent(typeof(EnemyPoolable))]
public class Enemy : Targetable
{
    [SerializeField] private UI_EnemyHealthBar healthBar;

    [SerializeField, Tooltip("Hero와 충돌 시 주는 데미지 (임시)")]
    public float attackDamage = 10f;

    [SerializeField, Tooltip("사망 시 지급하는 골드 (임시)")]
    public int goldReward = 10;

    private EnemyPoolable _poolable;

    protected override void Awake()
    {
        base.Awake();
        if (healthBar == null)
            healthBar = GetComponent<UI_EnemyHealthBar>();
        _poolable = GetComponent<EnemyPoolable>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Hit += OnHit;
    }

    private void OnHit(HitInfo hitInfo)
    {
        if (healthBar == null) return;
        healthBar.ShowAndUpdate(configuration.NormalisedHealth);
    }

    public override void Remove()
    {
        base.Remove();

        ClearAllEvents();
        _poolable.ReturnToPool();
    }

    void OnDestroy()
    {
        ClearAllEvents();
    }
}

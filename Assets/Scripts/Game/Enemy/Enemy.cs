using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(UI_EnemyHealthBar))]
[RequireComponent(typeof(EnemyPoolable))]
public class Enemy : Targetable
{
    [SerializeField] private UI_EnemyHealthBar healthBar;

    public EnemyData enemyData;

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
        configuration.Initialize(enemyData);
        Hit += OnHit;
    }

    private void OnHit(HitInfo hitInfo)
    {
        if (healthBar == null) return;
        healthBar.ShowAndUpdate(configuration.NormalisedHealth);
    }

    /// <summary>
    /// 최대 체력을 재정의합니다. (무한 웨이브처럼 웨이브마다 체력이 달라지는 경우 사용)
    /// 스폰 직후, 아직 데미지를 받기 전에 호출되어야 합니다.
    /// </summary>
    public void SetMaxHealth(float maxHealth)
    {
        configuration.OverrideMaxHealth(maxHealth);
    }

    /// <summary>적 클릭 시 호출되어 정보 패널을 표시</summary>
    public void OnClicked()
    {
        GameUIManager.Instance.ShowEnemyInfo(this);
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

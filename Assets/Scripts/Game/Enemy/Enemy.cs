using ActionGameFramework.Health;
using Core.Health;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyMover))]
[RequireComponent(typeof(UI_EnemyHealthBar))]
[RequireComponent(typeof(EnemyPoolable))]
public class Enemy : Targetable
{
    public EnemyData enemyData;

    private readonly List<StatusEffect> statusEffects = new();

    private UI_EnemyHealthBar healthBar;

    private EnemyPoolable _poolable;

    protected override void Awake()
    {
        base.Awake();
        healthBar = GetComponent<UI_EnemyHealthBar>();
        _poolable = GetComponent<EnemyPoolable>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        configuration.Initialize(enemyData);
        Hit += OnHit;
        statusEffects.Clear();
    }

    private void Update()
    {
        for (int i = statusEffects.Count - 1; i >= 0; i--)
        {
            statusEffects[i].Tick(this, Time.deltaTime);

            if (statusEffects[i].IsFinished)
            {
                statusEffects[i].OnRemove(this);
                statusEffects.RemoveAt(i);
            }
        }
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

    /// <summary>상태 이상 적용</summary>
    public void ApplyStatus(StatusEffect effect)
    {
        if (effect.IsStackable)
        {
            statusEffects.Add(effect);
        }
        else
        {
            StatusEffect existing = statusEffects.Find(x => x.GetType() == effect.GetType());

            if (existing == null)
                statusEffects.Add(effect);
        }
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

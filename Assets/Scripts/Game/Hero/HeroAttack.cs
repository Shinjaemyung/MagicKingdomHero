using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

/// <summary>
/// HeroControlMode에서 좌클릭 시 great sword slash 공격을 수행하는 컴포넌트.
/// 공격 범위 내 Enemy에게 데미지를 적용한다.
/// </summary>
[RequireComponent(typeof(Hero))]
[RequireComponent(typeof(Animator))]
public class HeroAttack : MonoBehaviour
{
    [SerializeField, Tooltip("공격 데미지")]
    private float attackDamage = 2f;

    [SerializeField, Tooltip("공격 판정 범위 (전방 구체 거리)")]
    private float attackRange = 2.5f;

    [SerializeField, Tooltip("공격 판정 범위 (전방 부채꼴 각도, 도)")]
    private float attackAngle = 90f;

    [SerializeField, Tooltip("공격 쿨다운(초). 애니메이션 길이와 맞춰서 연타를 막는다.")]
    private float attackCooldown = 0.8f;

    [SerializeField, Tooltip("공격 판정이 들어가는 시점 (애니메이션 진행 비율, 0~1)")]
    private float hitTimingNormalized = 0.4f;

    [SerializeField, Tooltip("Enemy 레이어")]
    private LayerMask enemyLayer;

    private Animator _animator;
    private StarterAssetsInputs _input;
    private SwordSlashFX _swordSlashFX;

    private float _cooldownTimer;
    private bool _isAttacking;

    private static readonly int AttackHash = Animator.StringToHash("Attack");

private void Awake()
    {
        _animator = GetComponent<Animator>();
        _input = GetComponent<StarterAssetsInputs>();
        _swordSlashFX = GetComponentInChildren<SwordSlashFX>(true);
    }

    private void Update()
    {
        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;

        // 공격 중에는 이동 입력을 막아 그 자리에서 슬래시를 수행하게 한다.
        if (_isAttacking && _input != null)
            _input.move = Vector2.zero;
    }

    /// <summary>공격 처리</summary>
    public void TryAttack()
    {
        if (_isAttacking || _cooldownTimer > 0f) return;

        _isAttacking = true;
        _cooldownTimer = attackCooldown;
        _animator.ResetTrigger(AttackHash);
        _animator.SetTrigger(AttackHash);

        StartCoroutine(PerformAttack());
    }

private IEnumerator PerformAttack()
    {
        // 애니메이션 클립 길이를 기준으로 타격 타이밍을 계산
        float clipLength = GetAttackClipLength();
        float hitDelay = clipLength * hitTimingNormalized;

        yield return new WaitForSeconds(hitDelay);
        _swordSlashFX?.PlaySlash();
        ApplyDamageToEnemiesInRange();

        yield return new WaitForSeconds(Mathf.Max(0f, clipLength - hitDelay));
        _isAttacking = false;
    }

    private float GetAttackClipLength()
    {
        var clips = _animator.runtimeAnimatorController.animationClips;
        foreach (var c in clips)
        {
            if (c != null && c.name.ToLower().Contains("mixamo"))
                return c.length;
        }
        return 1f;
    }

    /// <summary>전방 부채꼴 범위 내 Enemy 전체에게 데미지 적용</summary>
    private void ApplyDamageToEnemiesInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, enemyLayer);
        foreach (var hit in hits)
        {
            var enemy = hit.GetComponentInParent<Enemy>();
            if (enemy == null || enemy.IsDead) continue;

            Vector3 toEnemy = enemy.Position - transform.position;
            toEnemy.y = 0f;
            if (toEnemy.sqrMagnitude < 0.0001f)
            {
                DealDamage(enemy);
                continue;
            }

            float angle = Vector3.Angle(transform.forward, toEnemy);
            if (angle <= attackAngle * 0.5f)
                DealDamage(enemy);
        }
    }

    private void DealDamage(Enemy enemy)
    {
        enemy.TakeDamage(attackDamage, enemy.Position, null);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif
}

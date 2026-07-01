using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적(Enemy) 클릭 시 표시되는 정보 패널.
/// 적 이름, 체력, 공격력, 골드 보상을 표시.
/// </summary>
public class UI_EnemyInfoPanel : UI_Panel
{
    [SerializeField, Tooltip("적 이름 텍스트")]
    private TextMeshProUGUI enemyNameText;

    [SerializeField, Tooltip("체력 텍스트 (현재/최대)")]
    private TextMeshProUGUI healthText;

    [SerializeField, Tooltip("Normal 타입 데미지 계산")]
    private TextMeshProUGUI DamageTypeCalculationText_Normal;

    [SerializeField, Tooltip("Water 타입 데미지 계산")]
    private TextMeshProUGUI DamageTypeCalculationText_Water;

    [SerializeField, Tooltip("Fire 타입 데미지 계산")]
    private TextMeshProUGUI DamageTypeCalculationText_Fire;

    [SerializeField, Tooltip("Electro 타입 데미지 계산")]
    private TextMeshProUGUI DamageTypeCalculationText_Electro;

    [SerializeField, Tooltip("Ice 타입 데미지 계산")]
    private TextMeshProUGUI DamageTypeCalculationText_Ice;

    private Enemy _currentEnemy;

    /// <summary>적 정보를 받아 패널을 업데이트하고 표시</summary>
    public void ShowEnemyInfo(Enemy enemy)
    {
        Unsubscribe();

        _currentEnemy = enemy;
        _currentEnemy.Died += OnCurrentEnemyDied;

        var data = enemy.enemyData;

        if (enemyNameText != null)
            enemyNameText.text = data.enemyName;

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(enemy.configuration.CurrentHealth) + " / " + Mathf.CeilToInt(enemy.configuration.MaxHealth);

        SetDamageTypeText(DamageTypeCalculationText_Normal,  DamageType.Normal,  enemy);
        SetDamageTypeText(DamageTypeCalculationText_Water,   DamageType.Water,   enemy);
        SetDamageTypeText(DamageTypeCalculationText_Fire,    DamageType.Fire,    enemy);
        SetDamageTypeText(DamageTypeCalculationText_Electro, DamageType.Electro, enemy);
        SetDamageTypeText(DamageTypeCalculationText_Ice,     DamageType.Ice,     enemy);

        Show();
    }

    /// <summary>해당 DamageType의 배율을 찾아 텍스트에 표시. 등록된 항목이 없으면 x1.0으로 표시</summary>
    private void SetDamageTypeText(TextMeshProUGUI label, DamageType damageType, Enemy enemy)
    {
        if (label == null) return;

        float multiplier = 1f;
        var calcs = enemy.configuration.typeCalculations;
        if (calcs != null)
        {
            foreach (var calc in calcs)
            {
                if (calc.damageType == damageType)
                {
                    multiplier = calc.multiplier;
                    break;
                }
            }
        }

        label.text = "x" + multiplier.ToString("0.0");
    }

    private void Update()
    {
        // 패널이 켜져 있는 동안 체력 변화를 실시간으로 갱신
        if (_currentEnemy == null) return;

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(_currentEnemy.configuration.CurrentHealth) + " / " + Mathf.CeilToInt(_currentEnemy.configuration.MaxHealth);
    }

    /// <summary>현재 표시 중인 적이 사망했을 때 패널을 닫음</summary>
    private void OnCurrentEnemyDied(Core.Health.DamageableBehaviour deadEnemy)
    {
        Hide();
    }

    private void Unsubscribe()
    {
        if (_currentEnemy != null)
            _currentEnemy.Died -= OnCurrentEnemyDied;
    }

    public override void Hide()
    {
        Unsubscribe();
        _currentEnemy = null;
        base.Hide();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }
}

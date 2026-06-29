using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 적(Enemy) 클릭 시 표시되는 정보 패널.
/// 적 이름, 체력, 공격력, 골드 보상을 표시.
/// </summary>
public class UI_EnemyInfoPanel : UI_Panel
{
    [SerializeField, Tooltip("적 이름 텍스트")]
    private Text enemyNameText;

    [SerializeField, Tooltip("체력 텍스트 (현재/최대)")]
    private Text healthText;

    private Enemy _currentEnemy;

    /// <summary>적 정보를 받아 패널을 업데이트하고 표시</summary>
    public void ShowEnemyInfo(Enemy enemy)
    {
        Unsubscribe();

        _currentEnemy = enemy;
        _currentEnemy.Died += OnCurrentEnemyDied;

        var data = enemy.enemyData;

        if (enemyNameText != null)
            enemyNameText.text = data != null ? data.enemyName : enemy.name;

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(enemy.configuration.CurrentHealth) + " / " + Mathf.CeilToInt(enemy.configuration.maxHealth);

        Show();
    }

    private void Update()
    {
        // 패널이 켜져 있는 동안 체력 변화를 실시간으로 갱신
        if (_currentEnemy == null) return;

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(_currentEnemy.configuration.CurrentHealth) + " / " + Mathf.CeilToInt(_currentEnemy.configuration.maxHealth);
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

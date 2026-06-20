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

    [SerializeField, Tooltip("공격력 텍스트")]
    private Text attackDamageText;

    [SerializeField, Tooltip("골드 보상 텍스트")]
    private Text goldRewardText;

    private Enemy _currentEnemy;

    /// <summary>적 정보를 받아 패널을 업데이트하고 표시</summary>
    public void ShowEnemyInfo(Enemy enemy)
    {
        _currentEnemy = enemy;

        var data = enemy.enemyData;

        if (enemyNameText != null)
            enemyNameText.text = data != null ? data.enemyName : enemy.name;

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(enemy.configuration.CurrentHealth) + " / " + Mathf.CeilToInt(enemy.configuration.maxHealth);

        if (attackDamageText != null)
            attackDamageText.text = "ATK " + (data != null ? data.attackDamage : 0);

        if (goldRewardText != null)
            goldRewardText.text = (data != null ? data.goldReward : 0) + "G";

        Show();
    }

    private void Update()
    {
        // 패널이 켜져 있는 동안 체력 변화를 실시간으로 갱신
        if (_currentEnemy == null) return;

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(_currentEnemy.configuration.CurrentHealth) + " / " + Mathf.CeilToInt(_currentEnemy.configuration.maxHealth);
    }

    public override void Hide()
    {
        _currentEnemy = null;
        base.Hide();
    }
}

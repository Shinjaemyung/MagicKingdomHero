using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UI_SystemMessagePanel;

/// <summary>
/// 타워 클릭 시 표시되는 정보 패널.
/// 타워 이름, 설명, 썸네일, 판매/업그레이드 버튼을 관리.
/// </summary>
public class UI_TowerInfoPanel : UI_Panel
{
    [SerializeField, Tooltip("데미지 타입 이미지")]
    private Image damageTypeIcon;

    [SerializeField, Tooltip("타워 이름 텍스트")]
    private Text towerNameText;

    [SerializeField, Tooltip("데미지 텍스트")]
    private Text damageText;

    [SerializeField, Tooltip("사거리 텍스트")]
    private Text attackRangeText;

    [SerializeField, Tooltip("공격 속도 텍스트")]
    private Text attackSpeedText;

    [SerializeField, Tooltip("업그레이드 버튼들이 생성될 부모(Layout)")]
    private Transform upgradeButtonContainer;

    [SerializeField, Tooltip("판매 버튼")]
    private Button sellButton;

    [SerializeField, Tooltip("판매 버튼 텍스트")]
    private Text sellButtonText;

    [SerializeField, Tooltip("업그레이드 버튼 프리팹")]
    private UI_TowerUpgradeButton upgradeButtonPrefab;

    private Tower _currentTower;
    private readonly List<UI_TowerUpgradeButton> _upgradeButtons = new List<UI_TowerUpgradeButton>();

    UI_SystemMessagePanel systemMessage;

    private void Awake()
    {
        systemMessage = FindAnyObjectByType<UI_SystemMessagePanel>();
    }

    /// <summary>타워 정보를 받아 패널을 업데이트하고 표시</summary>
    public void ShowTowerInfo(Tower tower)
    {
        _currentTower = tower;

        var data = tower.towerData;

        if (towerNameText != null)
            towerNameText.text = data.towerName;

        if (damageTypeIcon != null)
            damageTypeIcon.sprite = data.damageType.GetIcon();

        if (damageText != null)
            damageText.text = data.damage.ToString();

        if (attackRangeText != null)
            attackRangeText.text = data.attackRange.ToString();

        if (attackSpeedText != null)
            attackSpeedText.text = data.attackSpeed.ToString();

        // 판매 버튼: 판매 금액 표시
        if (sellButtonText != null)
            sellButtonText.text = "Sell (" + Mathf.RoundToInt(tower.TotalCost * tower.refundRatio) + "G)";

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnSellClicked);

        BuildUpgradeButtons(data);

        Show();
    }

    /// <summary>data.upgradeTowers 개수만큼 업그레이드 버튼을 생성</summary>
    private void BuildUpgradeButtons(TowerData data)
    {
        ClearUpgradeButtons();

        if (upgradeButtonContainer == null || upgradeButtonPrefab == null)
            return;

        if (data.upgradeTowers == null || data.upgradeTowers.Length == 0)
            return;

        for (int i = 0; i < data.upgradeTowers.Length; i++)
        {
            Tower upgradeTower = data.upgradeTowers[i];
            if (upgradeTower == null) continue;

            var buttonInstance = Instantiate(upgradeButtonPrefab, upgradeButtonContainer);
            buttonInstance.Setup(upgradeTower.towerData.towerName, () => OnUpgradeClicked(upgradeTower));
            _upgradeButtons.Add(buttonInstance);
        }
    }

    private void ClearUpgradeButtons()
    {
        for (int i = 0; i < _upgradeButtons.Count; i++)
        {
            if (_upgradeButtons[i] != null)
                Destroy(_upgradeButtons[i].gameObject);
        }
        _upgradeButtons.Clear();
    }

    public override void Hide()
    {
        _currentTower = null;
        ClearUpgradeButtons();
        base.Hide();
    }

    private void OnSellClicked()
    {
        if (_currentTower == null) return;
        _currentTower.Sell();
        Hide();
    }

    private void OnUpgradeClicked(Tower upgradeTower)
    {
        if (_currentTower == null || upgradeTower == null) return;

        var currentTowerData = _currentTower.towerData;
        if (GamePlayManager.Instance.PlayerGold >= currentTowerData.cost)
        {
            var upgradedTower = _currentTower.UpgradeTower(upgradeTower);
            ShowTowerInfo(upgradedTower);
        }
        else
        {
            systemMessage.ActivateMessage(SystemMessageType.NotEnoughGold);
        }
    }
}

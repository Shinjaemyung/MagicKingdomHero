using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UI_SystemMessagePanel;

/// <summary>
/// 타워 클릭 시 표시되는 정보 패널.
/// 타워 이름, 설명, 썸네일, 판매/업그레이드 버튼을 관리.
/// </summary>
public class UI_TowerInfoPanel : UI_Panel
{
    [SerializeField, Tooltip("타워 썸네일 이미지")]
    private Image thumbnailImage;

    [SerializeField, Tooltip("타워 이름 텍스트")]
    private Text towerNameText;

    [SerializeField, Tooltip("타워 설명 텍스트")]
    private Text descriptionText;

    [SerializeField, Tooltip("판매 버튼")]
    private Button sellButton;

    [SerializeField, Tooltip("업그레이드 버튼")]
    private Button upgradeButton;

    [SerializeField, Tooltip("판매 버튼 텍스트")]
    private Text sellButtonText;

    private Tower _currentTower;
    private RectTransform _rectTransform;
    private Canvas _parentCanvas;

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

        if (thumbnailImage != null)
            thumbnailImage.sprite = data.thumbnail;

        if (towerNameText != null)
            towerNameText.text = data.description;

        if (descriptionText != null)
            descriptionText.text = data.upgradeDescription;

        // 판매 버튼: 판매 금액 표시 (구매가의 75%)
        if (sellButtonText != null)
            sellButtonText.text = "Sell (" + Mathf.RoundToInt(tower.TotalCost * 0.75f) + "G)";

        // 업그레이드 가능 여부에 따라 버튼 활성화
        if (upgradeButton != null)
            upgradeButton.gameObject.SetActive(data.upgradeTowers != null && data.upgradeTowers.Length > 0);

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(OnSellClicked);

        upgradeButton.onClick.RemoveAllListeners();
        upgradeButton.onClick.AddListener(OnUpgradeClicked);

        //SetPositionToTower(tower);
        Show();
    }
    /*
    private void SetPositionToTower(Tower tower)
    {
        if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();

        Vector2 screenPos = Camera.main.WorldToScreenPoint(tower.transform.position);

        // 화면 하단 절반이면 패널이 위로, 상단 절반이면 아래로 표시
        float pivotY = screenPos.y < Screen.height * 0.5f ? 0f : 1f;
        _rectTransform.pivot = new Vector2(0f, pivotY);
        _rectTransform.anchorMin = Vector2.zero;
        _rectTransform.anchorMax = Vector2.zero;
        _rectTransform.anchoredPosition = screenPos;
    }
    */

    public override void Hide()
    {
        _currentTower = null;
        base.Hide();
    }

    private void OnSellClicked()
    {
        if (_currentTower == null) return;
        _currentTower.Sell();
        Hide();
    }

    private void OnUpgradeClicked()
    {
        if (_currentTower == null) return;

        var currentTowerData = _currentTower.towerData;
        if (GamePlayManager.Instance.PlayerGold >= currentTowerData.cost)
        {
            if (currentTowerData.upgradeTowers != null && currentTowerData.upgradeTowers.Length > 0)
            {
                var upgradedTower = _currentTower.UpgradeTower(currentTowerData.upgradeTowers[0]);
                ShowTowerInfo(upgradedTower);
            }
        }
        else
        {
            systemMessage.ActivateMessage(SystemMessageType.NotEnoughGold);
        }
    }
}

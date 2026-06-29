using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 업그레이드 가능한 타워 하나를 나타내는 버튼.
/// UI_TowerInfoPanel에서 data.upgradeTowers 개수만큼 동적으로 생성된다.
/// </summary>
public class UI_TowerUpgradeButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Text _buttonText;


    /// <summary>버튼 텍스트와 클릭 콜백 설정</summary>
    public void Setup(string upgradeTowerName, Action onClick)
    {
        SetText(upgradeTowerName);

        _button.onClick.RemoveAllListeners();
        if (onClick != null)
            _button.onClick.AddListener(() => onClick());
    }

    public void SetText(string upgradeTowerName)
    {
        if (_buttonText != null)
            _buttonText.text = upgradeTowerName;
    }
}

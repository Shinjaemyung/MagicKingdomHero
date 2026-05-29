using UnityEngine;
using UnityEngine.UI;
using static PlayerModeManager;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    UI_TowerList towerList;
    UI_TowerInfo towerInfoPanel;
    UI_ModeChangeButton modeChangeButton;
    UI_GameOverPanel gameOverPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        towerList = GetComponentInChildren<UI_TowerList>(true);
        towerInfoPanel = GetComponentInChildren<UI_TowerInfo>(true);
        modeChangeButton = GetComponentInChildren<UI_ModeChangeButton>(true);
        gameOverPanel = GetComponentInChildren<UI_GameOverPanel>(true);

        ShowTowerList();
    }

    public void SetTowerPlacementModeUI()
    {
        towerList.Show();
        modeChangeButton.ChangeButtonText(PlayerMode.TowerPlacementMode);
    }

    public void SetHeroControlModeUI()
    {
        towerList.Hide();
        towerInfoPanel.Hide();
        modeChangeButton.ChangeButtonText(PlayerMode.HeroControlMode);
    }

    /// <summary>타워 정보 패널 표시. TowerList를 숨기고 타워 정보 패널을 표시.</summary>
    public void ShowTowerInfo(Tower tower)
    {
        if (towerInfoPanel == null) return;
        towerList.Hide();
        towerInfoPanel.Show(tower);
    }

    /// <summary>타워 정보 패널 숨김.</summary>
    public void ShowTowerList()
    {
        if (towerInfoPanel == null) return;
        towerInfoPanel.Hide();
        towerList.Show();
    }

    /// <summary>게임 오버 패널 표시</summary>
    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;
        gameOverPanel.transform.SetAsLastSibling();
        gameOverPanel.Show();
    }
}

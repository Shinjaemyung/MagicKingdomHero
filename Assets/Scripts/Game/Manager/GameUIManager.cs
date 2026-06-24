using UnityEngine;
using UnityEngine.UI;
using static PlayerModeManager;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    UI_TowerListPanel towerList;
    UI_TowerInfoPanel towerInfoPanel;
    UI_EnemyInfoPanel enemyInfoPanel;
    UI_ModeChangeButton modeChangeButton;
    UI_GameOverPanel gameOverPanel;
    UI_HeroInfoPanel heroInfoPanel;
    UI_SettingsPanel settingsPanel;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        towerList = GetComponentInChildren<UI_TowerListPanel>(true);
        towerInfoPanel = GetComponentInChildren<UI_TowerInfoPanel>(true);
        enemyInfoPanel = GetComponentInChildren<UI_EnemyInfoPanel>(true);
        modeChangeButton = GetComponentInChildren<UI_ModeChangeButton>(true);
        gameOverPanel = GetComponentInChildren<UI_GameOverPanel>(true);
        heroInfoPanel = GetComponentInChildren<UI_HeroInfoPanel>(true);
        settingsPanel = GetComponentInChildren<UI_SettingsPanel>(true);

        ShowTowerList();
    }

    private void Start()
    {
        if (Hero.Instance != null)
        {
            Hero.Instance.OnDied += OnHeroDied;
            Hero.Instance.OnRevived += OnHeroRevived;
        }  
    }

    private void OnHeroDied()
    {
        modeChangeButton.SetButtonInteractable(false);
    }

    private void OnHeroRevived()
    {
        modeChangeButton.SetButtonInteractable(true);
    }

    public void BeginTowerPlacementMode()
    {
        modeChangeButton.SetButtonInteractable(false);
    }

    public void CompleteTowerPlacementMode()
    {
        towerList.Show();
        modeChangeButton.ChangeButtonText(PlayerMode.TowerPlacementMode);
        modeChangeButton.SetButtonInteractable(true);
    }

    public void BeginHeroControlMode()
    {
        modeChangeButton.SetButtonInteractable(false);
        towerList.Hide();
        towerInfoPanel.Hide();
        enemyInfoPanel.Hide();
    }

    public void CompleteHeroControlMode()
    {
        heroInfoPanel.Show();
        modeChangeButton.ChangeButtonText(PlayerMode.HeroControlMode);
        modeChangeButton.SetButtonInteractable(true);
    }

    /// <summary>타워 정보 패널 표시. TowerList를 숨기고 타워 정보 패널을 표시.</summary>
    public void ShowTowerInfo(Tower tower)
    {
        if (towerInfoPanel == null) return;
        towerList.Hide();
        towerInfoPanel.ShowTowerInfo(tower);
    }

    /// <summary>타워 정보 패널 숨김.</summary>
    public void ShowTowerList()
    {
        if (towerInfoPanel == null) return;
        towerInfoPanel.Hide();
        if (enemyInfoPanel != null) enemyInfoPanel.Hide();
        towerList.Show();
    }

    /// <summary>적 정보 패널 표시. 타워 정보 패널과 타워 리스트를 숨기고 적 정보 패널을 표시.</summary>
    public void ShowEnemyInfo(Enemy enemy)
    {
        if (enemyInfoPanel == null) return;
        towerList.Hide();
        if (towerInfoPanel != null) towerInfoPanel.Hide();
        enemyInfoPanel.ShowEnemyInfo(enemy);
    }

    /// <summary>설정 패널을 열고 게임 일시정지.</summary>
    public void ShowSettings()
    {
        if (settingsPanel == null || IsPaused) return;

        IsPaused = true;
        GamePlayManager.Instance.PauseGame();
        MouseManager.Instance.SetCursorLockState(false);
        settingsPanel.transform.SetAsLastSibling();
        settingsPanel.Show();
    }

    /// <summary>설정 패널을 닫고 게임 진행.</summary>
    public void HideSettings()
    {
        if (settingsPanel == null || !IsPaused) return;

        IsPaused = false;
        GamePlayManager.Instance.ResumeGame();
        if (PlayerModeManager.Instance.playerMode == PlayerMode.HeroControlMode)
            MouseManager.Instance.SetCursorLockState(true);
        settingsPanel.Hide();
    }

    /// <summary>설정 패널 표시 상태를 토글</summary>
    public void ToggleSettings()
    {
        if (IsPaused) HideSettings();
        else ShowSettings();
    }

    /// <summary>게임 오버 패널 표시</summary>
    public void ShowGameOver()
    {
        if (gameOverPanel == null) return;
        gameOverPanel.transform.SetAsLastSibling();
        gameOverPanel.Show();
    }

    private void OnDestroy()
    {
        if (Hero.Instance != null)
        {
            Hero.Instance.OnDied -= OnHeroDied;
            Hero.Instance.OnRevived -= OnHeroRevived;
        }
    }
}

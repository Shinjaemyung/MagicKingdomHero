using UnityEngine;
using UnityEngine.UI;
using static PlayerModeManager;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager Instance { get; private set; }

    UI_TowerListPanel towerList;
    UI_TowerInfoPanel towerInfoPanel;
    UI_ModeChangeButton modeChangeButton;
    UI_GameOverPanel gameOverPanel;
    UI_HeroInfoPanel heroInfoPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        towerList = GetComponentInChildren<UI_TowerListPanel>(true);
        towerInfoPanel = GetComponentInChildren<UI_TowerInfoPanel>(true);
        modeChangeButton = GetComponentInChildren<UI_ModeChangeButton>(true);
        gameOverPanel = GetComponentInChildren<UI_GameOverPanel>(true);
        heroInfoPanel = GetComponentInChildren<UI_HeroInfoPanel>(true);

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
        heroInfoPanel.Hide();
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
        towerList.Show();
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

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TowerDefense.UI;
using static PlayerModeManager;

public class UserInputManager : MonoBehaviour
{
    public static UserInputManager Instance { get; private set; }

    public LayerMask towerLayer;
    public LayerMask enemyLayer;

    public event Action OnLeftMouseReleased;
    public event Action OnRightMouseReleased;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePlayManager playManager = GamePlayManager.Instance;

            if (playManager.IsPaused)
            {
                playManager.ResumeGame();
            }
            else
            {
                playManager.PauseGame();
            }
        }

        if (GamePlayManager.Instance.IsPaused) return;

        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayerModeManager.Instance.RequestPlayerModeChange();
        }

        switch (PlayerModeManager.Instance.playerMode)
        {
            case PlayerMode.TowerPlacementMode:
                HandleTowerPlacementInput();
                break;

            case PlayerMode.HeroControlMode:
                HandleHeroControlInput();
                break;
        }
    }

    void HandleHeroControlInput()
    {
        if (CameraManager.Instance.IsBlending)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Hero.Instance.GetComponent<HeroAttack>()?.TryAttack();
        }
    }

    void HandleTowerPlacementInput()
    {
        if (CameraManager.Instance.IsBlending)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            OnLeftMouseButtonDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnLeftMouseButtonUp();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnRightMouseButtonDown();
        }

        if (Input.GetMouseButtonUp(1))
        {
            OnRightMouseButtonUp();
        }
    }

    void OnLeftMouseButtonDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit towerHit, float.MaxValue, towerLayer))
        {
            var clickedTower = towerHit.collider.GetComponent<Tower>();
            if (clickedTower != null)
            {
                clickedTower.OnClicked();
                RadiusVisualizerController.Instance.SetupRadiusVisualizers(clickedTower);
            }
            return;
        }

        if (Physics.Raycast(ray, out RaycastHit enemyHit, float.MaxValue, enemyLayer))
        {
            var clickedEnemy = enemyHit.collider.GetComponent<Enemy>();
            if (clickedEnemy != null)
            {
                clickedEnemy.OnClicked();
                RadiusVisualizerController.Instance.HideRadiusVisualizers();
            }
            return;
        }

        GameUIManager.Instance.ShowTowerList();
        RadiusVisualizerController.Instance.HideRadiusVisualizers();
    }

    void OnLeftMouseButtonUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        OnLeftMouseReleased?.Invoke();
    }

    void OnRightMouseButtonDown()
    {

    }

    void OnRightMouseButtonUp()
    {
        OnRightMouseReleased?.Invoke();
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static PlayerModeManager;

public class UI_ModeChangeButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClickButton);
    }

    void OnClickButton()
    {
        PlayerModeManager.Instance.RequestPlayerModeChange();
    }

    public void ChangeButtonText(PlayerMode mode)
    {
        switch (mode)
        {
            case PlayerMode.TowerPlacementMode:
                break;
            case PlayerMode.HeroControlMode:
                break;
        }
    }

    public void SetButtonInteractable(bool isInteractable)
    {
        _button.interactable = isInteractable;
    }
}

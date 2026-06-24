using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SettingsButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnClickButton);
    }

    void OnClickButton()
    {
        GamePlayManager.Instance.PauseGame();
    }
}

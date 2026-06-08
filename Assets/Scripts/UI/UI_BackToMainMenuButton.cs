using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_BackToMainMenuButton : MonoBehaviour
{
    Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadMainMenuScene);
    }

    /// <summary>로비 씬 로드</summary>
    public void LoadMainMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}

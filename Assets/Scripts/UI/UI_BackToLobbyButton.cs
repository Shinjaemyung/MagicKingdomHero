using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_BackToLobbyButton : MonoBehaviour
{
    Button _button;

    private void OnEnable()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(LoadLobbyScene);
    }

    /// <summary>로비 씬 로드</summary>
    public void LoadLobbyScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("LobbyScene");
    }
}

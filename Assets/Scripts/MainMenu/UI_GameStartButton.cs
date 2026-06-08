using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GameStartButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnStartGameClicked);
    }

    /// <summary>게임 씬 로드</summary>
    public void OnStartGameClicked()
    {
        SceneManager.LoadScene("GameScene");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_QuitButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();

        _button.onClick.AddListener(OnQuitProgram);
    }

    /// <summary>게임 종료</summary>
    void OnQuitProgram()
    {
        Application.Quit();
    }
}

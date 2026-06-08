using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인 메뉴 UI 담당 스크립트
/// </summary>
public class MainMenuUIManager : MonoBehaviour
{
    UI_GameStartButton gameStartButton;

    private void Awake()
    {
        gameStartButton = GetComponentInChildren<UI_GameStartButton>();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 로비 씬 버튼 이벤트 처리.
/// </summary>
public class LobbyUIManager : MonoBehaviour
{
    UI_GameStartButton gameStartButton;

    private void Awake()
    {
        gameStartButton = GetComponentInChildren<UI_GameStartButton>();
    }
}

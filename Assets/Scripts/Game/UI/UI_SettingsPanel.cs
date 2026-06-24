using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ESC로 열리는 설정(일시정지) 패널.
/// </summary>
public class UI_SettingsPanel : UI_Panel
{
    [SerializeField] private Button resumeButton;

    private void Awake()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);
    }

    private void OnResumeClicked()
    {
        GameUIManager.Instance.HideSettings();
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeClicked);
    }
}

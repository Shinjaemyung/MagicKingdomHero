using UnityEngine;
using UnityEngine.UI;

public class UI_WaveStartButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(WaveStart);
    }

    private void Start()
    {
        WaveManager.Instance.OnWaveCleared += ActivateButton;
    }

    void WaveStart()
    {
        DeactivateButton();
        WaveManager.Instance.StartWave();
    }

    public void ActivateButton()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateButton()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        WaveManager.Instance.OnWaveCleared -= ActivateButton;
    }
}

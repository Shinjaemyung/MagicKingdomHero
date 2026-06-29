using UnityEngine;
using UnityEngine.UI;

public class UI_WaveStartButton : MonoBehaviour
{
    Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(StartWave);
    }

    private void Start()
    {
        WaveManager.Instance.WaveCleared += ActivateButton;
    }

    void StartWave()
    {
        DeactivateButton();
        WaveManager.Instance.StartSpawnerActivate();
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
        WaveManager.Instance.WaveCleared -= ActivateButton;
    }
}

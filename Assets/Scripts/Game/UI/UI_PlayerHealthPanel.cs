using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerHealthPanel : UI_Panel
{
    TextMeshProUGUI healthText;

    private void Awake()
    {
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        GamePlayManager.Instance.OnPlayerHealthChanged += SetHealthDisplay;
        SetHealthDisplay(GamePlayManager.Instance.PlayerHealth);
    }

    void SetHealthDisplay(int playerHealth)
    {
        healthText.text = playerHealth.ToString();
    }

    private void OnDestroy()
    {
        GamePlayManager.Instance.OnPlayerHealthChanged -= SetHealthDisplay;
    }
}

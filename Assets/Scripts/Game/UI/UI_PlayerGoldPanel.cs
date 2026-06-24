using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_PlayerGoldPanel : UI_Panel
{
    TextMeshProUGUI goldText;

    private void Awake()
    {
        goldText = GetComponentInChildren<TextMeshProUGUI>(); 
    }

    private void Start()
    {
        GamePlayManager.Instance.OnPlayerGoldChanged += SetGoldDisplay;
        SetGoldDisplay(GamePlayManager.Instance.PlayerGold);
    }

    void SetGoldDisplay(int playerGold)
    {
        goldText.text = playerGold.ToString();
    }

    private void OnDestroy()
    {
        GamePlayManager.Instance.OnPlayerGoldChanged -= SetGoldDisplay;
    }
}

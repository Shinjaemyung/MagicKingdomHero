using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UI_SystemMessagePanel;

public class UI_TowerSpawnButton : MonoBehaviour
{
    [SerializeField]
    Tower spawnTower;

    [SerializeField]
    TextMeshProUGUI text_Price;

    Button _button;
    UI_SystemMessagePanel systemMessage;

    public event Action<Tower> OnButtonClicked;

    private void Awake()
    {
        _button = GetComponent<Button>();
        systemMessage = FindAnyObjectByType<UI_SystemMessagePanel>();
    }

    private void Start()
    {
        _button.onClick.AddListener(OnClickButton);

        text_Price.text = spawnTower.towerData.cost.ToString();
    }

    void OnClickButton()
    {
        if (GamePlayManager.Instance.PlayerGold >= spawnTower.towerData.cost) 
        {
            OnButtonClicked?.Invoke(spawnTower);
        }
        else
        {
            systemMessage.ActivateMessage(SystemMessageType.NotEnoughGold);
        }
    }
}

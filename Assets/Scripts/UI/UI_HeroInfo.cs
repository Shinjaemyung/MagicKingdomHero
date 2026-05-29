using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// HeroControlMode에서 표시되는 Hero 정보 패널.
/// Hero.OnHealthChanged 이벤트를 구독해 체력바를 자동 갱신.
/// </summary>
public class UI_HeroInfo : MonoBehaviour
{
    [SerializeField, Tooltip("체력바 Fill 이미지")]
    private Image healthBarFill;

    [SerializeField, Tooltip("체력 수치 텍스트")]
    private Text healthText;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (Hero.Instance == null) return;
        Hero.Instance.OnHealthChanged += UpdateHealth;
        // 활성화 시 현재 체력으로 즉시 갱신
        UpdateHealth(Hero.Instance.Health, Hero.Instance.MaxHealth);
    }

    private void OnDisable()
    {
        if (Hero.Instance == null) return;
        Hero.Instance.OnHealthChanged -= UpdateHealth;
    }

    /// <summary>체력 비율과 수치로 체력바 업데이트</summary>
    private void UpdateHealth(float current, float max)
    {
        float ratio = Mathf.Clamp01(current / Mathf.Max(max, 1f));

        if (healthBarFill != null)
        {
            var rt = healthBarFill.GetComponent<RectTransform>();
            rt.anchorMax = new Vector2(ratio, rt.anchorMax.y);
            rt.sizeDelta = new Vector2(0f, rt.sizeDelta.y);
            healthBarFill.color = ratio > 0.3f
                ? new Color(0.2f, 0.8f, 0.2f)
                : new Color(0.8f, 0.2f, 0.2f);
        }

        if (healthText != null)
            healthText.text = (int)current + " / " + (int)max;
    }
}

using UnityEngine;

/// <summary>
/// 배치 모드일 때 Hero의 위치를 표시해 줄 이펙트
/// </summary>
public class HeroPos : MonoBehaviour
{
    private void Start()
    {
        Show();

        Hero.Instance.OnActivated += Hide;
        Hero.Instance.OnDeactivated += Show;
    }

    public void Show()
    {
        transform.position = Hero.Instance.transform.position;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Hero.Instance.OnActivated -= Hide;
        Hero.Instance.OnDeactivated -= Show;
    }
}

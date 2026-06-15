using UnityEngine;

public class HeroPos : MonoBehaviour
{
    private void Start()
    {
        Show();
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

}

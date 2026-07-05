using UnityEngine;
using UnityEngine.UI;

public class UI_HowToPlayPanel : UI_Panel
{
    [SerializeField] private Button okButton;

    private void OnEnable()
    {
        if (okButton != null)
            okButton.onClick.AddListener(OnOKClicked);
    }

    public override void Show()
    {
        base.Show();
        transform.SetAsLastSibling();
    }

    void OnOKClicked()
    {
        Hide();
    }
}

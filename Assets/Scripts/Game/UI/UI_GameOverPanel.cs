using UnityEngine;

public class UI_GameOverPanel : UI_Panel
{
    public override void Show()
    {
        base.Show();
        transform.SetAsLastSibling();
    }
}

using UnityEngine;

/// <summary>
/// 게임 커서를 관리하는 컴포넌트.
/// </summary>
public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField, Tooltip("기본 커서 텍스처")]
    private Texture2D defaultCursor;

    [SerializeField, Tooltip("커서 핫스팟 (클릭 기준점)")]
    private Vector2 hotspot = Vector2.zero;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        ApplyCursor(defaultCursor);
    }

    /// <summary>커서 텍스처 변경</summary>
    public void ApplyCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance { get; private set; }

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

    /// <summary>커서 락 상태 변경</summary>
    public void SetCursorLockState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>커서 텍스처 변경</summary>
    public void ApplyCursor(Texture2D cursor)
    {
        Cursor.SetCursor(cursor, hotspot, CursorMode.Auto);
    }
}

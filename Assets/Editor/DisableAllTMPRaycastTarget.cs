using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DisableAllTMPRaycastTarget
{
    [MenuItem("Tools/TMP Settings/Disable Raycast Target In Scene")]
    private static void DisableAllRaycastTarget()
    {
        TextMeshProUGUI[] texts = Object.FindObjectsByType<TextMeshProUGUI>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None);

        Undo.RecordObjects(texts, "Disable All TMP Raycast Target");

        int changedCount = 0;

        foreach (TextMeshProUGUI text in texts)
        {
            if (!text.raycastTarget)
                continue;

            text.raycastTarget = false;
            EditorUtility.SetDirty(text);
            changedCount++;
        }

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log($"완료! {changedCount}개의 TMP Raycast Target을 비활성화했습니다.");
    }
}
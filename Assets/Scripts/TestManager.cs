using UnityEngine;

public class TestManager : MonoBehaviour
{
#if UNITY_EDITOR

    [Header("Test_MoveCamera")]
    [SerializeField, Tooltip("T 키를 누르면 TowerPlacementCamera가 이동할 위치")]
    private Vector3 testCameraPosition = new Vector3(0f, 30f, 0f);

    void Update()
    {
        Test_ChangeHealthAndGold();
        Test_MoveCamera();
    }

    void Test_ChangeHealthAndGold()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            GamePlayManager.Instance.UpdatePlayerHealth(-10);

        if (Input.GetKeyDown(KeyCode.X))
            GamePlayManager.Instance.UpdatePlayerGold(10);

        if (Input.GetKeyDown(KeyCode.Y))
            Hero.Instance.UpdateHealth(-50);
    }

    void Test_MoveCamera()
    {
        // T 키: TowerPlacementCamera를 testCameraPosition으로 이동시키고 고정
        if (Input.GetKeyDown(KeyCode.T))
            CameraManager.Instance.MoveTowerPlacementCameraTo(testCameraPosition);
    }

#endif
}

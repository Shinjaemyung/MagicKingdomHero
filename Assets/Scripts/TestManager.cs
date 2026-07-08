using UnityEngine;

public class TestManager : MonoBehaviour
{
#if UNITY_EDITOR

    void Update()
    {
        Test_ChangeHealthAndGold();
    }

    void Test_ChangeHealthAndGold()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            GamePlayManager.Instance.UpdatePlayerHealth(-10);

        if (Input.GetKeyDown(KeyCode.X))
            GamePlayManager.Instance.UpdatePlayerGold(100);

        if (Input.GetKeyDown(KeyCode.C))
            Hero.Instance.UpdateHealth(-50);
    }
#endif
}

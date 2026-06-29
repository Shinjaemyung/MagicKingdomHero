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
            GamePlayManager.Instance.UpdatePlayerGold(10);

        if (Input.GetKeyDown(KeyCode.Y))
            Hero.Instance.UpdateHealth(-50);
    }
#endif
}

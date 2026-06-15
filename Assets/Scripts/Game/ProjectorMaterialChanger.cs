using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ProjectorMaterialChanger : MonoBehaviour
{
    private float value = 0.01f;
    private float TimeRate = 0;
    public float Timer = 2f;
    private float TimerPlus = 0f;
    private float TimerMinus = 0f;

    private bool Undo;
    public bool opacity = false;
    public bool PlusAndMinus = false;
    private Material mat;
    private DecalProjector _decalProjector;

    void Start()
    {
        TimerPlus = (Timer / 10f) * 1f;
        TimerMinus = (Timer / 10f) * 9f;
        Undo = false;

        _decalProjector = GetComponent<DecalProjector>();

        // 인스턴스 머티리얼 생성 (원본 머티리얼 보호)
        if (!_decalProjector.material.name.EndsWith("(Instance)"))
            _decalProjector.material = new Material(_decalProjector.material) { name = _decalProjector.material.name + " (Instance)" };
        mat = _decalProjector.material;
    }

    void Update()
    {
        if (opacity == true && TimeRate <= Timer && Undo == false)
        {
            TimeRate += Time.deltaTime;
            if (PlusAndMinus == false)
            {
                value = Mathf.Lerp(1f, 0f, TimeRate / Timer);
            }
            else
            {
                if (TimeRate < TimerPlus)
                    value = Mathf.Lerp(0f, 1f, TimeRate / TimerPlus);
                if (TimeRate > TimerPlus)
                    value = Mathf.Lerp(1f, 0f, TimeRate / TimerMinus);
            }
            // Projector의 _Opacity → DecalProjector의 fadeFactor
            _decalProjector.fadeFactor = value;
        }

        if (opacity == false)
        {
            if (TimeRate <= Timer && Undo == false)
            {
                TimeRate += Time.deltaTime;
                value = Mathf.Lerp(0.01f, 4f, TimeRate / Timer);
                mat.SetFloat("_MoveCirle", value);
            }

            if (TimeRate >= Timer && Undo == false)
            {
                check();
            }

            if (Undo == true && TimeRate <= Timer)
            {
                TimeRate += Time.deltaTime;
                value = Mathf.Lerp(4f, 0.01f, TimeRate / Timer);
                mat.SetFloat("_MoveCirle", value);
            }
        }
    }

    void check()
    {
        Undo = true;
        TimeRate = 0;
    }
}

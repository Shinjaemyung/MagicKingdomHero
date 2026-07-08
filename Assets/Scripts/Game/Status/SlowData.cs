using UnityEngine;

[CreateAssetMenu(fileName = "SlowData", menuName = "Scriptable Objects/StatusEffectData/SlowData")]
public class SlowData : StatusEffectData
{
    public float slowRatio;

    public override StatusEffect CreateEffect()
    {
        return new SlowEffect(this);
    }
}

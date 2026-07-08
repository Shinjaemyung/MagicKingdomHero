using UnityEngine;

public abstract class StatusEffectData : ScriptableObject
{
    public float duration;
    public bool isStackable;

    public abstract StatusEffect CreateEffect();
}

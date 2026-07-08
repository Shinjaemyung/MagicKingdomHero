using ActionGameFramework.Health;
using Cinemachine.Utility;
using Core.Health;
using UnityEngine;

public abstract class StatusEffect
{
    protected float elapsed;
    protected StatusEffectData data;

    public StatusEffectData Data => data;
    public bool IsFinished => elapsed >= data.duration;

    protected StatusEffect(StatusEffectData data)
    {
        this.data = data;
    }

    public virtual void OnApply(DamageableBehaviour damageable)
    {
        elapsed = 0;
    }

    public virtual void Tick(DamageableBehaviour damageable, float deltaTime)
    {
        elapsed += deltaTime;
    }

    public virtual void OnRemove(DamageableBehaviour damageable)
    {
    }
}

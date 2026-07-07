using ActionGameFramework.Health;
using Core.Health;
using UnityEngine;

public abstract class StatusEffect : MonoBehaviour
{
    public float Duration = 3f;

    public virtual bool IsStackable => false;

    protected float elapsed;

    public bool IsFinished => elapsed >= Duration;

    private void Awake()
    {
        var damager = GetComponent<Damager>();
        damager.Damaged += OnApply;
    }

    public virtual void OnApply(DamageableBehaviour damageable)
    {
        elapsed = 0;
        damageable.GetComponent<Enemy>().ApplyStatus(this);
    }

    public virtual void Tick(DamageableBehaviour damageable, float deltaTime)
    {
        elapsed += deltaTime;
    }

    public virtual void OnRemove(DamageableBehaviour damageable) { }
}

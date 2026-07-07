using Core.Health;
using UnityEngine;

public class SlowEffect : StatusEffect
{
    public float slowRatio;

    public override void OnApply(DamageableBehaviour damageable)
    {
        base.OnApply(damageable);

        EnemyMover enemyMover = damageable.GetComponent<EnemyMover>();
        if (enemyMover != null)
            enemyMover.ApplySlowStatus(slowRatio);
    }

    public override void OnRemove(DamageableBehaviour damageable)
    {
        EnemyMover enemyMover = damageable.GetComponent<EnemyMover>();
        if (enemyMover != null)
            enemyMover.RemoveSlowStatus();
    }
}

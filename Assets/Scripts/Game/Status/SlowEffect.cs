using Core.Health;
using UnityEngine;

public class SlowEffect : StatusEffect
{
    private SlowData slowData;

    public SlowEffect(SlowData data) : base(data)
    {
        slowData = data;
    }

    public override void OnApply(DamageableBehaviour damageable)
    {
        base.OnApply(damageable);

        EnemyMover enemyMover = damageable.GetComponent<EnemyMover>();
        if (enemyMover != null)
            enemyMover.ApplySlowStatus(slowData.slowRatio);
    }

    public override void OnRemove(DamageableBehaviour damageable)
    {
        EnemyMover enemyMover = damageable.GetComponent<EnemyMover>();
        if (enemyMover != null)
            enemyMover.RemoveSlowStatus();
    }
}

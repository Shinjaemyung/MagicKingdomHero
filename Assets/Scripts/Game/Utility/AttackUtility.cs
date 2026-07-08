using Core.Health;
using System.Collections.Generic;
using UnityEngine;

public static class AttackUtility
{
    public class AttackContext
    {
        public float Damage { get; }
        public DamageType DamageType { get; }
        public IReadOnlyList<StatusEffectData> StatusEffects { get; }

        public AttackContext(float damage, DamageType damageType, IReadOnlyList<StatusEffectData> statusEffects)
        {
            Damage = damage;
            DamageType = damageType;
            StatusEffects = new List<StatusEffectData>(statusEffects);
        }
    }
    /*
    public static void ApplyDamage(DamageableBehaviour damageable, AttackContext context)
    {
        Enemy enemy = damageable.GetComponent<Enemy>();

        enemy.TakeDamage(context.Damage, enemy.Position, )
    }
    */
    public static void ApplyStatusEffects(DamageableBehaviour damageable, AttackContext context)
    {
        Enemy enemy = damageable.GetComponent<Enemy>();

        if (enemy != null) 
        {
            foreach (var effect in context.StatusEffects)
            {
                enemy.ApplyStatus(effect.CreateEffect());
            }
        }
    }
}
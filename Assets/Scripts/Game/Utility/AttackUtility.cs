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

        public SerializableIAlignmentProvider alignment;
        public IAlignmentProvider AlignmentProvider => alignment?.GetInterface();

        public AttackContext(float damage, DamageType damageType, IReadOnlyList<StatusEffectData> statusEffects)
        {
            Damage = damage;
            DamageType = damageType;
            StatusEffects = new List<StatusEffectData>(statusEffects);
        }
    }

    public static void ApplyDamage(DamageableBehaviour damageable, AttackContext context)
    {
        damageable.TakeDamage(context.Damage, context.DamageType, damageable.Position, null);
    }

    public static void ApplyStatusEffects(Enemy enemy, AttackContext context)
    {
        foreach (var effect in context.StatusEffects)
        {
            enemy.ApplyStatus(effect.CreateEffect());
        }
    }
}
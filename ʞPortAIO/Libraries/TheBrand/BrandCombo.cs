using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common;
using LeagueSharp.Common.Data;
using TheBrand.Commons;
using TheBrand.Commons.ComboSystem;

namespace TheBrand
{
    class BrandCombo : ComboProvider
    {
        // ReSharper disable once InconsistentNaming
        public bool ForceAutoAttacks;

        public BrandCombo(IEnumerable<Skill> skills, float range) : base(range, skills) { }

        public BrandCombo(float range, params Skill[] skills) : base(range, skills) { }

        public override void Update()
        {
            if (!(ForceAutoAttacks && Orbwalker.IsAutoAttacking))
                base.Update();

            var target = TargetSelector.GetTarget(600, DamageType.True);
            if (target.IsValidTarget())
            {
                var passiveBuff = target.GetBuff("brandablaze");
                if (passiveBuff != null)
                {
                    return;
                }
            }
        }

        public override bool ShouldBeDead(AIHeroClient target, float additionalSpellDamage = 0f)
        {
            var passive = target.GetBuff("brandablaze");
            return base.ShouldBeDead(target, passive != null ? GetRemainingPassiveDamage(target, passive) : 0f);
        }


        private float GetRemainingPassiveDamage(Obj_AI_Base target, BuffInstance passive)
        {
            return (float)ObjectManager.Player.CalcDamage(target, DamageType.Magical, ((int)(passive.EndTime - Game.Time) + 1) * target.MaxHealth * 0.02f);
        }

        public static float GetPassiveDamage(AIHeroClient target)
        {
            return (float)ObjectManager.Player.CalcDamage(target, DamageType.Magical, target.MaxHealth * 0.08);
        }
    }
}

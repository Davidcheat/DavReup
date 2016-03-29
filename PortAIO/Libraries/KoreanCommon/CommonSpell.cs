namespace KoreanCommon
{
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

    public class CommonSpell : LeagueSharp.Common.Spell
    {
        public CommonSpell(
            SpellSlot slot,
            float range = 0,
            DamageType damageType = DamageType.Physical)
            : base(slot, range, damageType)
        {
        }

        public bool UseOnComboMenu { get; set; }

        public bool UseOnHarasMenu { get; set; }

        public bool UseOnLaneClearMenu { get; set; }

        public bool UseOnCombo
        {
            get
            {
                bool b;

                if (UseOnComboMenu != false)
                {
                    b = UseOnComboMenu;
                }
                else
                {
                    b = false;
                }
                return b;
            }
        }

        public bool UseOnHaras
        {
            get
            {
                bool b;

                if (UseOnComboMenu != false)
                {
                    b = UseOnHarasMenu;
                }
                else
                {
                    b = false;
                }
                return b;
            }
        }

        public bool UseOnLaneClear
        {
            get
            {
                bool b;

                if (UseOnComboMenu != false)
                {
                    b = UseOnLaneClearMenu;
                }
                else
                {
                    b = false;
                }
                return b;
            }
        }

        public float LastTimeUsed
        {
            get
            {
                return Instance.CooldownExpires - Instance.Cooldown;
            }
        }

        public float UsedforXSecAgo
        {
            get
            {
                return Game.Time - LastTimeUsed;
            }
        }

        public bool CanCast(int maxToggleState = 1)
        {
            if (ObjectManager.Player.ChampionName.ToLowerInvariant() == "vladimir")
            {
                return true;
            }
            else
            {
                return ObjectManager.Player.Mana > Instance.SData.Mana && Instance.ToggleState <= maxToggleState;
            }
        }

        public bool IsReadyToCastOn(AIHeroClient target, int maxToggleState = 1)
        {
            return this.IsReady() && CanCast(maxToggleState) && CanCast(target) && target.IsValidTarget(Range) && !target.IsDead;
        }

    }
}
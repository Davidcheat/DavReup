using System;
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

namespace TheBrand.Commons
{
    public static class Extensions
    {
        public static SpellState GetState(this SpellDataInst spellData)
        {
            switch ((int)spellData.State)
            {
                case 0:
                    return SpellState.Ready;
                case 2:
                    return SpellState.NotLearned;
                case 4:
                    return SpellState.Surpressed;
                case 5:
                    return SpellState.Cooldown;
                case 6:
                    return SpellState.NoMana;
                case 10:
                    return SpellState.Surpressed;
                default:
                    return SpellState.Unknown;
            }
        }

        public static SpellState GetState(this LeagueSharp.Common.Spell spellData)
        {
            return spellData.Instance.GetState();
        }

        public static T ToEnum<T>(this string str)
        {
            return (T)Enum.Parse(typeof(T), str);
        }

        public static float GetHealthPercent(this AIHeroClient entity, float health)
        {
            return health / entity.MaxHealth * 100f;
        }

        public static bool HasSpellShield(this AIHeroClient entity)
        {
            return entity.HasBuff("bansheesveil") || entity.HasBuff("SivirE") || entity.HasBuff("NocturneW"); 
        }
    }
}

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

namespace KoreanCommon
{
    public abstract class CommonOrbwalkComplementation
    {
        protected CommonChampion champion { get; set; }
        protected readonly CommonSpells spells;
        protected readonly CommonSpell Q;
        protected readonly CommonSpell W;
        protected readonly CommonSpell E;
        protected readonly CommonSpell R;
        protected readonly CommonSpell RFlash;

        public abstract void LastHitMode();
        public abstract void HarasMode();
        public abstract void LaneClearMode();
        public abstract void ComboMode();
        public abstract void Ultimate(AIHeroClient target);

        public CommonOrbwalkComplementation(CommonChampion champion)
        {
            this.champion = champion;
            spells = champion.Spells;
            Q = champion.Spells.Q;
            W = champion.Spells.W;
            E = champion.Spells.E;
            R = champion.Spells.R;
            RFlash = champion.Spells.RFlash;

            Game.OnUpdate += UseSkills;
        }

        public void UseSkills(EventArgs args)
        {
            if (champion != null)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                {
                    LastHitMode();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                {
                    HarasMode();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    LaneClearMode();
                }
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                {
                    ComboMode();
                }
            }
        }
    }
}

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

    public class CommonEvolveUltimate
    {
        public CommonEvolveUltimate()
        {
            Obj_AI_Base.OnLevelUp += EvolveUltimate;
        }

        private static void EvolveUltimate(Obj_AI_Base sender, EventArgs args)
        {
            if (sender.IsMe)
            {
                sender.Spellbook.EvolveSpell(SpellSlot.R);
            }
        }
    }
}
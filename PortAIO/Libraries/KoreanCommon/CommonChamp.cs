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

    public abstract class CommonChampion
    {
        protected CommonChampion(string menuDisplay)
        {
            Player = ObjectManager.Player;

            MainMenu = new CommonMenu(menuDisplay, true);
            Spells = new CommonSpells(this);
            ForceUltimate = new CommonForceUltimate(this);
            DrawDamage = new CommonDamageDrawing(this);
            DrawDamage.AmountOfDamage = Spells.MaxComboDamage;
            DrawDamage.Active = true;
            commonEvolveUltimate = new CommonEvolveUltimate();
            DisableAA = new CommonDisableAA(this);
        }

        private CommonDisableAA DisableAA { get; set; }

        private CommonEvolveUltimate commonEvolveUltimate;

        protected CommonDamageDrawing DrawDamage { get; set; }

        protected CommonForceUltimate ForceUltimate { get; set; }

        public CommonSpells Spells { get; set; }

        public Orbwalking.Orbwalker Orbwalker { get; set; }

        public AIHeroClient Player { get; set; }

        public CommonMenu MainMenu { get; set; }
    }
}
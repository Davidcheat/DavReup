namespace ElUtilitySuite.Summoners
{
    using System.Linq;

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

    public class Barrier : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the barrier spell.
        /// </summary>
        /// <value>
        ///     The barrier spell.
        /// </value>
        public LeagueSharp.Common.Spell BarrierSpell { get; set; }

        /// <summary>
        /// Gets or sets the menu.
        /// </summary>
        /// <value>
        /// The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the player.
        /// </summary>
        /// <value>
        ///     The player.
        /// </value>
        private AIHeroClient Player
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu barrierMenu;

        public static bool getCheckBoxItem(string item)
        {
            return barrierMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return barrierMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return barrierMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerbarrier") == SpellSlot.Unknown)
            {
                return;
            }

            barrierMenu = rootMenu.AddSubMenu("Barrier", "Barrier");
            barrierMenu.Add("Barrier.Activated", new CheckBox("Barrier activated"));
            barrierMenu.Add("Barrier.HP", new Slider("Barrier percentage", 20, 1, 100));
            barrierMenu.Add("Barrier.Damage", new Slider("Barrier on damage dealt %", 20, 1, 100));
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var barrierSlot = this.Player.GetSpellSlot("summonerbarrier");

            if (barrierSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.BarrierSpell = new LeagueSharp.Common.Spell(barrierSlot, 550);

            AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
        }

        #endregion

        #region Methods

        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!getCheckBoxItem("Barrier.Activated"))
            {
                return;
            }

            var source = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Source.NetworkId);
            var obj = ObjectManager.GetUnitByNetworkId<GameObject>((uint)args.Target.NetworkId);

            if (obj.Type != GameObjectType.AIHeroClient || source.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var hero = (AIHeroClient)obj;

            if (!hero.IsMe)
            {
                return;
            }

            if (((int)(args.Damage / this.Player.MaxHealth * 100) > getSliderItem("Barrier.Damage") || this.Player.HealthPercent < getSliderItem("Barrier.HP")) && this.Player.CountEnemiesInRange(1000) >= 1)
            {
                this.BarrierSpell.Cast();
            }
        }

        #endregion
    }
}

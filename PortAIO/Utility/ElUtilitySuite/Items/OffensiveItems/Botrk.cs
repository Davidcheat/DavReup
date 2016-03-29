namespace ElUtilitySuite.Items.OffensiveItems
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

    internal class Botrk : ElUtilitySuite.Items.Item
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the identifier.
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public override ItemId Id
        {
            get
            {
                return ItemId.Blade_of_the_Ruined_King;
            }
        }

        /// <summary>
        ///     Gets or sets the name of the item.
        /// </summary>
        /// <value>
        ///     The name of the item.
        /// </value>
        public override string Name
        {
            get
            {
                return "Blade of the Ruined King";
            }
        }

        public static AIHeroClient Player = ObjectManager.Player;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// 
        public bool getCheckBoxItem(string item)
        {
            return this.Menu[item].Cast<CheckBox>().CurrentValue;
        }

        public int getSliderItem(string item)
        {
            return this.Menu[item].Cast<Slider>().CurrentValue;
        }

        public bool getKeyBindItem(string item)
        {
            return this.Menu[item].Cast<KeyBind>().CurrentValue;
        }

        public override void CreateMenu()
        {
            this.Menu.AddGroupLabel("Botrk");
            this.Menu.Add("UseBotrkCombo", new CheckBox("Use on Combo"));
            this.Menu.Add("BotrkEnemyHp", new Slider("Use on Enemy Hp %", 100, 1, 100));
            this.Menu.Add("BotrkMyHp", new Slider("Use on My Hp %", 100, 1, 100));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseBotrkCombo") && this.ComboModeActive && (HeroManager.Enemies.Any(x => x.HealthPercent < getSliderItem("BotrkEnemyHp") && x.Distance(Player) < 550) || Player.HealthPercent < getSliderItem("BotrkMyHp"));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem(
                (int)this.Id,
                HeroManager.Enemies.First(x => x.HealthPercent < getSliderItem("BotrkEnemyHp") && x.Distance(ObjectManager.Player) < 550));
        }

        #endregion
    }
}
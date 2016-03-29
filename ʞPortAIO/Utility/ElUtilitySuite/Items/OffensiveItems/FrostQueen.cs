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

    class FrostQueen : ElUtilitySuite.Items.Item
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
                return ItemId.Frost_Queens_Claim;
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
                return "Frost Queen's Claim";
            }
        }

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
            this.Menu.AddGroupLabel("Frost Queen's Claim");
            this.Menu.Add("UseFrostQueenCombo", new CheckBox("Use on Combo"));
            this.Menu.Add("FrostQueenEnemyHp", new Slider("Use on Enemy Hp %", 70, 1, 100));
            this.Menu.Add("FrostQueenMyHp", new Slider("Use on My Hp %", 100, 1, 100));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("UseFrostQueenCombo") && this.ComboModeActive
                   && (HeroManager.Enemies.Any(
                       x =>
                       x.HealthPercent < getSliderItem("FrostQueenEnemyHp")
                       && x.Distance(ObjectManager.Player) < 1500)
                       || ObjectManager.Player.HealthPercent < getSliderItem("FrostQueenMyHp"));
        }

        /// <summary>
        ///     Uses the item.
        /// </summary>
        public override void UseItem()
        {
            Items.UseItem((int)this.Id);
        }

        #endregion
    }
}
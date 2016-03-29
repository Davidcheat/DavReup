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

    internal class Hydra : ElUtilitySuite.Items.Item
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
                return ItemId.Ravenous_Hydra_Melee_Only;
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
                return "Hydra";
            }
        }

        #endregion

        #region Public Methods and Operators

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
            this.Menu.AddGroupLabel("Ravenous Hydra");
            this.Menu.Add("Hydracombo", new CheckBox("Use on Combo"));
        }

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("Hydracombo") && this.ComboModeActive && HeroManager.Enemies.Any(x => x.Distance(ObjectManager.Player) < 400 && !x.IsDead && !x.IsZombie);
        }

        #endregion
    }
}
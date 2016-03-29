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

    internal class TitanicHydra : ElUtilitySuite.Items.Item
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
                return (ItemId)3053;
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
                return "Titanic Hydra";
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Shoulds the use item.
        /// </summary>
        /// <returns></returns>
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
            this.Menu.AddGroupLabel("Titanic Hydra");
            this.Menu.Add("TitanicHydracombo", new CheckBox("Use on Combo"));
        }

        public override bool ShouldUseItem()
        {
            return getCheckBoxItem("TitanicHydracombo") && this.ComboModeActive && !Orbwalking.CanAttack();
        }

        #endregion
    }
}
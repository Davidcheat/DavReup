namespace ElUtilitySuite.Utility
{
    using System;
    using System.Collections.Generic;
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

    using ItemData = LeagueSharp.Common.Data.ItemData;

    internal class AntiStealth : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an anti stealth item.
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetAntiStealthItemDelegate();

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<AntiStealthRevealItem> Items { get; set; }

        #endregion

        #region Public Methods and Operators

        public static bool getCheckBoxItem(string item)
        {
            return protectMenu[item].Cast<CheckBox>().CurrentValue;
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu protectMenu;
        public void CreateMenu(Menu rootMenu)
        {
            protectMenu = rootMenu.AddSubMenu("Anti-Stealth", "AntiStealth");
            protectMenu.Add("AntiStealthActive", new CheckBox("Place Pink Ward on Unit Stealth"));
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<AntiStealthRevealItem>
                             {
                                 new AntiStealthRevealItem { GetItem = () => ItemData.Vision_Ward.GetItem() },
                                 new AntiStealthRevealItem { GetItem = () => ItemData.Greater_Vision_Totem_Trinket.GetItem() }
                             };

            GameObject.OnIntegerPropertyChange += this.GameObject_OnIntegerPropertyChange;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Fired when an integral property is changed in a GameObject.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="GameObjectIntegerPropertyChangeEventArgs" /> instance containing the event data.</param>
        private void GameObject_OnIntegerPropertyChange(
            GameObject sender,
            GameObjectIntegerPropertyChangeEventArgs args)
        {
            if (!(sender is AIHeroClient) || args.Property != "ActionState" || !getCheckBoxItem("AntiStealthActive"))
            {
                return;
            }

            var hero = (AIHeroClient)sender;

            if (hero.IsAlly)
            {
                return;
            }

            if (((GameObjectCharacterState)args.Value).HasFlag(GameObjectCharacterState.IsStealth) || !((GameObjectCharacterState)args.Value).HasFlag(GameObjectCharacterState.IsStealth)) // ?
            {
                return;
            }

            var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsInRange(hero) && x.IsReady());

            if (item != null)
            {
                item.Cast(hero.Position.Randomize(10, 300));
            }
        }

        #endregion

        /// <summary>
        ///     Represents an item that can reveal stealthed units.
        /// </summary>
        private class AntiStealthRevealItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetAntiStealthItemDelegate GetItem { get; set; }

            /// <summary>
            ///     Gets the item.
            /// </summary>
            /// <value>
            ///     The item.
            /// </value>
            public Items.Item Item
            {
                get
                {
                    return this.GetItem();
                }
            }

            #endregion
        }
    }
}
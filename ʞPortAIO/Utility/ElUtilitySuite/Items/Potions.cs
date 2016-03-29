namespace ElUtilitySuite.Items
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

    internal class Potions : IPlugin
    {
        #region Delegates

        /// <summary>
        ///     Gets an health item
        /// </summary>
        /// <returns></returns>
        private delegate Items.Item GetHealthItemDelegate();

        #endregion

        #region Public Properties

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

        /// <summary>
        ///     Gets or sets the items.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        private List<HealthItem> Items { get; set; }

        /// <summary>
        ///     Gets the set player hp menu value.
        /// </summary>
        /// <value>
        ///     The player hp hp menu value.
        /// </value>
        private int PlayerHp
        {
            get
            {
                return getSliderItem("Potions.Player.Health");
            }
        }

        #endregion

        #region Public Methods and Operators

        public static bool getCheckBoxItem(string item)
        {
            return potionsMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return potionsMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return potionsMenu[item].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu potionsMenu;
        public void CreateMenu(Menu rootMenu)
        {
            potionsMenu = rootMenu.AddSubMenu("Potions", "Potions");
            potionsMenu.Add("Potions.Activated", new CheckBox("Potions activated"));
            potionsMenu.Add("Potions.Health", new CheckBox("Health potions"));
            potionsMenu.Add("Potions.Biscuit", new CheckBox("Biscuits"));
            potionsMenu.Add("Potions.RefillablePotion", new CheckBox("Refillable Potion"));
            potionsMenu.Add("Potions.HuntersPotion", new CheckBox("Hunters Potion"));
            potionsMenu.Add("Potions.CorruptingPotion", new CheckBox("Corrupting Potion"));
            potionsMenu.AddSeparator();
            potionsMenu.Add("Potions.Player.Health", new Slider("Health percentage", 20, 0, 100));

        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            this.Items = new List<HealthItem>
                             {
                                 new HealthItem { GetItem = () => ItemData.Health_Potion.GetItem() },
                                 new HealthItem { GetItem = () => ItemData.Total_Biscuit_of_Rejuvenation2.GetItem() },
                                 new HealthItem { GetItem = () => ItemData.Refillable_Potion.GetItem() }, //2031
                                 new HealthItem { GetItem = () => ItemData.Hunters_Potion.GetItem() }, //2032
                                 new HealthItem { GetItem = () => ItemData.Corrupting_Potion.GetItem() } //2033
                             };

            Game.OnUpdate += this.OnUpdate;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the player buffs
        /// </summary>
        /// <value>
        ///     The player buffs
        /// </value>
        private bool CheckPlayerBuffs()
        {
            return this.Player.HasBuff("RegenerationPotion") || this.Player.HasBuff("ItemMiniRegenPotion") || this.Player.HasBuff("ItemCrystalFlask") || this.Player.HasBuff("ItemCrystalFlaskJungle") || this.Player.HasBuff("ItemDarkCrystalFlask");
        }

        private void OnUpdate(EventArgs args)
        {
            try
            {
                if (!getCheckBoxItem("Potions.Activated") || this.Player.IsDead || this.Player.InFountain() || this.Player.IsRecalling())
                {
                    return;
                }

                if (this.Player.HealthPercent < this.PlayerHp)
                {
                    if (this.CheckPlayerBuffs())
                    {
                        return;
                    }

                    var item = this.Items.Select(x => x.Item).FirstOrDefault(x => x.IsReady() && x.IsOwned());

                    if (item != null)
                    {
                        item.Cast();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        /// <summary>
        ///     Represents an item that can heal
        /// </summary>
        private class HealthItem
        {
            #region Public Properties

            /// <summary>
            ///     Gets or sets the get item.
            /// </summary>
            /// <value>
            ///     The get item.
            /// </value>
            public GetHealthItemDelegate GetItem { get; set; }

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
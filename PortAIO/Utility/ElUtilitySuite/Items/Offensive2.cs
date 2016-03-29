namespace ElUtilitySuite.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

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

    internal class Offensive2 : IPlugin
    {
        #region Fields

        private readonly List<Item> offensiveItems;

        #endregion

        #region Constructors and Destructors

        public Offensive2()
        {
            this.offensiveItems =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(
                        x =>
                        x.Namespace != null && x.Namespace.Contains("OffensiveItems") && x.IsClass
                        && typeof(Item).IsAssignableFrom(x))
                    .Select(x => (Item)Activator.CreateInstance(x))
                    .ToList();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the menu.
        /// </summary>
        /// <value>
        ///     The menu.
        /// </value>
        public Menu Menu { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        public void CreateMenu(Menu rootMenu)
        {
            Menu = rootMenu.AddSubMenu("Offensive", "omenu2");

            foreach (var item in this.offensiveItems)
            {
                item.Menu = Menu;
                item.CreateMenu();
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            Game.OnUpdate += this.Game_OnUpdate;
        }

        #endregion

        #region Methods

        private void Game_OnUpdate(EventArgs args)
        {
            foreach (var item in this.offensiveItems.Where(x => x.ShouldUseItem() && Items.CanUseItem((int)x.Id)))
            {
                item.UseItem();
            }
        }

        #endregion
    }
}
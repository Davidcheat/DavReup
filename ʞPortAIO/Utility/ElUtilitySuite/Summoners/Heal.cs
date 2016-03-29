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

    public class Heal : IPlugin
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the heal spell.
        /// </summary>
        /// <value>
        ///     The heal spell.
        /// </value>
        public LeagueSharp.Common.Spell HealSpell { get; set; }

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

        public static bool getCheckBoxItem(string item)
        {
            return healMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return healMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return healMenu[item].Cast<KeyBind>().CurrentValue;
        }

        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu healMenu;
        public void CreateMenu(Menu rootMenu)
        {
            if (this.Player.GetSpellSlot("summonerheal") == SpellSlot.Unknown)
            {
                return;
            }

            healMenu = rootMenu.AddSubMenu("Heal", "Heal");
            healMenu.Add("Heal.Activated", new CheckBox("Heal"));
            healMenu.Add("Heal.HP", new Slider("Health percentage", 20, 1, 100));
            healMenu.Add("Heal.Damage", new Slider("Heal on % incoming damage", 20, 1, 100));
            healMenu.AddSeparator();

            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                healMenu.Add("healon" + x.ChampionName, new CheckBox("Use for " + x.ChampionName));
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public void Load()
        {
            var healSlot = this.Player.GetSpellSlot("summonerheal");

            if (healSlot == SpellSlot.Unknown)
            {
                return;
            }

            this.HealSpell = new LeagueSharp.Common.Spell(healSlot, 550);

            AttackableUnit.OnDamage += this.AttackableUnit_OnDamage;
        }

        #endregion

        #region Methods

        private void AttackableUnit_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (!getCheckBoxItem("Heal.Activated"))
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

            if (hero.IsEnemy || (!hero.IsMe && !this.HealSpell.IsInRange(obj)) || !getCheckBoxItem(string.Format("healon{0}", hero.ChampionName)))
            {
                return;
            }

            if (((int)(args.Damage / hero.Health) > getSliderItem("Heal.Damage")) || (hero.HealthPercent < getSliderItem("Heal.HP")))
            {
                this.Player.Spellbook.CastSpell(this.HealSpell.Slot);
            }
        }

        #endregion
    }
}

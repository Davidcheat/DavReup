namespace ElUtilitySuite.Items
{
    using System;
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

    internal static class DefensiveExtensions
    {
        #region Public Methods and Operators

        public static int CountHerosInRange(this AIHeroClient target, bool checkteam, float range = 1200f)
        {
            var objListTeam = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(range, false));

            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }

        public static bool IsValidState(this AIHeroClient target)
        {
            return !target.HasBuffOfType(BuffType.SpellShield) && !target.HasBuffOfType(BuffType.SpellImmunity)
                   && !target.HasBuffOfType(BuffType.Invulnerability);
        }

        #endregion
    }

    internal class Defensive : IPlugin
    {
        #region Static Fields

        public static AIHeroClient AggroTarget;

        private static float incomingDamage, minionDamage;

        #endregion

        #region Public Properties

        public Menu Menu { get; set; }

        #endregion

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

        #region Public Methods and Operators

        private AIHeroClient Allies()
        {
            var target = this.Player;
            foreach (var unit in
                ObjectManager.Get<AIHeroClient>()
                    .Where(x => x.IsAlly && x.IsValidTarget(900, false))
                    .OrderByDescending(xe => xe.Health / xe.MaxHealth * 100))
            {
                target = unit;
            }

            return target;
        }


        /// <summary>
        ///     Creates the menu.
        /// </summary>
        /// <param name="rootMenu">The root menu.</param>
        /// <returns></returns>
        /// 
        public static Menu rootMenu = ElUtilitySuite.Entry.menu;
        public static Menu defenseMenu;

        public static bool getCheckBoxItem(string item)
        {
            return defenseMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(string item)
        {
            return defenseMenu[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(string item)
        {
            return defenseMenu[item].Cast<KeyBind>().CurrentValue;
        }

        public void CreateMenu(Menu rootMenu)
        {
            defenseMenu = rootMenu.AddSubMenu("Defensive", "DefensiveMenu");

            foreach (var x in ObjectManager.Get<AIHeroClient>().Where(x => x.IsAlly))
            {
                defenseMenu.Add("DefenseOn" + x.CharData.BaseSkinName, new CheckBox("Use for " + x.CharData.BaseSkinName));
            }

            CreateDefensiveItem("Randuin's Omen", "Randuins", "selfcount", 40, 40);
            CreateDefensiveItem("Face of the Mountain", "Mountain", "allyhealth", 20, 45);
            CreateDefensiveItem("Locket of Iron Solari", "Locket", "allyhealth", 40, 45);
            CreateDefensiveItem("Seraph's Embrace", "Seraphs", "selfhealth", 40, 45);

            defenseMenu.Add("useTalisman", new CheckBox("Use Talisman of Ascension"));
            defenseMenu.Add("useAllyPct", new Slider("Use on ally %", 50, 1, 100));
            defenseMenu.Add("useEnemyPct", new Slider("Use on enemy %", 50, 1, 100));
            defenseMenu.Add("talismanMode", new Slider("Mode (0 : Always | 1 : Combo): ", 0, 0, 1));
        }

        public void Load()
        {
            try
            {
                Game.OnUpdate += this.OnUpdate;
                Obj_AI_Base.OnProcessSpellCast += this.OnProcessSpellCast;
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        #endregion

        #region Methods

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.Type == GameObjectType.AIHeroClient && sender.IsEnemy)
            {
                var heroSender = ObjectManager.Get<AIHeroClient>().First(x => x.NetworkId == sender.NetworkId);
                if (heroSender.GetSpellSlot(args.SData.Name) == SpellSlot.Unknown
                    && args.Target.Type == Entry.Player.Type)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);
                    incomingDamage = (float)heroSender.GetAutoAttackDamage(AggroTarget);
                }

                if (heroSender.ChampionName == "Jinx" && args.SData.Name.Contains("JinxQAttack")
                    && args.Target.Type == Entry.Player.Type)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);
                    incomingDamage = (float)heroSender.GetAutoAttackDamage(AggroTarget);
                }
            }

            if (sender.Type == GameObjectType.obj_AI_Minion && sender.IsEnemy)
            {
                if (args.Target.NetworkId == Entry.Player.NetworkId)
                {
                    AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);

                    minionDamage = (float)sender.CalcDamage(AggroTarget, DamageType.Physical, sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);
                }
            }

            if (sender.Type == GameObjectType.obj_AI_Turret && sender.IsEnemy)
            {
                if (args.Target.Type == Entry.Player.Type)
                {
                    if (sender.Distance(this.Allies().ServerPosition, true) <= 900 * 900)
                    {
                        AggroTarget = ObjectManager.GetUnitByNetworkId<AIHeroClient>((uint)args.Target.NetworkId);

                        incomingDamage =
                            (float)
                            sender.CalcDamage(
                                AggroTarget,
                                DamageType.Physical,
                                sender.BaseAttackDamage + sender.FlatPhysicalDamageMod);
                    }
                }
            }
        }

        private void CreateDefensiveItem(string displayname, string name, string type, int hpvalue, int dmgvalue)
        {
            defenseMenu.Add("use" + name, new CheckBox("Use " + displayname));

            if (!type.Contains("count"))
            {
                defenseMenu.Add("use" + name + "Pct", new Slider("Use on HP %", hpvalue, 0, 100));
                defenseMenu.Add("use" + name + "Dmg", new Slider("Use on damage dealt %", dmgvalue, 0, 100));
            }

            if (type.Contains("count"))
            {
                defenseMenu.Add("use" + name + "Count", new Slider("Use on Count", 3, 1, 5));
            }
        }

        private void DefensiveItemManager()
        {
            if (Entry.getComboMenu())
            {
                this.UseItemCount("Randuins", 3143, 450f);
            }

            this.UseItem("allyshieldlocket", "Locket", 3190, 600f);
            this.UseItem("allyshieldmountain", "Mountain", 3401, 700f);
            this.UseItem("selfshieldseraph", "Seraphs", 3040);

            if (Items.HasItem(3069) && Items.CanUseItem(3069) && getCheckBoxItem("useTalisman"))
            {
                if (!Entry.getComboMenu() && getSliderItem("talismanMode") == 1)
                {
                    return;
                }

                var target = this.Allies();
                if (target.Distance(Entry.Player.ServerPosition, true) > 600 * 600)
                {
                    return;
                }

                var lowTarget =
                    ObjectManager.Get<AIHeroClient>()
                        .OrderBy(ex => ex.Health / ex.MaxHealth * 100)
                        .First(x => x.IsValidTarget(1000));

                var aHealthPercent = target.Health / target.MaxHealth * 100;
                var eHealthPercent = lowTarget.Health / lowTarget.MaxHealth * 100;

                if (lowTarget.Distance(target.ServerPosition, true) <= 900 * 900
                    && (target.CountHerosInRange(false) > target.CountHerosInRange(true)
                        && eHealthPercent <= getSliderItem("useEnemyPct")))
                {
                    Items.UseItem(3069);
                }

                if (target.CountHerosInRange(false) > target.CountHerosInRange(true)
                    && aHealthPercent <= getSliderItem("useAllyPct"))
                {
                    Items.UseItem(3069);
                }
            }
        }

        private void OnUpdate(EventArgs args)
        {
            if (Entry.Player.IsDead)
            {
                return;
            }

            try
            {
                this.DefensiveItemManager();
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred: '{0}'", e);
            }
        }

        private void UseItem(string menuvar, string name, int itemId, float itemRange = float.MaxValue)
        {
            if (Entry.Player.InFountain())
            {
                return;
            }

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
            {
                return;
            }

            if (!getCheckBoxItem("use" + name))
            {
                return;
            }

            var target = itemRange > 5000 ? Entry.Player : this.Allies();
            if (target.Distance(Entry.Player.ServerPosition, true) > itemRange * itemRange || !target.IsValidState())
            {
                return;
            }

            var aHealthPercent = (int)((target.Health / target.MaxHealth) * 100);
            var iDamagePercent = (int)(incomingDamage / target.MaxHealth * 100);

            if (!getCheckBoxItem("DefenseOn" + target.CharData.BaseSkinName))
            {
                return;
            }

            if (menuvar.Contains("shield"))
            {
                if (aHealthPercent <= getSliderItem("use" + name + "Pct"))
                {
                    if ((iDamagePercent >= 1 || incomingDamage >= target.Health))
                    {
                        if (AggroTarget.NetworkId == target.NetworkId)
                        {
                            Items.UseItem(itemId, target);
                        }
                    }

                    if (iDamagePercent >= getSliderItem("use" + name + "Dmg"))
                    {
                        if (AggroTarget.NetworkId == target.NetworkId)
                        {
                            Items.UseItem(itemId, target);
                        }
                    }
                }
            }
        }

        private void UseItemCount(string name, int itemId, float itemRange)
        {
            if (Entry.Player.InFountain())
            {
                return;
            }

            if (!Items.HasItem(itemId) || !Items.CanUseItem(itemId))
            {
                return;
            }

            if (getCheckBoxItem("use" + name))
            {
                if (Entry.Player.CountHerosInRange(true, itemRange) >= getSliderItem("use" + name + "Count"))
                {
                    Items.UseItem(itemId);
                }
            }
        }

        #endregion
    }
}
namespace KoreanCommon
{
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
    public class CommonMenu
    {
        public static Menu _comboMenu;

        public static Menu _drwaingsMenu;

        public static Menu _harasMenu;

        public static Menu _laneClearMenu;

        public static Menu _mainMenu;

        public static string _menuName;

        public static Menu _miscMenu;

        public CommonMenu(string displayName, bool misc)
        {
            _menuName = displayName.Replace(" ", "_").ToLowerInvariant();

            _mainMenu = EloBuddy.SDK.Menu.MainMenu.AddMenu(displayName, _menuName);

            addHarasMenu(_mainMenu);
            addLaneClearMenu(_mainMenu);
            addComboMenu(_mainMenu);

            if (misc)
            {
                addMiscMenu(_mainMenu);
            }

            addDrawingMenu(_mainMenu);
        }

        public Menu MainMenu
        {
            get
            {
                return _mainMenu;
            }
        }

        public Menu HarasMenu
        {
            get
            {
                return _harasMenu;
            }
        }

        public Menu LaneClearMenu
        {
            get
            {
                return _laneClearMenu;
            }
        }

        public Menu ComboMenu
        {
            get
            {
                return _comboMenu;
            }
        }

        public Menu MiscMenu
        {
            get
            {
                return _miscMenu;
            }
        }

        public Menu DrawingMenu
        {
            get
            {
                return _drwaingsMenu;
            }
        }

        public static bool getComboCH(string item)
        {
            return _comboMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static bool getLaneClearCH(string item)
        {
            return _laneClearMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static bool getHarassCH(string item)
        {
            return _harasMenu[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getAAMode()
        {
            return _comboMenu[string.Format("{0}.disableaa", MenuName)].Cast<Slider>().CurrentValue;
        }

        public static bool getForceUlt()
        {
            return _comboMenu[string.Format("{0}.forceultusingmouse", MenuName)].Cast<CheckBox>().CurrentValue;
        }

        public static bool getkillableindicator()
        {
            return _drwaingsMenu[string.Format("{0}.killableindicator", MenuName)].Cast<CheckBox>().CurrentValue;
        }

        public static bool getdamageindicator()
        {
            return _drwaingsMenu[string.Format("{0}.damageindicator", MenuName)].Cast<CheckBox>().CurrentValue;
        }

        public static bool getDrawSkillRange()
        {
            return _drwaingsMenu[string.Format("{0}.drawskillranges", MenuName)].Cast<CheckBox>().CurrentValue;
        }

        public static bool getUseELaneClear()
        {
            return _laneClearMenu[string.Format("{0}.useetolaneclear", MenuName)].Cast<CheckBox>().CurrentValue;
        }

        public static bool harassOnLC()
        {
            return _laneClearMenu[string.Format("{0}.harasonlaneclear", MenuName)].Cast<CheckBox>().CurrentValue;
        }

        public static int manaHarass()
        {
            return _harasMenu[string.Format("{0}.manalimittoharas", MenuName)].Cast<Slider>().CurrentValue;
        }

        public static int manaLaneClear()
        {
            return _laneClearMenu[string.Format("{0}.manalimittolaneclear", MenuName)].Cast<Slider>().CurrentValue;
        }

        public static int getMinMinQ()
        {
            return _laneClearMenu["minminionstoq"].Cast<Slider>().CurrentValue;
        }

        public static int getMinMinW()
        {
            return _laneClearMenu["minminionstow"].Cast<Slider>().CurrentValue;
        }

        public static string MenuName
        {
            get
            {
                return _menuName;
            }
        }

        private void addHarasMenu(Menu RootMenu)
        {
            _harasMenu = RootMenu.AddSubMenu("Harass", string.Format("{0}.haras", MenuName));

            _harasMenu.Add(string.Format("{0}.useqtoharas", MenuName), new CheckBox("Use Q"));
            _harasMenu.Add(string.Format("{0}.usewtoharas", MenuName), new CheckBox("Use W"));
            //_harasMenu.Add(string.Format("{0}.useetoharas", MenuName), new CheckBox("Use E"));
            _harasMenu.Add(string.Format("{0}.manalimittoharas", MenuName), new Slider("Min. % Mana to Harass", 0, 0, 100));
        }

        private void addLaneClearMenu(Menu RootMenu)
        {
            _laneClearMenu = RootMenu.AddSubMenu("Lane Clear", string.Format("{0}.laneclear", MenuName));
            _laneClearMenu.Add(string.Format("{0}.useqtolaneclear", MenuName), new CheckBox("Use Q"));
            _laneClearMenu.Add(string.Format("{0}.usewtolaneclear", MenuName), new CheckBox("Use W"));
            _laneClearMenu.Add(string.Format("{0}.useetolaneclear", MenuName), new CheckBox("Use E"));
            _laneClearMenu.Add(string.Format("{0}.manalimittolaneclear", MenuName), new Slider("Min. % Mana to LC", 50, 0, 100));
            _laneClearMenu.Add(string.Format("{0}.harasonlaneclear", MenuName), new CheckBox("Harass Enemies in LC"));
        }

        private void addComboMenu(Menu RootMenu)
        {
            _comboMenu = RootMenu.AddSubMenu("Combo", string.Format("{0}.combo", MenuName));
            _comboMenu.Add(string.Format("{0}.useqtocombo", MenuName), new CheckBox("Use Q"));
            _comboMenu.Add(string.Format("{0}.usewtocombo", MenuName), new CheckBox("Use W"));
            _comboMenu.Add(string.Format("{0}.useetocombo", MenuName), new CheckBox("Use E"));
            _comboMenu.Add(string.Format("{0}.usertocombo", MenuName), new CheckBox("Use R"));
            _comboMenu.Add(string.Format("{0}.minenemiestor", MenuName), new Slider("Only R if X Enemies Hit", 3, 1, 5));
            _comboMenu.Add(string.Format("{0}.disableaa", MenuName), new Slider("Disable AA When (0 : Never | 1 : Always | 2 : Some Skills Ready | 3 : Harass Combo Ready | 4 : Full Combo Ready)", 1, 0, 4));
            _comboMenu.Add(string.Format("{0}.forceultusingmouse", MenuName), new CheckBox("Force R Using Mouse-buttons (Cursor Sprite)"));
        }

        private void addMiscMenu(Menu RootMenu)
        {
            _miscMenu = RootMenu.AddSubMenu("Options", string.Format("{0}.misc", MenuName));
        }

        private void addDrawingMenu(Menu RootMenu)
        {
            _drwaingsMenu = RootMenu.AddSubMenu("Drawings", string.Format("{0}.drawings", MenuName));
            _drwaingsMenu.Add(string.Format("{0}.drawskillranges", MenuName), new CheckBox("Draw Skill Ranges"));
            _drwaingsMenu.Add(string.Format("{0}.damageindicator", MenuName), new CheckBox("Draw Damage Indicator"));
            _drwaingsMenu.Add(string.Format("{0}.killableindicator", MenuName), new CheckBox("Draw Killable Enemy"));
        }
    }
}

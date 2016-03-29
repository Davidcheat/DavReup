using System;

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

    public enum CommonDisableAAMode
    {
        Never,

        Always,

        SomeSkillReady,

        HarasComboReady,

        FullComboReady
    };

    public class CommonDisableAA
    {
        private CommonChampion champion;

        public CommonDisableAA(CommonChampion champion)
        {
            this.champion = champion;

            Orbwalking.BeforeAttack += CancelAA;
        }

        public int Mode
        {
            get
            {
                if (CommonMenu.getAAMode() != null)
                {
                    return CommonMenu.getAAMode();
                }
                else
                {
                    return 0;
                }
            }
        }

        public bool CanUseAA()
        {
            bool canHit = true;
            return canHit;
        }

        private void CancelAA(Orbwalking.BeforeAttackEventArgs args)
        {
            if (args.Target != null)
            {
                if (champion.Orbwalker.ActiveMode == Orbwalking.OrbwalkingMode.Combo)
                {
                    switch (Mode)
                    {
                        case 1:
                            args.Process = false;
                            break;
                        case 0:
                            args.Process = true;
                            break;
                        case 2:
                            if (champion.Spells.SomeSkillReady())
                            {
                                args.Process = false;
                            }
                            break;
                        case 3:
                            if (champion.Spells.HarasReady())
                            {
                                args.Process = false;
                            }
                            break;
                        case 4:
                            if (champion.Spells.ComboReady())
                            {
                                args.Process = false;
                            }
                            break;
                    }
                }
                else
                {
                    if (args.Target is Obj_AI_Base && ((Obj_AI_Base)args.Target).IsMinion && !CanUseAA())
                    {
                        args.Process = false;
                    }
                }
            }
        }
    }
}
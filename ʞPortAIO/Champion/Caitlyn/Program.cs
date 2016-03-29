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
using SebbyLib;

namespace PortAIO.Champion.Caitlyn
{
    public class Program
    {
        private static Menu Config = SebbyLib.Program.Config;
        public static Menu drawMenu, qMenu, wMenu, eMenu, rMenu, farmMenu;
        private static LeagueSharp.Common.Spell E, Q, Qc, R, W;
        private static float QMANA = 0, WMANA = 0, EMANA = 0, RMANA = 0;

        private static float QCastTime = 0;

        public static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static string[] Spells =
        {
            "katarinar","drain","consume","absolutezero", "staticfield","reapthewhirlwind","jinxw","jinxr","shenstandunited","threshe","threshrpenta","threshq","meditate","caitlynpiltoverpeacemaker", "volibearqattack",
            "cassiopeiapetrifyinggaze","ezrealtrueshotbarrage","galioidolofdurand","luxmalicecannon", "missfortunebullettime","infiniteduress","alzaharnethergrasp","lucianq","velkozr","rocketgrabmissile"
        };

        public static void LoadOKTW()
        {
            Q = new LeagueSharp.Common.Spell(SpellSlot.Q, 1250f);
            Qc = new LeagueSharp.Common.Spell(SpellSlot.Q, 1250f);
            W = new LeagueSharp.Common.Spell(SpellSlot.W, 800f);
            E = new LeagueSharp.Common.Spell(SpellSlot.E, 750f);
            R = new LeagueSharp.Common.Spell(SpellSlot.R, 3000f);


            Q.SetSkillshot(0.65f, 60f, 2200f, false, SkillshotType.SkillshotLine);
            Qc.SetSkillshot(0.65f, 60f, 2200f, true, SkillshotType.SkillshotLine);
            W.SetSkillshot(1.5f, 20f, float.MaxValue, false, SkillshotType.SkillshotCircle);
            E.SetSkillshot(0.25f, 70f, 1600f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.7f, 200f, 1500f, false, SkillshotType.SkillshotCircle);

            LoadMenuOKTW();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
        }

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        private static void LoadMenuOKTW()
        {
            drawMenu = Config.AddSubMenu("Draw");
            drawMenu.Add("noti", new CheckBox("Show notification & line"));
            drawMenu.Add("qRange", new CheckBox("Q range"));
            drawMenu.Add("wRange", new CheckBox("W range"));
            drawMenu.Add("eRange", new CheckBox("E rang"));
            drawMenu.Add("rRange", new CheckBox("R range"));
            drawMenu.Add("onlyRdy", new CheckBox("Draw only ready spells"));

            qMenu = Config.AddSubMenu("Q Config");
            qMenu.Add("autoQ2", new CheckBox("Auto Q"));
            qMenu.Add("autoQ", new CheckBox("Reduce Q usee"));

            wMenu = Config.AddSubMenu("W Config");
            wMenu.Add("autoW", new CheckBox("Auto W on hard CC"));
            wMenu.Add("telE", new CheckBox("Auto W teleport"));
            wMenu.Add("bushW", new CheckBox("Auto W bush"));
            wMenu.Add("Wspell", new CheckBox("W on special spell detection"));
            wMenu.Add("WmodeGC", new Slider("Gap Closer position mode (0 : Dash end position | 1 : My Hero Position)", 0, 0, 1));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                wMenu.Add("WGCchampion" + enemy.ChampionName, new CheckBox("Cast on enemy: " + enemy.ChampionName));

            eMenu = Config.AddSubMenu("E Config");
            eMenu.Add("autoE", new CheckBox("Auto E"));
            eMenu.Add("harrasEQ", new CheckBox("Harass E + Q"));
            eMenu.Add("EQks", new CheckBox("Ks E + Q + AA"));
            eMenu.Add("useE", new KeyBind("Dash E HotKeySmartcast", false, KeyBind.BindTypes.HoldActive, 'T'));
            eMenu.Add("EmodeGC", new Slider("Gap Closer position mode (0 : Dash end position | 1 : Cursor Pos | 2 : Enemy Position)", 2, 0, 2));
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.IsEnemy))
                eMenu.Add("EGCchampion" + enemy.ChampionName, new CheckBox("Cast on enemy:" + enemy.ChampionName));

            rMenu = Config.AddSubMenu("R Config");
            rMenu.Add("autoR", new CheckBox("Auto R KS"));
            rMenu.Add("Rcol", new Slider("R collision width [400]", 400, 1, 1000));
            rMenu.Add("Rrange", new Slider("R minimum range [1000]", 1000, 1, 1500));
            rMenu.Add("useR", new KeyBind("Semi-manual cast R key", false, KeyBind.BindTypes.HoldActive, 'T'));
            rMenu.Add("Rturrent", new CheckBox("Don't R under turret"));

            farmMenu = Config.AddSubMenu("Farm");
            farmMenu.Add("farmQ", new CheckBox("Lane clear Q"));
            farmMenu.Add("Mana", new Slider("LaneClear Mana", 80, 30, 100));
            farmMenu.Add("LCminions", new Slider("LaneClear minimum minions", 2, 0, 10));
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && (args.SData.Name == "CaitlynPiltoverPeacemaker" || args.SData.Name == "CaitlynEntrapment"))
            {
                QCastTime = Game.Time;
            }

            if (!W.IsReady() || sender.IsMinion || !sender.IsEnemy || !getCheckBoxItem(wMenu, "Wspell") || !sender.IsValid<AIHeroClient>() || !sender.IsValidTarget(W.Range))
                return;

            var foundSpell = Spells.Find(x => args.SData.Name.ToLower() == x);
            if (foundSpell != null)
            {
                W.Cast(sender.Position);
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(ActiveGapcloser gapcloser)
        {
            if (Player.Mana > RMANA + WMANA)
            {
                var t = gapcloser.Sender;
                if (E.IsReady() && t.IsValidTarget(E.Range) && getCheckBoxItem(eMenu, "EGCchampion" + t.ChampionName))
                {
                    if (getSliderItem(eMenu, "EmodeGC") == 0)
                        E.Cast(gapcloser.End);
                    else if (getSliderItem(eMenu, "EmodeGC") == 1)
                        E.Cast(Game.CursorPos);
                    else
                        E.Cast(t.ServerPosition);
                }
                else if (W.IsReady() && t.IsValidTarget(W.Range) && getCheckBoxItem(wMenu, "WGCchampion" + t.ChampionName))
                {
                    if (getSliderItem(wMenu, "WmodeGC") == 0)
                        W.Cast(gapcloser.End);
                    else
                        W.Cast(Player.ServerPosition);
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (getKeyBindItem(rMenu, "useR") && R.IsReady())
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                if (t.IsValidTarget())
                    R.CastOnUnit(t);
            }

            if (SebbyLib.Program.LagFree(0))
            {
                SetMana();
                R.Range = (500 * R.Level) + 1500;
            }

            if (SebbyLib.Program.LagFree(1) && E.IsReady() && !Orbwalker.IsAutoAttacking)
                LogicE();
            if (SebbyLib.Program.LagFree(2) && W.IsReady() && !Orbwalker.IsAutoAttacking)
                LogicW();
            if (SebbyLib.Program.LagFree(3) && Q.IsReady() && !Orbwalker.IsAutoAttacking && getCheckBoxItem(qMenu, "autoQ2"))
                LogicQ();
            if (SebbyLib.Program.LagFree(4) && R.IsReady() && getCheckBoxItem(rMenu, "autoR") && !ObjectManager.Player.UnderTurret(true) && Game.Time - QCastTime > 1)
                LogicR();
            return;
        }

        private static void LogicR()
        {
            bool cast = false;

            if (Player.UnderTurret(true) && getCheckBoxItem(rMenu, "Rturrent"))
                return;


            foreach (var target in SebbyLib.Program.Enemies.Where(target => target.IsValidTarget(R.Range) && Player.Distance(target.Position) > getSliderItem(rMenu, "Rrange") && target.CountEnemiesInRange(getSliderItem(rMenu, "Rcol")) == 1 && target.CountAlliesInRange(500) == 0 && OktwCommon.ValidUlt(target)))
            {
                if (OktwCommon.GetKsDamage(target, R) > target.Health)
                {
                    cast = true;
                    PredictionOutput output = R.GetPrediction(target);
                    Vector2 direction = output.CastPosition.To2D() - Player.Position.To2D();
                    direction.Normalize();
                    List<AIHeroClient> enemies = SebbyLib.Program.Enemies.Where(x => x.IsValidTarget()).ToList();
                    foreach (var enemy in enemies)
                    {
                        if (enemy.BaseSkinName == target.BaseSkinName || !cast)
                            continue;
                        PredictionOutput prediction = R.GetPrediction(enemy);
                        Vector3 predictedPosition = prediction.CastPosition;
                        Vector3 v = output.CastPosition - Player.ServerPosition;
                        Vector3 w = predictedPosition - Player.ServerPosition;
                        double c1 = Vector3.Dot(w, v);
                        double c2 = Vector3.Dot(v, v);
                        double b = c1 / c2;
                        Vector3 pb = Player.ServerPosition + ((float)b * v);
                        float length = Vector3.Distance(predictedPosition, pb);
                        if (length < (getSliderItem(rMenu, "Rcol") + enemy.BoundingRadius) && Player.Distance(predictedPosition) < Player.Distance(target.ServerPosition))
                            cast = false;
                    }
                    if (cast)
                        R.CastOnUnit(target);
                }
            }
        }

        private static void LogicW()
        {
            if (Player.Mana > RMANA + WMANA)
            {
                if (getCheckBoxItem(wMenu, "autoW"))
                    foreach (var enemy in SebbyLib.Program.Enemies.Where(enemy => enemy.IsValidTarget(W.Range) && !OktwCommon.CanMove(enemy) && !enemy.HasBuff("caitlynyordletrapinternal")))
                        W.Cast(enemy.Position, true);

                if (getCheckBoxItem(wMenu, "telE"))
                    foreach (var Object in ObjectManager.Get<Obj_AI_Base>().Where(Obj => Obj.Distance(Player.ServerPosition) < W.Range && Obj.Team != Player.Team && (Obj.HasBuff("teleport_target", true) || Obj.HasBuff("Pantheon_GrandSkyfall_Jump", true))))
                        W.Cast(Object.Position, true);
            }
        }

        private static void LogicQ()
        {
            var t = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if (t.IsValidTarget(Q.Range))
            {
                if (GetRealDistance(t) > bonusRange() + 250 && !SebbyLib.Orbwalking.InAutoAttackRange(t) && OktwCommon.GetKsDamage(t, Q) > t.Health && Player.CountEnemiesInRange(400) == 0)
                {
                    SebbyLib.Program.CastSpell(Q, t);
                    SebbyLib.Program.debug("Q KS");
                }
                else if (SebbyLib.Program.Combo && Player.Mana > RMANA + QMANA + EMANA + 10 && Player.CountEnemiesInRange(bonusRange() + 100 + t.BoundingRadius) == 0 && !getCheckBoxItem(qMenu, "autoQ"))
                    SebbyLib.Program.CastSpell(Q, t);
                if ((SebbyLib.Program.Combo || SebbyLib.Program.Farm) && Player.Mana > RMANA + QMANA && Player.CountEnemiesInRange(400) == 0)
                {
                    foreach (var enemy in SebbyLib.Program.Enemies.Where(enemy => enemy.IsValidTarget(Q.Range) && (!OktwCommon.CanMove(enemy) || enemy.HasBuff("caitlynyordletrapinternal"))))
                        Q.Cast(enemy, true);
                    if (Player.CountEnemiesInRange(bonusRange()) == 0 && OktwCommon.CanHarras())
                    {
                        if (t.HasBuffOfType(BuffType.Slow))
                            Q.Cast(t);

                        Q.CastIfWillHit(t, 2, true);
                    }
                }
            }
            else if (SebbyLib.Program.LaneClear && Player.ManaPercent > getSliderItem(farmMenu, "Mana") && getCheckBoxItem(farmMenu, "farmQ") && Player.Mana > RMANA + QMANA)
            {
                var minionList = Cache.GetMinions(Player.ServerPosition, Q.Range);
                var farmPosition = Q.GetLineFarmLocation(minionList, Q.Width);
                if (farmPosition.MinionsHit > getSliderItem(farmMenu, "LCminions"))
                    Q.Cast(farmPosition.Position);
            }
        }

        private static void LogicE()
        {
            if (getCheckBoxItem(eMenu, "autoE"))
            {
                var t = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                if (t.IsValidTarget())
                {
                    var positionT = Player.ServerPosition - (t.Position - Player.ServerPosition);

                    if (Q.IsReady() && Player.Position.Extend(positionT, 400).CountEnemiesInRange(700) < 2)
                    {
                        var eDmg = E.GetDamage(t);
                        var qDmg = Q.GetDamage(t);
                        if (getCheckBoxItem(eMenu, "EQks") && qDmg + eDmg + Player.GetAutoAttackDamage(t) > t.Health && Player.Mana > EMANA + QMANA)
                        {
                            SebbyLib.Program.CastSpell(E, t);
                            SebbyLib.Program.debug("E + Q FINISH");
                        }
                        else if ((SebbyLib.Program.Farm || SebbyLib.Program.Combo) && getCheckBoxItem(eMenu, "harrasEQ") && Player.Mana > EMANA + QMANA + RMANA)
                        {
                            SebbyLib.Program.CastSpell(E, t);
                            SebbyLib.Program.debug("E + Q Harras");
                        }
                    }

                    if (Player.Mana > RMANA + EMANA && Player.Health < Player.MaxHealth * 0.3)
                    {
                        if (GetRealDistance(t) < 500)
                            E.Cast(t, true);
                        if (Player.CountEnemiesInRange(250) > 0)
                            E.Cast(t, true);
                    }

                }
            }
            if (getKeyBindItem(eMenu, "useE"))
            {
                var position = Player.ServerPosition - (Game.CursorPos - Player.ServerPosition);
                E.Cast(position, true);
            }
        }

        private static float GetRealRange(GameObject target)
        {
            return 680f + Player.BoundingRadius + target.BoundingRadius;
        }

        private static float GetRealDistance(GameObject target)
        {
            return Player.ServerPosition.Distance(target.Position) + ObjectManager.Player.BoundingRadius + target.BoundingRadius;
        }
        public static float bonusRange()
        {
            return 720f + Player.BoundingRadius;
        }
        private static void SetMana()
        {
            if ((SebbyLib.Program.getCheckBoxItem("manaDisable") && SebbyLib.Program.Combo) || Player.HealthPercent < 20)
            {
                QMANA = 0;
                WMANA = 0;
                EMANA = 0;
                RMANA = 0;
                return;
            }

            QMANA = Q.Instance.SData.Mana;
            WMANA = W.Instance.SData.Mana;
            EMANA = E.Instance.SData.Mana;

            if (!R.IsReady())
                RMANA = QMANA - Player.PARRegenRate * Q.Instance.Cooldown;
            else
                RMANA = R.Instance.SData.Mana;
        }

        public static void drawLine(Vector3 pos1, Vector3 pos2, int bold, System.Drawing.Color color)
        {
            var wts1 = Drawing.WorldToScreen(pos1);
            var wts2 = Drawing.WorldToScreen(pos2);

            Drawing.DrawLine(wts1[0], wts1[1], wts2[0], wts2[1], bold, color);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

            if (getCheckBoxItem(drawMenu, "qRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (Q.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Cyan, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "wRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (W.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, W.Range, System.Drawing.Color.Orange, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "eRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (E.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, E.Range, System.Drawing.Color.Yellow, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "rRange"))
            {
                if (getCheckBoxItem(drawMenu, "onlyRdy"))
                {
                    if (R.IsReady())
                        LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
                }
                else
                    LeagueSharp.Common.Utility.DrawCircle(ObjectManager.Player.Position, R.Range, System.Drawing.Color.Gray, 1, 1);
            }
            if (getCheckBoxItem(drawMenu, "noti"))
            {
                var t = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (t.IsValidTarget() && R.IsReady())
                {
                    var rDamage = R.GetDamage(t);
                    if (rDamage > t.Health)
                    {
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.5f, System.Drawing.Color.Red, "Ult can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                        drawLine(t.Position, Player.Position, 10, System.Drawing.Color.Yellow);
                    }
                }

                var tw = TargetSelector.GetTarget(W.Range, DamageType.Physical);
                if (tw.IsValidTarget())
                {
                    if (Q.GetDamage(tw) > tw.Health)
                        Drawing.DrawText(Drawing.Width * 0.1f, Drawing.Height * 0.4f, System.Drawing.Color.Red, "Q can kill: " + t.ChampionName + " have: " + t.Health + "hp");
                }
            }
        }
    }
}

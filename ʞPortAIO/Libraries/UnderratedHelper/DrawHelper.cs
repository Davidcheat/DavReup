using System.Drawing;
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

namespace UnderratedAIO.Helpers
{
    public class DrawHelper
    {
        public static AIHeroClient player = ObjectManager.Player;

        public static void DrawCircle(Circle circle, float spellRange)
        {
            if (circle.Active) Render.Circle.DrawCircle(player.Position, spellRange, circle.Color);
            
        }

        public static void popUp(string text, int time, Color fontColor ,Color boxColor, Color borderColor)
        {
            var popUp = new Notification(text).SetTextColor(fontColor);
            popUp.SetBoxColor(boxColor);
            popUp.SetBorderColor(borderColor);
            Notifications.AddNotification(popUp);
            LeagueSharp.Common.Utility.DelayAction.Add(time, () => popUp.Dispose());
        }
    }
}
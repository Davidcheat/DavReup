#region

using System;
using System.Collections.Generic;
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

#endregion

namespace UnderratedAIO.Helpers.SkillShot
{
    public static class Extensions {}

    public class SpellList<T> : List<T>
    {
        public event EventHandler OnAdd;
        public event EventHandler OnRemove;

        public new void Add(T item)
        {
            if (OnAdd != null)
            {
                OnAdd(this, null); // TODO: return item
            }

            base.Add(item);
        }

        public new void Remove(T item)
        {
            if (OnRemove != null)
            {
                OnRemove(this, null); // TODO: return item
            }

            base.Remove(item);
        }

        public new void RemoveAll(Predicate<T> match)
        {
            if (OnRemove != null)
            {
                OnRemove(this, null); // TODO: return items
            }

            base.RemoveAll(match);
        }
    }
}
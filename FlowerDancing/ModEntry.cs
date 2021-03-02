using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace FlowerDancing
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            EventPatched.Initialize(Monitor);
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            harmony.Patch
            (
               original: AccessTools.Method(typeof(StardewValley.Event), nameof(StardewValley.Event.setUpFestivalMainEvent)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpFestivalMainEvent_Kelly))
            );
			
			harmony.Patch(
               original: AccessTools.Method(typeof(Event), nameof(Event.setUpPlayerControlSequence)),
               postfix: new HarmonyMethod(typeof(EventPatched), nameof(EventPatched.setUpPlayerControlSequence_Kelly))
		    );

            Monitor.Log("Kelly's Flower Dancing started using Harmony.", LogLevel.Debug);
        }
    }
}

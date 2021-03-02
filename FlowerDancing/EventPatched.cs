using StardewModdingAPI;
using StardewValley;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerDancing
{
    public class EventPatched
    {
		public static IMonitor Monitor;
		private static bool isMalePosition;

		public static void Initialize(IMonitor monitor)
		{
			Monitor = monitor;
		}

		public static void setUpPlayerControlSequence_Kelly(Event __instance, string id)
        {
			try
			{
				// using the default condition for the method
				if(Monitor is null || __instance is null || id != "flowerFestival")
				{
					return;
				}
				
				Hat hat = null;
				Clothing shirt = null;
				Clothing pants = null;
				Boots boots = null;

				// undress without actually taking off the items

				if (Game1.player?.hat?.Value != null)
				{
					hat = Game1.player.hat.Value;
					Game1.player.hat.Value = null;
				}

				if (Game1.player?.shirtItem?.Value != null)
				{
					shirt = Game1.player.shirtItem.Value;
					Game1.player.shirtItem.Value = null;
				}

				if (Game1.player?.pantsItem?.Value != null)
				{
					pants = Game1.player.pantsItem.Value;
					Game1.player.pantsItem.Value = null;
				}

				if (Game1.player?.boots?.Value != null)
				{
					boots = Game1.player.boots.Value;
					Game1.player.boots.Set(null);

					Game1.player.changeShoeColor(12);
				}

				// reequip the clothes

				__instance.onEventFinished += delegate
				{
					if (hat != null)
					{
						Game1.player.hat.Value = hat;
					}
					if (shirt != null)
					{
						Game1.player.shirtItem.Value = shirt;
					}
					if (pants != null)
					{
						Game1.player.pantsItem.Value = pants;
					}
					if (boots != null)
					{
						Game1.player.boots.Set(boots);
						if (boots != null)
						{
							Game1.player.changeShoeColor(boots.indexInColorSheet);
						}
					}
				};
			}
			catch(Exception e)
			{
				Monitor.Log($"Failed in {nameof(EventPatched)}:\n{e}", LogLevel.Error);
			}
        }

		// TODO I think you currently make the players invisible if they are not participating

		public static void setUpFestivalMainEvent_Kelly(StardewValley.Event __instance)
		{
			try
			{
				// using the default condition for the method
				if(Monitor is null || __instance is null || !__instance.isSpecificFestival("spring24"))
				{
					return;
				}
				
				isMalePosition = true;
				string[] eventCommands = __instance.eventCommands;

				for(int i = 0; i < eventCommands.Length; i++)
				{
					string curr = eventCommands[i];

					if (!curr.Contains("farmer"))
					{
						continue;
					}

					curr = fixFarmerDance(curr);
					eventCommands[i] = curr;
				}

				if(!isMalePosition) // Make the farmer (if they are in the female position) do a final pose with their arms in the air
				{
					string farmerFinalPose = "showFrame 96";
					string[] newEventCommands = new string[eventCommands.Length + 1];

					for (int i = 0; i < newEventCommands.Length; i++)
					{
						if (i < 291)
						{
							newEventCommands[i] = eventCommands[i];
						}

						else if (i == 291)
						{
							newEventCommands[i] = farmerFinalPose;
						}

						else
						{
							newEventCommands[i] = eventCommands[i - 1];
						}
					}

					__instance.eventCommands = newEventCommands;
				}
			}
			catch(Exception e)
			{
				Monitor.Log($"Failed in {nameof(EventPatched)}:\n{e}", LogLevel.Error);
			}
		}

		private static string fixFarmerDance(string curr)
		{
			curr = fixWarps(curr);
			curr = fixAnimates(curr);

			return curr;
		}

		private static string fixWarps(string curr)
		{
			if(curr.Contains("warp"))
			{
				curr = curr.Replace("21", "24"); // Move Girl Pos Down
			}

			return curr;
		}

		private static string fixAnimates(string curr)
		{
			if(!curr.Contains("animate"))
			{
				return curr;
			}

			string farmerName = "";
			string[] words = curr.Split(' ');

			foreach (string word in words)
			{
				if (word.Contains("farmer"))
				{
					farmerName = word;
					break;
				}
			}

			if(curr.Contains("animate " + farmerName + " false true 596 4 0"))
			{
				isMalePosition = false;
			}

			if (!isMalePosition)
			{
				curr = curr.Replace("animate " + farmerName + " false true 596 4 0", "animate " + farmerName + " false true 596 95 67 95 70"); // Phase 1 - Animate Girl
				curr = curr.Replace("animate " + farmerName + " false true 600 0 3", "animate " + farmerName + " false true 600 96 100"); // Phase 2 - Animate Girl
			}

			else
			{
				curr = curr.Replace("animate " + farmerName + " false true 600 12 13 12 14", "animate " + farmerName + " false true 600 12 33"); // Phase 1 - Animate Guy
				curr = curr.Replace("animate " + farmerName + " false true 150 12 13 12 14", "animate " + farmerName + " false true 300 13 14"); // Phase 2 - Animate Guy
			}

			return curr;
		}
	}
}

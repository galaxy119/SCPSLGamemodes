using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using Smod2.API;
using Smod2.Commands;

namespace MassacreGamemode
{
	public class Functions
	{
		private readonly Massacre plugin;

		public Functions(Massacre plugin) => this.plugin = plugin;
		
		public bool IsAllowed(ICommandSender sender)
		{
			if (!(sender is Player player)) 
				return true;
			List<string> roleList = plugin.ValidRanks != null && plugin.ValidRanks.Length > 0 ? plugin.ValidRanks.Select(role => role.ToLower()).ToList() : new List<string>();

			if (roleList.Count > 0 && (roleList.Contains(player.GetUserGroup().Name.ToLower()) || roleList.Contains(player.GetRankName().ToLower())))
				return true;
			return roleList.Count == 0;
		}

		public void EnableGamemode()
		{
			plugin.Enabled = true;
			if (!plugin.RoundStarted)
			{
				plugin.Server.Map.ClearBroadcasts();
				plugin.Server.Map.Broadcast(10, "<color=#50c878>Massacre Gamemode</color> is starting...", false);
			}
		}

		public void DisableGamemode()
		{
			plugin.Enabled = false;
			plugin.Server.Map.ClearBroadcasts();
		}
		

		public Vector SpawnLoc()
		{
			Vector spawn;

			switch (plugin.SpawnRoom.ToLower())
			{
				case "jail":
					{
						plugin.Info("Jail room selected.");
						spawn = new Vector(53, 1020, -44);

						return spawn;
					}
				case "939":
					{
						plugin.Info("939 Spawn Room selected");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53);

						return spawn;
					}
				case "049":
					{
						plugin.Info("049 Spawn room selected");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049);

						return spawn;
					}
				case "106":
					{
						plugin.Info("106 Spawn room selected");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106);

						return spawn;
					}
				case "173":
					{
						plugin.Info("173 Spawn room selected.");
						spawn = plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173);

						return spawn;
					}
				case "random":
					{
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_939_53));
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_173));
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_049));
						plugin.SpawnLocs.Add(plugin.Server.Map.GetRandomSpawnPoint(Role.SCP_106));
						plugin.SpawnLocs.Add(new Vector(53, 1020, -44));

						int randomInt = new Random().Next(plugin.SpawnLocs.Count);

						return plugin.SpawnLocs[randomInt];
					}
				default:
					{
						plugin.Info("Invalid location selected, defaulting to Jail.");
						spawn = new Vector(53, 1020, -44);

						return spawn;
					}
			}
		}
		public IEnumerator<float> SpawnDboi(Player player)
		{
			foreach (Item item in player.GetInventory()) 
				item.Remove();
			player.ChangeRole(Role.CLASSD, false, false, false, true);

			player.Teleport(plugin.SpawnLoc);

			yield return Timing.WaitForSeconds(1f);

			foreach (Item item in player.GetInventory()) 
				item.Remove();

			player.GiveItem(ItemType.FLASHLIGHT);
			player.GiveItem(ItemType.CUP);

			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(25, "You are a <color=#ffa41a>D-Boi</color>! Get ready to die!", false);
		}
		
		public IEnumerator<float> SpawnNut(Player player)
		{
			player.ChangeRole(Role.SCP_173, false, false, false);

			yield return Timing.WaitForSeconds(5.5f);

			player.SetGodmode(false);

			player.Teleport(plugin.SpawnLoc);

			plugin.Info("Spawned " + player.Name + " as SCP-173");
			player.PersonalClearBroadcasts();
			player.PersonalBroadcast(35, "You are a <color=#c50000>Neck-Snappy Boi</color>! Kill all of the D-bois!", false);

			player.SetHealth(plugin.NutHealth > 0? plugin.NutHealth: 1);
		}
		
		public void EndGamemodeRound()
		{
			plugin.Info("EndgameRound Function");
			plugin.RoundStarted = false;
			plugin.Server.Round.EndRound();
		}
	}
}
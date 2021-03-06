using Smod2.Commands;

namespace MassacreGamemode
{
	internal class Commands : ICommandHandler
	{
		private readonly Massacre plugin;
		public Commands(Massacre plugin) => this.plugin = plugin;

		public string[] OnCall(ICommandSender sender, string[] args)
		{
			if (args.Length <= 0) return new string[] { GetUsage() };
			if (!plugin.Functions.IsAllowed(sender)) return new string[] { "Permission Denied." };
			
			switch (args[0].ToLower())
			{
				case "help":
					return new[]
					{
						"Massacre in the Dark command list:", 
						"mass enable - Enables the gamemode.",
						"mass disable - Disables the gamemode."
					};
				case "enable":
					plugin.Functions.EnableGamemode();
					return new[] { "Massacre of the D-Bois gamemode enabled." };
				case "disable":
					plugin.Functions.DisableGamemode();
					return new[] { "Massacre of the D-Bois gamemode disabled." };
				case "spawn":
				{
					switch (args[1].ToLower())
					{
						case "939":
							plugin.SpawnRoom = "939";
							return new[] { "SCP-939 spawn location selected." };
						case "049":
							plugin.SpawnRoom = "049";
							return new[] { "SCP-049 spawn location selected." };
						case "173":
							plugin.SpawnRoom = "173";
							return new[] { "SCP-173 spawn location selected." };
						case "jail":
							plugin.SpawnRoom = "jail";
							return new[] { "Jail spawn location selected." };
						case "rand":
						case "random":
							plugin.SpawnRoom = "random";
							return new[] { "Random spawn location selected." };
						default:
							return new[] { GetUsage() };
					}
				}
				default:
					return new[] { GetUsage() };
			}
		}

		public string GetUsage() => "A command argument must be used.";

		public string GetCommandDescription() => "";
	}
}
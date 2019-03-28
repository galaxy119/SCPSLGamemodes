using Smod2;
using Smod2.API;
using Smod2.Events;
using Smod2.EventSystem;
using Smod2.EventHandlers;
using System.Collections.Generic;
using scp4aiur;
using UnityEngine;

namespace Gungame
{
    internal class EventsHandler : IEventHandlerRoundStart, IEventHandlerRoundEnd, IEventHandlerPlayerDie, IEventHandlerPlayerJoin, IEventHandlerCheckRoundEnd, IEventHandlerThrowGrenade, IEventHandlerPlayerHurt, IEventHandlerWaitingForPlayers
    {
        private readonly GunGame plugin;

        public EventsHandler(GunGame plugin) => this.plugin = plugin;

        public void OnWaitingForPlayers(WaitingForPlayersEvent ev)
        {
            plugin.ReloadConfig();
        }

        public void OnRoundStart(RoundStartEvent ev)
        {
            if (!plugin.Enabled) return;

            plugin.RoundStarted = true;
            List<Player> players = ev.Server.GetPlayers();

            foreach (Player player in players)
            {
                Timing.Run(plugin.Functions.Spawn(player));
                (player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
            }
        }

        public void OnPlayerJoin(PlayerJoinEvent ev)
        {
            if (!plugin.RoundStarted) return;

            (ev.Player.GetGameObject() as GameObject).GetComponent<WeaponManager>().NetworkfriendlyFire = true;
            Timing.Run(plugin.Functions.Spawn(ev.Player));
        }

        public void OnThrowGrenade(PlayerThrowGrenadeEvent ev)
        {
            if (!plugin.Enabled && !plugin.RoundStarted) return;

            ev.Player.GiveItem(ItemType.FRAG_GRENADE);
        }

        public void OnPlayerDie(PlayerDeathEvent ev)
        {
            if (!plugin.Enabled && !plugin.RoundStarted) return;

            plugin.Functions.ReplaceGun(ev.Killer);

            if (!plugin.Reversed && ev.DamageTypeVar == DamageType.E11_STANDARD_RIFLE)
            {
                plugin.Functions.AnnounceWinner(ev.Killer);
                plugin.Winner = ev.Killer;
            }
            else if (plugin.Reversed && ev.DamageTypeVar == DamageType.FRAG)
            {
                plugin.Functions.AnnounceWinner(ev.Killer);
                plugin.Winner = ev.Killer;
            }
            else
                Timing.Run(plugin.Functions.Spawn(ev.Player));
        }

        public void OnPlayerHurt(PlayerHurtEvent ev)
        {
            if (!plugin.Enabled && !plugin.RoundStarted) return;

            if (ev.Player.SteamId == ev.Attacker.SteamId && ev.DamageType == DamageType.FRAG)
                ev.Damage = 0;

        }

        public void OnCheckRoundEnd(CheckRoundEndEvent ev)
        {
            if (!plugin.Enabled && !plugin.RoundStarted) return;

            if (!(plugin.Winner is Player))
                ev.Status = ROUND_END_STATUS.ON_GOING;
        }

        public void OnRoundEnd(RoundEndEvent ev)
        {
            if (!plugin.Enabled && !plugin.RoundStarted) return;

            plugin.Functions.EndGamemodeRound();
        }
    }
}
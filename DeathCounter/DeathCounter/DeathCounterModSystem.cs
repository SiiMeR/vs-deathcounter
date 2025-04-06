using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace DeathCounter;

public record DeathDescriptor(string Reason, string DeathTimestamp);

public record PlayerDeathData(int DeathCount, List<DeathDescriptor> Deaths)
{
    public int DeathCount { get; set; } = DeathCount;
}

public class DeathCounterModSystem : ModSystem
{
    private ICoreServerAPI _api;


    public override void StartServerSide(ICoreServerAPI api)
    {
        _api = api;

        _api.Event.PlayerDeath += (player, source) =>
        {
            if(!player.ServerData.CustomPlayerData.ContainsKey("PlayerDeaths"))
            {
                player.ServerData.CustomPlayerData.Add("PlayerDeaths",
                    JsonUtil.ToPrettyString(new PlayerDeathData(0, new List<DeathDescriptor>())));
            }
           

            var deathData = JsonUtil.FromString<PlayerDeathData>(player.ServerData.CustomPlayerData["PlayerDeaths"]);

            deathData.Deaths.Add(new DeathDescriptor( source.Source.ToString(),DateTime.UtcNow.ToString("o")));
            deathData.DeathCount = deathData.Deaths.Count;

            player.ServerData.CustomPlayerData["PlayerDeaths"] = JsonUtil.ToString(deathData);
        };
    }
}
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;
using TNCSSPluginFoundation;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Util;

public static class LoggingExtension
{
    public static void LogAdminAction(this TncssPluginBase plugin, CCSPlayerController? player, string log)
    {
        plugin.Logger.LogInformation($"[{PlayerUtil.GetPlayerName(player)}:{player?.SteamID ?? 0}]: {log}");
        
        // TODO() Add database save logic
    }
}
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Entities;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.UserManagement.Command;

public sealed class UserList(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_users";
    public override string CommandDescription => "Show list of users online and details";

    protected override ICommandValidator GetValidator() => new PermissionValidator("css/generic");

    private const int WidthPlayerType = 8;
    private const int WidthIsAlive = 8;
    private const int WidthPlayerSlot = 8;
    private const int WidthSteamId = 20;
    private const int WidthIpAddress = 24;
    private const int WidthPing = 5;
    private const int MaxPlayerNameLength = 36;

    private const string ConsoleLineSeparator = "-------------------------------------------------------------------------------------------------------------";
    
    // tuna: I've shortened the variable name because it was too long.
    // _os means _outputString
    private static readonly string OsPlayerType = "Type".PadRightByWidth(WidthPlayerType);
    private static readonly string OsIsAlive = "Alive".PadRightByWidth(WidthIsAlive);
    private static readonly string OsPlayerName = "Name".PadRightByWidth(MaxPlayerNameLength);
    private static readonly string OsPlayerSlot = "Slot".PadRightByWidth(WidthPlayerSlot);
    private static readonly string OsSteamId = "SteamId64".PadRightByWidth(WidthSteamId);
    private static readonly string OsIpAddress = "IpAddress".PadRightByWidth(WidthIpAddress);
    private static readonly string OsPing = "Ping".PadRightByWidth(WidthPing);
    
    
    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Plugin.LogAdminAction(context.Player, $"Issued a {CommandName}, but failed in validator {context.Validator.ValidatorName}");
        switch (context.Validator)
        {
            case PermissionValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.Permission"));
                return ValidationFailureResult.SilentAbort();
        }
        
        return ValidationFailureResult.UseDefaultFallback();
    }

    protected override void ExecuteCommand(CCSPlayerController? player, CommandInfo commandInfo, ValidatedArguments? validatedArguments)
    {
        if (player == null)
            return;
        
        bool isAdminHasRootRole = AdminManager.PlayerHasPermissions(player, "css/root");

        if (isAdminHasRootRole)
        {
            player.PrintToConsole($"{OsPlayerType}{OsIsAlive}{OsPlayerName}{OsPlayerSlot}{OsSteamId}{OsIpAddress}{OsPing}");
        }
        else
        {
            player.PrintToConsole($"{OsPlayerType}{OsIsAlive}{OsPlayerName}{OsPlayerSlot}{OsSteamId}{OsPing}");
        }

        
        player.PrintToConsole(ConsoleLineSeparator);
        
        foreach (CCSPlayerController cl in Utilities.GetPlayers())
        {
            string playerType = GetPlayerTypeName(cl).PadRightByWidth(WidthPlayerType);
            
            string isAlive = PlayerUtil.IsPlayerAlive(cl) ? "Alive" : "Dead";
            isAlive = isAlive.PadRightByWidth(WidthIsAlive);

            string playerName = cl.PlayerName.TruncateByWidth(MaxPlayerNameLength).PadRightByWidth(MaxPlayerNameLength);
            
            string playerSlot = cl.Slot.ToString().PadRightByWidth(WidthPlayerSlot);

            ulong clSteamId = cl.SteamID;

            var playerSteamId = clSteamId.ToString().PadRightByWidth(WidthSteamId);

            string playerIp = cl.IpAddress ?? "NONE";
            playerIp = playerIp.PadRightByWidth(WidthIpAddress);

            string playerPing = cl.Ping.ToString().PadRightByWidth(WidthPing);


            if (isAdminHasRootRole)
            {
                player.PrintToConsole($"{playerType}{isAlive}{playerName}{playerSlot}{playerSteamId}{playerIp}{playerPing}");
            }
            else
            {
                player.PrintToConsole($"{playerType}{isAlive}{playerName}{playerSlot}{playerSteamId}{playerPing}");
            }
        }
        
        player.PrintToChat(LocalizeWithPluginPrefix(player, "Common.Command.Notification.SeeClientConsoleOutput"));
    }
    
    private string GetPlayerTypeName(CCSPlayerController client)
    {
        string result;

        if (client.IsBot)
        {
            result = "Bot";
        }
        else if (client.IsHLTV)
        {
            result = "HLTV";
        }
        else
        {
            result = "Player";
        }
        
        return result;
    }
}
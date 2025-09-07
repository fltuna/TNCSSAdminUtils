using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.InGameManagement.Commands;

public sealed class RespawnPlayer(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_respawn";
    public override string CommandDescription => "Respawn a player";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new PermissionValidator("css/slay"))
        .Add(new ArgumentCountValidator(1, true))
        .Add(new ExtendedTargetValidator(1, true));

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Plugin.LogAdminAction(context.Player, $"Issued a {CommandName}, but failed in validator {context.Validator.ValidatorName}");
        switch (context.Validator)
        {
            case PermissionValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.Permission"));
                return ValidationFailureResult.SilentAbort();
            
            case ArgumentCountValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Respawn.Command.Notification.Usage"));
                return ValidationFailureResult.SilentAbort();
            
            case ExtendedTargetValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.ExtendedTarget"));
                return ValidationFailureResult.SilentAbort();
        }
        
        return ValidationFailureResult.UseDefaultFallback();
    }

    protected override void ExecuteCommand(CCSPlayerController? player, CommandInfo commandInfo, ValidatedArguments? validatedArguments)
    {
        var targets = validatedArguments!.GetArgument<TargetResult>(1)!;
        
        string executorName = PlayerUtil.GetPlayerName(player);
        int actualRespawnCount = 0;
        
        foreach(CCSPlayerController target in targets) {
            if(target.IsHLTV)
                continue;
                
            if (PlayerUtil.IsPlayerAlive(target))
                continue;
                
            if (target.Team is CsTeam.None or CsTeam.Spectator)
                continue;
            
            target.PrintToChat(LocalizeWithPluginPrefix(player, "Respawn.Command.Notification.YouHaveRespawned", executorName));
            target.Respawn();
            actualRespawnCount++;
        }
        
        
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(commandInfo.GetArg(1));

        if (hasTypedTargets && actualRespawnCount >= 1)
        {
            string targetName = LocalizeString(player, TargetTypeStringConverter.GetTargetTypeName(commandInfo.GetArg(1)));
            
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and respawned {targetName}");
            PrintLocalizedChatToAll("Respawn.Command.Broadcast.PlayerRespawned", executorName, targetName);
            return;
        }
        
        if (actualRespawnCount <= 0)
        {
            Plugin.LogAdminAction(player, $"Issued a {CommandName} but no dead players found");
            PrintMessageToServerOrPlayerChat(player, LocalizeWithPluginPrefix(player, "Respawn.Command.Notification.NoDeadPlayersFound"));
            return;
        }
        
        if (targets.Count() > 1)
        {
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and respawned {actualRespawnCount} players");
            PrintLocalizedChatToAll("Respawn.Command.Broadcast.MultiplePlayerRespawned", executorName, actualRespawnCount);
            return;
        }
        
        Plugin.LogAdminAction(player, $"Issued a {CommandName} and respawned {targets.First().PlayerName}");
        PrintLocalizedChatToAll("Respawn.Command.Broadcast.PlayerRespawned", executorName, targets.First().PlayerName);
    }
}
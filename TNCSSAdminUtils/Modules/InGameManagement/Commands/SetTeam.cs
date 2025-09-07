using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Models.Command.Validators.RangedValidators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.InGameManagement.Commands;

public sealed class SetTeam(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_team";
    public override string CommandDescription => "Set team of player";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new PermissionValidator("css/slay"))
        .Add(new ArgumentCountValidator(2, true))
        .Add(new ExtendedTargetValidator(1, true))
        .Add(new RangedArgumentValidator<int>((int)CsTeam.Spectator, (int)CsTeam.CounterTerrorist, 2, 0, true));

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Plugin.LogAdminAction(context.Player, $"Issued a {CommandName}, but failed in validator {context.Validator.ValidatorName}");
        switch (context.Validator)
        {
            case PermissionValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.Permission"));
                return ValidationFailureResult.SilentAbort();
            
            case ArgumentCountValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "SetTeam.Command.Notification.Usage"));
                return ValidationFailureResult.SilentAbort();
            
            case ExtendedTargetValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.ExtendedTarget"));
                return ValidationFailureResult.SilentAbort();
            
            case RangedArgumentValidator<int> rangedValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.ArgumentOutOfRange", rangedValidator.GetRangeDescription()));
                return ValidationFailureResult.SilentAbort();
        }
        
        return ValidationFailureResult.UseDefaultFallback();
    }

    protected override void ExecuteCommand(CCSPlayerController? player, CommandInfo commandInfo, ValidatedArguments? validatedArguments)
    {
        var targets = validatedArguments!.GetArgument<TargetResult>(1)!;
        var newTeam = (CsTeam)validatedArguments.GetArgument<int>(2);
        
        string executorName = PlayerUtil.GetPlayerName(player);
        int actualExecutedTargets = 0;
        
        foreach(CCSPlayerController target in targets) {
            if(target.IsHLTV)
                continue;

            if (newTeam == CsTeam.Spectator && target.Team != CsTeam.Spectator)
            {
                target.CommitSuicide(false, true);
            }

            if (target.Team == CsTeam.Spectator && newTeam != CsTeam.Spectator)
            {
                MovePlayerFromSpectator(target, newTeam);
            }
            
            PlayerUtil.SetPlayerTeam(target, newTeam);
            
            actualExecutedTargets++;
        }
        
                
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(commandInfo.GetArg(1));

        if (hasTypedTargets && actualExecutedTargets >= 2)
        {
            string targetName = LocalizeString(player, TargetTypeStringConverter.GetTargetTypeName(commandInfo.GetArg(1)));
            
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targetName} health to {newTeam}");
            PrintLocalizedChatToAll("SetTeam.Command.Broadcast.SetTeam", executorName, targetName, newTeam);
            return;
        }
        
        if (actualExecutedTargets <= 0)
        {
            Plugin.LogAdminAction(player, $"Issued a {CommandName} but no players were affected");
            PrintMessageToServerOrPlayerChat(player, LocalizeWithPluginPrefix(player, "Common.Command.Notification.NoPlayersWereAffected"));
            return;
        }
        
        if (targets.Count() > 1)
        {
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {actualExecutedTargets} players health to {newTeam}");
            PrintLocalizedChatToAll("SetTeam.Command.Broadcast.MultipleSetTeam", executorName, actualExecutedTargets, newTeam);
            return;
        }
        
        Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targets.First().PlayerName}'s health to {newTeam}");
        PrintLocalizedChatToAll("SetTeam.Command.Broadcast.SetTeam", executorName, targets.First().PlayerName, newTeam);
    }
    
    
    
    
    // This is the detour method for player is not move team correctly
    // When player is moved from spectator sometimes player is stuck at world origin.
    private void MovePlayerFromSpectator(CCSPlayerController client, CsTeam targetTeam)
    {
        PlayerUtil.SetPlayerTeam(client, targetTeam);
        Server.NextFrame(() =>
        {
            client.Respawn();
            client.CommitSuicide(false, true);
            Server.NextFrame(() =>
            {
                PlayerUtil.SetPlayerTeam(client, CsTeam.Spectator);
                Server.NextFrame(() =>
                {
                    PlayerUtil.SetPlayerTeam(client, targetTeam);
                });
            });
        });
    }
}
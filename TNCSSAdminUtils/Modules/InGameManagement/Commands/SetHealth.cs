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

public sealed class SetHealth(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_hp";
    public override string CommandDescription => "Set health of player";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new PermissionValidator("css/slay"))
        .Add(new ArgumentCountValidator(2, true))
        .Add(new ExtendedTargetValidator(1, true))
        .Add(new RangedArgumentValidator<int>(1, 99999999, 2, 0, true));

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Plugin.LogAdminAction(context.Player, $"Issued a {CommandName}, but failed in validator {context.Validator.ValidatorName}");
        switch (context.Validator)
        {
            case PermissionValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.Permission"));
                return ValidationFailureResult.SilentAbort();
            
            case ArgumentCountValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "SetHealth.Command.Notification.Usage"));
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
        int newHealth = validatedArguments.GetArgument<int>(2);
        
        string executorName = PlayerUtil.GetPlayerName(player);
        int actualExecutedTargets = 0;
        
        foreach(CCSPlayerController target in targets) {
            if(target.IsHLTV)
                continue;
                
            if (!PlayerUtil.IsPlayerAlive(target))
                continue;
                
            if (target.Team is CsTeam.None or CsTeam.Spectator)
                continue;
            
            if (!PlayerUtil.SetPlayerHealth(target, newHealth)
                || !PlayerUtil.SetPlayerMaxHealth(target, newHealth))
                continue;
                
            actualExecutedTargets++;
        }
        
                
        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(commandInfo.GetArg(1));

        if (hasTypedTargets && actualExecutedTargets >= 2)
        {
            string targetName = LocalizeString(player, TargetTypeStringConverter.GetTargetTypeName(commandInfo.GetArg(1)));
            
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targetName} health to {newHealth}");
            PrintLocalizedChatToAll("SetHealth.Command.Broadcast.SetHealth", executorName, targetName, newHealth);
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
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {actualExecutedTargets} players health to {newHealth}");
            PrintLocalizedChatToAll("SetHealth.Command.Broadcast.MultipleSetHealth", executorName, actualExecutedTargets, newHealth);
            return;
        }
        
        Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targets.First().PlayerName}'s health to {newHealth}");
        PrintLocalizedChatToAll("SetHealth.Command.Broadcast.SetHealth", executorName, targets.First().PlayerName, newHealth);
    }
}
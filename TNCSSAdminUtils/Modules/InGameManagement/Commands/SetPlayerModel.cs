using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using TNCSSAdminUtils.Util;
using TNCSSAdminUtils.Validators;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.InGameManagement.Commands;

public sealed class SetPlayerModel(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_setmodel";
    public override string CommandDescription => "Set player model of player";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new PermissionValidator("css/generic"))
        .Add(new ArgumentCountValidator(2, true))
        .Add(new ExtendedTargetValidator(1, true))
        .Add(new PlayerModelPathValidator(2, true));

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Plugin.LogAdminAction(context.Player, $"Issued a {CommandName}, but failed in validator {context.Validator.ValidatorName}");
        switch (context.Validator)
        {
            case PermissionValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.Permission"));
                return ValidationFailureResult.SilentAbort();
            
            case ArgumentCountValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "SetPlayerClanTag.Command.Notification.Usage"));
                return ValidationFailureResult.SilentAbort();
            
            case ExtendedTargetValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.ExtendedTarget"));
                return ValidationFailureResult.SilentAbort();
            
            case PlayerModelPathValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "SetPlayerModel.Command.Validation.Failure.PathShouldEndWithVmdl"));
                return ValidationFailureResult.SilentAbort();
        }
        
        return ValidationFailureResult.UseDefaultFallback();
    }

    protected override void ExecuteCommand(CCSPlayerController? player, CommandInfo commandInfo, ValidatedArguments? validatedArguments)
    {
        var targets = validatedArguments!.GetArgument<TargetResult>(1)!;
        var modelPath = commandInfo.GetArgsAfter(2);
        
        
        string executorName = PlayerUtil.GetPlayerName(player);
        int actualExecutedTargets = 0;

        foreach(CCSPlayerController target in targets)
        {
            if (!PlayerUtil.SetPlayerModel(target, modelPath))
                continue;
            
            actualExecutedTargets++;
        }
        

        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(commandInfo.GetArg(1));

        if (hasTypedTargets && actualExecutedTargets >= 2)
        {
            string targetName = LocalizeString(player, TargetTypeStringConverter.GetTargetTypeName(commandInfo.GetArg(1)));
            
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targetName}'s player model to {modelPath}");
            PrintLocalizedChatToAll("SetPlayerModel.Command.Broadcast.SetModel", executorName, targetName, modelPath);
            return;
        }
        
        if (targets.Count() > 1)
        {
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {actualExecutedTargets} player's player model to {modelPath}");
            PrintLocalizedChatToAll("SetPlayerModel.Command.Broadcast.MultipleSetModel", executorName, actualExecutedTargets, modelPath);
            return;
        }
        
        Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targets.First().PlayerName}'s player model to {modelPath}");
        PrintLocalizedChatToAll("SetPlayerModel.Command.Broadcast.SetModel", executorName, targets.First().PlayerName, modelPath);
    }
}
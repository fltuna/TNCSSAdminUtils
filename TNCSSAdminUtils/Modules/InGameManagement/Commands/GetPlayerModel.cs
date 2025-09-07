using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using TNCSSAdminUtils.Util;
using TNCSSAdminUtils.Validators;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.InGameManagement.Commands;

public sealed class GetPlayerModel(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_getmodel";
    public override string CommandDescription => "Get player model of player";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new ExecutorValidator(AllowedExecutorType.Player))
        .Add(new PermissionValidator("css/generic"))
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
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "GetPlayerModel.Command.Notification.Usage"));
                return ValidationFailureResult.SilentAbort();
            
            case ExtendedTargetValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.ExtendedTarget"));
                return ValidationFailureResult.SilentAbort();
        }
        
        return ValidationFailureResult.UseDefaultFallback();
    }

    protected override void ExecuteCommand(CCSPlayerController? player, CommandInfo commandInfo, ValidatedArguments? validatedArguments)
    {
        if (player == null) throw new ArgumentNullException(nameof(player));
        
        var targets = validatedArguments!.GetArgument<TargetResult>(1)!;
        
        if (targets.Count() > 1)
        {
            player.PrintToConsole("=PLAYER MODEL INFORMATION=");
            player.PrintToConsole("User | ModelName");
            player.PrintToConsole("-------------------------------");
            foreach(CCSPlayerController target in targets) {
                if(target.IsHLTV)
                    continue;
            
                player.PrintToConsole($"{target.PlayerName} | {PlayerUtil.GetPlayerModel(target)}");
            }
            
            player.PrintToChat(LocalizeWithPluginPrefix(player, "Common.Command.Notification.SeeClientConsoleOutput"));
        }
        else
        {
            string targetName = targets.First().PlayerName;
            string targetModel = PlayerUtil.GetPlayerModel(targets.First());
            
            player.PrintToChat(LocalizeWithPluginPrefix(player, "GetPlayerModel.Command.Notification.PlayerModel", targetName, targetModel));
            player.PrintToConsole("=PLAYER MODEL INFORMATION=");
            player.PrintToConsole("User | ModelName");
            player.PrintToConsole("-------------------------------");
            player.PrintToConsole($"{targetName} | {targetModel}");
        }
        
        Plugin.LogAdminAction(player, $"Issued a {CommandName} to get players model");
    }
}
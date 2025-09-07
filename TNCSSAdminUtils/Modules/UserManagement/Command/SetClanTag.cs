using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.UserManagement.Command;

public sealed class SetClanTag(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_clantag";
    public override string CommandDescription => "Set clan tag of player";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new PermissionValidator("css/generic"))
        .Add(new ArgumentCountValidator(2, true))
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
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "SetPlayerClanTag.Command.Notification.Usage"));
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
        var clanTag = commandInfo.GetArgsAfter(2);
        
        
        string executorName = PlayerUtil.GetPlayerName(player);
        int actualExecutedTargets = 0;

        foreach(CCSPlayerController target in targets)
        {
            PlayerUtil.SetPlayerClanTag(target, clanTag);
            actualExecutedTargets++;
        }
        

        bool hasTypedTargets = Target.TargetTypeMap.ContainsKey(commandInfo.GetArg(1));

        if (hasTypedTargets && actualExecutedTargets >= 2)
        {
            string targetName = LocalizeString(player, TargetTypeStringConverter.GetTargetTypeName(commandInfo.GetArg(1)));
            
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targetName}'s clan tag to {clanTag}");
            PrintLocalizedChatToAll("SetPlayerClanTag.Command.Broadcast.TagChanged", executorName, targetName, clanTag);
            return;
        }
        
        if (targets.Count() > 1)
        {
            Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {actualExecutedTargets} player's clan tag to {clanTag}");
            PrintLocalizedChatToAll("SetPlayerClanTag.Command.Broadcast.MultipleTagChanged", executorName, actualExecutedTargets, clanTag);
            return;
        }
        
        Plugin.LogAdminAction(player, $"Issued a {CommandName} and set {targets.First().PlayerName}'s clan tag to {clanTag}");
        PrintLocalizedChatToAll("SetPlayerClanTag.Command.Broadcast.TagChanged", executorName, targets.First().PlayerName, clanTag);
    }
}
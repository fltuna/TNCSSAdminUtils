using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;
using TNCSSPluginFoundation.Models.Command.Validators.RangedValidators;
using TNCSSPluginFoundation.Utils.Entity;

namespace TNCSSAdminUtils.Modules.InGameManagement.Commands;

public sealed class SetTeamName(IServiceProvider provider) : TncssAbstractCommandBase(provider)
{
    public override string CommandName => "css_teamname";
    public override string CommandDescription => "Set name of team";

    protected override ICommandValidator GetValidator() => new CompositeValidator()
        .Add(new PermissionValidator("css/generic"))
        .Add(new ArgumentCountValidator(2, true))
        .Add(new RangedArgumentValidator<int>((int)CsTeam.Terrorist, (int)CsTeam.CounterTerrorist, 1, -1, true));

    protected override ValidationFailureResult OnValidationFailed(ValidationFailureContext context)
    {
        Plugin.LogAdminAction(context.Player, $"Issued a {CommandName}, but failed in validator {context.Validator.ValidatorName}");
        switch (context.Validator)
        {
            case PermissionValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.Permission"));
                return ValidationFailureResult.SilentAbort();
            
            case ArgumentCountValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "SetTeamName.Command.Notification.Usage"));
                return ValidationFailureResult.SilentAbort();
            
            case RangedArgumentValidator<int> rangedValidator:
                PrintMessageToServerOrPlayerChat(context.Player, LocalizeWithPluginPrefix(context.Player, "Common.Validation.Failure.ArgumentOutOfRange", rangedValidator.GetRangeDescription()));
                return ValidationFailureResult.SilentAbort();
        }
        
        return ValidationFailureResult.UseDefaultFallback();
    }

    protected override void ExecuteCommand(CCSPlayerController? player, CommandInfo commandInfo, ValidatedArguments? validatedArguments)
    {
        var targetTeam = (CsTeam)validatedArguments!.GetArgument<int>(1)!;
        var newName = commandInfo.GetArgsAfter(2);


        CsTeamUtil.SetTeamName(targetTeam, newName);
        
        string executorName = PlayerUtil.GetPlayerName(player);
        Plugin.LogAdminAction(player, $"Issued a {CommandName} and set team {targetTeam} name to {newName}");
        PrintLocalizedChatToAll("SetTeamName.Command.Broadcast.NameChanged", executorName, targetTeam, newName);
    }
}
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;

namespace TNCSSAdminUtils.Validators;

/// <summary>
/// Checks player model path is ended with .vmdl
/// </summary>
/// <param name="argumentIndex"></param>
/// <param name="dontNotifyWhenFailed"></param>
public class PlayerModelPathValidator(int argumentIndex, bool dontNotifyWhenFailed = false): CommandValidatorBase
{
    public override string ValidatorName => "TNCSSAdminUtilPlayerModelValidator";
    public override string ValidationFailureMessage => "SetPlayerModel.Command.Validation.Failure.PathShouldEndWithVmdl";
    
    public override TncssCommandValidationResult Validate(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (argumentIndex <= commandInfo.ArgCount && commandInfo.GetArgsAfter(argumentIndex).EndsWith(".vmdl"))
            return TncssCommandValidationResult.Success;
        
        return dontNotifyWhenFailed
            ? TncssCommandValidationResult.FailedIgnoreDefault
            : TncssCommandValidationResult.Failed;
    }

}
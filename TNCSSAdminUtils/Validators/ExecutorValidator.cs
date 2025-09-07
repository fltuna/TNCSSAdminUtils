using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using TNCSSAdminUtils.Util;
using TNCSSPluginFoundation.Models.Command;
using TNCSSPluginFoundation.Models.Command.Validators;

namespace TNCSSAdminUtils.Validators;

/// <summary>
/// Temporarily validator for detecting executors until base framework implemented.
/// </summary>
/// <param name="executorType"></param>
/// <param name="dontNotifyWhenFailed"></param>
public class ExecutorValidator(AllowedExecutorType executorType, bool dontNotifyWhenFailed = false): CommandValidatorBase
{
    public override string ValidatorName => "TNCSSAdminUtilExecutorValidator";
    public override string ValidationFailureMessage => "";
    
    public override TncssCommandValidationResult Validate(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (player == null && executorType == AllowedExecutorType.Console)
            return TncssCommandValidationResult.Success;

        if (player != null && executorType == AllowedExecutorType.Player)
            return TncssCommandValidationResult.Success;
        
        return dontNotifyWhenFailed
            ? TncssCommandValidationResult.FailedIgnoreDefault
            : TncssCommandValidationResult.Failed;
    }
}

public enum AllowedExecutorType
{
    Console,
    Player,
}
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Cvars.Validators;
using Microsoft.Extensions.DependencyInjection;
using TNCSSPluginFoundation.Configuration;
using TNCSSPluginFoundation.Models.Logger;

namespace TNCSSAdminUtils;

public sealed class SimpleDebugLogger : AbstractDebugLoggerBase
{
    public readonly FakeConVar<int> DebugLogLevelConVar = new("____TNCSS_PLUGIN_TEMPLATE_DEBUG_LOG_LEVEL",
        "0: Nothing, 1: Print info, warn, error message, 2: Print previous one and debug message, 3: Print previous one and trace message", 0, ConVarFlags.FCVAR_NONE,
        new RangeValidator<int>(0, 3));
    
    public readonly FakeConVar<bool> PrintToAdminClientsConsoleConVar = new("____TNCSS_PLUGIN_TEMPLATE_DEBUG_SHOW_CONSOLE", "Debug message shown in client console?", false);
    
    public readonly FakeConVar<string> RequiredFlagForPrintToConsoleConVar = new ("____TNCSS_PLUGIN_TEMPLATE_DEBUG_CONSOLE_PRINT_REQUIRED_FLAG", "Required flag for print to client console", "css/generic");

    
    public override int DebugLogLevel => DebugLogLevelConVar.Value;
    public override bool PrintToAdminClientsConsole => PrintToAdminClientsConsoleConVar.Value;
    public override string RequiredFlagForPrintToConsole => RequiredFlagForPrintToConsoleConVar.Value;

    public override string LogPrefix => "[TNCSSAdminUtils]";

    
    private const string ModuleName = "DebugLoggerTNCSSAdminUtils";
    
    public SimpleDebugLogger(IServiceProvider serviceProvider)
    {
        var conVarService = serviceProvider.GetRequiredService<ConVarConfigurationService>();
        conVarService.TrackConVar(ModuleName, DebugLogLevelConVar);
        conVarService.TrackConVar(ModuleName, PrintToAdminClientsConsoleConVar);
        conVarService.TrackConVar(ModuleName, RequiredFlagForPrintToConsoleConVar);
    }
}
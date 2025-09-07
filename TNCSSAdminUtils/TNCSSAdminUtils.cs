using Microsoft.Extensions.DependencyInjection;
using TNCSSAdminUtils.Modules.InGameManagement;
using TNCSSAdminUtils.Modules.UserManagement;
using TNCSSPluginFoundation;

namespace TNCSSAdminUtils;

public sealed class TncssAdminUtils: TncssPluginBase
{
    public override string ModuleName => "TNCSSAdminUtils";
    public override string ModuleVersion => "0.0.1";
    
    public override string BaseCfgDirectoryPath => "unused";
    public override string ConVarConfigPath => "TncssAdminUtils/convars.cfg";
    public override string PluginPrefix => "Plugin.Prefix";
    public override bool UseTranslationKeyInPluginPrefix => true;

    protected override void RegisterRequiredPluginServices(IServiceCollection collection, IServiceProvider provider)
    {
        DebugLogger = new SimpleDebugLogger(provider);
    }

    protected override void TncssOnPluginLoad(bool hotReload)
    {
        RegisterModule<InGamePlayerManagementCommands>();
        RegisterModule<UserManagementCommands>();
    }
}

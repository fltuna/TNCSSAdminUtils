using Microsoft.Extensions.DependencyInjection;
using TNCSSPluginFoundation;
using TNCSSAdminUtils.Modules;

namespace TNCSSAdminUtils;

public sealed class TNCSSAdminUtils: TncssPluginBase
{
    public override string ModuleName => "TNCSSAdminUtils";
    public override string ModuleVersion => "0.0.1";
    
    public override string BaseCfgDirectoryPath => "unused";
    public override string ConVarConfigPath => "";
    public override string PluginPrefix => "[]";
    public override bool UseTranslationKeyInPluginPrefix => false;

    protected override void RegisterRequiredPluginServices(IServiceCollection collection, IServiceProvider provider)
    {
        DebugLogger = new SimpleDebugLogger(provider);
    }

    protected override void TncssOnPluginLoad(bool hotReload)
    {
        RegisterModule<ModuleTemplate>();
    }
}

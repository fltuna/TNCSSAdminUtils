using Microsoft.Extensions.DependencyInjection;
using TNCSSPluginFoundation;
using TNCSSPluginTemplate.Modules;

namespace TNCSSPluginTemplate;

public sealed class TNCSSPluginTemplate: TncssPluginBase
{
    public override string ModuleName => "TNCSSPluginTemplate";
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

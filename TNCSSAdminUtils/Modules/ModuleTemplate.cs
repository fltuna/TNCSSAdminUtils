using TNCSSPluginFoundation.Models.Plugin;

namespace TNCSSAdminUtils.Modules;

public class ModuleTemplate(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "TNCSSAdminUtilsModule";
    public override string ModuleChatPrefix => string.Empty;
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
    }

    protected override void OnAllPluginsLoaded()
    {
    }

    protected override void OnUnloadModule()
    {
    }
}
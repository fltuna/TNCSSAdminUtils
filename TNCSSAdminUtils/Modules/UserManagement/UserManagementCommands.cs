using TNCSSAdminUtils.Modules.UserManagement.Command;
using TNCSSPluginFoundation.Models.Plugin;

namespace TNCSSAdminUtils.Modules.UserManagement;

public class UserManagementCommands(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "ServerManagementCommands";
    public override string ModuleChatPrefix => "unused";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTncssCommand<SetClanTag>();
        RegisterTncssCommand<SetPlayerName>();
        RegisterTncssCommand<UserList>();
    }
}
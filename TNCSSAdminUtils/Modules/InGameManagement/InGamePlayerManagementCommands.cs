using TNCSSAdminUtils.Modules.InGameManagement.Commands;
using TNCSSPluginFoundation.Models.Plugin;

namespace TNCSSAdminUtils.Modules.InGameManagement;

public sealed class InGamePlayerManagementCommands(IServiceProvider serviceProvider) : PluginModuleBase(serviceProvider)
{
    public override string PluginModuleName => "InGamePlayerManagementCommands";
    public override string ModuleChatPrefix => "unused";
    protected override bool UseTranslationKeyInModuleChatPrefix => false;

    protected override void OnInitialize()
    {
        RegisterTncssCommand<RespawnPlayer>();
        RegisterTncssCommand<SetCash>();
        RegisterTncssCommand<SetHealth>();
        RegisterTncssCommand<SetKevlar>();
        RegisterTncssCommand<SetPlayerModel>();
        RegisterTncssCommand<GetPlayerModel>();
        RegisterTncssCommand<SetTeam>();
        RegisterTncssCommand<SetTeamName>();
        RegisterTncssCommand<TerminateRound>();
        RegisterTncssCommand<SetBuyZoneStatus>();
    }
}
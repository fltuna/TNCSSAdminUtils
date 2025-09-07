using System.Text;

namespace TNCSSAdminUtils.Util;

public static class TargetTypeStringConverter
{
    private const string LanguageKeyBase = "Common.Target.Name.";
    
    public static string GetTargetTypeName(string targetString)
    {
        StringBuilder resultBuilder = new(LanguageKeyBase);

        switch (targetString.ToLower())
        {
            case "@all":
                resultBuilder.Append("All");
                break;
            case "@bots":
                resultBuilder.Append("Bots");
                break;
            case "@human":
                resultBuilder.Append("Humans");
                break;
            case "@alive":
                resultBuilder.Append("Alive");
                break;
            case "@dead":
                resultBuilder.Append("Dead");
                break;
            case "@!me":
                resultBuilder.Append("All");
                break;
            case "@ct":
                resultBuilder.Append("CounterTerrorist");
                break;
            case "@t":
                resultBuilder.Append("Terrorist");
                break;
            case "@spec":
                resultBuilder.Append("Spectator");
                break;
            
            default:
                throw new ArgumentException($"Unknown target type: {targetString}");
        }
        
        return resultBuilder.ToString();
    }
}
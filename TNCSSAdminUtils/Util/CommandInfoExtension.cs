using System.Text;
using CounterStrikeSharp.API.Modules.Commands;

namespace TNCSSAdminUtils.Util;

public static class CommandInfoExtension
{
    /// <summary>
    /// Obtain arg string from after x index of argument
    /// </summary>
    /// <param name="commandInfo">CommandInfo</param>
    /// <param name="index">Argumet index</param>
    /// <returns></returns>
    public static string GetArgsAfter(this CommandInfo commandInfo, int index)
    {
        StringBuilder builder = new StringBuilder();

        for (int i = index; i < commandInfo.ArgCount; i++)
        {
            builder.Append(commandInfo.GetArg(i));
            
            if (i < commandInfo.ArgCount - 1)
                builder.Append(' ');
        }
        return builder.ToString();
    }
}
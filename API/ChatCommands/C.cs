using CommandSystem;
using LabApi.Features.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTextChat.API.ChatCommands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class C : ICommand
    {
        public string Command => "c";

        public string[] Aliases => Array.Empty<string>();

        public string Description => "队伍聊天";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandSender commandSender = sender as CommandSender;
            Player player = Player.Get(commandSender);
            if (player == null) { response = "null"; return false; }
            ;
            TextManager.Send(arguments.ToString(), ChatType.C, player);
            response = "ok";
            return true;
        }
    }
}

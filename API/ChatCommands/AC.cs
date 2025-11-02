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
    public class AC : ICommand
    {
        public string Command => "ac";

        public string[] Aliases => Array.Empty<String>();

        public string Description => "管理员聊天";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            CommandSender commandSender = sender as CommandSender;
            Player player = Player.Get(commandSender);
            if (player == null) { response = "null"; return false; }
            ;
            TextManager.Send(arguments.ToString(), ChatType.AC, player);
            response = "ok";
            return true;
        }
    }
}

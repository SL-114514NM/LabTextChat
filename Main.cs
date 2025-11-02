using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Models.Hints;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabTextChat.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTextChat
{
    public class Main : Plugin<Config>
    {
        public override string Name => "打字插件";

        public override string Description => "基于LabApi以及HSM";

        public override string Author => "灰";

        public override Version Version => new Version(1,0);
        public static Main MainP;
        public override Version RequiredApiVersion => new Version(LabApi.Features.LabApiProperties.CompiledVersion);
        public override void Enable()
        {
            MainP = this;
            string Paths = Path.Combine(LabApi.Loader.Features.Paths.PathManager.LabApi.ToString(), "TextLog");
            if (!Directory.Exists(Paths)) { Directory.CreateDirectory(Paths); }
            LogPath = Path.Combine(Paths, $"{Today}.txt");
            if (!File.Exists(LogPath)) { File.Create(LogPath); }
            Logger.Debug($"==============");
            Logger.Debug($"打字插件已启动");
            Logger.Debug($"作者: 灰");
            Logger.Debug($"聊天记录目录: {Paths}");
            Logger.Debug($"==============");
            LabApi.Events.Handlers.PlayerEvents.Joined += OnJoined;
        }
        public override void Disable()
        {
            MainP = null;
            LabApi.Events.Handlers.PlayerEvents.Joined -= OnJoined;
        }
        public HintAlignment GetAlignment()
        {
            string t = Config.ShowAlignment.ToLower();
            switch(t)
            {
                case "right":
                    return HintAlignment.Right;
                case "left":
                    return HintAlignment.Left;
                default:
                    return HintAlignment.Center;
            }
        }
        public void OnJoined(PlayerJoinedEventArgs ev)
        {
            if (ev.Player!=null)
            {
                Hint hint = new Hint()
                {
                    YCoordinate = Config.YCoordinate,
                    Alignment = GetAlignment(),
                    FontSize = Config.TextSize,
                    AutoText = text =>
                    {
                        return "====><color=blue>聊天栏</color><====\n" + TextManager.GetText(ev.Player);
                    }
                };
                ev.Player.AddHint(hint);
            }
        }
        public static string LogPath;
        public static string Today = System.DateTime.Now.ToString("d");
        public static string NowTime = System.DateTime.Now.ToString("F");
    }
}

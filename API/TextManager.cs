using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTextChat.API
{
    public static class TextManager
    {
        public static List<ChatText> Texts = new List<ChatText>();
        private static Dictionary<string, (StringBuilder builder, DateTime createTime)> cachedBuilders = new Dictionary<string, (StringBuilder, DateTime)>();
        public static void Send(string msg, ChatType type, Player Sender)
        {
            ChatText text = new ChatText()
            {
                ChatType = type,
                Sender = Sender,
                Text = msg,
                CreateTime = DateTime.Now
            };
            Texts.Add(text);
            SaveMessage(text);
            ClearExpiredCache();
            Logger.Debug($"[{Sender.Nickname}][{type.GetChatTranslation()}]发送[{msg}]");
        }
        public static (List<ChatText>BC, List<ChatText> AC, List<ChatText> C) GetChatMessage(Player Target)
        {
            List<ChatText> BCList = new List<ChatText>();
            List<ChatText> ACList = new List<ChatText>();
            List<ChatText> CList = new List<ChatText>();
            ClearExpiredMessages();

            foreach (ChatText chatText in Texts)
            {
                if (chatText.ChatType == ChatType.BC)
                    BCList.Add(chatText);
                if (chatText.ChatType == ChatType.AC && Target.RemoteAdminAccess)
                    ACList.Add(chatText);
                if (Target.Team == chatText.Sender.Team)
                    CList.Add(chatText);
            }
            return (BCList, ACList, CList);
        }
        public static string GetText(Player Target)
        {
            (List<ChatText> BC, List<ChatText> AC, List<ChatText> C) = GetChatMessage(Target);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var chatText in BC)
            {
                int remainingSeconds = GetRemainingSeconds(chatText.CreateTime);
                if (remainingSeconds > 0)
                {
                    stringBuilder.AppendLine($"[{remainingSeconds}s][{chatText.Sender.Nickname}][{chatText.ChatType.GetChatTranslation()}]{chatText.Text}");
                }
            }
            if (Target.RemoteAdminAccess)
            {
                foreach (var chatText in AC)
                {
                    int remainingSeconds = GetRemainingSeconds(chatText.CreateTime);
                    if (remainingSeconds > 0)
                    {
                        stringBuilder.AppendLine($"[{remainingSeconds}s][{chatText.Sender.Nickname}][{chatText.ChatType.GetChatTranslation()}]{chatText.Text}");
                    }
                }
            }
            foreach (var chatText in C)
            {
                if (Target.Team == chatText.Sender.Team)
                {
                    int remainingSeconds = GetRemainingSeconds(chatText.CreateTime);
                    if (remainingSeconds > 0)
                    {
                        stringBuilder.AppendLine($"[{remainingSeconds}s][{chatText.Sender.Nickname}][{chatText.ChatType.GetChatTranslation()}|{chatText.Sender.Team.GetCNTeamTranslation()}]{chatText.Text}");
                    }
                }
            }

            return stringBuilder.ToString();
        }
        public static void SaveMessage(ChatText chatText)
        {
            string Fottmsg = $"[{Main.NowTime}]";
            string ChatLog = $"{Fottmsg}[{chatText.Sender.Nickname}][{chatText.ChatType.GetChatTranslation()}][{chatText.Text}]";
            File.AppendAllText(Main.LogPath,ChatLog);
        }
        public static string GetChatTranslation(this ChatType chatType)
        {
            switch(chatType)
            {
                case ChatType.BC:
                    return "[<color=green>全体聊天</color>]";
                case ChatType.AC:
                    return "[<color=blue>管理员聊天</color>]";
                case ChatType.C:
                    return "[<color=yellow>队伍聊天</color>]";
                default:
                    return "未知";
            }
            
        }
        public static string GetCNTeamTranslation(this Team team)
        {
            switch(team)
            {
                case Team.SCPs:
                    return "<color=red>奢侈品</color>";
                case Team.FoundationForces:
                    return "<color=blue>九尾狐</color>";
                case Team.ChaosInsurgency:
                    return "<color=green>混沌</color>";
                case Team.Scientists:
                    return "<color=blue>九尾狐</color>";
                case Team.ClassD:
                    return "<color=green>混沌</color>";
                case Team.Dead:
                    return "私人";
                case Team.OtherAlive:
                    return "其他";
                case Team.Flamingos:
                    return "<color=pink>火烈鸟</color>";
                default:
                    return "<color=red>未知</color>";
            }
        }
        private static int GetRemainingSeconds(DateTime createTime)
        {
            var elapsedSeconds = (DateTime.Now - createTime).TotalSeconds;
            var remainingSeconds = Main.MainP.Config.MessageExpireSeconds - (int)elapsedSeconds;
            return Math.Max(0, remainingSeconds); 
        }

        private static void ClearExpiredMessages()
        {
            var expiredTime = DateTime.Now.AddSeconds(-Main.MainP.Config.MessageExpireSeconds);
            Texts.RemoveAll(text => text.CreateTime < expiredTime);
        }
        private static void ClearExpiredCache()
        {
            var expiredTime = DateTime.Now.AddSeconds(-Main.MainP.Config.MessageExpireSeconds);
            var expiredKeys = cachedBuilders
                .Where(kv => kv.Value.createTime < expiredTime)
                .Select(kv => kv.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                cachedBuilders.Remove(key);
            }
        }
    }
    
    public class ChatText
    {
        public Player Sender { get; set; }
        public ChatType ChatType { get; set; }
        public string Text { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public enum ChatType
    {
        BC,
        AC,
        C
    }
}

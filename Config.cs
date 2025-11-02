using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabTextChat
{
    public class Config
    {
        public bool IsEnabled { get; set; } = true;
        [Description("聊天内容显示的横坐标")]
        public float YCoordinate { get; set; } = 100;
        [Description("显示在左侧还是右侧可填right/left")]
        public string ShowAlignment { get; set; } = "right";
        [Description("字体大小")]
        public int TextSize { get; set; } = 25;
        [Description("消息过期时间")]
        public int MessageExpireSeconds { get; set; } = 6;
    }
}

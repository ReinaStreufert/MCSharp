using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSharp.GameplayComponents
{
    public class Chat
    {
        public List<ChatComponent> Components = new List<ChatComponent>();
        public JArray ToJSON()
        {
            JArray Chat = new JArray();
            JObject inheritingComponent = new JObject();
            inheritingComponent.Add("text", "");
            inheritingComponent.Add("color", "reset");
            Chat.Add(inheritingComponent);
            foreach (ChatComponent Component in Components)
            {
                JObject componentObject = new JObject();
                componentObject.Add("text", Component.Text);
                if (Component.Color != ChatColor.Default)
                {
                    componentObject.Add("color", getColorName(Component.Color));
                } else
                {
                    componentObject.Add("color", "reset");
                }
                if (Component.Style.HasFlag(ChatSyle.Bold))
                {
                    componentObject.Add("bold", "true");
                } else
                {
                    componentObject.Add("bold", "false");
                }
                if (Component.Style.HasFlag(ChatSyle.Italic))
                {
                    componentObject.Add("italic", "true");
                } else
                {
                    componentObject.Add("italic", "false");
                }
                if (Component.Style.HasFlag(ChatSyle.Underlined))
                {
                    componentObject.Add("underlined", "true");
                }
                else
                {
                    componentObject.Add("underlined", "false");
                }
                if (Component.Style.HasFlag(ChatSyle.Strikethrough))
                {
                    componentObject.Add("strikethrough", "true");
                }
                else
                {
                    componentObject.Add("strikethrough", "false");
                }
                if (Component.Style.HasFlag(ChatSyle.Obfuscated))
                {
                    componentObject.Add("obfuscated", "true");
                }
                else
                {
                    componentObject.Add("obfuscated", "false");
                }

                if (Component.ClickEvent != ClickEvent.None)
                {
                    JObject clickEvent = new JObject();
                    clickEvent.Add("action", getClickEventTypeName(Component.ClickEvent.EventType));
                    clickEvent.Add("value", Component.ClickEvent.EventArgument);

                    componentObject.Add("clickEvent", clickEvent);
                }
                if (Component.HoverEvent != HoverEvent.None)
                {
                    JObject hoverEvent = new JObject();
                    hoverEvent.Add("action", "show_text");
                    hoverEvent.Add("value", Component.HoverEvent.EventArgument.ToJSON());

                    componentObject.Add("hoverEvent", hoverEvent);
                }

                Chat.Add(componentObject);
            }
            return Chat;
        }
        private static string getColorName(ChatColor Color)
        {
            switch (Color)
            {
                case ChatColor.Black:
                    return "black";
                case ChatColor.DarkBlue:
                    return "dark_blue";
                case ChatColor.DarkGreen:
                    return "dark_green";
                case ChatColor.Aqua:
                    return "dark_aqua";
                case ChatColor.DarkRed:
                    return "dark_red";
                case ChatColor.Purple:
                    return "dark_purple";
                case ChatColor.Gold:
                    return "gold";
                case ChatColor.Gray:
                    return "gray";
                case ChatColor.DarkGray:
                    return "dark_gray";
                case ChatColor.Blue:
                    return "blue";
                case ChatColor.Green:
                    return "green";
                case ChatColor.Cyan:
                    return "aqua";
                case ChatColor.Red:
                    return "red";
                case ChatColor.Pink:
                    return "light_purple";
                case ChatColor.Yellow:
                    return "yellow";
                case ChatColor.White:
                    return "white";
            }
            return "reset";
        }
        private static string getClickEventTypeName(ClickEventType type)
        {
            switch (type)
            {
                case ClickEventType.OpenUrl:
                    return "open_url";
                case ClickEventType.RunCommand:
                    return "run_command";
                case ClickEventType.SuggestCommand:
                    return "suggest_command";
                case ClickEventType.ChangePage:
                    return "change_page";
            }
            return "";
        }
    }
    public class ChatComponent
    {
        public string Text;
        public ChatSyle Style = ChatSyle.Normal;
        public ChatColor Color = ChatColor.Default;
        public ClickEvent ClickEvent = ClickEvent.None;
        public HoverEvent HoverEvent = HoverEvent.None;

        public ChatComponent(string Text, ChatSyle Style = ChatSyle.Normal, ChatColor Color = ChatColor.Default)
        {
            this.Text = Text;
            this.Style = Style;
            this.Color = Color;
        }
    }
    public class ClickEvent
    {
        public ClickEventType EventType;
        public string EventArgument;

        public ClickEvent(ClickEventType EventType, string EventArgument) { this.EventType = EventType; this.EventArgument = EventArgument; }

        public static readonly ClickEvent None = new ClickEvent(ClickEventType.None, "");
    }
    public class HoverEvent
    {
        public HoverEventType EventType;
        public Chat EventArgument;

        public HoverEvent(HoverEventType EventType, Chat EventArgument) { this.EventType = EventType; this.EventArgument = EventArgument; }

        public static readonly HoverEvent None = new HoverEvent(HoverEventType.None, null);
    }
    [Flags]
    public enum ChatSyle : byte
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Underlined = 4,
        Strikethrough = 8,
        Obfuscated = 16
    }
    public enum ClickEventType : byte
    {
        None = 0,
        OpenUrl = 1,
        RunCommand = 2,
        SuggestCommand = 3,
        ChangePage = 4
    }
    public enum HoverEventType : byte
    {
        None = 0,
        ShowText = 1
    }
    public enum ChatColor : byte
    {
        Black = 0,
        DarkBlue = 1,
        DarkGreen = 2,
        Aqua = 3,
        DarkRed = 4,
        Purple = 5,
        Gold = 6,
        Gray = 7,
        DarkGray = 8,
        Blue = 9,
        Green = 10,
        Cyan = 11,
        Red = 12,
        Pink = 13,
        Yellow = 14,
        White = 15,
        Default = 16
    }
}

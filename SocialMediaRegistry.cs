using System.Collections.Generic;

namespace PaintTrek.Shared
{
    public class SocialMediaButtonInfo
    {
        public string Name { get; }
        public string Url { get; }
        public string IconPath { get; }

        public SocialMediaButtonInfo(string name, string url, string iconPath)
        {
            Name = name;
            Url = url;
            IconPath = iconPath;
        }
    }

    public static class SocialMediaRegistry
    {
        public static readonly List<SocialMediaButtonInfo> Links = new List<SocialMediaButtonInfo>
        {
            new SocialMediaButtonInfo("X (Twitter)", "https://x.com/arargamesstudio", "UI/XIcon"),
            new SocialMediaButtonInfo("YouTube", "https://www.youtube.com/@koreaaria", "UI/YoutubeIcon"),
            new SocialMediaButtonInfo("TikTok", "https://www.tiktok.com/@arargamesstudio", "UI/TikTokIcon"),
            new SocialMediaButtonInfo("Instagram", "https://www.instagram.com/arargamesstudio", "UI/InstagramIcon"),
            new SocialMediaButtonInfo("Threads", "https://www.threads.net/@arargamesstudio", "UI/ThreadsIcon"),
            new SocialMediaButtonInfo("Facebook", "https://www.facebook.com/arargamesstudio", "UI/FacebookIcon")
        };
    }
}

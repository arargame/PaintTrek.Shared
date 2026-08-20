using System;
using System.Collections.Generic;
using System.Linq;
using PaintTrek.Shared.Localization;

namespace PaintTrek.Shared.Tips
{
    public enum TipCategory
    {
        Combat,
        Survival,
        Movement,
        Enemies,
        Strategy,
        CollectableObjects,
        Guns,
        Screens,
        Menus
    }

    public enum Platform
    {
        All,
        Desktop,
        Mobile
    }

    public class GameTip
    {
        public string LocKey { get; set; } = string.Empty;
        public TipCategory Category { get; set; }
        public Platform Platform { get; set; } = Platform.All;
    }

    public static class LoadingTipProvider
    {
        private static readonly List<GameTip> Tips = new List<GameTip>
        {
            // Combat
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip1, 
                Category = TipCategory.Combat 
            },

            // Survival
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip2,
                Category = TipCategory.Survival
            },
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip3,
                Category = TipCategory.Strategy
            },
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip4, 
                Category = TipCategory.Survival 
            },

            // Enemies
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip5,
                Category = TipCategory.Enemies
            },
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip6,
                Category = TipCategory.Enemies
            },
             new GameTip
            {
                LocKey = LocKeys.Tips.Tip7,
                Category = TipCategory.Strategy
            },

            // CollectableObjects & Guns
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip8,
                Category = TipCategory.CollectableObjects
            },
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip9, 
                Category = TipCategory.CollectableObjects 
            },
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip10, 
                Category = TipCategory.CollectableObjects 
            },
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip11, 
                Category = TipCategory.CollectableObjects 
            },
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip12, 
                Category = TipCategory.CollectableObjects 
            },

            // Menus & Screens
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip13,
                Category = TipCategory.Menus
            },
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip14,
                Category = TipCategory.Menus
            },
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip15, 
                Category = TipCategory.Menus 
            },
            // Mobile Specific
            new GameTip 
            { 
                LocKey = LocKeys.Tips.Tip16, 
                Category = TipCategory.Menus,
                Platform = Platform.Mobile
            },
            new GameTip
            {
                LocKey = LocKeys.Tips.Tip17,
                Category = TipCategory.Survival
            }
        };

        public static GameTip GetRandom(Platform currentPlatform)
        {
            var validTips = Tips.Where(t => t.Platform == Platform.All || t.Platform == currentPlatform).ToList();
            if (validTips.Count == 0) return Tips[0];
            
            // Basic random
            return validTips[new Random().Next(validTips.Count)];
        }
    }
}

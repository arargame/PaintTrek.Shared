using System;
using System.Collections.Generic;
using System.Linq;

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
        public string Text { get; set; } = string.Empty;
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
                Text = "Some enemies love it when you stay still and can easily deal damage to you. Keep moving to confuse them.", 
                Category = TipCategory.Combat 
            },

            // Survival
            new GameTip
            {
                Text = "You don't have to kill every enemy. Sometimes dodging is the best strategy.",
                Category = TipCategory.Survival
            },
            new GameTip
            {
                Text = "Survival isn't about firepower alone. Positioning and timing matter.",
                Category = TipCategory.Strategy
            },
            new GameTip 
            { 
                Text = "You can track the duration of diamond abilities just below the health bar.", 
                Category = TipCategory.Survival 
            },

            // Enemies
            new GameTip
            {
                Text = "Some enemies become more dangerous if ignored. Eliminate them quickly. (For example Ufo)",
                Category = TipCategory.Enemies
            },
            new GameTip
            {
                Text = "UFO-type enemies can overwhelm you if you don't escape immediately.",
                Category = TipCategory.Enemies
            },
             new GameTip
            {
                Text = "Each enemy has unique behaviors. Observing them for a short time before making your move can give you a significant advantage.",
                Category = TipCategory.Strategy
            },

            // CollectableObjects & Guns
            new GameTip
            {
                Text = "There are three different types of diamonds. Each one grants unique abilities that temporarily enhance your firepower.",
                Category = TipCategory.CollectableObjects
            },
            new GameTip 
            { 
                Text = "The Green Diamond activates a Poison Attack, firing toxic projectiles that apply special effects to enemies.", 
                Category = TipCategory.CollectableObjects 
            },
            new GameTip 
            { 
                Text = "The Black Diamond unlocks a Critical Attack, firing slower but extremely powerful shots capable of dealing massive damage.", 
                Category = TipCategory.CollectableObjects 
            },
            new GameTip 
            { 
                Text = "The Blue Diamond increases firing speed and bullet velocity, allowing faster attacks with slightly increased damage.", 
                Category = TipCategory.CollectableObjects 
            },
            new GameTip 
            { 
                Text = "The Red Diamond boosts weapon damage significantly while keeping normal firing speed.", 
                Category = TipCategory.CollectableObjects 
            },

            // Menus & Screens
            new GameTip
            {
                Text = "In the Extra menu, you can learn about all in-game enemies and collectible items. You can also listen to the game's music tracks.",
                Category = TipCategory.Menus
            },
            new GameTip
            {
                Text = "All in-game music tracks are available in the Extra section of the Main Menu.",
                Category = TipCategory.Menus
            },
            new GameTip 
            { 
                Text = "In the Sound Settings menu, you can enable or disable in-game music, sound effects, and menu sounds.", 
                Category = TipCategory.Menus 
            },
            // Mobile Specific
            new GameTip 
            { 
                Text = "In the Options menu, you can choose joystick configurations that allow you to place the controls on either the bottom-left or bottom-right of the screen.", 
                Category = TipCategory.Menus,
                Platform = Platform.Mobile
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

using DMG.Parser.Validation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DMG.Parser
{
    class DMGSetting
    {
        public string Name { get; }
        public bool TakesValue { get; }

        public bool AllowedInBombDefinition { get; }

        public Action<MissionFactory, object> Action { get; }

        public DMGSetting(string name, bool takesValue, bool allowedInBombDefinition, Action<MissionFactory, object> action)
        {
            Name = name;
            TakesValue = takesValue;
            AllowedInBombDefinition = allowedInBombDefinition;
            Action = action;
        }

    }

    class DMGSettings
    {
        public static readonly Regex STRIKE_COUNT_REGEX = new Regex(@"^\d+X$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static readonly Dictionary<string, DMGSetting> GENERATOR_SETTINGS = new Dictionary<string, DMGSetting>()
        {
            { "nopacing", new DMGSetting("nopacing", false, false, (f, _) => f.DisablePacingEvents()) },
            { "frontfaceonly", new DMGSetting("frontfaceonly", false, true, (f, val) => f.SetFrontFaceOnly(val.ValidateCastToBool())) },
            { "strikes", new DMGSetting("strikes", true, true, (f, strikes) => f.SetStrikeCount(strikes.ValidateCastToInt())) },
            { "needyactivationtime", new DMGSetting("needyactivationtime", true, true, (f, time) => f.SetNeedyActivationTime(time.ValidateCastToInt())) },
            { "time", new DMGSetting("time", true, true, (f, time) => f.SetTime(time.ValidateCastToInt())) },
            { "widgets", new DMGSetting("widgets", true, true, (f, count) => f.SetWidgetCount(count.ValidateCastToInt())) },
        };

        public static readonly Dictionary<string, DMGSetting> DMG_SETTINGS = new Dictionary<string, DMGSetting>()
        {
            { "ruleseed", new DMGSetting("ruleseed", true, false, (f, seed) => f.SetRuleSeed(seed.ValidateCastToInt())) },
            { "factory", new DMGSetting("factory", true, false, (f, mode) => f.SetFactoryMode(mode.ValidateCastToEnum<FactoryMode>())) },
            { "room", new DMGSetting("room", true, false, (f, room) => f.SetGameplayRoom(room.ValidateCastToString())) },
        };

        public static readonly Dictionary<string, DMGSetting> TWEAKS_SETTINGS = new Dictionary<string, DMGSetting>()
        {
            { "mode", new DMGSetting("mode", true, false, (f, mode) => f.SetGameMode(mode.ValidateCastToEnum<Mode>())) },
        };
    }
}

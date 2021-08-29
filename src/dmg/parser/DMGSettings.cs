using DMG.Parser.Validation;

using System;

namespace DMG.Parser {
    public class DMGSetting {
        public string Name { get; }
        public bool TakesValue { get; }

        public bool AllowedInBombDefinition { get; }

        public Action<MissionFactory, object> Action { get; }

        public DMGSetting(string name, bool takesValue, bool allowedInBombDefinition, Action<MissionFactory, object> action) {
            Name = name;
            TakesValue = takesValue;
            AllowedInBombDefinition = allowedInBombDefinition;
            Action = action;
        }
    }

    public class BooleanSetting : DMGSetting {
        public BooleanSetting(string name, bool allowedInBombDefinition, Action<MissionFactory> action) : base(name, false, allowedInBombDefinition, (f, v) => action(f)) { }
    }

    public class IntegerSetting : DMGSetting {
        public IntegerSetting(string name, bool allowedInBombDefinition, Action<MissionFactory, int> action) : base(name, true, allowedInBombDefinition, (f, v) => action(f, v.ValidateCastToInt())) { }
    }

    public class StringSetting : DMGSetting {
        public StringSetting(string name, bool allowedInBombDefinition, Action<MissionFactory, string> action) : base(name, true, allowedInBombDefinition, (f, v) => action(f, v.ValidateCastToString())) { }
    }

    public class EnumSetting<E> : DMGSetting {
        public EnumSetting(string name, bool allowedInBombDefinition, Action<MissionFactory, E> action) : base(name, true, allowedInBombDefinition, (f, v) => action(f, v.ValidateCastToEnum<E>())) { }
    }

    // TODO: remove
    //public class DMGSettings {
    //    public static readonly Dictionary<string, DMGSetting> GENERATOR_SETTINGS = new Dictionary<string, DMGSetting>()
    //    {
    //        { "nopacing", new DMGSetting("nopacing", false, false, (f, _) => f.DisablePacingEvents()) },
    //        { "frontfaceonly", new DMGSetting("frontfaceonly", false, true, (f, val) => f.SetFrontFaceOnly(val.ValidateCastToBool())) },
    //        { "strikes", new DMGSetting("strikes", true, true, (f, strikes) => f.SetStrikeCount(strikes.ValidateCastToInt())) },
    //        { "needyactivationtime", new DMGSetting("needyactivationtime", true, true, (f, time) => f.SetNeedyActivationTime(time.ValidateCastToInt())) },
    //        { "time", new DMGSetting("time", true, true, (f, time) => f.SetTime(time.ValidateCastToInt())) },
    //        { "widgets", new DMGSetting("widgets", true, true, (f, count) => f.SetWidgetCount(count.ValidateCastToInt())) },
    //    };

    //    public static readonly Dictionary<string, DMGSetting> DMG_SETTINGS = new Dictionary<string, DMGSetting>()
    //    {
    //        { "ruleseed", new DMGSetting("ruleseed", true, false, (f, seed) => f.SetRuleSeed(seed.ValidateCastToInt())) },
    //        { "factory", new DMGSetting("factory", true, false, (f, mode) => f.SetFactoryMode(mode.ValidateCastToEnum<FactoryMode>())) },
    //        { "room", new DMGSetting("room", true, false, (f, room) => f.SetGameplayRoom(room.ValidateCastToString())) },
    //    };

    //    public static readonly Dictionary<string, DMGSetting> TWEAKS_SETTINGS = new Dictionary<string, DMGSetting>()
    //    {
    //        { "mode", new DMGSetting("mode", true, false, (f, mode) => f.SetGameMode(mode.ValidateCastToEnum<Mode>())) },
    //    };
    //}
}

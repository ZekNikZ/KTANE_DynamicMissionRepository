namespace DMG.Parser.Modules {
    public class TweaksModule : ParserModule {
        public TweaksModule() : base("builtin_tweaks") {
            AddSetting(new EnumSetting<Mode>("mode", false, (f, v) => f.SetGameMode(v)));
        }
    }
}

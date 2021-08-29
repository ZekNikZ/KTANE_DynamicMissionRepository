namespace DMG.Parser.Modules {
    public class DMGModule : ParserModule {
        public DMGModule() : base("builtin_dmg") {
            AddSetting(new IntegerSetting("ruleseed", false, (f, v) => f.SetRuleSeed(v)));
            AddSetting(new EnumSetting<FactoryMode>("factory", false, (f, v) => f.SetFactoryMode(v)));
            AddSetting(new StringSetting("room", false, (f, v) => f.SetGameplayRoom(v)));
        }
    }
}

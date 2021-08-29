namespace DMG.Parser.Modules {
    public class BaseModule : ParserModule {
        public BaseModule() : base("builtin_base") {
            AddSetting(new BooleanSetting("nopacing", false, f => f.DisablePacingEvents()));
            AddSetting(new BooleanSetting("frontfaceonly", true, f => f.SetFrontFaceOnly(true)));
            AddSetting(new IntegerSetting("strikes", true, (f, v) => f.SetStrikeCount(v)));
            AddSetting(new IntegerSetting("needyactivationtime", true, (f, v) => f.SetNeedyActivationTime(v)));
            AddSetting(new IntegerSetting("time", true, (f, v) => f.SetTime(v)));
            AddSetting(new IntegerSetting("widgets", true, (f, v) => f.SetWidgetCount(v)));
        }
    }
}

using System.Collections.Generic;

namespace DMG.Parser {
    public class ParserModule {
        public string Id { get; }

        public readonly Dictionary<string, DMGSetting> Settings;

        public ParserModule(string id) {
            Id = id;
            Settings = new Dictionary<string, DMGSetting>();
        }

        public void AddSetting(DMGSetting setting) {
            Settings.Add(setting.Name, setting);
        }

        public bool TryGetSetting(string id, out DMGSetting setting) {
            return Settings.TryGetValue(id, out setting);
        }
    }
}
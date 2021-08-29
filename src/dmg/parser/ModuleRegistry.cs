using System;
using System.Collections.Generic;
using DMG.Parser.Modules;

namespace DMG.Parser {
    public class ModuleRegistry {
        public static readonly ModuleRegistry Instance = new ModuleRegistry();

        private readonly Dictionary<string, ParserModule> Modules;

        private ModuleRegistry() {
            Modules = new Dictionary<string, ParserModule>();
            RegisterModule(new BaseModule());
            RegisterModule(new DMGModule());
            RegisterModule(new TweaksModule());
        }

        public ParserModule GetModule(string id) {
            return Modules.TryGetValue(id, out ParserModule result) ? result : null;
        }

        public HashSet<ParserModule> GetAllModules() {
            return new HashSet<ParserModule>(Modules.Values);
        }

        public void RegisterModule(ParserModule module) {
            Modules.Add(module.Id, module);
        }

        public void RegisterModule(string id) {
            RegisterModule(new ParserModule(id));
        }

        public void UnregisterModule(string id) {
            Modules.Remove(id);
        }

        public bool TryGetSetting(string id, out DMGSetting setting, bool onlyBase = false) {
            if (onlyBase) {
                return GetModule("builtin_base").TryGetSetting(id, out setting);
            }

            foreach (var module in Modules.Values) {
                if (module.TryGetSetting(id, out setting)) {
                    return true;
                }
            }

            setting = null;
            return false;
        }

        public DMGSetting GetSetting(string id) {
            return TryGetSetting(id, out DMGSetting setting) ? setting : throw new ArgumentException("Invalid setting id");
        }
    }
}

using System;

namespace DMG {
    public class ModuleData {
        public string ModuleType { get; }
        public string DisplayName { get; }

        public ModuleData(string moduleType, string displayName) {
            this.ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
            this.DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }
    }
}


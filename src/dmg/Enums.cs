using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DMG {
    public static class EnumConstants {
        private static readonly Dictionary<FactoryMode, string> FactoryModeNames = new Dictionary<FactoryMode, string>()
        {
            { FactoryMode.STATIC, "Factory: Static" },
            { FactoryMode.FINITE, "Factory: Finite" },
            { FactoryMode.FINITE_GLOBAL_TIME, "Factory: Finite + global time" },
            { FactoryMode.FINITE_GLOBAL_STRIKES, "Factory: Finite + global strikes" },
            { FactoryMode.FINITE_GLOBAL_TIME_STRIKES, "Factory: Finite + global time and strikes" },
            { FactoryMode.INFINITE, "Factory: Infinite" },
            { FactoryMode.INFINITE_GLOBAL_TIME, "Factory: Infinite + global time" },
            { FactoryMode.INFINITE_GLOBAL_STRIKES, "Factory: Infinite + global strikes" },
            { FactoryMode.INFINITE_GLOBAL_TIME_STRIKES, "Factory: Infinite + global time and strikes" }
        };

        private static readonly Dictionary<Mode, string> TweaksModeNames = new Dictionary<Mode, string>()
        {
            { Mode.NORMAL, "Normal Mode" },
            { Mode.TIME, "Time Mode" },
            { Mode.ZEN, "Zen Mode" },
            { Mode.STEADY, "Steady Mode" },
            { Mode.NONE, "N/A Mode" },
        };
    }

    public enum FactoryMode {
        STATIC,
        FINITE,
        FINITE_GLOBAL_TIME,
        FINITE_GLOBAL_STRIKES,
        FINITE_GLOBAL_TIME_STRIKES,
        INFINITE,
        INFINITE_GLOBAL_TIME,
        INFINITE_GLOBAL_STRIKES,
        INFINITE_GLOBAL_TIME_STRIKES
    }

    public enum Mode {
        NORMAL,
        TIME,
        ZEN,
        STEADY,
        NONE
    }
}

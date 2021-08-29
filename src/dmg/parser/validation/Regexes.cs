using System.Text.RegularExpressions;

namespace DMG.Parser {
    public static class Regexes {
        public static readonly Regex TIMER_REGEX = new Regex(@"^(?<Time>(?<One>\d+):(?<Two>\d{1,2})(:(?<Three>\d{1,2}))?)", RegexOptions.Compiled);
        public static readonly Regex STRIKE_COUNT_REGEX = new Regex(@"^\d+X$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    }
}

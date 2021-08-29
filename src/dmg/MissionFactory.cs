using System.Collections.Generic;

namespace DMG {
    public class MissionFactory {
        private MissionProxy Mission = new MissionProxy();
        private int? RuleSeed;
        private Mode? Mode;
        private FactoryMode? FactoryMode;
        private string GameplayRoom;

        private GeneratorSettingProxy CurrentBomb;
        private int CurrentBombCount = 1;

        private MissionFactory() { }

        public static MissionFactory Create() {
            return new MissionFactory();
        }

        public MissionFactory SetTime(int time) {
            if (CurrentBomb != null) {
                if (CurrentBomb.TimeLimit != null) throw new MissionConstructionException("Cannot set the time of a bomb twice");
                CurrentBomb.TimeLimit = time;
            } else Mission.DefaultBomb.TimeLimit = time;

            return this;
        }

        public MissionFactory SetNeedyActivationTime(int needyActivationTime) {
            if (CurrentBomb != null) {
                if (CurrentBomb.TimeBeforeNeedyActivation != null) throw new MissionConstructionException("Cannot set the needy activation time of a bomb twice");
                CurrentBomb.TimeBeforeNeedyActivation = needyActivationTime;
            } else Mission.DefaultBomb.TimeBeforeNeedyActivation = needyActivationTime;

            return this;
        }

        public MissionFactory SetFrontFaceOnly(bool frontFaceOnly) {
            if (CurrentBomb != null) {
                if (CurrentBomb.FrontFaceOnly != null) throw new MissionConstructionException("Cannot set the front face only property of a bomb twice");
                CurrentBomb.FrontFaceOnly = frontFaceOnly;
            } else Mission.DefaultBomb.FrontFaceOnly = frontFaceOnly;

            return this;
        }

        public MissionFactory SetStrikeCount(int strikes) {
            if (CurrentBomb != null) {
                if (CurrentBomb.NumStrikes != null) throw new MissionConstructionException("Cannot set the strike count of a bomb twice");
                CurrentBomb.NumStrikes = strikes;
            } else Mission.DefaultBomb.NumStrikes = strikes;

            return this;
        }

        public MissionFactory SetWidgetCount(int widgets) {
            if (CurrentBomb != null) {
                if (CurrentBomb.OptionalWidgetCount != null) throw new MissionConstructionException("Cannot set the widget count of a bomb twice");
                CurrentBomb.OptionalWidgetCount = widgets;
            } else Mission.DefaultBomb.OptionalWidgetCount = widgets;

            return this;
        }

        public MissionFactory DisablePacingEvents() {
            if (CurrentBomb != null) throw new MissionConstructionException("Pacing events is not a bomb-level setting");

            if (Mission.PacingEventsEnabled != null) throw new MissionConstructionException("Pacing events have already been disabled");
            Mission.PacingEventsEnabled = false;

            return this;
        }

        public MissionFactory SetRuleSeed(int seed) {
            if (CurrentBomb != null) throw new MissionConstructionException("Rule seed is not a bomb-level setting");

            if (RuleSeed != null) throw new MissionConstructionException("Cannot set the rule seed of the mission twice");
            RuleSeed = seed;

            return this;
        }

        public MissionFactory SetGameMode(Mode mode) {
            if (CurrentBomb != null) throw new MissionConstructionException("Game mode is not a bomb-level setting");

            if (Mode != null) throw new MissionConstructionException("Cannot set the game mode of the mission twice");
            Mode = mode;

            return this;
        }

        public MissionFactory SetFactoryMode(FactoryMode mode) {
            // TODO ensure factory mode is enabled
            // TODO ensure room has not been set (unless it is factory)

            if (CurrentBomb != null) throw new MissionConstructionException("Factory mode is not a bomb-level setting");

            if (FactoryMode != null) throw new MissionConstructionException("Cannot set the factory mode of the mission twice");
            FactoryMode = mode;

            return this;
        }

        public MissionFactory SetGameplayRoom(string room) {
            // TODO do this properly
            // TODO ensure room is enabled
            // TODO ensure factory setting has not been set

            if (CurrentBomb != null) throw new MissionConstructionException("Gameplay room is not a bomb-level setting");

            if (GameplayRoom != null) throw new MissionConstructionException("Cannot set the gameplay room of the mission twice");

            GameplayRoom = room;

            return this;
        }

        public MissionFactory StartBombDefinition(int count = 1, bool noDuplicates = false) {
            // TODO: no duplicates handling
            if (CurrentBomb != null) throw new MissionConstructionException("Already editing a bomb instance");
            CurrentBomb = new GeneratorSettingProxy();
            CurrentBombCount = count;

            return this;
        }

        public MissionFactory EndBombDefinition() {
            if (CurrentBomb == null) throw new MissionConstructionException("No bomb instance to stop editing");
            Mission.AddBomb(CurrentBomb, CurrentBombCount);
            CurrentBomb = null;

            return this;
        }

        public MissionFactory AddModulePool(List<string> modules, bool noDuplicates = false) {
            KMComponentPool pool = new KMComponentPool();

            if (CurrentBomb != null) CurrentBomb.ComponentPools.Add(pool);
            else Mission.DefaultBomb.ComponentPools.Add(pool);

            return this;
        }

        public DMGMission Build() {
            if (FactoryMode != null) {
                Mission.FactoryMode = (int) FactoryMode;
            }

            var result = new DMGMission {
                KMMission = Mission.Generate()
            };

            return result;
        }
    }
}

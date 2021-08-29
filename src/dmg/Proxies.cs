using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace DMG {
    public class GeneratorSettingProxy {
        public float? TimeLimit;
        public int? NumStrikes;
        public int? TimeBeforeNeedyActivation;
        public bool? FrontFaceOnly;
        public int? OptionalWidgetCount;
        public List<KMComponentPool> ComponentPools = new List<KMComponentPool>();

        public KMGeneratorSetting Generate() {
            return new KMGeneratorSetting() {
                TimeLimit = TimeLimit ?? 300,
                NumStrikes = NumStrikes ?? 3,
                TimeBeforeNeedyActivation = TimeBeforeNeedyActivation ?? 30,
                FrontFaceOnly = FrontFaceOnly ?? false,
                OptionalWidgetCount = OptionalWidgetCount ?? 5,
                ComponentPools = ComponentPools
            };
        }
    }

    public class MissionProxy {
        public string DisplayName;
        public string Description;
        public GeneratorSettingProxy DefaultBomb = new GeneratorSettingProxy();
        public List<GeneratorSettingProxy> Bombs;
        public bool? PacingEventsEnabled;
        public int? FactoryMode;

        public void AddBomb(GeneratorSettingProxy bomb, int count = 1) {
            for (int i = 0; i < count; i++) {
                Bombs.Add(new GeneratorSettingProxy() {
                    TimeLimit = bomb.TimeLimit ?? DefaultBomb.TimeLimit,
                    NumStrikes = bomb.NumStrikes ?? DefaultBomb.NumStrikes,
                    TimeBeforeNeedyActivation = bomb.TimeBeforeNeedyActivation ?? DefaultBomb.TimeBeforeNeedyActivation,
                    FrontFaceOnly = bomb.FrontFaceOnly ?? DefaultBomb.FrontFaceOnly,
                    OptionalWidgetCount = bomb.OptionalWidgetCount ?? DefaultBomb.OptionalWidgetCount,
                    ComponentPools = new List<KMComponentPool>(bomb.ComponentPools).Concat(DefaultBomb.ComponentPools).ToList()
                });
            }
        }

        public KMMission Generate() {
            var instance = ScriptableObject.CreateInstance<KMMission>();
            instance.DisplayName = DisplayName ?? "Dynamic Mission";
            instance.Description = Description ?? "This is a dynamically-generated mission";
            instance.PacingEventsEnabled = PacingEventsEnabled ?? true;

            // TODO: Create bombs

            return instance;
        }
    }
}

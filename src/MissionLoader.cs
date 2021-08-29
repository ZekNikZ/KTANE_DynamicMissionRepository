using System;
using UnityEngine;

namespace DynamicMissionRepository {
    class MissionLoader {
        private static object LoadMissionFromText(string text) {
            ModdedAPI.TryGetAs("DMG_ParseTextToMission", out Func<string, KMMission> parseMission);
            ModMission modMission = ScriptableObject.CreateInstance<ModMission>();
            // TODO: hardcoded mission value
            modMission.ConfigureFrom(parseMission(text), "dmg_custom");
            return modMission;
        }

        private static void TestLoadTestMission() {
        }

        public static void LoadTestMission() {
            //var testMissionString = TestMissions.SIMON_SAYS;
            //var testMission = LoadMissionFromText(testMissionString);
            //Debug.Log(testMission);
        }
    }
}
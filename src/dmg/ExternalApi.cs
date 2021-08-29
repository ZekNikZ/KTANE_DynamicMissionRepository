using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace DMG {
    // TODO: cache these results?
    public class ExternalApis {
        public static void Load(MonoBehaviour parentComponent) {
            parentComponent.StartCoroutine(FindModSelector());
        }

        private static IDictionary<string, object> ModSelectorApi;
        private static IEnumerator FindModSelector() {
            while (true) {
                GameObject modSelectorObject = GameObject.Find("ModSelector_Info");
                if (modSelectorObject != null) {
                    ModSelectorApi = modSelectorObject.GetComponent<IDictionary<string, object>>();
                    yield break;
                }

                yield return null;
            }
        }

        public static HashSet<string> AllSolvableModuleIds {
            get {
                return new HashSet<string>((IEnumerable<string>) ModSelectorApi?["AllSolvableModules"] ?? new string[0]);
            }
        }

        public static HashSet<string> AllNeedyModuleIds {
            get {
                return new HashSet<string>((IEnumerable<string>) ModSelectorApi?["AllNeedyModules"] ?? new string[0]);
            }
        }

        public static HashSet<string> DisabledSolvableModuleIds {
            get {
                return new HashSet<string>((IEnumerable<string>) ModSelectorApi?["DisabledSolvableModules"] ?? new string[0]);
            }
        }

        public static HashSet<string> DisabledNeedyModuleIds {
            get {
                return new HashSet<string>((IEnumerable<string>) ModSelectorApi?["DisabledNeedyModules"] ?? new string[0]);
            }
        }
        public static HashSet<string> EnabledSolvableModuleIds {
            get {
                return new HashSet<string>(AllSolvableModuleIds.Except(DisabledSolvableModuleIds));
            }
        }

        public static HashSet<string> EnabledNeedyModuleIds {
            get {
                return new HashSet<string>(AllNeedyModuleIds.Except(DisabledNeedyModuleIds));
            }
        }

        public static Dictionary<string, string> AllSolvableModules {
            get {
                var result = new Dictionary<string, string>();

                var assembly = ModSelectorApi.GetType().Assembly;
                var serviceType = assembly.GetType("ModSelectorService");
                object service = serviceType.GetProperty("Instance").GetValue(null, null);
                var allSolvableModules = (IDictionary) serviceType.GetField("_allSolvableModules", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(service);

                foreach (object entry in allSolvableModules.Cast<object>()) {
                    string id = (string) entry.GetType().GetProperty("Key").GetValue(entry, null);
                    object value = entry.GetType().GetProperty("Value").GetValue(entry, null);
                    string name = (string) value.GetType().GetProperty("ModuleName").GetValue(value, null);
                    result.Add(id, name);
                }

                return result;
            }
        }

        public static Dictionary<string, string> AllNeedyModules {
            get {
                var result = new Dictionary<string, string>();

                var assembly = ModSelectorApi.GetType().Assembly;
                var serviceType = assembly.GetType("ModSelectorService");
                object service = serviceType.GetProperty("Instance").GetValue(null, null);
                var allNeedyModules = (IDictionary) serviceType.GetField("_allNeedyModules", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(service);

                foreach (object entry in allNeedyModules.Cast<object>()) {
                    string id = (string) entry.GetType().GetProperty("Key").GetValue(entry, null);
                    object value = entry.GetType().GetProperty("Value").GetValue(entry, null);
                    string name = (string) value.GetType().GetProperty("ModuleName").GetValue(value, null);
                    result.Add(id, name);
                }

                return result;
            }
        }

        public static Dictionary<string, string> GameplayRooms {
            get {
                var instance = ReflectionHelper.FindType("ModManager").GetValue<object>("Instance");

                var roomList = instance.CallMethod<IList>("GetGameplayRooms").Cast<MonoBehaviour>().ToDictionary(room => room.gameObject.name.ToLowerInvariant().Replace(' ', '_'), room => room.gameObject.name);
                roomList.Add("default", "FacilityRoom");

                return roomList;
            }
        }
    }
}

using DMG;
using DMG.Parser;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

using Assets.Scripts.Mods.Mission;

namespace DynamicMissionRepository
{
    class TestBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            Debug.Log(new DMGParser().Parse(TestMissions.SIMON_SAYS));

            var generatorSetting = new KMGeneratorSetting
            {
                TimeLimit = 400f,
                NumStrikes = 10,
                TimeBeforeNeedyActivation = 30,
                FrontFaceOnly = true,
                ComponentPools = new List<KMComponentPool>
                {
                    new KMComponentPool {
                        Count = 3,
                        AllowedSources = KMComponentPool.ComponentSource.Base,
                        ComponentTypes = new List<KMComponentPool.ComponentTypeEnum> { KMComponentPool.ComponentTypeEnum.Morse },
                        SpecialComponentType = KMComponentPool.SpecialComponentTypeEnum.None,
                        ModTypes = new List<string>()
                    }
                }
            };

            var kmMission = ScriptableObject.CreateInstance<KMMission>();
            kmMission.DisplayName = "This is my test mission";
            kmMission.Description = "This is a test description";
            kmMission.PacingEventsEnabled = false;
            kmMission.GeneratorSetting = generatorSetting;

            var mission = ScriptableObject.CreateInstance<ModMission>();
            mission.ConfigureFrom(kmMission, "dynamic_testpack");

            ModManager.Instance.ModMissions.Add(mission);


            var kmToc = ScriptableObject.CreateInstance<KMMissionTableOfContents>();
            kmToc.Title = "Test TOC";
            kmToc.Sections = new List<KMMissionTableOfContents.Section>
            {
                new KMMissionTableOfContents.Section
                {
                    Title = "Section Numero Uno",
                    MissionIDs = new List<string>()
                    {
                        kmMission.ID
                    }
                }
            };

            var toc = new ModTableOfContentsMetaData();
            toc.ConfigureFrom(kmToc, "dynamic_testpack", 1);

            ModManager.Instance.ModMissionToCs.Add(toc);
        }

        //private void Start()
        //{
        //    StartCoroutine(Setup());
        //}

        //private IEnumerator Setup()
        //{
        //    yield return new WaitForSeconds(15f);
        //    Debug.Log("Loading test mission");
        //    MissionLoader.LoadTestMission();
        //}
    }
}

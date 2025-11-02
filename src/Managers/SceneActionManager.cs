using OutwardModsCommunicator.EventBus;
using OutwardSceneTester.Scene;
using OutwardSceneTester.Utility.Helpers;
using SceneTester;
using SideLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutwardSceneTester.Managers
{
    public class SceneActionManager
    {
        private static SceneActionManager _instance;

        private SceneActionManager()
        {
        }

        public static SceneActionManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SceneActionManager();

                return _instance;
            }
        }

        public int CurrentSceneLoop { get => currentSceneLoop; set => currentSceneLoop = value; }

        private int currentSceneLoop = 0;

        List<SceneActionRule> sceneActions = new List<SceneActionRule>();

        public void AddSceneLoadSubscribtion()
        {
            SL.OnSceneLoaded += () =>
            {
                TryToLoadNextScene();
            };
        }

        public void TryToLoadNextScene()
        {
            try
            {
                if (SceneManagerHelper.ActiveSceneName == "LowMemory_TransitionScene" ||
                SceneManagerHelper.ActiveSceneName == "MainMenu_Empty" ||
                !ST.hasStartedLooping)
                    return;

                foreach (SceneActionRule sceneAction in sceneActions)
                {
                    SceneActionManager.Instance.TryExecuteAction(sceneAction);
                }

                if (sceneActions.Count > 0)
                {
                    AreaHelpers.TravelTo(sceneActions[0].GetUnvisitedAreas().First());
                }
            }
            catch(Exception ex)
            {
                ST.LogMessage($"SceneActionManager@TryToLoadNextScene received an error: \"{ex.Message}\"!");
            }
        }

        public void RemoveSceneLoopAction(EventPayload payload)
        {
            if (payload == null)
                return;

            string id = payload.Get<string>("actionId", null);

            if (string.IsNullOrEmpty(id))
                return;

            foreach (SceneActionRule rule in sceneActions.Where(action => id == action.ID))
            {
                FinishedLoopAction(rule);
            }
            sceneActions.RemoveAll(action => id == action.ID);
        }

        public void AddSceneLoopAction(EventPayload payload)
        {
            if (payload == null)
                return;

            Action function = payload.Get<Action>("action", null);

            if (function == null)
                return;

            HashSet<AreaManager.AreaEnum> areas = payload.Get<HashSet<AreaManager.AreaEnum>>("hashSetOfAreas", new HashSet<AreaManager.AreaEnum>());

            if (areas.Count < 1)
                return;

            string id = payload.Get<string>("actionId", null);

            if(string.IsNullOrEmpty(id))
                sceneActions.Add( new SceneActionRule(function, areas) );
            else
                sceneActions.Add( new SceneActionRule(id, function, areas) );
        }

        public void TryExecuteAction(SceneActionRule rule)
        {
            try
            {
                NetworkLevelLoader.Instance.SetContinueAfterLoading();
                rule.ValidateRule();

                if (rule.VisitedAreas.Count == rule.Areas.Count)
                {
                    FinishedLoopAction(rule);
                    sceneActions.Remove(rule);
                }
            }
            catch(Exception ex)
            {
                ST.LogMessage($"SceneActionManager@TryExecuteAction received an error: \"{ex.Message}\"!");
            }
        }

        public static void FinishedLoopAction(SceneActionRule rule)
        {
            var payload = new EventPayload
            {
                ["actionId"] =  rule.ID
            };

            EventBus.Publish(ST.GUID, "FinishedSceneLoopAction", payload);
        }
    }
}

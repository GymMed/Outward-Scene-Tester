using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SideLoader;
using OutwardModsCommunicator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using OutwardModsCommunicator.EventBus;
using OutwardSceneTester.Managers;

namespace SceneTester
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(OutwardModsCommunicator.OMC.GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public class ST : BaseUnityPlugin
    {
        public const string GUID = "gymmed.scene_tester";
        public const string NAME = "Scene Tester";
        public const string VERSION = "0.0.1";

        public static string prefix = "[Scene-Tester]";

        public const string EVENT_LISTENER_GUID = GUID + "_*";

        public static bool hasStartedLooping = false;

        public const string SCENE_TESTER_KEY = "Scene Tester Start/Stop Looping";

        internal static ManualLogSource Log;

        // If you need settings, define them like so:
        //public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            // You can find BepInEx logs in directory "BepInEx\LogOutput.log"
            Log = this.Logger;
            LogMessage($"Hello world from {NAME} {VERSION}!");

            CustomKeybindings.AddAction(SCENE_TESTER_KEY, KeybindingsCategory.CustomKeybindings, ControlType.Both);

            // Any config settings you define should be set up like this:
            //ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");

            new Harmony(GUID).PatchAll();

            // subscribed
            EventBus.RegisterEvent(
                EVENT_LISTENER_GUID, 
                "AddSceneLoopAction", 
                ("actionId", typeof(string), "Optional. You will need this as unique value if you want to tack(remove/listen for finish) action execution."),
                ("action", typeof(Action), "Required. Action to execute after each area load."),
                ("hashSetOfAreas", typeof(HashSet<AreaManager.AreaEnum>), "Required. HashSet of AreaManager.AreaEnum to fast loop through and execute \"action\" on.")
            );

            EventBus.RegisterEvent(
                EVENT_LISTENER_GUID,
                "RemoveSceneLoopAction",
                ("actionId", typeof(string), "Required. Scene loop action id that will be removed.")
            );

            // published
            EventBus.RegisterEvent(
                ST.GUID,
                "FinishedSceneLoopAction",
                ("actionId", typeof(string), "Finished scene loop action id.")
            );

            EventBus.Subscribe(EVENT_LISTENER_GUID, "AddSceneLoopAction", SceneActionManager.Instance.AddSceneLoopAction);
            EventBus.Subscribe(EVENT_LISTENER_GUID, "RemoveSceneLoopAction", SceneActionManager.Instance.RemoveSceneLoopAction);

            SceneActionManager.Instance.AddSceneLoadSubscribtion();
        }

        internal void Update()
        {
            if (!CustomKeybindings.GetKeyDown(SCENE_TESTER_KEY))
            {
                return;
            }

            if(hasStartedLooping)
            {
                hasStartedLooping = false;
                SceneActionManager.Instance.CurrentSceneLoop = 0;
                return;
            }

            if (SceneManagerHelper.ActiveSceneName == "LowMemory_TransitionScene" || SceneManagerHelper.ActiveSceneName == "MainMenu_Empty")
                return;

            hasStartedLooping = true;
            SceneActionManager.Instance.TryToLoadNextScene();
        }

        //  Log message with prefix
        public static void LogMessage(string message)
        {
            Log.LogMessage($"{ST.prefix} {message}");
        }

        // Log message through side loader, helps to see it
        // if you are using UnityExplorer and want to see live logs
        public static void LogSL(string message)
        {
            SL.Log($"{ST.prefix} {message}");
        }

        // Gets mod dll location
        public static string GetProjectLocation()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        // This is an example of a Harmony patch.
        // If you're not using this, you should delete it.
        [HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
        public class ResourcesPrefabManager_Load
        {
            static void Postfix(ResourcesPrefabManager __instance)
            {
                // use Debug build for things you don't want to release
                #if DEBUG
                // provide class and method separated by @ for easier live debugging
                LogSL("ResourcesPrefabManager@Load called!");
                #endif
            }
        }
    }
}

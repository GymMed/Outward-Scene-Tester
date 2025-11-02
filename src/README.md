<h1 align="center">
    Outward Scene Tester
</h1>
<br/>
<div align="center">
  <img src="https://raw.githubusercontent.com/GymMed/Outward-Scene-Tester/refs/heads/main/preview/images/Logo.png" alt="Logo"/>
</div>

<details>
    <summary>Outward Scene Tester is a tool that automatically loops through scenes and executes functions from your mods.</summary>
It uses the <a href="https://thunderstore.io/c/outward/p/GymMed/Mods_Communicator/">Mods Communicator</a> framework to communicate with other mods.
Through this connection, it provides event listeners and emitters to exchange data.

You can also start or stop scene looping via a dedicated key binding.

When you register functions through events, they are added as <code>SceneActionRules</code>.
Each rule is checked whenever a scene loads — ensuring your code only runs once per area, avoiding duplicate executions.
</details>
 
<details>
    <summary>Use Case</summary>
I used this in <a
href="https://thunderstore.io/c/outward/p/GymMed/Loot_Manager/">Loot
Manager</a> to get all unique enemies data and determine if certain loot should
be aplied. Outward wiki doesn't provide all data correctly, neither the game is
programmed in consistant order, this tool can help you to retrieve the needed scene data.
</details>

<details>
    <summary>Subscribing and Publishing Events</summary>

<details>
    <summary>Listening to Events</summary>
Uses <code>gymmed.scene_tester</code> as mod namespace.
Here are provided all events you can subscribe to and listen:
<details>
    <summary>Finished Scene Loop Action</summary>
Scene Tester mod <code>gymmed.scene_tester</code> fires event
<code>FinishedSceneLoopAction</code> with payload <code>actionId</code>. It
fires it after all action areas a looped and provided action code is executed.
<code>actionId</code> — the identifier you assigned when adding your action
(used to verify it’s your event).

<details>
    <summary>Example</summary>
We'll use a <code>ResourcesPrefabManager.Load</code> patch to ensure Scene Tester is initialized before subscribing:
<pre><code>using OutwardModsCommunicator;
[HarmonyPatch(typeof(ResourcesPrefabManager), nameof(ResourcesPrefabManager.Load))]
public class ResourcesPrefabManager_Load
{
    static void Postfix(ResourcesPrefabManager __instance)
    {
        EventBus.Subscribe("gymmed.scene_tester", "FinishedSceneLoopAction", OnFinish);
    }
}
public static void OnFinish(EventPayload payload)
{
    if (payload == null) return;
    string actionId = payload.Get<string>("actionId", null);
    if(string.IsNullOrEmpty(actionId))
    {
        Log.LogMessage($"MyMod@OnFinish didn't receive actionId variable!");
        return;
    }
    ...continue with execution...
}</code></pre>
</details>
</details>

</details>

<details>
    <summary>Publishing Events</summary>
Uses <code>gymmed.scene_tester_*</code> as mod namespace.
Here are provided all events you can publish:
<details>
    <summary>Add Scene Loop Action</summary>
Event:<code>AddSceneLoopAction</code><br>
Registers a new looping action that will be executed on each scene load.
<br><br>
Payload:
<br><br>
actionId (optional) — a string to identify your action (useful for tracking or removing later).
<br><br>
action — a C# Action delegate (your function reference).
<br><br>
hashSetOfAreas — a <code>HashSet< AreaManager.AreaEnum ></code> defining which areas to loop over.

<details>
    <summary>Example:</summary>
<pre><code>using OutwardModsCommunicator;
HashSet<AreaManager.AreaEnum> areas = new HashSet<AreaManager.AreaEnum>();
areas.Add(AreaManager.AreaEnum.Abrassar);
areas.Add(AreaManager.AreaEnum.Emercar);
areas.Add(AreaManager.AreaEnum.HallowedMarsh);
Action function = () =>
{
    string areaName = AreaManager.Instance.CurrentArea.GetName();
    Log.LogMessage($"Current Area GetName:{areaName}");
};
var payload = new EventPayload
{
    ["actionId"] = actionId,
    ["action"] = function,
    ["hashSetOfAreas"] = areas,
};
EventBus.Publish("gymmed.scene_tester_*", "AddSceneLoopAction", payload);</code></pre>
            
</details>

</details>

<details>
    <summary>Remove Scene Loop Action</summary>
Listens for event named <code>RemoveSceneLoopAction</code> with payload
<code>actionId</code>.<code>actionId</code> — the ID of the action you want to remove.

<details>
    <summary>Code example:</summary>
<pre><code>using OutwardModsCommunicator;
var payload = new EventPayload
{
    ["actionId"] = actionId,
};
EventBus.Publish("gymmed.scene_tester_*", "RemoveSceneLoopAction", payload);</code></pre>
</details>

</details>

</details>

</details>

## How to set up

To manually set up, do the following

1. Create the directory: `Outward\BepInEx\plugins\OutwardSceneTester\`.
2. Extract the archive into any directory(recommend empty).
3. Move the contents of the plugins\ directory from the archive into the `BepInEx\plugins\OutwardSceneTester\` directory you created.
4. It should look like `Outward\BepInEx\plugins\OutwardSceneTester\OutwardSceneTester.dll`
   Launch the game.

### If you liked the mod leave a star on [GitHub](https://github.com/GymMed/Outward-Scene-Tester) it's free

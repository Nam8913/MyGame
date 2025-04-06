using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class CurrentGame
{
    public static void GlobalNotifyChangedScene()
    {
        TryGetSceneObject();
    }
    public static OverworldData getWorld
    {
        get
        {
            if(worldData == null)
            {
                Debug.LogError("No world data found in the current game.");
                return null;
            }
            return worldData;
        }
    }
    public static OverworldData setWorld
    {
        set
        {
            worldData = value;
        }
    }

    private static void TryGetSceneObject()
    {
        if(InEntryScene)
        {
            entryScene = GameObject.FindFirstObjectByType<SceneEntry>();
            if(entryScene == null)
            {
                entryScene = GameObject.Find("Host").GetComponent<SceneEntry>();
            }
            if (entryScene == null)
            {
                Debug.LogError("No scene object found in the current game.");
                return;
            }
            currScene = entryScene;
        }else if(InPlayScene)
        {
            playScene = GameObject.FindFirstObjectByType<ScenePlay>();
            if(playScene == null)
            {
                playScene = GameObject.Find("Host").GetComponent<ScenePlay>();
            }
            if (playScene == null)
            {
                Debug.LogError("No scene object found in the current game.");
                return;
            }
            currScene = playScene;
        }else
        {
            Debug.LogError("No scene object valid found in the current game.");
            return;
        }
    }

    public static SceneAbstract currScene;
    public static ScenePlay playScene;
    public static SceneEntry entryScene;
    public static OverworldData worldData;

    private static readonly List<string> sceneNames = new List<string>()
    {
        "Play",
        "Entry",
    };
    public static bool InEntryScene
    {
        get
        {
            return SceneManager.GetActiveScene().name == "Entry";
        }
    }
    public static bool InPlayScene
    {
        get
        {
            return SceneManager.GetActiveScene().name == "Entry";
        }
    }
}

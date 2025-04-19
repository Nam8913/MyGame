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

    public static GameObject getSingletonObject
    {
        get
        {
            if(singletonObject == null)
            {
                singletonObject = GameObject.Find("Singletons");
                if(singletonObject == null)
                {
                    singletonObject = new GameObject("Singletons");
                    UnityEngine.Object.DontDestroyOnLoad(singletonObject);
                }
            }
            return singletonObject;
        }
    }
    public static SceneAbstract GetSceneObject
    {
        get
        {
            if (currScene == null)
            {
                Debug.LogError("No scene object found in the current game.");
                return null;
            }
            return currScene;
        }
    }

    public static ScenePlay GetScenePlay
    {
        get
        {
            if (playScene == null)
            {
                Debug.LogError("Not in scene play in the current game.");
            }
            return playScene;
        }
    }
    public static SceneEntry GetSceneEntry
    {
        get
        {
            if (entryScene == null)
            {
                Debug.LogError("Not in scene entry in the current game.");
            }
            return entryScene;
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

    private static SceneAbstract currScene;
    private static ScenePlay playScene;
    private static SceneEntry entryScene;
    private static OverworldData worldData;
    private static GameObject singletonObject = null;

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
            return SceneManager.GetActiveScene().name == "Play";
        }
    }
}

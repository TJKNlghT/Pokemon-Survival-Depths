using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [SerializeField] private string lastMapSceneName;

    private readonly List<string> currentMapBuffSources = new();
    private Entity_Buffs playerBuffs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Called once when the persistent player exists / changes
    public void RegisterPlayer(Entity_Buffs buffs)
    {
        playerBuffs = buffs;
    }

    // Called by each map's manager at Start
    public void BeginMap(string sceneName)
    {
        lastMapSceneName = sceneName;
        currentMapBuffSources.Clear();
    }

    public string GetLastMapSceneName() => lastMapSceneName;

    // Called whenever a map-scoped buff is picked in THIS map
    public void RegisterMapBuffSource(string sourceId)
    {
        if (string.IsNullOrEmpty(sourceId)) return;
        currentMapBuffSources.Add(sourceId);
    }

    // Remove only buffs applied in this map
    public void ClearCurrentMapBuffs()
    {
        if (playerBuffs == null) return;

        foreach (var src in currentMapBuffSources)
        {
            playerBuffs.RemoveBySource(src);
        }
        currentMapBuffSources.Clear();
    }

    public void ResetRun()
    {
        currentMapBuffSources.Clear();
        lastMapSceneName = "";
    }

    public void ResetAllProgress()
    {
        // reset runtime data
        ResetRun();

        // wipe map progression PlayerPrefs
        PlayerPrefs.DeleteKey("ForestCompleted");
        PlayerPrefs.DeleteKey("DesertCompleted");
        PlayerPrefs.DeleteKey("CurrentMapScene");

        // if you also store meta stuff like coins, unlocks, etc:
        // PlayerPrefs.DeleteKey("SomeOtherMetaKey");

        PlayerPrefs.Save();
    }


}
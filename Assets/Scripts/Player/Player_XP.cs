using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player_XP : MonoBehaviour
{
    [Header("Level / XP")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentXP = 0;

    [SerializeField] private int baseXPToLevel = 10;
    [SerializeField] private float xpGrowthFactor = 1.5f;

    [Header("Level-up Buffs")]
    [SerializeField] private Buff_DataSO[] candidateBuffs; // drag all possible level-up buffs here

    [Header("UI")]
    [SerializeField] private Slider xpBar;

    private int xpToNextLevel;

    private LevelUpQueue levelUpQueue;
    private Entity_Buffs playerBuffs;

    public int Level => level;
    public int CurrentXP => currentXP;
    public int XPToNextLevel => xpToNextLevel;

    public event Action<int> OnLevelUp;   // still available if you want listeners

    private void Awake()
    {
        xpToNextLevel = GetXPNeededForLevel(level);

        levelUpQueue = FindFirstObjectByType<LevelUpQueue>();
        playerBuffs = GetComponent<Entity_Buffs>();

        TryResolveLevelUpQueue();

        UpdateXPBar();

        if (!levelUpQueue)
            Debug.LogWarning("[Player_XP] LevelUpQueue not found in scene.");
        if (!playerBuffs)
            Debug.LogWarning("[Player_XP] Entity_Buffs not found on Player.");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // each time a new scene loads, re-find the LevelUpQueue in that scene
        TryResolveLevelUpQueue();
    }

    private void TryResolveLevelUpQueue()
    {
        levelUpQueue = FindFirstObjectByType<LevelUpQueue>();
        if (!levelUpQueue)
            Debug.LogWarning("[Player_XP] LevelUpQueue not found in scene.");
    }

    public void ResetXPForCurrentLevel()
    {
        // Keep the current level, just reset XP progress for this level
        currentXP = 0;
        xpToNextLevel = GetXPNeededForLevel(level);
        UpdateXPBar();
    }

    public void ResetXPAndLevel(int startLevel = 1)
    {
        // If you ever want a full run reset
        level = startLevel;
        currentXP = 0;
        xpToNextLevel = GetXPNeededForLevel(level);
        UpdateXPBar();
    }

    public void BindXPBar(Slider slider)
    {
        xpBar = slider;
        UpdateXPBar();
    }

    private void UpdateXPBar()
    {
        if (!xpBar)
            return;

        xpBar.minValue = 0;
        xpBar.maxValue = Mathf.Max(1, xpToNextLevel);
        xpBar.value = Mathf.Clamp(currentXP, 0, xpToNextLevel);
    }


    private int GetXPNeededForLevel(int lvl)
    {
        return Mathf.RoundToInt(baseXPToLevel * Mathf.Pow(xpGrowthFactor, lvl - 1));
    }

    public void AddXP(int amount)
    {
        if (amount <= 0) return;

        currentXP += amount;

        // If you strictly want "XP goes back to 0" after *each* level:
        while (currentXP >= xpToNextLevel)
        {
            currentXP = 0;          // reset instead of subtracting
            LevelUp();
            // if you don't want to allow multiple levels in one pickup, break; here
        }

        UpdateXPBar();
    }

    private void LevelUp()
    {
        level++;
        xpToNextLevel = GetXPNeededForLevel(level);

        Debug.Log($"Player leveled up! Level {level}, XP {currentXP}/{xpToNextLevel}");

        OnLevelUp?.Invoke(level);

        // Open the buff-pick UI via LevelUpQueue

        UpdateXPBar();
        TryEnqueueLevelUpPanel();
    }

    private void TryEnqueueLevelUpPanel()
    {
        // ensure we have the queue for THIS scene
        if (!levelUpQueue)
            TryResolveLevelUpQueue();

        if (!levelUpQueue || !playerBuffs || candidateBuffs == null || candidateBuffs.Length == 0)
        {
            Debug.LogWarning("[Player_XP] Cannot show level-up panel: missing queue, buffs or candidateBuffs.");
            return;
        }

        var options = PickThree(candidateBuffs);
        levelUpQueue.Enqueue(options, playerBuffs);
    }

    private static Buff_DataSO[] PickThree(Buff_DataSO[] pool)
    {
        var outArr = new Buff_DataSO[3];
        if (pool == null || pool.Length == 0) return outArr;

        int count = 0, guard = 100;
        while (count < 3 && guard-- > 0)
        {
            var pick = pool[UnityEngine.Random.Range(0, pool.Length)];
            bool dup = false;
            for (int i = 0; i < count; i++)
                if (outArr[i] == pick) { dup = true; break; }

            if (!dup) outArr[count++] = pick;
            if (pool.Length < 3 && count >= pool.Length) break;
        }
        return outArr;
    }
}
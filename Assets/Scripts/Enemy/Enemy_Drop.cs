using UnityEngine;

public class Enemy_Drop : MonoBehaviour
{
    [System.Serializable]
    public class LootEntry
    {
        public GameObject prefab;                 // your PickUps_* prefab
        [Range(0f, 1f)] public float dropChance;  // 0–1 probability
        public int minCount = 1;
        public int maxCount = 1;
    }

    [Header("Loot Table")]
    [SerializeField] private LootEntry[] lootEntries;

    [Header("Options")]
    [SerializeField] private bool dropOnlyOnce = true;
    [SerializeField] private Vector2 randomOffsetRadiusRange = new Vector2(0f, 0.5f);

    private bool hasDropped;

    public void TryDrop()
    {
        if (dropOnlyOnce && hasDropped)
            return;

        hasDropped = true;

        if (lootEntries == null) return;

        foreach (var entry in lootEntries)
        {
            if (!entry.prefab)
                continue;

            // roll chance
            if (Random.value > entry.dropChance)
                continue;

            int count = Random.Range(entry.minCount, entry.maxCount + 1);

            for (int i = 0; i < count; i++)
            {
                Vector3 pos = transform.position;

                // slight spread to avoid perfect overlap
                if (randomOffsetRadiusRange.y > 0f)
                {
                    float r = Random.Range(randomOffsetRadiusRange.x, randomOffsetRadiusRange.y);
                    float angle = Random.Range(0f, Mathf.PI * 2f);
                    pos += new Vector3(Mathf.Cos(angle) * r, Mathf.Sin(angle) * r, 0f);
                }

                Instantiate(entry.prefab, pos, Quaternion.identity);
            }
        }
    }
}

using UnityEngine;

/// <summary>
/// Спаунит один из трёх объектов с одним ползунком распределения вероятности (0-1)
/// </summary>
public class RandomObjectSpawnerSingleSlider : MonoBehaviour
{
    [SerializeField] private GameObject m_Prefab1;
    [SerializeField] private GameObject m_Prefab2;
    [SerializeField] private GameObject m_Prefab3;

    [Range(0f, 1f)]
    [SerializeField] private float m_Distribution = 0.33f;
    [Tooltip("Как распределяется вероятность для объектов 2 и 3 относительно друг друга (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float m_SecondaryDistribution = 0.5f;

    [SerializeField] private Transform m_SpawnPoint;

    private void Start()
    {
        Spawn();
    }

    public GameObject Spawn()
    {
        // Первый объект получает вероятность m_Distribution
        // Остальные две делят (1 - m_Distribution)
        
        float prob1 = m_Distribution;
        float remainingProb = 1f - m_Distribution;
        float prob2 = remainingProb * m_SecondaryDistribution;
        float prob3 = remainingProb * (1f - m_SecondaryDistribution);

        float random = Random.value;

        if (random <= prob1)
            return SpawnObject(m_Prefab1, "Объект 1");
        else if (random <= prob1 + prob2)
            return SpawnObject(m_Prefab2, "Объект 2");
        else
            return SpawnObject(m_Prefab3, "Объект 3");
    }

    private GameObject SpawnObject(GameObject prefab, string name)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"{name} не указан!");
            return null;
        }

        Vector3 spawnPosition = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;
        Quaternion spawnRotation = m_SpawnPoint != null ? m_SpawnPoint.rotation : transform.rotation;

        GameObject spawnedObject = Instantiate(prefab, spawnPosition, spawnRotation);
        Debug.Log($"Спаунен {name}: {spawnedObject.name}");
        return spawnedObject;
    }

    [ContextMenu("Показать вероятности")]
    public void ShowProbabilities()
    {
        float prob1 = m_Distribution;
        float remainingProb = 1f - m_Distribution;
        float prob2 = remainingProb * m_SecondaryDistribution;
        float prob3 = remainingProb * (1f - m_SecondaryDistribution);

        Debug.Log($"Объект 1: {prob1:P}\nОбъект 2: {prob2:P}\nОбъект 3: {prob3:P}");
    }

    [ContextMenu("Test Spawn")]
    public void TestSpawn()
    {
        Spawn();
    }
}

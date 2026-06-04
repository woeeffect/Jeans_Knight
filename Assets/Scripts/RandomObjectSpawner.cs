using UnityEngine;

/// <summary>
/// Спаунит один из трёх объектов с распределённой вероятностью
/// </summary>
public class RandomObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class SpawnObject
    {
        public GameObject prefab;
        [Range(0f, 1f)] public float probability = 0.33f;
    }

    [SerializeField] private SpawnObject[] m_Objects = new SpawnObject[3];
    [SerializeField] private Transform m_SpawnPoint;

    private void OnValidate()
    {
        // Убедимся, что массив имеет 3 элемента
        if (m_Objects.Length != 3)
        {
            System.Array.Resize(ref m_Objects, 3);
        }
    }

    private void Start()
    {
        Spawn();
    }

    public GameObject Spawn()
    {
        // Нормализуем вероятности так, чтобы они в сумме равнялись 1
        float totalProbability = m_Objects[0].probability + m_Objects[1].probability + m_Objects[2].probability;
        
        if (totalProbability <= 0)
        {
            Debug.LogWarning("Сумма вероятностей равна 0!");
            return null;
        }

        float[] normalizedProbs = new float[3];
        for (int i = 0; i < 3; i++)
        {
            normalizedProbs[i] = m_Objects[i].probability / totalProbability;
        }

        // Генерируем случайное число
        float random = Random.value; // 0 до 1
        float cumulative = 0f;

        for (int i = 0; i < 3; i++)
        {
            cumulative += normalizedProbs[i];
            if (random <= cumulative)
            {
                return InstantiateObject(i);
            }
        }

        // Страховка - спаунит последний объект
        return InstantiateObject(2);
    }

    private GameObject InstantiateObject(int index)
    {
        if (m_Objects[index].prefab == null)
        {
            Debug.LogWarning($"Объект #{index} не указан!");
            return null;
        }

        Vector3 spawnPosition = m_SpawnPoint != null ? m_SpawnPoint.position : transform.position;
        Quaternion spawnRotation = m_SpawnPoint != null ? m_SpawnPoint.rotation : transform.rotation;

        GameObject spawnedObject = Instantiate(
            m_Objects[index].prefab,
            spawnPosition,
            spawnRotation
        );

        Debug.Log($"Спаунен объект #{index}: {spawnedObject.name}");
        return spawnedObject;
    }

    // Вызов через инспектор для тестирования
    [ContextMenu("Test Spawn")]
    public void TestSpawn()
    {
        Spawn();
    }
}

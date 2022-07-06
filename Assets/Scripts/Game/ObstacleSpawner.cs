using UnityEngine;
using Unity.AI.Navigation;

namespace ExpertCenTest.Game
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField]
        private int cubeCount;
        [SerializeField]
        private NavMeshSurface[] surfaces;
        //Параметр, отвечающий за то, сколько попыток спауна будет произведено.
        [SerializeField]
        private int safetyCounter;

        [Header("Reference")]
        [SerializeField]
        private GameObject obstaclePrefab;
        [SerializeField]
        private GameObject field;

        //Аналогично спауну агентов из AgentsSpawner
        //При создании каждого препятствия в цикле выбирается случайная позиция и проверяется есть ли там достаточно места для него. 
        //Если места нет, то цикл продолжается пока такое место не будет найдено или не будет достигнут лимит попыток из safetyCounter.
        //Если лимит достигнут, то препятствие заспаунится в последней позиции.
        //Можно написать более надежный алгоритм с использованием k-d tree.
        public void Initialize()
        {
            for (int i = 0; i < cubeCount; i++)
            {
                bool isOverlapped = true;
                Vector3 position = Vector3.zero;
                Vector3 scaleMultiplier = new Vector3(Random.Range(0.5f, 2f), 1f, Random.Range(0.5f, 2f));
                int counter = 0;
                while (isOverlapped && counter < safetyCounter)
                {
                    position = new Vector3(Random.Range(field.GetComponent<Collider>().bounds.min.x+scaleMultiplier.x/2, field.GetComponent<Collider>().bounds.max.x - scaleMultiplier.x / 2), 0.5f, Random.Range(field.GetComponent<Collider>().bounds.min.z + scaleMultiplier.z / 2, field.GetComponent<Collider>().bounds.max.z - scaleMultiplier.z / 2));
                    Bounds bounds = new Bounds(position, new Vector3(scaleMultiplier.x, scaleMultiplier.y, scaleMultiplier.z));
                    int overlappedCount = Physics.OverlapBox(position, bounds.size).Length;
                    if (overlappedCount > 1)
                    {
                        isOverlapped = true;
                    }
                    else
                    {
                        isOverlapped = false;
                    }
                    counter++;
                }
                GameObject instance = Instantiate(obstaclePrefab, position, Quaternion.identity, transform);
                instance.transform.localScale = scaleMultiplier;
                instance.name = i.ToString();
            }
            //Когда все препятствия расставлены запекается NavMesh по которому будут двигаться агенты.
            for (int i = 0; i < surfaces.Length; i++)
            {
                surfaces[i].BuildNavMesh();
            }
        }
    }
}
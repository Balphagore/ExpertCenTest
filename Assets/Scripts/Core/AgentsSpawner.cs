using System.Collections.Generic;
using UnityEngine;

namespace ExpertCenTest.Core
{
    public class AgentsSpawner : MonoBehaviour
    {
        //Параметр, отвечающий за то, сколько попыток спауна будет произведено.
        [SerializeField]
        private int safetyCounter;

        [Header("References")]
        [SerializeField]
        private GameObject agentPrefab;
        [SerializeField]
        private GameObject field;

        public delegate void SpawnAgentsHandle(List<Agent> agents);
        public event SpawnAgentsHandle SpawnAgentsEvent;

        public void Initialize(GameInterface gameInterface)
        {
            gameInterface.CreateAgentsEvent += OnCreateAgentsEvent;
        }

        private void OnCreateAgentsEvent(int agentsNumber)
        {
            //При создании каждого агента в цикле выбирается случайная позиция и проверяется есть ли там достаточно места для агента. 
            //Если места нет, то цикл продолжается пока такое место не будет найдено или не будет достигнут лимит попыток из safetyCounter.
            //Если лимит достигнут, то агент заспаунится в последней позиции.
            //Можно написать более надежный алгоритм с использованием k-d tree.
            List<Agent> agents = new List<Agent>();
            for (int i = 0; i < agentsNumber; i++)
            {
                bool isOverlapped = true;
                Vector3 position = Vector3.zero;
                int counter = 0;
                while (isOverlapped && counter < safetyCounter)
                {
                    position = new Vector3(Random.Range(field.GetComponent<Collider>().bounds.min.x, field.GetComponent<Collider>().bounds.max.x), 0.5f, Random.Range(field.GetComponent<Collider>().bounds.min.z, field.GetComponent<Collider>().bounds.max.z));
                    Bounds bounds = new Bounds(position, new Vector3(1f, 1f, 1f));
                    Debug.DrawLine(bounds.min, bounds.max, Color.green, 60f);
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
                GameObject instance = Instantiate(agentPrefab, position, Quaternion.identity, transform);
                instance.name = i.ToString();
                agents.Add(instance.GetComponent<Agent>());
            }
            SpawnAgentsEvent?.Invoke(agents);
        }
    }
}
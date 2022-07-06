using System.Collections.Generic;
using UnityEngine;

namespace ExpertCenTest.Core
{
    public class AgentsController : MonoBehaviour
    {
        [SerializeField]
        private List<Agent> hunterAgents;
        [SerializeField]
        private List<Agent> wildAgents;

        public delegate void UpdateTargetsHandle(List<Agent> hunterAgents, List<Agent> wildAgents);
        public event UpdateTargetsHandle UpdateTargetsEvent;

        public delegate void UpdateAgentsNumberHandle(int hunterAgentsCount, int wildAgentsCount);
        public event UpdateAgentsNumberHandle UpdateAgentsNumberEvent;

        private void Update()
        {
            //Ќа этот ивент подписан FindTargetSystem. ѕри его получении он пробежитс€ по обоим спискам агентов и определит куда им надо двигатьс€.
            //ћожно уменьшить частоту вызова ивента дл€ оптимизации
            UpdateTargetsEvent?.Invoke(hunterAgents, wildAgents);
        }

        public void Initialize(AgentsSpawner agentsSpawner, GameInterface gameInterface)
        {
            agentsSpawner.SpawnAgentsEvent += OnSpawnAgentsEvent;
            gameInterface.DestroyAgentsEvent += OnDestroyAgentsEvent;
            gameInterface.InvertAllAgentsEvent += OnInvertAllAgentsEvent;
        }

        //¬ ответ на нажатие кнопки удалени€ агентов удал€ем всех агентов из списков а затем очищаем списки.
        //≈сли есть необходимость оптимизации, можно прикрутить пул объектов. —паунер будет брать агентов из списка, а так как все удалени€ агентов происход€т в этом классе,
        //то отсюда их можно возвращать в пул
        private void OnDestroyAgentsEvent()
        {
            for (int i = 0; i < hunterAgents.Count; i++)
            {
                Destroy(hunterAgents[i].gameObject);
            }
            HunterAgents.Clear();
            for (int i = 0; i < wildAgents.Count; i++)
            {
                Destroy(wildAgents[i].gameObject);
            }
            WildAgents.Clear();
        }

        // огда спаунер инстанциирует агентов, он передает их в контроллер с этим ивентом.
        private void OnSpawnAgentsEvent(List<Agent> agents)
        {
            foreach (Agent agent in agents)
            {
                agent.RemoveAgentEvent += OnRemoveAgentEvent;
                agent.InvertAgentEvent += OnInvertAgentEvent;
                //“ип агента задаетс€ тут и полностью случайно. ѕри необходимости можно расширить или перенести в спаунер
                bool isHunter = UnityEngine.Random.Range(0, 2) == 0 ? true : false;
                agent.IsHunter = isHunter;
                if (isHunter)
                {
                    HunterAgents.Add(agent);
                }
                else
                {
                    WildAgents.Add(agent);
                }
            }
        }

        private void OnInvertAgentEvent(Agent agent)
        {
            if (agent.IsHunter)
            {
                HunterAgents.Remove(agent);
                WildAgents.Add(agent);
                agent.IsHunter = false;
            }
            else
            {
                HunterAgents.Add(agent);
                WildAgents.Remove(agent);
                agent.IsHunter = true;
            }
        }

        private void OnRemoveAgentEvent(Agent agent)
        {
            if (hunterAgents.Contains(agent))
            {
                agent.RemoveAgentEvent -= OnRemoveAgentEvent;
                HunterAgents.Remove(agent);
            }
            else
            {
                agent.RemoveAgentEvent -= OnRemoveAgentEvent;
                WildAgents.Remove(agent);
            }
            Destroy(agent.gameObject);
        }

        //ѕодсчет количества активных агентов происходит при помощи этих оберток. 
        // аждый раз когда контроллер как-то измен€ет состав списков агентов вызываетс€ ивент, на который подписан GameInterface
        private List<Agent> HunterAgents
        {
            get
            {
                UpdateAgentsNumberEvent?.Invoke(hunterAgents.Count, wildAgents.Count);
                return hunterAgents;
            }
        }

        private List<Agent> WildAgents
        {
            get
            {
                UpdateAgentsNumberEvent?.Invoke(hunterAgents.Count, wildAgents.Count);
                return wildAgents;
            }
        }


        private void OnInvertAllAgentsEvent()
        {
            List<Agent> bufferList = new List<Agent>();
            bufferList = wildAgents;
            wildAgents = hunterAgents;
            hunterAgents = bufferList;
            foreach (Agent agent in hunterAgents)
            {
                agent.IsHunter = true;
            }
            foreach (Agent agent in wildAgents)
            {
                agent.IsHunter = false;
            }
            UpdateAgentsNumberEvent?.Invoke(hunterAgents.Count, wildAgents.Count);
        }
    }
}
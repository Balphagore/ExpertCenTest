using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ExpertCenTest.Core
{
    public class FindTargetSystem : MonoBehaviour
    {
        //–ассто€ние на котором дичь начинает убегать от охотников. «адаетс€ из соответствующего пол€ интерфейса.
        [SerializeField]
        private float escapeDistance;
        //–ассто€ние на котором дичь ищет безопасное место по пр€мой от ближайшего охотника. —лишком большие и слишком маленькие значени€ вызывают не совсем корректный выбор маршрута.
        [SerializeField]
        private float escapeTargetDistance;
        //Ўаг с которым вычисл€етс€ безопасное место при побеге.
        [Range(1f, 360f)]
        [SerializeField]
        private float escapeDirectionStep;

        public void Initialize(AgentsController agentsController, GameInterface gameInterface)
        {
            agentsController.UpdateTargetsEvent += OnUpdateTargetsEvent;
            gameInterface.AgentsEscapeChangeEvent += OnAgentsEscapeChangeEvent;
        }

        private void OnAgentsEscapeChangeEvent(int escapeDistance)
        {
            this.escapeDistance = escapeDistance;
        }

        //»вент вызываетс€ AgentsContoller.  аждый вызов ивента происходит перерасчет того куда каждому агенту надо двигатьс€.
        private void OnUpdateTargetsEvent(List<Agent> hunterAgents, List<Agent> wildAgents)
        {
            //“ак как и охотники и дичь €вл€ютс€ одним классом, то у них одна универсальна€ логика выбора цели. ≈сли их логика будет различатьс€ сильнее, то можно создать два
            //разных класса и реализовать логику индивидуально.

            //ѕробегаем по всему списку охотников и находим дл€ каждого ближайшего агента из списка дичи и устанавливаем на него трансформ, к которому будет двигатьс€ охотник.
            if (hunterAgents.Count > 0)
            {
                foreach (Agent agent in hunterAgents)
                {
                    if (wildAgents.Count > 0)
                    {
                        Transform target = wildAgents.OrderBy(target => (agent.AgentTransform.position - target.transform.position).magnitude).First().transform;
                        agent.TargetTransform.position = target.position;
                    }
                    else
                    {
                        //≈сли дичи больше не осталось, то трансформ ставитс€ на самого агента и он стоит на месте.
                        agent.TargetTransform.position = agent.transform.position;
                    }
                }
            }

            //ѕробегаем по всему списку дичи и находим дл€ каждого ближайшего агента из списка охотников.
            //≈сли рассто€ние до него меньше рассто€ни€ побега, то начинаем искать безопасное место.
            //¬ первой попытке это точка на NavMesh по пр€мой в противоположной стороне от охотника на дистанции указанной в escapeTargetDistance.
            //≈сли эта точка и маршрут до нее свободны, то туда ставитс€ трансформ к которому двигаетс€ дичь.
            //≈сли эта точка зан€та, то поворачиваем вектор на количество градусов, указанное в escapeDirectionStep и провер€ем там. 
            //ѕроверка идет по кругу, пока не будет пройдено все 360 градусов или не будет найдено безопасное место. 
            if (wildAgents.Count > 0)
            {
                foreach (Agent agent in wildAgents)
                {
                    if (hunterAgents.Count > 0)
                    {
                        Agent hunterAgent = hunterAgents.OrderBy(target => (agent.AgentTransform.position - target.transform.position).magnitude).First();
                        float distance = (agent.transform.position - hunterAgent.transform.position).magnitude;
                        if (distance < escapeDistance)
                        {
                            Debug.DrawLine(agent.transform.position, hunterAgent.transform.position);

                            bool isClear = false;
                            int tries = 0;
                            Vector3 checkDirection = (agent.transform.position - hunterAgent.transform.position).normalized * escapeTargetDistance;

                            while (tries < 360/escapeDirectionStep && !isClear)
                            {
                                tries++;
                                NavMeshHit hit;
                                NavMeshAgent navMeshAgent = agent.NavMeshAgent;
                                if (!navMeshAgent.Raycast(agent.transform.position + checkDirection, out hit))
                                {
                                    isClear = true;
                                    break;
                                }
                                else
                                {
                                    checkDirection = Quaternion.Euler(0, escapeDirectionStep, 0) * checkDirection;
                                }
                            }
                            Debug.DrawLine(agent.transform.position, agent.transform.position + checkDirection, Color.yellow);
                            agent.TargetTransform.position = agent.transform.position + checkDirection;
                        }
                        else
                        {
                            agent.TargetTransform.position = agent.transform.position;
                        }
                    }
                }
            }
        }
    }
}
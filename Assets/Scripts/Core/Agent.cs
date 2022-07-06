using UnityEngine;
using UnityEngine.AI;

namespace ExpertCenTest.Core
{
    public class Agent : MonoBehaviour
    {
        [SerializeField]
        private bool isHunter;

        //Агент двигается к трансформу, который задает ему класс FindTargetSystem
        [SerializeField]
        private Transform targetTransform;
        [SerializeField]
        private float size = 1;

        [Header("References")]
        [SerializeField]
        private NavMeshAgent navMeshAgent;
        [SerializeField]
        private Transform agentTransform;
        [SerializeField]
        private Renderer agentRenderer;

        public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }
        public Transform AgentTransform { get => agentTransform; set => agentTransform = value; }
        public NavMeshAgent NavMeshAgent { get => navMeshAgent; set => navMeshAgent = value; }

        public delegate void RemoveAgentHandle(Agent agent);
        public event RemoveAgentHandle RemoveAgentEvent;

        public delegate void InvertAgentHandle(Agent agent);
        public event InvertAgentHandle InvertAgentEvent;

        public bool IsHunter 
        {
            get
            {
                return isHunter;
            }
            set
            {
                isHunter = value;
                if (isHunter)
                {
                    agentRenderer.material.color = Color.red;
                }
                else
                {
                    agentRenderer.material.color = Color.blue;
                }
            }
        }

        private void Update()
        {
            if (targetTransform != null)
            {
                //NavMeshAgent получает в качестве цели трансформ, полученный от FindTargetSystem и прокладывает до него маршрут.
                navMeshAgent.destination = targetTransform.position;
            }
        }

        void OnMouseDown()
        {
            InvertAgentEvent?.Invoke(this);
        }

        public void Consume()
        {
            RemoveAgentEvent?.Invoke(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Agent agent = collision.gameObject.GetComponentInParent<Agent>();
            if (isHunter && agent != null && !agent.isHunter)
            {
                //Охотник при поглощении дичи увеличивается на 10%. Если необходимо - это можно вынести в отдельный параметр.
                size += 0.1f;
                agentTransform.localScale = new Vector3(size, size, size);
                agent.Consume();
            }
        }
    }
}
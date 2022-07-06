using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ExpertCenTest.Core
{
    public class FindTargetSystem : MonoBehaviour
    {
        //���������� �� ������� ���� �������� ������� �� ���������. �������� �� ���������������� ���� ����������.
        [SerializeField]
        private float escapeDistance;
        //���������� �� ������� ���� ���� ���������� ����� �� ������ �� ���������� ��������. ������� ������� � ������� ��������� �������� �������� �� ������ ���������� ����� ��������.
        [SerializeField]
        private float escapeTargetDistance;
        //��� � ������� ����������� ���������� ����� ��� ������.
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

        //����� ���������� AgentsContoller. ������ ����� ������ ���������� ���������� ���� ���� ������� ������ ���� ���������.
        private void OnUpdateTargetsEvent(List<Agent> hunterAgents, List<Agent> wildAgents)
        {
            //��� ��� � �������� � ���� �������� ����� �������, �� � ��� ���� ������������� ������ ������ ����. ���� �� ������ ����� ����������� �������, �� ����� ������� ���
            //������ ������ � ����������� ������ �������������.

            //��������� �� ����� ������ ��������� � ������� ��� ������� ���������� ������ �� ������ ���� � ������������� �� ���� ���������, � �������� ����� ��������� �������.
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
                        //���� ���� ������ �� ��������, �� ��������� �������� �� ������ ������ � �� ����� �� �����.
                        agent.TargetTransform.position = agent.transform.position;
                    }
                }
            }

            //��������� �� ����� ������ ���� � ������� ��� ������� ���������� ������ �� ������ ���������.
            //���� ���������� �� ���� ������ ���������� ������, �� �������� ������ ���������� �����.
            //� ������ ������� ��� ����� �� NavMesh �� ������ � ��������������� ������� �� �������� �� ��������� ��������� � escapeTargetDistance.
            //���� ��� ����� � ������� �� ��� ��������, �� ���� �������� ��������� � �������� ��������� ����.
            //���� ��� ����� ������, �� ������������ ������ �� ���������� ��������, ��������� � escapeDirectionStep � ��������� ���. 
            //�������� ���� �� �����, ���� �� ����� �������� ��� 360 �������� ��� �� ����� ������� ���������� �����. 
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
using UnityEngine;

namespace ExpertCenTest.Core
{
    //Менеджер тестовой сцены с минимальным функционалом. От него унаследуется полноценный менеджер.
    public class CoreManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private AgentsSpawner agentsSpawner;
        [SerializeField]
        private AgentsController agentsController;
        [SerializeField]
        private FindTargetSystem findTargetSystem;
        [SerializeField]
        private GameInterface gameInterface;

        public virtual void Start()
        {
            findTargetSystem.Initialize(agentsController,gameInterface);
            agentsController.Initialize(agentsSpawner,gameInterface);
            agentsSpawner.Initialize(gameInterface);
            gameInterface.Initialize(agentsController);
        }
    }
}
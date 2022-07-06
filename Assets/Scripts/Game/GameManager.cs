using UnityEngine;
using ExpertCenTest.Core;

namespace ExpertCenTest.Game
{
    //Полноценный менеджер, унаследованный от CoreManager. Расширяет его логику при добавлении новых классов.
    public class GameManager : CoreManager
    {
        [SerializeField]
        private ObstacleSpawner obstacleSpawner;

        public override void Start()
        {
            obstacleSpawner.Initialize();
            base.Start();
        }
    }
}

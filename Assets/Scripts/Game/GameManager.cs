using UnityEngine;
using ExpertCenTest.Core;

namespace ExpertCenTest.Game
{
    //����������� ��������, �������������� �� CoreManager. ��������� ��� ������ ��� ���������� ����� �������.
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

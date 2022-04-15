
using MoreMountains.TopDownEngine;

namespace Events
{
    public struct NewWaveEvent
    {
        public WaveNode CurrentWave { get; private set;}

        public NewWaveEvent(WaveNode currentWave)
        {
            this.CurrentWave = currentWave;
        }
    }

    public struct EnemySpawnEvent
    {
        public Character SpawnedEnemy { get; private set; }

        public EnemySpawnEvent(Character spawnedEnemy)
        {
            SpawnedEnemy = spawnedEnemy;
        }
    }

    public struct EnemyDiedEvent
    {
        public int EnemyCount { get; private set; }
        public Character DeadEnemy { get; private set; }

        public EnemyDiedEvent(int enemyCount, Character deadEnemy)
        {
            EnemyCount = enemyCount;
            DeadEnemy = deadEnemy;
        }
    }
    
    
}
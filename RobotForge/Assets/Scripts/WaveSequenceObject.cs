using System;
using System.Collections.Generic;
using Events;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(menuName = "Gameplay/WaveSequence")]
[InlineEditor()]
public class WaveSequenceObject : SerializedScriptableObject
{
    [OdinSerialize]
    
    [ListDrawerSettings(DraggableItems = true, ShowPaging = false)]
    public List<WaveNode> waves;

    
    public bool HasFinished { get; private set; }

    private WaveNode currentNode;
    private int currentWaveIndex = 0;
    
   public void StartSequence()
    {
        if (waves.IsNullOrEmpty()) return;
        
        HasFinished = false;
        currentWaveIndex = 0;

        currentNode = waves[currentWaveIndex];
        currentNode.OnFinish += StartNextNode;
        currentNode.Start();
    }

    public void StopSequence()
    {
        currentNode.Stop();
    }
    public void StartNextNode()
    {
        currentNode.OnFinish -= StartNextNode;
        currentWaveIndex++;
        if (waves.Count > currentWaveIndex)
        {
            currentNode = waves[currentWaveIndex];
            currentNode.OnFinish += StartNextNode;
            currentNode.Start();
        }
        else
        {
            HasFinished = true;
        }
    }
    
    public void OnUpdate()
    {
        if (currentNode == null) return;
        currentNode.Update();
    }
}


[Serializable]
public abstract class WaveNode
{
    //public int id;
    private bool finished;
    public bool HasFinished => finished;
    [HideInInspector]
    public Action OnFinish;

    protected virtual void OnStart(){}

    protected virtual void OnUpdate() { }

    public void Start()
    {
        finished = false;
        OnStart();
    }

    public void Stop()
    {
        finished = true;
    }

    public void Update()
    {
        if (HasFinished) return;
        OnUpdate();
    }
    
    public virtual void Finish()
    {
        if (finished) return;
        
        finished = true;
        OnFinish?.Invoke();
        OnFinish = null;
    }
}

[Serializable]
public class SpawnNode : WaveNode, MMEventListener<EnemySpawnEvent>
{
    [SerializeField] private Character enemyPrefab;
    [SerializeField] private int enemyCount;
    [SerializeField] private float spawnTime;

    public Character EnemyPrefab => enemyPrefab;
    public float SpawnTime => spawnTime;
    public int EnemyCount => enemyCount;
    
    private int spawnedEnemies = 0;
    private float delayBetweenEnemies;
    private float lastSpawn = 0f;
    
    protected override void OnStart()
    {
        base.OnStart();
        this.MMEventStartListening<EnemySpawnEvent>();
        spawnedEnemies = 0;
        lastSpawn = Time.time;
        delayBetweenEnemies = spawnTime / enemyCount;
    }

    public override void Finish()
    {
        base.Finish();
        this.MMEventStopListening<EnemySpawnEvent>();
    }
    
    protected override void OnUpdate()
    {
        if (Time.time - lastSpawn >= delayBetweenEnemies)
        {
           MMEventManager.TriggerEvent(new EnemySpawnEvent(enemyPrefab));
           lastSpawn = Time.time;
        }
    }
    
    public void OnMMEvent(EnemySpawnEvent eventType)
    {
        if (eventType.SpawnedEnemy == enemyPrefab)
        {
            spawnedEnemies++;
        }

        if (spawnedEnemies >= enemyCount)
        {
            Finish();
        }
    }
}

[Serializable]
public class DebugNode : WaveNode{
    [SerializeField] private string debugText;

    protected override void OnStart()
    {
        Debug.Log(debugText);
        Finish();
    }
}

[Serializable]
public class OrNode : WaveNode
{
    [BoxGroup("Node1")]
    [HideLabel]
    public WaveNode node1;
    [BoxGroup("Node2")]
    [HideLabel]
    public WaveNode node2;

    protected override void OnStart()
    {
        base.OnStart();
        node1.OnFinish += TryFinish;
        node2.OnFinish += TryFinish;
        node1.Start();
        node2.Start();
    }

    protected override void OnUpdate()
    {
        node1.Update();
        node2.Update();
    }

    private void TryFinish()
    {
        if (!node1.HasFinished && !node2.HasFinished) return;
        
        node1.OnFinish -= TryFinish;
        node2.OnFinish -= TryFinish;
        
        node1.Stop();
        node2.Stop();
        Finish();
    }
}

[Serializable]
public class AndNode : WaveNode
{
    [BoxGroup("Node1")]
    [HideLabel]
    public WaveNode node1;
    [BoxGroup("Node2")]
    [HideLabel]
    public WaveNode node2;

    protected override void OnStart()
    {
        base.OnStart();
        node1.Start();
        node2.Start();
        node1.OnFinish += TryFinish;
        node2.OnFinish += TryFinish;
    }

    protected override void OnUpdate()
    {
        node1.Update();
        node2.Update();
    }

    private void TryFinish()
    {
        if (node1.HasFinished && node2.HasFinished)
        {
            node1.OnFinish -= TryFinish;
            node2.OnFinish -= TryFinish;
            Finish();
        }
    }
}


[Serializable]
public class EnemyCountNode : WaveNode, MMEventListener<EnemyDiedEvent>
{
    [SerializeField] private int enemyCount;

    protected override void OnStart()
    {
        base.OnStart();
        this.MMEventStartListening<EnemyDiedEvent>();
    }

    public override void Finish()
    {
        base.Finish();
        this.MMEventStopListening<EnemyDiedEvent>();
    }
    
    public void OnMMEvent(EnemyDiedEvent eventType)
    {
        if (eventType.EnemyCount <= enemyCount) Finish();
    }
} 


[Serializable]
public class TimeDelayNode : WaveNode
{
    [SerializeField] private float timeDelay;
    private float startTime;
    protected override void OnStart()
    {
        base.OnStart();
        startTime = Time.time;
    }

    protected override void OnUpdate()
    {
        if (Time.time - startTime >= timeDelay) Finish();
    }
}
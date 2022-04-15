using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;
using Object = System.Object;
using Random = UnityEngine.Random;


public class WaveSpawnManager : MMSingleton<WaveSpawnManager>, MMEventListener<EnemySpawnEvent>
{
    
    [PreviewField]
    [SerializeField] private WaveSequenceObject waveSequence;

    [ListDrawerSettings(DraggableItems = false)]
    [SerializeField] private List<Transform> spawnPoints;

    [ShowInInspector] [ReadOnly] private Dictionary<Character, int> enemyCount;

    private Dictionary<Character, ObjectPool<Character>> enemyPools;

    private bool PoolsAreNotNull()
    {
        if (enemyPools == null) return false;
        if (enemyPools.Count == 0) return false;
        return true;
    }
    
    protected override void Awake()
    {
        base.Awake();
        enemyPools = new Dictionary<Character, ObjectPool<Character>>();
        enemyCount = new Dictionary<Character, int>();
    }

    private void Start()
    {
        this.MMEventStartListening();
    }

    [Button]
    private void StartWave()
    {
        waveSequence.StartSequence();
    }

    private void Update()
    {
        if(waveSequence.HasFinished) return;
        waveSequence.OnUpdate();
    }

    private void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(EnemySpawnEvent eventType)
    {
        var enemy = eventType.SpawnedEnemy;
        var spawnPoint = spawnPoints.GetRandom();
        
        if (!enemyPools.ContainsKey(enemy))
        {
            enemyPools.Add(enemy, new ObjectPool<Character>(() => CreateEnemy(enemy), OnTakeFromPool, (ch)=>{}));
            enemyCount.Add(enemy, 0);
        }

        var spawnedEnemy = enemyPools[enemy].Get();
        spawnedEnemy.RespawnAt(spawnPoint, Character.FacingDirections.North);
        enemyCount[enemy]++;
    }

    private Character CreateEnemy(Character enemy)
    {
        var inst = Instantiate(enemy);
        var health = inst.GetComponent<Health>();
        health.OnDeath += () =>
        {
            enemyCount[enemy]--;
            OnEnemyDied(enemy);
        };
        health.OnDestroy += () =>
        {
            enemyPools[enemy].Release(inst);
        };
        return inst;
    }

    private void OnTakeFromPool(Character character)
    {
        character.Reset();
    }
    
    private void OnEnemyDied(Character enemy)
    {
        MMEventManager.TriggerEvent(new EnemyDiedEvent(enemyCount[enemy], enemy)); 
    }
}

public static class ListUtil
{
    public static T GetRandom<T>(this List<T> list)
    {
        var index = Random.Range(0, list.Count);
        return list[index];
    }
}
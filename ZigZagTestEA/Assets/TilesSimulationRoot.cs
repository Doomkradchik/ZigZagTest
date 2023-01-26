using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Simulation<T> : MonoBehaviour
{
    protected List<T> _entities = new List<T>();

    public IEnumerable<T> Entities => _entities;

    public event Action<T> Start;
    public event Action<T> End;

    public virtual void Simulate(T placedEntity)
    {
        _entities.Add(placedEntity);
        Start?.Invoke(placedEntity);
    }

    protected void Stop(T placedEntity)
    {
        _entities.Remove(placedEntity);
        End?.Invoke(placedEntity);
        OnStoped(placedEntity);
    }

    protected virtual void OnStoped(T placedEntity) { }
}

public sealed class TilesSimulationRoot : Simulation<GameObject>
{
    [SerializeField, Min(0.2f)]
    private float _fallSpeed = 1f;

    [SerializeField, Min(1f)]
    private float _lifeTime = 2f;

    public static TilesSimulationRoot Instance;

    private void Start()
    {
        if (Instance != null)
            throw new InvalidOperationException();

        Instance = this;
    }

    public override void Simulate(GameObject placedEntity)
    {
        base.Simulate(placedEntity);
        _timers.Start(placedEntity, _lifeTime, Stop);
    }

    private readonly Timers<GameObject> _timers = new Timers<GameObject>();

    private void Update()
    {
        foreach(var tile in _entities)
        {
            tile.transform.position += -_fallSpeed * Vector3.up * Time.deltaTime;
        }

        _timers.Tick(Time.deltaTime);
    }

    protected override void OnStoped(GameObject placedEntity)
    {
        Destroy(placedEntity);
    }

}

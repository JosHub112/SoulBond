using UnityEngine;

/// <summary>
/// A live, per-battle instance of an EnemySO. EnemySO is a shared asset —
/// if we damaged it directly, every future battle would start with that
/// enemy already hurt. This class copies the starting stats and tracks
/// current HP only for the duration of one battle.
/// </summary>
public class RuntimeEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
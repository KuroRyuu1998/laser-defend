using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathing : MonoBehaviour
{
    WaveConfig waveConfig;
    List<Transform> waypoints;
    int waypointIndex = 0;

    private void Start()
    {
        if (!waveConfig) { return; }
        waypoints = waveConfig.GetWaypoint();        
        transform.position = waypoints[waypointIndex].transform.position;
    }
    private void Update()
    {
        if (!waveConfig) { return; }
        Move();
    }
    public void SetWaveConfig(WaveConfig waveConfiguration)
    {
        waveConfig = waveConfiguration;
    }

    private void Move()
    {
        if (waypointIndex <= waypoints.Count - 1)
        {
            var targetPos = waypoints[waypointIndex].transform.position;
            var movementThisFrame = waveConfig.GetMoveSpeed()* Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, movementThisFrame);

            if (transform.position == targetPos)
            {
                waypointIndex++;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

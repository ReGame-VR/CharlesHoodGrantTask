using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    /// <summary>
    /// The particle system that is spawned for a successful touch
    /// </summary>
    [SerializeField]
    private GameObject successParticles;

    /// <summary>
    /// The particle system that is spawned when a target times out
    /// </summary>
    [SerializeField]
    private GameObject failParticles;

    /// <summary>
    /// Spawns a set of success particles to celebrate target touch
    /// </summary>
    /// <param name="position"></param>world position to spawn particles
    public void SpawnSuccessParticles(Vector3 position)
    {
        GameObject obj = Instantiate(successParticles, position, Quaternion.identity);
        obj.SetActive(true);
    }

    /// <summary>
    /// Spawns a set of fail particles to lament target timeout
    /// </summary>
    /// <param name="position"></param>world position to spawn particles
    public void SpawnFailParticles(Vector3 position)
    {
        GameObject obj = Instantiate(failParticles, position, Quaternion.identity);
        obj.SetActive(true);
    }
}

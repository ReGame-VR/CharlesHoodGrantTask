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
    /// The particle system that is spawned on game over
    /// </summary>
    [SerializeField]
    private GameObject gameOverParticles;

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

    /// <summary>
    /// Spawns a set of game over particles to celebrate game over
    /// </summary>
    /// <param name="position"></param>world position to spawn particles
    public void SpawnGameOverParticles(Vector3 position)
    {
        GameObject obj = Instantiate(gameOverParticles, position, Quaternion.identity);
        obj.SetActive(true);

        // Activate all the different colors of confetti
        GameObject obj0 = obj.transform.GetChild(0).gameObject;
        GameObject obj1 = obj.transform.GetChild(1).gameObject;
        GameObject obj2 = obj.transform.GetChild(2).gameObject;
        GameObject obj3 = obj.transform.GetChild(3).gameObject;
        GameObject obj4 = obj.transform.GetChild(4).gameObject;
        obj0.SetActive(true);
        obj1.SetActive(true);
        obj2.SetActive(true);
        obj3.SetActive(true);
        obj4.SetActive(true);

    }
}

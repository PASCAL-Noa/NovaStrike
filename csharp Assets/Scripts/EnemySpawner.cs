public class EnemySpawner : MonoBehaviour
{
    public float spawnInterval = 2f; // intervalle de base
    private float baseInterval;

    private void Awake() { baseInterval = spawnInterval; }

    public void SetSpawnMultiplier(float multiplier)
    {
        // multiplier <1 -> spawn plus rapide (intervalle réduit)
        spawnInterval = baseInterval * multiplier;
    }

    // ton loop d'apparition doit utiliser spawnInterval
}
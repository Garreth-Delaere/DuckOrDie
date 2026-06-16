using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] _wallPrefabs;

    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private float _spawnZ = -20f;


    private float _timer = 3f;

    void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        _timer += Time.deltaTime;

        if (_timer >= _spawnInterval)
        {
            SpawnWall();
            _timer = 0f;
        }
    }

    void SpawnWall()
    {
        int randomIndex = Random.Range(0, _wallPrefabs.Length);
        GameObject prefab = _wallPrefabs[randomIndex];
        Instantiate(prefab, new Vector3(0f, 0f, _spawnZ), prefab.transform.rotation);
    }
}
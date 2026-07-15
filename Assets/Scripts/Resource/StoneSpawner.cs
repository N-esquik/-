using UnityEngine;

public class StoneSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Vector2 _area = new Vector2(10f, 10f);
    [SerializeField] private float _count = 4f;
    [SerializeField] private float _interval = 30f;

    private float _divider = 2f;
    private float _height = 0.5f;

    private float _timeSpawn;

    private void Start()
    {
        Create();
    }

    private void Update()
    {
        _timeSpawn += Time.deltaTime;

        if(_timeSpawn >= _interval)
        {
            Create();
            _timeSpawn = 0;
        }
    }

    private void Create()
    {
        for(int  i = 0; i < _count; i++)
        {
            Vector3 randomPosition = GetRandomPosition();
            Instantiate(_prefab,randomPosition,Quaternion.identity);
        }
    }

    private Vector3 GetRandomPosition()
    {
        float x = Random.Range(-_area.x / _divider, _area.x / _divider);
        float z = Random.Range(-_area.y / _divider, _area.y / _divider);
        return transform.position + new Vector3(x, _height, z);
    }
}

using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ResourceDropZone : MonoBehaviour
{
    [SerializeField] private BaseResourceDetector _base;
    [SerializeField] private ResourceStorage _storage;

    private void Awake()
    {
        if (_storage == null)
        {
            _storage = GetComponentInParent<ResourceStorage>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit == null)
            return;

        Resource resource = unit.DropOffResource();
        if (resource == null)
            return;

        if (_storage != null)
        {
            _storage.AddResource(1);
        }

        Destroy(resource.gameObject);
    }
}

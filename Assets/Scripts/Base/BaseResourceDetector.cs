using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BaseResourceDetector : MonoBehaviour
{
    [SerializeField] public List<Unit> units = new List<Unit>();
    [SerializeField] private ResourceDropZone _dropZone;

    private HashSet<Resource> _detectedResource = new HashSet<Resource>();
    private Queue<Resource> _pendingResources = new Queue<Resource>();
    private Dictionary<Unit, Resource> _assignedResources = new Dictionary<Unit, Resource>();

    public Vector3 DropZonePosition => _dropZone.transform.position;

    private void Awake()
    {
        _dropZone = GetComponentInChildren<ResourceDropZone>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Resource resource = other.GetComponent<Resource>();

        if (resource == null)
            return;

        if (_detectedResource.Add(resource))
        {
            NotifyUnits(resource);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Resource resource = other.GetComponent<Resource>();
        if (resource == null)
            return;

        _detectedResource.Remove(resource);

        if (_pendingResources.Contains(resource))
        {
            RemoveFromQueue(resource);
        }
    }

    private void RemoveFromQueue(Resource resource)
    {

        if (resource == null) return;

        Queue<Resource> filtered = new Queue<Resource>();

        foreach (Resource r in _pendingResources)
        {
            if (r != null && r != resource)
            {
                filtered.Enqueue(r);
            }
        }

        _pendingResources.Clear();

        foreach (Resource r in filtered)
        {
            _pendingResources.Enqueue(r);
        }
    }

    private void NotifyUnits(Resource resource)
    {
        if (IsResourceAlreadyHandled(resource))
            return;

        Unit freeUnit = GetFreeUnit();

        if (freeUnit != null)
        {
            AssignResourceToUnit(freeUnit, resource);
        }
        else
        {
            _pendingResources.Enqueue(resource);
        }
    }

    private bool IsResourceAlreadyHandled(Resource resource)
    {
        if (_assignedResources.ContainsValue(resource))
            return true;

        if (_pendingResources.Contains(resource))
            return true;

        return false;
    }

    private void AssignResourceToUnit(Unit unit, Resource resource)
    {
        if (unit == null || resource == null)
            return;

        if (_assignedResources.ContainsValue(resource))
            return;

        _assignedResources[unit] = resource;
        unit.ReceiveResourceLocation(resource.transform.position, resource);
    }

    private Unit GetFreeUnit()
    {
        foreach (Unit unit in units)
        {
            if (unit != null && _assignedResources.ContainsKey(unit) == false)
                return unit;
        }

        return null;
    }

    public void OnUnitFreed(Unit unit)
    {
        if (unit == null)
            return;

        _assignedResources.Remove(unit);


        while (_pendingResources.Count > 0)
        {
            Resource nextResource = _pendingResources.Dequeue();

            if (nextResource == null)
                continue;

            AssignResourceToUnit(unit, nextResource);
            return;
        }
    }

    public void OnResourceCollected(Resource resource)
    {
        if (resource == null) return;

        _detectedResource.Remove(resource);

        if (_pendingResources.Contains(resource))
            RemoveFromQueue(resource);
    }
}

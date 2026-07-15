using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Unit : MonoBehaviour
{
    [SerializeField] private BaseResourceDetector _base;
    [SerializeField] private Transform _slotForResource;

    private NavMeshAgent _agent;
    private Resource _resource;

    private enum State
    {
        Idle,
        MovingToResource,
        CarryingToBase
    }

    private State _state = State.Idle;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        switch (_state)
        {
            case State.MovingToResource:

                if (_resource == null)
                {
                    _state = State.Idle;

                    if (_base != null)
                    {
                        _base.OnUnitFreed(this);
                    }

                    break;
                }

                if (HasReachedDestination())
                {
                    PickUpResource();
                }

                break;

            case State.CarryingToBase:

                if (_base != null && HasReachedDestination())
                {
                    DropOffResource();
                }

                break;
        }
    }

    private void PickUpResource()
    {
        if (_resource == null || _slotForResource == null)
            return;

        if (_resource.transform.parent != null && _resource.transform.parent != _slotForResource)
        {
            _resource = null;
            _state = State.Idle;
            _base?.OnUnitFreed(this);

            return;
        }

        _base?.OnResourceCollected(_resource);

        _resource.transform.SetParent(_slotForResource);
        _resource.transform.localPosition = Vector3.zero;
        _resource.transform.localRotation = Quaternion.identity;

        Collider collider = _resource.transform.GetComponent<Collider>();
        Rigidbody rigidbody = _resource.GetComponent<Rigidbody>();

        if (collider != null)
        {
            collider.enabled = false;
        }

        if (rigidbody != null)
        {
            rigidbody.isKinematic = true;
        }

        _state = State.CarryingToBase;

        if (_base != null)
        {
            _agent.SetDestination(_base.DropZonePosition);
        }
    }

    private bool HasReachedDestination()
    {
        if (_agent.pathPending)
            return false;

        if (_agent.pathStatus == NavMeshPathStatus.PathInvalid)
            return false;

        if (_agent.remainingDistance <= _agent.stoppingDistance + 0.05f)
            return true;

        Vector3 agentPos = _agent.transform.position;
        Vector3 targetPos = _agent.destination;

        float horizontalDistance = Vector2.Distance(
            new Vector2(agentPos.x, agentPos.z),
            new Vector2(targetPos.x, targetPos.z));

        return horizontalDistance <= _agent.stoppingDistance + 0.15f;
    }

    public Resource DropOffResource()
    {
        if (_resource == null || _state != State.CarryingToBase)
        {
            return null;
        }

        Resource resource = _resource;

        _resource = null;

        _state = State.Idle;

        if (_base != null)
        {
            _base.OnUnitFreed(this);
        }

        return resource;
    }

    public void ReceiveResourceLocation(Vector3 position, Resource resource)
    {
        if (_agent == null)
            return;

        _state = State.MovingToResource;
        _resource = resource;
        _agent.SetDestination(position);
    }
}

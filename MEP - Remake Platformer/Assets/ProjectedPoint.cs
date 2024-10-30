using UnityEngine;

public class ProjectedPoint : MonoBehaviour
{
    [SerializeField] public Transform _legPoint;

    [SerializeField] private AnimationCurve _heightCurve;

    [SerializeField] private float _elapsedTime, _normalizedTime;
    private Vector3 _startPoint, _targetPoint;

    [SerializeField, Range(0.01f, 1f)] private float _duration = 0.2f;
    [SerializeField, Range(0.05f, 1f)] private float _moveThreshold = 0.3f;
    public bool ShouldMove { get; private set; }
    public bool IsMoving { get; private set; }
    public bool CanMove { get; set; }
    private float _distance;


    private void Start()
    {
        _legPoint.transform.position = transform.position;
    }

    void Update()
    {
        RaycastHit hit;
        int mask = ~LayerMask.GetMask("Player");

        Vector3 p1 = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);

        if (Physics.Raycast(p1, Vector3.down, out hit, 8, mask))
        {
            transform.position = hit.point;

            _distance = Vector3.Distance(hit.point, _legPoint.position);
            

        if(_distance > _moveThreshold)
            {
                ShouldMove = true;
            }
        }

        if(ShouldMove && CanMove) { MoveLeg(hit); }
    }

    private void MoveLeg(RaycastHit hit)
    {
        if(_elapsedTime <= 0.02f)
        {
            _startPoint = _legPoint.position;
        }

        _targetPoint = hit.point;

        _elapsedTime += Time.deltaTime;
        _normalizedTime = _elapsedTime / _duration;
        float heightOffset = _heightCurve.Evaluate(_normalizedTime);

        Vector3 nextPosition = Vector3.Lerp(_startPoint, _targetPoint, _normalizedTime);
        IsMoving = true;
        nextPosition.y += heightOffset;

        _legPoint.position = nextPosition;

        if (_elapsedTime >= _duration)
        {
            _elapsedTime = 0;
            ShouldMove = false;
            IsMoving = false;
        }
    }
}

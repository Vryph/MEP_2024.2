using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Range(0.5f, 2f)] private float _dampDistance = 0.5f;
    [SerializeField] private AnimationCurve _curve;
    [SerializeField, Range(0.2f, 10f)] private float _duration = 0.5f;

    private Camera _camera;
    private Rigidbody2D _body;
    private PlayerMovement _movement;
    private float _direction, _hInput, _elapsedTime, _normalizedTime;
    [SerializeField] private float _t;
    [SerializeField] private Vector3 _dampPoint, _position;
    private bool _isMoving;


    void Awake()
    {
        _camera = Camera.main;
        _body = GetComponent<Rigidbody2D>();
        _movement = GetComponent<PlayerMovement>();
    }


    public void CameraFollow()
    {
        if(Mathf.Sign(_hInput) != Mathf.Sign(_movement.GetHorizontalInput()))
        {
            _elapsedTime = 0;
        }

       _hInput = _movement.GetHorizontalInput();
       _position = _body.position;
       _position.y += 2.5f;
       _position.z -= 10;
       _elapsedTime += Time.deltaTime;
       _normalizedTime = _elapsedTime / _duration;
        _t = _curve.Evaluate(_normalizedTime);
       


        if (_hInput != 0)
        {
            if (_isMoving == false)
            {
                _isMoving = true;
                _elapsedTime = 0;
            }
            _dampPoint = _position;
            _dampPoint.x = _body.position.x + (_dampDistance * -Mathf.Sign(_hInput));
        }

        if (_isMoving)
        {
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, _dampPoint, _t);

            if (_hInput == 0 ) { _isMoving = false; if(_movement.GetGrounded()) _elapsedTime = 0; }
        }
        else { _camera.transform.position = Vector3.Lerp(_camera.transform.position, _position, _normalizedTime); }

    }
}

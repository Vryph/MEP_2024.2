using UnityEngine;

public class SpiderIKManager : MonoBehaviour
{

    [SerializeField] private int _legNumber = 4;
    [SerializeField] private ProjectedPoint[] _projectedPoints;
    [SerializeField] private SpiderBody _body;

    [SerializeField] private float _tiltMultiplier = 2f;


    private Vector3 _legsAvgPosition;

    private float _leftLegHeight;
    private float _rightLegHeight;
    [SerializeField, Range(0.1f, 1f)]private float _yBodyOffset = 0.5f;

    private void Awake()
    {
        _projectedPoints = new ProjectedPoint[_legNumber];
        _projectedPoints = (GetComponentsInChildren<ProjectedPoint>());
        _body = GetComponentInChildren<SpiderBody>();
    }

    private void Update()
    {
        _legsAvgPosition = Vector3.zero;
        _leftLegHeight = 0f;
        _rightLegHeight = 0f;

        for (int i = 0; i < _projectedPoints.Length; i++)
        {
            if(i == 0 || i == 3)
            {
                _leftLegHeight += _projectedPoints[i]._legPoint.position.y;
            }
            else
            {
                _rightLegHeight += _projectedPoints[i]._legPoint.position.y;
            }

            _legsAvgPosition += _projectedPoints[i]._legPoint.position;
            _projectedPoints[i].CanMove = true;
            for (int j = 0; j < _projectedPoints.Length; j++)
            {
                if (j % 2 != i % 2)
                {
                    if (_projectedPoints[i].IsMoving)
                    {
                        _projectedPoints[j].CanMove = false;
                    }
                    else if (_projectedPoints[j].IsMoving)
                    {
                        //.Log(("Leg", i, "Can't move because opposite leg is Moving"));
                        _projectedPoints[i].CanMove = false;
                        break;
                    }
                    //Debug.Log((i, j, projectedPoints[i].CanMove, projectedPoints[j].CanMove));
                }
            }
        }


        //Moves Body According to avg position and angle
        float _heightDifference = (_leftLegHeight / 2) - (_rightLegHeight / 2);

        _legsAvgPosition /= _projectedPoints.Length;

        _body.transform.position = new Vector3(transform.position.x, Mathf.Lerp(_body.transform.position.y, (_legsAvgPosition.y + _yBodyOffset), 0.25f), transform.position.z);

        
        float tiltAngle = Mathf.Atan(-_heightDifference / 4f) * Mathf.Rad2Deg * _tiltMultiplier;
        _body.transform.rotation = Quaternion.Euler(_body.transform.rotation.eulerAngles.x, _body.transform.rotation.eulerAngles.y, tiltAngle);
    }
}
using UnityEngine;

public class LampFlicker : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;
    [SerializeField, Range(0.3f, 2f)] private float _duration;
    public bool isLampOn = false;

    private Light _light;

    private float _intensity, _elapsedTime, _normalizedTime;

    void Start()
    {
        _light = GetComponent<Light>();
    }

    void Update()
    {
        if (_normalizedTime <= 1)
        {
            if (isLampOn)
            {
                _elapsedTime += Time.deltaTime;
                _normalizedTime = _elapsedTime / _duration;

                _intensity = _curve.Evaluate(_normalizedTime);
                _light.intensity = _intensity;
            }
        }
        if(!isLampOn) 
        {
            _elapsedTime = 0;
            _normalizedTime = 0;
            _light.intensity = 0;
        }
    }
}

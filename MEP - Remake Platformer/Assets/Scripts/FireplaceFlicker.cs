using UnityEngine;

[ExecuteInEditMode]
public class FireplaceFlicker : MonoBehaviour
{
    [SerializeField] private AnimationCurve _intensityCurve;
     public bool playAnimation = false;
    public bool stopAnimation = false;

    private Light _light;
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _main;

    [SerializeField, Range(1f, 20f)] private float _duration;

    private bool _playingAnimation;
    private float _intensity, _normalizedTime, _elapsedTime;

    void Start()
    {
        _light = GetComponentInChildren<Light>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    void Update()
    {
        if (playAnimation)
        {
            playAnimation = false;
            _playingAnimation = true;
        }

        if (stopAnimation)
        {
            _playingAnimation = false;
            stopAnimation = false;
        }

        if (_playingAnimation)
        {
            _elapsedTime += Time.deltaTime;

            _normalizedTime = _elapsedTime / _duration;

            _intensity = _intensityCurve.Evaluate(_normalizedTime);

            _light.intensity = _intensity;
            _particleSystem.transform.localScale = new Vector3(0.0018f * _intensity, 0.0018f * _intensity, 0.0018f * _intensity);
        }

    }



}

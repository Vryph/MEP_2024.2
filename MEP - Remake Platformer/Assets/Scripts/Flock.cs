using UnityEngine;

public class Flock : MonoBehaviour
{
    Boid[] boids;

    [Header("Bounds Values")]
    [SerializeField] private Vector2 _boundsX;
    [SerializeField] private Vector2 _boundsY;
    private float _boundsWidth, _boundsHeight;

    [Header("Boid Implementation")]
    [SerializeField] private bool _activateSeparation;
    [SerializeField] private bool _activateAlignment, _activateCohesion, _activateHunt;
    [SerializeField, Range(0.1f, 4f)] private float _alignmentIntensity = 0.8f;
    [SerializeField, Range(0.1f, 4f)] private float _separationIntensity = 4f;
    [SerializeField, Range(0.1f, 2f)] private float _cohesionIntensity = 0.5f;
    [SerializeField, Range(0.1f, 5f)] private float _huntIntensity = 2f;
    [SerializeField, Range(0.1f, 5f)] private float _fleeIntensity = 4f;

    [SerializeField] private float _maxVelocityPredatorReductionMultiplier = 0.8f;
    private float _tempdistance;

    [Header("FOV")]
    [SerializeField] private float _viewRadius;
    [SerializeField] private float _predatorViewRadius;
    [SerializeField, Range(0.1f, 1f)] private float _separationRadiusReductionMultiplier = 0.6f;
    [SerializeField, Range(0f, 360f)] private float _fieldOfViewAngle = 270;

    private void Start()
    {
        boids = FindObjectsByType<Boid>(FindObjectsSortMode.None);
        _boundsHeight = _boundsY.y - _boundsY.x;
        _boundsWidth = _boundsX.y - _boundsX.x;
        SetGroups();
    }

    void FixedUpdate()
    {
        if (_activateSeparation)
        {
            Separation();
        }
        if (_activateAlignment)
        {
            Alignment();
        }
        if (_activateCohesion)
        {
            Cohesion();
        }
        if(_activateHunt)
        {
            foreach(var boid in boids)
            {
                if (boid.group == 5)
                {
                    Hunt(boid);
                }
                else
                {
                    Flee(boid);
                }
            }
        }

        UpdateBounds();
    }

    private void Separation()
    {
        foreach (var boid in boids)
        {
            if (boid.group != 5)
            {
                int count = 0;
                Vector3 separationForce = Vector3.zero;
                foreach (var other in boids)
                {
                    if (IsInFOV(boid, other, true, false))
                    {
                        separationForce += (boid.transform.position - other.transform.position).normalized / _tempdistance;
                        count++;
                    }
                }

                if (count > 0)
                {
                    separationForce /= count;
                    separationForce *= _separationIntensity;
                    boid.acceleration += separationForce;
                }
            }
        }
    }

    private void Cohesion()
    {
        foreach (var boid in boids)
        {
            if (boid.group != 5)
            {
                Vector3 center = Vector3.zero;
                int count = 0;
                foreach (var other in boids)
                {
                    if (IsInFOV(boid, other, false, false))
                    {
                        center += other.transform.position;
                        count++;
                    }
                }
                if (count > 0)
                {
                    center /= count;
                    boid.acceleration += (center - boid.transform.position) * _cohesionIntensity;
                }
            }
        }
    }

    private void Alignment()
    {
        foreach (var boid in boids)
        {
            if (boid.group != 5)
            {
                int count = 0;
                Vector3 alignmentForce = Vector3.zero;
                foreach (var other in boids)
                {
                    if (other != boid)
                    {

                        if (IsInFOV(boid, other, false, false))
                        {
                            alignmentForce += (other.velocity).normalized;
                            count++;
                        }
                    }
                }

                if (count != 0)
                {
                    alignmentForce /= count;
                }
                else
                {
                    alignmentForce = (boid.velocity).normalized;
                }

                alignmentForce *= _alignmentIntensity;
                boid.acceleration += alignmentForce;
            }
        }
    }

    private void Hunt(Boid boid)
    {
        Vector3 center = Vector3.zero;
        int count = 0;
        foreach (var other in boids)
        {
            if (other.group != 5)
            {
                if (IsInFOV(boid, other, false, true))
                {
                    center += other.transform.position;
                    count++;
                }
            }
        }
        if (count > 0)
        {
            center /= count;
            boid.acceleration += (center - boid.transform.position) * _huntIntensity;
        }
    }

    private void Flee(Boid boid)
    {
        int count = 0;
        Vector3 fleeForce = Vector3.zero;
        foreach (var other in boids)
        {
            if (other.group == 5)
            {
                if (IsInFOV(boid, other, true, false))
                {
                    fleeForce += (boid.transform.position - other.transform.position).normalized / _tempdistance;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            fleeForce /= count;
            fleeForce *= _fleeIntensity;
            boid.acceleration += fleeForce;
        }
    }

    private bool IsInFOV(Boid boid, Boid other, bool isSeparation, bool isHunt)
    {
        if (boid != other && (boid.group == other.group || isSeparation || isHunt))
        {
            float distance = Vector3.Distance(other.transform.position, boid.transform.position);
            float radius = isSeparation == true ? _viewRadius * _separationRadiusReductionMultiplier : _viewRadius; // If it's applying separation reduces the radius
            if (isHunt) radius = _predatorViewRadius;
            if (distance < radius)
            {
                if (boid.velocity.sqrMagnitude > 0.01f)
                {
                    float _cosFOV = Mathf.Cos(_fieldOfViewAngle * 0.5f * Mathf.Deg2Rad);
                    Vector3 targetNormal = (other.transform.position - boid.transform.position).normalized;
                    float dotProduct = Vector3.Dot((boid.velocity).normalized, targetNormal);

                    if (dotProduct > _cosFOV)
                    {
                        if (distance > 0.0001f) _tempdistance = distance;
                        else _tempdistance = 0.0002f;
                        return true;
                    }
                }
                else return true;
            }
        }
        return false;
    }
    
    private void UpdateBounds()
    {
        foreach (var boid in boids)
        {
            if (boid.transform.position.x >= _boundsX.y)
            {
                boid.transform.position -= new Vector3(_boundsWidth, 0f, 0f);
            }

            if (boid.transform.position.x <= _boundsX.x)
            {
                boid.transform.position += new Vector3(_boundsWidth, 0f, 0f);
            }

            if (boid.transform.position.y >= _boundsY.y)
            {
                boid.transform.position -= new Vector3(0f, _boundsHeight, 0f);
            }

            if (boid.transform.position.y <= _boundsY.x)
            {
                boid.transform.position += new Vector3(0f, _boundsHeight, 0f);
            }
        }
    }
    
    private void SetGroups()
    {
        float count = 1;
        foreach (var boid in boids)
        {
            boid.id = count;
            count++;

            if (boid.id == 1 || boid.id == 2)
            {
                boid.group = 5;
            }
            else
            {
                boid.group = boid.id % 5;
            }

            switch (boid.group)
            {
                case 0:
                    boid.spriteRenderer.color = Color.white;
                    break;
                case 1:
                    boid.spriteRenderer.color = Color.gray;
                    break;
                case 2:
                    boid.spriteRenderer.color = new Vector4(0.35f, 0.35f, 0.35f, 1);
                    break;
                case 3:
                    boid.spriteRenderer.color = new Vector4(0.2f, 0.2f, 0.2f, 1);
                    break;
                case 4:
                    boid.spriteRenderer.color = Color.black;
                    break;
                case 5:
                    boid.spriteRenderer.color = Color.red;
                    boid.maxVelocity *= _maxVelocityPredatorReductionMultiplier;
                    break;


            }
            //Debug.Log("Id:" + boid.id + " Name: " + boid.name + " Group: " + boid.group);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_4_5_MyScript : MonoBehaviour{
    public bool PauseMovement = true;

    public GameObject TravelingBall = null;
    public GameObject RedTarget = null;

    public float BallSpeed = 0.01f; // units per second
    public bool DrawVelocity = false;
    private float VelocityDrawFactor = 20f; // So that we can see the vector drawn

    public Vector3 WindDirection = Vector3.zero;
    public float WindSpeed = 0.01f;
    public bool ApplyWind = false;
    public bool DrawWind = false;
    public bool TurnOnGust = false;

    private Vector3 _gustDirection = Vector3.zero;
    private float _gustSpeed = 0;
    private float _gustDuration = 0f;
    private float _nextGustTime = 0f;
    private float _gustEndTime = 0f;
    private bool _gustActive = false;
    private Vector3 _currentGust = Vector3.zero;


    private MyVector ShowVelocity = null;
    private MyVector ShowWindVector = null;
    private MyVector ShowActualVelocity = null;


    // Start is called before the first frame update
    void Start(){
        Debug.Assert(TravelingBall != null);
        Debug.Assert(RedTarget != null);

        ShowVelocity = new MyVector(){
            VectorColor = Color.green,
            DrawVectorComponents = false
        };

        ShowWindVector = new MyVector(){
            VectorColor = new Color(0.8f, 0.3f, 0.3f, 1.0f),
            DrawVectorComponents = false
        };

        ShowActualVelocity = new MyVector(){
            VectorColor = new Color(0.3f, 0.3f, 0.8f, 1.0f),
            DrawVectorComponents = false
        };
    }

    // Update is called once per frame
    void Update(){
        Vector3 vDir = RedTarget.transform.localPosition - TravelingBall.transform.localPosition;
        float distance = vDir.magnitude;

        if (distance > Mathf.Epsilon) // if not already at the target
        {
            if (PauseMovement)
                return;

            if (Time.time > _nextGustTime && !_gustActive){
                GustTimeCalculation();
                _currentGust = CreateGustVelocity();
            }

            if (_gustActive && Time.time > _gustEndTime){
                _gustActive = false;
                _currentGust = Vector3.zero;
            }

            vDir.Normalize();
            WindDirection.Normalize();

            Vector3 vT = BallSpeed * vDir;
            Vector3 vWind = WindSpeed * WindDirection;
            Vector3 vGust = _gustActive ? _currentGust : Vector3.zero;
            Vector3 vA = vT - vWind;
            if (TurnOnGust)
                vA -= vGust;

            TravelingBall.transform.localPosition += (ApplyWind) ? vA * Time.deltaTime : vT * Time.deltaTime;

            #region Display the vectors

            ShowVelocity.VectorAt = TravelingBall.transform.localPosition;
            ShowVelocity.Magnitude = BallSpeed * VelocityDrawFactor;
            ShowVelocity.Direction = vDir;
            ShowVelocity.DrawVector = DrawVelocity;

            ShowWindVector.VectorAt = TravelingBall.transform.localPosition +
                                      (ShowVelocity.Magnitude * ShowVelocity.Direction);
            ShowWindVector.Direction = WindDirection;
            ShowWindVector.Magnitude = WindSpeed * VelocityDrawFactor;
            ShowWindVector.DrawVector = DrawWind;

            ShowActualVelocity.VectorAt = TravelingBall.transform.localPosition;
            ShowActualVelocity.Direction = vA;
            ShowActualVelocity.Magnitude = vA.magnitude * VelocityDrawFactor;
            ShowActualVelocity.DrawVector = DrawWind;

            #endregion
        }
    }

    private Vector3 CreateGustVelocity(){
        _gustSpeed = Random.Range(0.01f, 0.07f);
        float azimuthAngleDeg = Random.Range(0, 360);
        float elevationAngleDeg = Random.Range(-90f, 90f);
        float azimuthAngleRad = azimuthAngleDeg * Mathf.Deg2Rad;
        float elevationAngleRad = elevationAngleDeg * Mathf.Deg2Rad;
        float x = Mathf.Cos(elevationAngleRad) * Mathf.Sin(azimuthAngleRad);
        float y = Mathf.Sin(elevationAngleRad);
        float z = Mathf.Cos(elevationAngleRad) * Mathf.Cos(azimuthAngleRad);
        return new Vector3(x, y, z).normalized * _gustSpeed;
    }

    private void GustTimeCalculation(){
        _gustDuration = Random.Range(1f, 5f);
        _gustEndTime = Time.time + _gustDuration;
        _nextGustTime = _gustEndTime + Random.Range(1f, 7f);
        _gustActive = true;
    }
}
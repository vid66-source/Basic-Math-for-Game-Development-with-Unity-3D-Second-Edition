using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_3_3_MyScript : MonoBehaviour{
    private MySphereBound TaxiBound = null;
    private MySphereBound CarBound = null;
    private MySphereBound _carFrontBound = null;
    private MySphereBound _carBackBound = null;

    public GameObject TheTaxi = null;
    public float TaxiBoundRadius = 2.0f;
    public bool DrawTaxiBound = true;

    public GameObject TheCar = null;
    private GameObject _theCarFrontLeftWheel = null;
    private GameObject _theCarFrontRightWheel = null;
    private GameObject _theCarBackLeftWheel = null;
    private GameObject _theCarBackRightWheel = null;
    public float CarBoundRadius = 2.0f;
    public bool DrawCarBound = true;

    public float DistanceBetween = 0.0f;
    public float DistanceBetweenFront = 0.0f;
    public float DistanceBetweenBack = 0.0f;

    // Start is called before the first frame update
    void Start(){
        Debug.Assert(TheTaxi != null); // Make sure proper editor setup
        Debug.Assert(TheCar != null);

        TaxiBound = new MySphereBound();
        CarBound = new MySphereBound();
        _carFrontBound = new MySphereBound();
        _carBackBound = new MySphereBound();
        _theCarFrontLeftWheel = GameObject.Find("WDF");
        _theCarFrontRightWheel = GameObject.Find("WPR");
        _theCarBackLeftWheel = GameObject.Find("WDR");
        _theCarBackRightWheel = GameObject.Find("WPF");
    }

    // Update is called once per frame
    void Update(){
        // Step 1: Assume no intersection
        TaxiBound.BoundColor = MySphereBound.NoCollisionColor;
        CarBound.BoundColor = MySphereBound.NoCollisionColor;
        _carFrontBound.BoundColor = MySphereBound.NoCollisionColor;
        _carBackBound.BoundColor = MySphereBound.NoCollisionColor;

        // Step 2: Update the Taxi sphere bound
        TaxiBound.Center = TheTaxi.transform.localPosition;
        TaxiBound.Radius = TaxiBoundRadius;
        TaxiBound.DrawBound = DrawTaxiBound;

        // Step 3: Update the Car sphere bound
        Vector3 carCenter = TheCar.transform.localPosition;
// Беремо локальні координати коліс (відносно TheCar)
        Vector3 frontLeftLocal = _theCarFrontLeftWheel.transform.localPosition;
        Vector3 frontRightLocal = _theCarFrontRightWheel.transform.localPosition;
        Vector3 backLeftLocal = _theCarBackLeftWheel.transform.localPosition;
        Vector3 backRightLocal = _theCarBackRightWheel.transform.localPosition;

// Обчислюємо середину у локальних координатах
        Vector3 frontCenterLocal = 0.5f * (frontLeftLocal + frontRightLocal);
        Vector3 backCenterLocal = 0.5f * (backLeftLocal + backRightLocal);

// Переводимо ці локальні точки у світові
        Vector3 carFrontCenterWorld = TheCar.transform.TransformPoint(frontCenterLocal);
        Vector3 carBackCenterWorld = TheCar.transform.TransformPoint(backCenterLocal);
        CarBound.Center = carCenter;
        CarBound.Radius = CarBoundRadius;
        CarBound.DrawBound = DrawCarBound;
        _carFrontBound.Center = carFrontCenterWorld;
        _carFrontBound.Radius = CarBoundRadius / 3;
        _carBackBound.Center = carBackCenterWorld;
        _carBackBound.Radius = CarBoundRadius / 3;

        // Step 4: Compute the distance between the sphere bounds as magnitude of a Vector3
        Vector3 diff = TaxiBound.Center - CarBound.Center;
        DistanceBetween = diff.magnitude;
        Vector3 diffFront = TaxiBound.Center - carFrontCenterWorld;
        DistanceBetweenFront = diffFront.magnitude;
        Vector3 diffBack = TaxiBound.Center - carBackCenterWorld;
        DistanceBetweenBack = diffBack.magnitude;

        // Step 5: Testing and showing intersection status
        bool hasIntersectionAll = DistanceBetween <= (TaxiBound.Radius + CarBound.Radius) &&
                                     DistanceBetweenFront <= (TaxiBound.Radius + _carFrontBound.Radius) &&
                                     DistanceBetweenBack <= (TaxiBound.Radius + _carBackBound.Radius);
        bool hasIntersectionFront = DistanceBetween <= (TaxiBound.Radius + CarBound.Radius) &&
                                     DistanceBetweenFront <= (TaxiBound.Radius + _carFrontBound.Radius);
        bool hasIntersectionBack = DistanceBetween <= (TaxiBound.Radius + CarBound.Radius) &&
                                     DistanceBetweenBack <= (TaxiBound.Radius + _carBackBound.Radius);
        if (hasIntersectionAll){
            Debug.Log(
                $"Intersect all!! Distance: {DistanceBetween}, Distance Front: {DistanceBetweenFront}, Distance Back: {DistanceBetweenBack}");
            TaxiBound.BoundColor = MySphereBound.CollisionColor;
            CarBound.BoundColor = MySphereBound.CollisionColor;

            // The collision functionality is supported by the MySphereBound class as well
            Debug.Assert(TaxiBound.SpheresIntersects(CarBound));
        }
        else if (hasIntersectionFront){
            Debug.Log(
                $"Intersect front!! Distance: {DistanceBetween}, Distance Front: {DistanceBetweenFront}");
            TaxiBound.BoundColor = MySphereBound.CollisionColor;
            _carFrontBound.BoundColor = MySphereBound.CollisionColor;

            // The collision functionality is supported by the MySphereBound class as well
            Debug.Assert(TaxiBound.SpheresIntersects(CarBound));
        }
        else if (hasIntersectionBack){
            Debug.Log(
                $"Intersect back!! Distance: {DistanceBetween}, Distance Front: {DistanceBetweenFront}, Distance Back: {DistanceBetweenBack}");
            TaxiBound.BoundColor = MySphereBound.CollisionColor;
            _carBackBound.BoundColor = MySphereBound.CollisionColor;

            // The collision functionality is supported by the MySphereBound class as well
            Debug.Assert(TaxiBound.SpheresIntersects(CarBound));
        }
    }
}
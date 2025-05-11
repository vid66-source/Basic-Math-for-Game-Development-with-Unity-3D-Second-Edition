using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EX_2_4_MyScript : MonoBehaviour {
    private MyBoxBound CarBound = null; // For visualizing the three bounding boxes
    private MyBoxBound CarFrontWheelsBound = null; // For visualizing the three bounding boxes
    private MyBoxBound CarBackWheelsBound = null; // For visualizing the three bounding boxes
    private MyBoxBound TaxiBound = null;
    private MyBoxBound OverlapBox = null;
    private MyBoxBound OverlapBoxFrontWheels = null;
    private MyBoxBound OverlapBoxBackWheels = null;

    public bool DrawBox = true; // Controls what to show/hide
    public bool DrawIntervals = false;
    public bool ControlWithMinMaxValue = false;

    // The y-center of the car is at ground level

    public GameObject TheTaxi = null; // Reference to the Taxi game object
    public GameObject TheCar = null;
    // Reference to the Car game object

    [Header("BoundBoxes Size Params")]
    public Vector3 CarCenterOffset = Vector3.zero; // Offset between the geometry and box centers

    public Vector3 CarCenterFrontWheelsOffset = Vector3.zero; // Offset between the geometry and box centers
    public Vector3 CarCenterBackWheelsOffset = Vector3.zero; // Offset between the geometry and box centers

    public Vector3 TaxiBoundSize = Vector3.one;

    // User sets desirable taxi bounding box size
    public Vector3 CarBoundSize = Vector3.one; // User sets the desirable car bounding box size
    public Vector3 CarBoundFrontWheelsSize = Vector3.one; // User sets the desirable car bounding box size
    public Vector3 CarBoundBackWheelsSize = Vector3.one; // User sets the desirable car bounding box size

    [Header("MinMax Values")] public Vector3 CarBoundMaxValue = Vector3.one;
    public Vector3 CarBoundMinValue = -Vector3.zero;

    public Vector3 TaxiBoundMaxValue = Vector3.one;
    public Vector3 TaxiBoundMinValue = -Vector3.zero;
    // Max position of the overlapping bounding box

    [Header("Overlap Params")]
    public Vector3 OverlapBoxMin = Vector3.zero; // Min position of the overlapping bounding box

    public Vector3 OverlapBoxMax = Vector3.zero;

    public Vector3 OverlapBoxFrontWheelsMax = Vector3.zero; // Min position of the overlapping bounding box

    public Vector3 OverlapBoxFrontWheelsMin = Vector3.zero;

    public Vector3 OverlapBoxBackWheelsMax = Vector3.zero; // Min position of the overlapping bounding box

    public Vector3 OverlapBoxBackWheelsMin = Vector3.zero;


    // Start is called before the first frame update
    void Start() {
        Debug.Assert(TheTaxi != null); // Ensure that proper reference setup in Inspector Window
        Debug.Assert(TheCar != null);

        TaxiBound = new MyBoxBound(); // Instantiate the visualization variables
        CarBound = new MyBoxBound();
        CarFrontWheelsBound = new MyBoxBound();
        CarBackWheelsBound = new MyBoxBound();
        OverlapBox = new MyBoxBound();
        OverlapBox.SetBoxColor(new Color(0.4f, 0.9f, 0.9f, 0.6f));

        OverlapBox.DrawBoundingBox = false; // hide the overlap box initially
        OverlapBox.DrawIntervals = false; // not showing this in this example

        OverlapBoxFrontWheels = new MyBoxBound();
        OverlapBoxFrontWheels.SetBoxColor(new Color(0.9f, 0.1f, 0.9f, 0.6f));

        OverlapBoxFrontWheels.DrawBoundingBox = false; // hide the overlap box initially
        OverlapBoxFrontWheels.DrawIntervals = false; // not showing this in this example

        OverlapBoxBackWheels = new MyBoxBound();
        OverlapBoxBackWheels.SetBoxColor(new Color(0.9f, 0.9f, 0.1f, 0.6f));

        OverlapBoxBackWheels.DrawBoundingBox = false; // hide the overlap box initially
        OverlapBoxBackWheels.DrawIntervals = false;
    }

    // Update is called once per frame
    void Update() {
        // Step 1: Set the user specify drawing state
        TaxiBound.DrawBoundingBox = DrawBox;
        TaxiBound.DrawIntervals = DrawIntervals;
        CarBound.DrawBoundingBox = DrawBox;
        CarBound.DrawIntervals = DrawIntervals;
        CarFrontWheelsBound.DrawBoundingBox = DrawBox;
        CarFrontWheelsBound.DrawIntervals = DrawIntervals;
        CarBackWheelsBound.DrawBoundingBox = DrawBox;
        CarBackWheelsBound.DrawIntervals = DrawIntervals;

        if (!ControlWithMinMaxValue) {
            // Step 2: Update the bounds (Taxi first, then Car)
            TaxiBound.Center = TheTaxi.transform.localPosition + CarCenterOffset;
            TaxiBound.Size = TaxiBoundSize;
            CarBound.Center = TheCar.transform.localPosition + CarCenterOffset;
            CarBound.Size = CarBoundSize;
            CarFrontWheelsBound.Center = TheCar.transform.localPosition + CarCenterFrontWheelsOffset;
            CarFrontWheelsBound.Size = CarBoundFrontWheelsSize;
            CarBackWheelsBound.Center = TheCar.transform.localPosition + CarCenterBackWheelsOffset;
            CarBackWheelsBound.Size = CarBoundBackWheelsSize;
        }
        else {
            TaxiBound.MinPosition = TaxiBoundMinValue;
            TaxiBound.MaxPosition = TaxiBoundMaxValue;
            CarBound.MinPosition = CarBoundMinValue;
            CarBound.MaxPosition = CarBoundMaxValue;
        }

        // Step 3: test for intersection ...
        // Two box bounds overlap when all three intervals overlap ...
        if (isBoundingBoxinsideOroutside(TaxiBound.MinPosition, TaxiBound.MaxPosition, CarBound.MinPosition,
                CarBound.MaxPosition)) {
            // Min/Max of the overlap box bound
            Vector3 min = MinPos(TaxiBound.MinPosition, CarBound.MinPosition);
            Vector3 max = MaxPos(TaxiBound.MaxPosition, CarBound.MaxPosition); // max z position
            OverlapBox.DrawBox = TaxiBound.DrawBox;
            OverlapBox.DrawIntervals = TaxiBound.DrawIntervals;
            OverlapBox.MinPosition = min;
            OverlapBox.MaxPosition = max;
            // Update to show the overlap bound's min and max
            OverlapBoxMax = max;
            OverlapBoxMin = min;
        }
        else {
            OverlapBox.DrawBox = false;
            OverlapBox.DrawIntervals = false;
            OverlapBox.MinPosition = Vector3.zero;
            OverlapBox.MaxPosition = Vector3.zero;
            OverlapBoxMin = Vector3.zero;
            OverlapBoxMax = Vector3.zero;
        }

// The same functionality is implemented in the BoxBound
        // Debug.Assert(TaxiBound.BoxesIntersect(CarBound));

        if (isBoundingBoxinsideOroutside(TaxiBound.MinPosition, TaxiBound.MaxPosition, CarFrontWheelsBound.MinPosition,
                CarFrontWheelsBound.MaxPosition)) {
            // Min/Max of the overlap box bound
            Vector3 min = MinPos(TaxiBound.MinPosition, CarFrontWheelsBound.MinPosition);
            Vector3 max = MaxPos(TaxiBound.MaxPosition, CarFrontWheelsBound.MaxPosition); // max z position
            OverlapBoxFrontWheels.DrawBox = TaxiBound.DrawBox;
            OverlapBoxFrontWheels.DrawIntervals = TaxiBound.DrawIntervals;
            OverlapBoxFrontWheels.MinPosition = min;
            OverlapBoxFrontWheels.MaxPosition = max;
            // Update to show the overlap bound's min and max
            OverlapBoxFrontWheelsMax = max;
            OverlapBoxFrontWheelsMin = min;
        }
        else {
            OverlapBoxFrontWheels.DrawBox = false;
            OverlapBoxFrontWheels.DrawIntervals = false;
            OverlapBoxFrontWheels.MinPosition = Vector3.zero;
            OverlapBoxFrontWheels.MaxPosition = Vector3.zero;
            OverlapBoxFrontWheelsMin = Vector3.zero;
            OverlapBoxFrontWheelsMax = Vector3.zero;
        }

// The same functionality is implemented in the BoxBound
        // Debug.Assert(TaxiBound.BoxesIntersect(CarFrontWheelsBound));

        if (isBoundingBoxinsideOroutside(TaxiBound.MinPosition, TaxiBound.MaxPosition, CarBackWheelsBound.MinPosition,
                CarBackWheelsBound.MaxPosition)) {
            // Min/Max of the overlap box bound
            Vector3 min = MinPos(TaxiBound.MinPosition, CarBackWheelsBound.MinPosition);
            Vector3 max = MaxPos(TaxiBound.MaxPosition, CarBackWheelsBound.MaxPosition); // max z position
            OverlapBoxBackWheels.DrawBox = TaxiBound.DrawBox;
            OverlapBoxBackWheels.DrawIntervals = TaxiBound.DrawIntervals;
            OverlapBoxBackWheels.MinPosition = min;
            OverlapBoxBackWheels.MaxPosition = max;
            // Update to show the overlap bound's min and max
            OverlapBoxBackWheelsMax = max;
            OverlapBoxBackWheelsMin = min;
        }
        else {
            OverlapBoxBackWheels.DrawBox = false;
            OverlapBoxBackWheels.DrawIntervals = false;
            OverlapBoxBackWheels.MinPosition = Vector3.zero;
            OverlapBoxBackWheels.MaxPosition = Vector3.zero;
            OverlapBoxBackWheelsMin = Vector3.zero;
            OverlapBoxBackWheelsMax = Vector3.zero;
        }

// The same functionality is implemented in the BoxBound
        // Debug.Assert(TaxiBound.BoxesIntersect(CarBackWheelsBound));
    }

    private bool isBoundingBoxinsideOroutside(Vector3 firstMinPos, Vector3 firstMaxPos, Vector3 secondMinPos,
        Vector3 secondMaxPos) {
        return (((firstMinPos.x <= secondMaxPos.x) && //     X-interval overlap
                 (firstMaxPos.x >= secondMinPos.x))
                && // AND
                ((firstMinPos.y <= secondMaxPos.y) && //     Y-interval overlap
                 (firstMaxPos.y >= secondMinPos.y))
                && // AND
                ((firstMinPos.z <= secondMaxPos.z) && //     Z-interval overlap
                 (firstMaxPos.z >= secondMinPos.z)));
    }

    private Vector3 MinPos(Vector3 firstMinPos, Vector3 secondMinPos) {
        Vector3 min = Vector3.zero;
        return min = new Vector3(
            Mathf.Max(firstMinPos.x, secondMinPos.x), // min x position
            Mathf.Max(firstMinPos.y, secondMinPos.y), // min y position
            Mathf.Max(firstMinPos.z, secondMinPos.z)); // min z position
    }

    private Vector3 MaxPos(Vector3 firstMaxPos, Vector3 secondMaxPos) {
        Vector3 max = Vector3.zero;
        return max = new Vector3(
            Mathf.Min(firstMaxPos.x, secondMaxPos.x), // min x position
            Mathf.Min(firstMaxPos.y, secondMaxPos.y), // min y position
            Mathf.Min(firstMaxPos.z, secondMaxPos.z)); // min z position
    }
}
// if (((TaxiBound.MinPosition.x <= CarFrontWheelsBound.MaxPosition.x) && //     X-interval overlap
//      (TaxiBound.MaxPosition.x >= CarFrontWheelsBound.MinPosition.x))
//     && // AND
//     ((TaxiBound.MinPosition.y <= CarFrontWheelsBound.MaxPosition.y) && //     Y-interval overlap
//      (TaxiBound.MaxPosition.y >= CarFrontWheelsBound.MinPosition.y))
//     && // AND
//     ((TaxiBound.MinPosition.z <= CarFrontWheelsBound.MaxPosition.z) && //     Z-interval overlap
//      (TaxiBound.MaxPosition.z >= CarFrontWheelsBound.MinPosition.z))) {
//         Vector3 minFronWheels = new Vector3(
//             Mathf.Max(TaxiBound.MinPosition.x, CarBound.MinPosition.x), // min x position
//             Mathf.Max(TaxiBound.MinPosition.y, CarBound.MinPosition.y), // min y position
//             Mathf.Max(TaxiBound.MinPosition.z, CarBound.MinPosition.z)); // min z position
//         Vector3 maxFrontWheels = new Vector3(
//             Mathf.Min(TaxiBound.MaxPosition.x, CarBound.MaxPosition.x), // max x position
//             Mathf.Min(TaxiBound.MaxPosition.y, CarBound.MaxPosition.y), // max y position
//             Mathf.Min(TaxiBound.MaxPosition.z, CarBound.MaxPosition.z)); // max z position
//         OverlapBoxFrontWheels.DrawBox = TaxiBound.DrawBox;
//         OverlapBoxFrontWheels.DrawIntervals = TaxiBound.DrawIntervals;
//         OverlapBoxFrontWheels.MinPosition = minFronWheels;
//         OverlapBoxFrontWheels.MaxPosition = maxFrontWheels;
//     }
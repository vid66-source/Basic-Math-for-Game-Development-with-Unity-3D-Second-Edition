using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_2_1_MyScript : MonoBehaviour
{
    private MyIntervalBoundInY AnIntervalY = null;
    private MyIntervalBoundInX AnIntervalX = null;
    private MyIntervalBoundInZ AnIntervalZ = null;
    public float IntervalMaxY = 1.0f;
    public float IntervalMinY = 0.0f;
    public float IntervalMaxX = 1.0f;
    public float IntervalMinX = 0.0f;
    public float IntervalMaxZ = 1.0f;
    public float IntervalMinZ = 0.0f;

    public GameObject TestPosition = null;   // Use sphere to represent a position

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(TestPosition != null);
        AnIntervalY = new MyIntervalBoundInY();
        AnIntervalX = new MyIntervalBoundInX();
        AnIntervalZ = new MyIntervalBoundInZ();
    }

    // Update is called once per frame
    void Update(){
        SwapYMinMaxValue();

        // Updates AnInteval with values entered by the user
        AnIntervalY.MinValue = IntervalMinY;
        AnIntervalY.MaxValue = IntervalMaxY;
        AnIntervalX.MinValue = IntervalMinX;
        AnIntervalX.MaxValue = IntervalMaxX;
        AnIntervalZ.MinValue = IntervalMinZ;
        AnIntervalZ.MaxValue = IntervalMaxZ;

        AnIntervalY.IntervalColor = MyDrawObject.NoCollisionColor; // assume point is outside
        AnIntervalX.IntervalColor = MyDrawObject.NoCollisionColor; // assume point is outside
        AnIntervalZ.IntervalColor = MyDrawObject.NoCollisionColor; // assume point is outside

        // computes inside/outside of the current TestPosition.y value
        Vector3 pos = TestPosition.transform.localPosition;
        bool isInsideY = (pos.y >= IntervalMinY) && (pos.y <= IntervalMaxY);
        bool isInsideX = (pos.x >= IntervalMinX) && (pos.x <= IntervalMaxX);
        bool isInsideZ = (pos.z >= IntervalMinZ) && (pos.z <= IntervalMaxZ);

        if (isInsideY){
            Debug.Log("Position In Interval! (" + IntervalMinY + ", " + IntervalMaxY + ")");

            AnIntervalY.IntervalColor = MyDrawObject.CollisionColor;
            // The inside functionality is also supported by MyYInterval
            Debug.Assert(AnIntervalY.ValueInInterval(pos.y));
        }

        if (isInsideX){
            Debug.Log("Position In Interval! (" + IntervalMinX + ", " + IntervalMaxX + ")");
            AnIntervalX.IntervalColor = MyDrawObject.CollisionColor;
            // The inside functionality is also supported by MyYInterval
            Debug.Assert(AnIntervalX.ValueInInterval(pos.x));
        }

        if (isInsideZ){
            Debug.Log("Position In Interval! (" + IntervalMinZ + ", " + IntervalMaxZ + ")");

            AnIntervalZ.IntervalColor = MyDrawObject.CollisionColor;
            // The inside functionality is also supported by MyYInterval
            Debug.Assert(AnIntervalZ.ValueInInterval(pos.z));
        }
    }

    private void SwapYMinMaxValue(){
        if (IntervalMinY > IntervalMaxY){
            float temp = IntervalMinY;
            IntervalMinY = IntervalMaxY;
            IntervalMaxY = temp;
        }
    }
}


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_2_3_MyScript : MonoBehaviour
{
    private MyIntervalBoundInY GreenInterval = null;    // For visualzing the Green Interval
    public float GreenIntervalMax = 1.0f;               // Max/Min values for Green interval
    public float GreenIntervalMin = 0.0f;

    private MyIntervalBoundInY BlueInterval = null;     // For visualizing the Blue Interval
    public float BlueIntervalMax = 1.0f;                // Max/Min values of the Blue Interval
    public float BlueIntervalMin = 0.0f;

    private MyIntervalBoundInY OverlapInterval = null;  // For visualizing the overlap interval
    public float OverlapIntervalMax = float.NaN;        // Max/Min values of the overlap interval
    public float OverlapIntervalMin = float.NaN;

    public GameObject TestPosition = null;

    public Vector3 TestPositionCoord;
    static Color GreenColor = new Color(0.2f, 0.9f, 0.2f, 0.6f);
    static Color RedColor = new Color(0.9f, 0f, 0f, 0.8f);
    static Color BlueColor = new Color(0.2f, 0.2f, 0.9f, 0.6f);
    static Color OverlapColor = new Color(0.2f, 0.9f, 0.9f, 0.9f);

    // Start is called before the first frame update
    void Start()
    {

        // Define the Green Interval
        GreenInterval = new MyIntervalBoundInY();
        GreenInterval.IntervalColor = GreenColor;
        GreenInterval.PositionToDraw = new Vector3(0.6f, 0, 0);  // Slightly offset from the axis

        // Define the Blue Interval
        BlueInterval = new MyIntervalBoundInY();
        BlueInterval.IntervalColor = BlueColor;
        BlueInterval.PositionToDraw = new Vector3(-0.6f, 0, 0);  // Slightly offset from the axis

        // The overlap interval
        OverlapInterval = new MyIntervalBoundInY();
        OverlapInterval.DrawInterval = false; // Initially hide
        OverlapInterval.PositionToDraw = new Vector3(0.0f, 0, 0);  // One the axis
        OverlapInterval.IntervalColor =  OverlapColor;

        TestPositionCoord = TestPosition.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Update Green Interval with user input
        GreenInterval.MinValue = GreenIntervalMin;
        GreenInterval.MaxValue = GreenIntervalMax;

        // Update Blue Interval with user input
        BlueInterval.MinValue = BlueIntervalMin;
        BlueInterval.MaxValue = BlueIntervalMax;

        TestPosition.transform.position = TestPositionCoord;

        float testPosY = TestPositionCoord.y;

        bool inGreenRange = CheckInsideOrOut(testPosY, GreenIntervalMin, GreenIntervalMax);
        bool inBlueRange = CheckInsideOrOut(testPosY, BlueIntervalMin, BlueIntervalMax);

        // Intersect Green and Blue Intervals
        if (OverlapExists()) {   // overlap condition

            OverlapInterval.DrawInterval = true; // show the overlap interval

            // set the max/min values
            OverlapIntervalMax = Mathf.Min(GreenIntervalMax, BlueIntervalMax);
            OverlapIntervalMin = Mathf.Max(GreenIntervalMin, BlueIntervalMin);
            OverlapInterval.MaxValue = OverlapIntervalMax;   // display these values for the user
            OverlapInterval.MinValue = OverlapIntervalMin;

            Debug.Assert(GreenInterval.IntervalsIntersect(BlueInterval));

            bool inOverlapRange = CheckInsideOrOut(testPosY, OverlapIntervalMin, OverlapIntervalMax);

            if (inOverlapRange)
                Debug.Log("TestPosition Inside OverlapInterval");
            else {
                if (inGreenRange)
                    Debug.Log("TestPosition Inside GreenInterval");
                if (inBlueRange)
                    Debug.Log("TestPosition Inside BlueInterval");
            }

            }
                // This function is also implemented in the MyIntervalBound class
        else {
            OverlapInterval.DrawInterval = false;
            OverlapIntervalMax = float.NaN;
            OverlapIntervalMin = float.NaN;

            if (inGreenRange)
                Debug.Log("TestPosition Inside GreenInterval");
            if (inBlueRange)
                Debug.Log("TestPosition Inside BlueInterval");

        }
    }

    private bool CheckInsideOrOut(float testPoint, float min, float max) {
        return testPoint <= max && testPoint >= min;
    }

    private bool OverlapExists() {
        return GreenIntervalMin <= BlueIntervalMax &&
               GreenIntervalMax >= BlueIntervalMin;
    }
}


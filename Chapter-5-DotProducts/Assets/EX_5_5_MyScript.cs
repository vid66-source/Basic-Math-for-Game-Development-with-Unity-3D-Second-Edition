using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_5_5_MyScript : MonoBehaviour
{
    // Positions: to define the two lines.
    public GameObject P1, P2;  // define the line V1
    public GameObject Pa, Pb;  // define the line Va
    public GameObject Pd_1;    // point on V1 closest to Va
    public GameObject Pd_a;    // point on va closest to V1

    #region For visualizing the line
    private MyLineSegment ShowV1, ShowVa, ShowVp;
    private const float kScaleFactor = 0.4f;
    #endregion

    void Start()
    {
        Debug.Assert(P1 != null);   // Verify proper setting in the editor
        Debug.Assert(P2 != null);
        Debug.Assert(Pd_1 != null);
        Debug.Assert(Pa != null);
        Debug.Assert(Pb != null);
        Debug.Assert(Pd_a != null);

        #region For visualizing the line
        // To support visualizing the line
        ShowV1 = new MyLineSegment
        {
            VectorColor = Color.red,
            LineWidth = 0.1f
        };
        ShowVa = new MyLineSegment
        {
            VectorColor = Color.blue,
            LineWidth = 0.1f
        };
        ShowVp = new MyLineSegment
        {
            VectorColor = Color.black,
            LineWidth = 0.05f
        };
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pd_1, true);
        sv.DisablePicking(Pd_a, true);
        #endregion
    }

    void Update()
    {
        Vector3 v1 = (P2.transform.localPosition - P1.transform.localPosition);
        Vector3 va = (Pb.transform.localPosition - Pa.transform.localPosition);

        if ((v1.magnitude < float.Epsilon) || (va.magnitude < float.Epsilon))
            return;  // will only work with well defined line segments

        Vector3 va1 = P1.transform.localPosition - Pa.transform.localPosition;
        Vector3 v1n = v1.normalized;
        Vector3 van = va.normalized;
        float d = Vector3.Dot(v1n, van);

        bool almostParallel = (1f - Mathf.Abs(d) < float.Epsilon);

        float d1 = 0f, da = 0f;

        if (!almostParallel)  // two lines are not parallel
        {
            float dot1A1 = Vector3.Dot(v1n, va1);
            float dotAA1 = Vector3.Dot(van, va1);

            d1 = (-dot1A1 + d * dotAA1) / (1 - (d * d));
            da = (dotAA1 - d * dot1A1) / (1 - (d * d));

            d1 /= v1.magnitude;
            da /= va.magnitude;

            // Clamp parameters to segment bounds
            d1 = Mathf.Clamp01(d1);
            da = Mathf.Clamp01(da);

            Pd_1.transform.localPosition = P1.transform.localPosition + d1 * v1;
            Pd_a.transform.localPosition = Pa.transform.localPosition + da * va;

            float dist = (Pd_1.transform.localPosition - Pd_a.transform.localPosition).magnitude;
            Debug.Log("Non-parallel: d1=" + d1 + " da=" + da + " Distance=" + dist);
        }
        else  // Handle parallel lines
        {
            Debug.Log("Line segments are parallel - computing shortest distance");

            // For parallel lines, find the shortest distance by checking:
            // 1. Distance from each endpoint of segment 1 to segment 2
            // 2. Distance from each endpoint of segment 2 to segment 1

            float minDistance = float.MaxValue;
            Vector3 closestPoint1 = Vector3.zero;
            Vector3 closestPoint2 = Vector3.zero;

            // Check P1 to segment Va
            Vector3 tempClosest = ClosestPointOnSegment(P1.transform.localPosition,
                Pa.transform.localPosition, Pb.transform.localPosition);
            float tempDist = (P1.transform.localPosition - tempClosest).magnitude;
            if (tempDist < minDistance)
            {
                minDistance = tempDist;
                closestPoint1 = P1.transform.localPosition;
                closestPoint2 = tempClosest;
            }

            // Check P2 to segment Va
            tempClosest = ClosestPointOnSegment(P2.transform.localPosition,
                Pa.transform.localPosition, Pb.transform.localPosition);
            tempDist = (P2.transform.localPosition - tempClosest).magnitude;
            if (tempDist < minDistance)
            {
                minDistance = tempDist;
                closestPoint1 = P2.transform.localPosition;
                closestPoint2 = tempClosest;
            }

            // Check Pa to segment V1
            tempClosest = ClosestPointOnSegment(Pa.transform.localPosition,
                P1.transform.localPosition, P2.transform.localPosition);
            tempDist = (Pa.transform.localPosition - tempClosest).magnitude;
            if (tempDist < minDistance)
            {
                minDistance = tempDist;
                closestPoint1 = tempClosest;
                closestPoint2 = Pa.transform.localPosition;
            }

            // Check Pb to segment V1
            tempClosest = ClosestPointOnSegment(Pb.transform.localPosition,
                P1.transform.localPosition, P2.transform.localPosition);
            tempDist = (Pb.transform.localPosition - tempClosest).magnitude;
            if (tempDist < minDistance)
            {
                minDistance = tempDist;
                closestPoint1 = tempClosest;
                closestPoint2 = Pb.transform.localPosition;
            }

            Pd_1.transform.localPosition = closestPoint1;
            Pd_a.transform.localPosition = closestPoint2;

            Debug.Log("Parallel lines: Distance=" + minDistance);
        }

        #region  For visualizing the line
        ShowV1.VectorFromTo(P1.transform.localPosition, P2.transform.localPosition);
        ShowVa.VectorFromTo(Pa.transform.localPosition, Pb.transform.localPosition);
        ShowVp.DrawVector = true; // Always show the connection

        Pd_1.SetActive(true);
        Pd_a.SetActive(true);

        // Update visual properties
        Color c = Pd_1.GetComponent<Renderer>().material.color;
        c.a = almostParallel ? 0.8f : 0.25f; // Different alpha for parallel vs non-parallel
        Pd_1.GetComponent<Renderer>().material.color = c;
        Pd_a.GetComponent<Renderer>().material.color = c;

        ShowVp.VectorFromTo(Pd_1.transform.localPosition, Pd_a.transform.localPosition);

        float s = ShowVp.Magnitude * kScaleFactor;
        Pd_1.transform.localScale = new Vector3(s, s, s);
        Pd_a.transform.localScale = new Vector3(s, s, s);
        #endregion
    }

    /// <summary>
    /// Calculate the closest point on a line segment to a given point
    /// </summary>
    /// <param name="point">The point to find closest position to</param>
    /// <param name="segmentStart">Start of the line segment</param>
    /// <param name="segmentEnd">End of the line segment</param>
    /// <returns>Closest point on the segment</returns>
    public Vector3 ClosestPointOnSegment(Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
    {
        Vector3 segmentVector = segmentEnd - segmentStart;
        Vector3 pointVector = point - segmentStart;

        float segmentLengthSquared = segmentVector.sqrMagnitude;

        if (segmentLengthSquared < float.Epsilon)
        {
            // Degenerate segment - start and end are the same
            return segmentStart;
        }

        float t = Vector3.Dot(pointVector, segmentVector) / segmentLengthSquared;
        t = Mathf.Clamp01(t); // Clamp to [0,1] to stay within segment

        return segmentStart + t * segmentVector;
    }
}
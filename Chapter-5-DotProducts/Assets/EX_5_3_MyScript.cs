using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_5_3_MyScript : MonoBehaviour
{
    // Positions: to deine the interval, the test, and projected
    public GameObject P0 = null;   // Position P0
    public GameObject P1 = null;   // Position P1
    public GameObject Pt = null;   // Position Pt: test position
    public GameObject Pon = null;  // Position Pon: position on the interval-line
    private Vector3 _v1 = Vector3.zero;
    private Vector3 _vt = Vector3.zero;
    private Vector3 _v1N = Vector3.zero;
    private Vector3 _vOn = Vector3.zero;
    private float _d = Single.NaN;

    #region For visualizing the vectors
    private MyVector ShowV1;    // V1
    private MyLineSegment ShowLine; // Defined by P0P1
    private MyLineSegment ShowPv, ShowPa;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P0 != null);   // Verify proper setting in the editor
        Debug.Assert(P1 != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Pon != null);

        #region For visualizing the vectors
        // To support visualizing the vectors
        ShowV1 = new MyVector {
            VectorColor = Color.green
        };
        ShowLine = new MyLineSegment
        {
            VectorColor = MyDrawObject.NoCollisionColor,
            LineWidth = 0.6f
        };
        ShowPv = new MyLineSegment
        {
            VectorColor = Color.black,
            LineWidth = 0.02f
        };
        ShowPa = new MyLineSegment
        {
            VectorColor = Color.black,
            LineWidth = 0.02f
        };
        Pt.GetComponent<Renderer>().material.color = Color.black;
        Pon.GetComponent<Renderer>().material.color = Color.black;

        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pon, true);
        #endregion

    }

    // Update is called once per frame
    void Update()
    {
        _v1 = P1.transform.localPosition - P0.transform.localPosition;
        _vt = Pt.transform.localPosition - P0.transform.localPosition;
        _v1N = _v1.normalized;
        _d = Vector3.Dot(_vt, _v1N);
        _vOn = _d * _v1N - _vt;
        if (_v1.magnitude > float.Epsilon) {
            Pon.transform.localPosition = Pt.transform.localPosition + _vOn;

            if ((_d >= 0) && (_d <= _v1.magnitude))
                Debug.Log("V1.mag=" + _v1.magnitude + "  Projected Length=" + _d + "  ==> Inside!");
            else
                Debug.Log("V1.mag=" + _v1.magnitude + "  Projected Length=" + _d + "  ==> Outside!");
        }

        if ((Pt.transform.localPosition - P0.transform.position).magnitude > float.Epsilon) {
            float v1VonDot = Vector3.Dot(_v1, _vOn);
            float cosAngle = v1VonDot / (_v1.magnitude * _vOn.magnitude);
            float angleRad = Mathf.Acos(cosAngle);
            float angleDeg = angleRad * Mathf.Rad2Deg;
            Debug.Log("Dot production of Von and V1 = " + v1VonDot + ". Cos of angle between Von and V1 = " + cosAngle + ". Angle = " + angleDeg);
        }


        #region  For visualizing the vectors

        ShowLine.VectorFromTo(P0.transform.localPosition, P1.transform.localPosition);
        bool visible = _v1.magnitude > float.Epsilon;
        ShowV1.DrawVector = visible;
        ShowPv.DrawVector = visible;
        ShowPa.DrawVector = visible;

        if (visible)
        {
            Vector3 v1n = _v1.normalized;
            Vector3 vt = Pt.transform.localPosition - P0.transform.localPosition;
            float d = Vector3.Dot(vt, v1n);
            if (d >= 0 && d <= _v1.magnitude)
                ShowLine.VectorColor = MyDrawObject.CollisionColor;
            else
                ShowLine.VectorColor = MyDrawObject.NoCollisionColor;

            ShowPv.VectorFromTo(P0.transform.localPosition, Pt.transform.localPosition);
            ShowPa.VectorFromTo(Pt.transform.localPosition, Pon.transform.localPosition);

            float after = 0.45f;
            float before = 0.15f;
            Vector3 pv0 = P0.transform.localPosition - before * _v1; ;
            Vector3 pv1 = P1.transform.localPosition + after * _v1;

            if (d > (((1f + after) * _v1.magnitude) - 1f))
                pv1 = Pon.transform.localPosition + v1n;

            if (d < ((-before * _v1.magnitude) + 1f))
                pv0 = Pon.transform.localPosition - v1n;

            ShowV1.VectorFromTo(pv0, pv1);
        }
        #endregion

    }
}

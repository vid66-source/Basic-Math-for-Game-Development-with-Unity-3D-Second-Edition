using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_6_4_MyScript : MonoBehaviour {
    public bool ShowAxisFrame = true;
    public bool ShowProjections = true;
    public bool DefinePlaneByD;

    // Plane Equation: P dot Vn = D
    public Vector3 Vn = Vector3.up;
    public float D = 2f;
    public Vector3 PnPos = Vector3.one;

    public GameObject Pn = null;
    public GameObject Pt = null; // The point to be projected onto the plane
    public GameObject Pl = null; // Projection of Pt on Vn
    public GameObject Pon = null; // Projection of Pt on the plane

    #region For visualizing the vectors

    private MyVector ShowNormal, ShowPt; //
    private MyXZPlane ShowPlane; // Plane where XZ lies
    private MyLineSegment ShowPtOnPlane, ShowPtOnN;
    private Vector3 _initialScale;
    private float PtOnVnProjection;

    #endregion

    // Start is called before the first frame update
    void Start() {
        DefinePlaneByD = true;

        Debug.Assert(Pn != null); // Verify proper setting in the editor
        Debug.Assert(Pt != null);
        Debug.Assert(Pl != null);
        Debug.Assert(Pon != null);

        #region For visualizing the vectors

        // To support visualizing the vectors
        ShowNormal = new MyVector {
            VectorColor = Color.white
        };
        ShowPlane = new MyXZPlane {
            PlaneColor = new Color(0.8f, 0.3f, 0.3f, 1.0f),
            XSize = 0.5f,
            YSize = 0.5f,
            ZSize = 0.5f
        };
        ShowPtOnPlane = new MyLineSegment {
            VectorColor = Color.black,
            LineWidth = 0.05f
        };
        ShowPt = new MyVector {
            VectorColor = Color.red
        };
        ShowPtOnN = new MyLineSegment {
            VectorColor = Color.green,
            LineWidth = 0.05f
        };
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pn, true);
        sv.DisablePicking(Pl, true);
        sv.DisablePicking(Pon, true);
        _initialScale = Pon.transform.localScale;

        #endregion
    }

    // Update is called once per frame
    void Update() {
        if (DefinePlaneByD) {
            Vn.Normalize();
            Pn.transform.localPosition = D * this.Vn;
            PtOnVnProjection = Vector3.Dot(Pt.transform.localPosition, Vn);
            Pl.transform.localPosition = PtOnVnProjection * Vn;
            Pon.transform.localPosition = Pt.transform.localPosition - (PtOnVnProjection - D) * Vn;
        }
        else {
            if (Mathf.Abs(D) < 0.0001f) D = 0.0001f * Mathf.Sign(D == 0 ? 1f : D);
            D = Pn.transform.localPosition.magnitude;
            Vector3 VnNorm = Pn.transform.localPosition / D;
            PtOnVnProjection = Vector3.Dot(Pt.transform.localPosition, VnNorm);
            Pl.transform.localPosition = PtOnVnProjection * VnNorm;
            Pn.transform.localPosition = PnPos;
        }

        float angleDeg = Mathf.Acos(Vector3.Dot(Vn, Pt.transform.localPosition.normalized)) * Mathf.Rad2Deg;
        Debug.Log("Angle between Vn and Pt:" + angleDeg + "Deg");

        float pTPOnMagnitude = Mathf.Abs(PtOnVnProjection - D);
        bool moreThanHalf = (angleDeg < 90f);
        Pon.SetActive(moreThanHalf);

        var sv = UnityEditor.SceneVisibilityManager.instance;
        bool DIsNotZiro = (D != 0f);
        if (DIsNotZiro) {
            sv.DisablePicking(Pn, false);
        }


        float scaleFactor = 1 + pTPOnMagnitude / 10;
        Pon.transform.localScale = _initialScale * scaleFactor;
        Pon.transform.localScale = Vector3.Max(_initialScale, _initialScale * scaleFactor);

        #region For visualizing the vectors

        AxisFrame.ShowAxisFrame = ShowAxisFrame;
        ShowPtOnPlane.DrawVector = ShowProjections;
        ShowPtOnN.DrawVector = ShowProjections;
        ShowPt.DrawVector = ShowProjections;

        ShowNormal.VectorAt = Vector3.zero;
        ShowNormal.Direction = Vn;


        PtOnVnProjection= Vector3.Dot(Pt.transform.localPosition, Vn);
        Pon.transform.localPosition = Pt.transform.localPosition - (PtOnVnProjection- D) * Vn;
        Pl.transform.localPosition = PtOnVnProjection* Vn;
        // }

        float offset = 1.5f;
        float Dsize = Mathf.Abs(D) + offset;
        Vector3 from = Vector3.zero;

        if (D < 0) {
            from = D * Vn;
            Dsize = Mathf.Abs(D) + offset;
        }

        float useD = Mathf.Max(PtOnVnProjection, Vector3.Dot(Vn, (Pt.transform.localPosition - Pn.transform.localPosition)));
        float useSize = Dsize;
        // now consider d
        if ((useD + offset) > Dsize) {
            useSize = useD + offset;
        }
        else if (useD < 0) {
            Vector3 toFrom = Pl.transform.localPosition - from;
            if (Vector3.Dot(toFrom, Vn) < 0f) {
                from = Pl.transform.localPosition;
                useSize = Dsize + Pl.transform.localPosition.magnitude;
            }
        }

        ShowNormal.VectorAt = from;
        ShowNormal.Magnitude = useSize;

        float s = 2f;
        Vector3 ptTon = PtOnVnProjection* Vn - Pt.transform.localPosition;
        ShowPtOnN.Direction = ptTon;
        ShowPtOnN.Magnitude = ptTon.magnitude;
        ShowPtOnN.VectorAt = Pt.transform.localPosition;

        ShowPt.VectorFromTo(Vector3.zero, Pt.transform.localPosition);
        Vector3 von = Pon.transform.localPosition - Pn.transform.localPosition;
        s = von.magnitude * 1.2f;
        if (s < 2f)
            s = 2f;

        ShowPtOnPlane.VectorFromTo(Pon.transform.localPosition, Pt.transform.localPosition);
        Pon.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vn);

        ShowPlane.PlaneNormal = -Vn;

        ShowPlane.Center = Pn.transform.localPosition;
        ShowPlane.XSize = ShowPlane.ZSize = s;

        #endregion
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_6_4_MyScript : MonoBehaviour {
    public bool ShowAxisFrame = true;
    public bool ShowProjections = true;
    public bool DefinePlaneByD;
    public bool DefinePlanePn;
    public bool DefinePlaneP0P1P2;

    // Plane Equation: P dot Vn = D
    public Vector3 Vn = Vector3.up;
    public float D = 2f;
    public Vector3 PnPos = Vector3.one;

    public GameObject Pn = null;
    public GameObject Pt = null; // The point to be projected onto the plane
    public GameObject Pl = null; // Projection of Pt on Vn
    public GameObject Pon = null; // Projection of Pt on the plane
    public GameObject P0 = null;
    public GameObject P1 = null;
    public GameObject P2 = null;

    #region For visualizing the vectors

    private MyVector ShowNormal, ShowPt, ShowP1, ShowP2, ShowP1P2Normal, ShowP1NormalNormal;
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
        Debug.Assert(P0 != null);
        Debug.Assert(P1 != null);
        Debug.Assert(P2 != null);
        P0.SetActive(false);
        P1.SetActive(false);
        P2.SetActive(false);

        #region For visualizing the vectors

        // To support visualizing the vectors
        ShowNormal = new MyVector {
            VectorColor = Color.white
        };
        ShowP1 = new MyVector {
            VectorColor = Color.skyBlue
        };
        ShowP2 = new MyVector {
            VectorColor = Color.darkSlateBlue
        };
        ShowP1P2Normal = new MyVector {
            VectorColor = Color.white
        };
        ShowP1NormalNormal = new MyVector {
            VectorColor = Color.yellowNice
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

        ShowP1.DrawVector = false;
        ShowP2.DrawVector = false;
        ShowP1P2Normal.DrawVector = false;
        ShowP1NormalNormal.DrawVector = false;
        #endregion
    }

    // Update is called once per frame
    void Update() {
        Vector3 VnNorm = Vn;

        if (DefinePlaneByD){
            TurnOnOffVectorsAndPoints(false);
            // Режим: користувач задає D та Vn
            Vn.Normalize();
            VnNorm = Vn;
            Pn.transform.localPosition = D * Vn;
            PtOnVnProjection = Vector3.Dot(Pt.transform.localPosition, Vn);
            Pl.transform.localPosition = PtOnVnProjection * Vn;

            Pon.transform.localPosition = Pt.transform.localPosition - (PtOnVnProjection - D) * Vn;
        }
        else if (DefinePlanePn){
            TurnOnOffVectorsAndPoints(false);
            // Режим: користувач маніпулює позицією Pn
            Pn.transform.localPosition = PnPos;
            D = Pn.transform.localPosition.magnitude;

            if (Mathf.Abs(D) < 0.0001f) {
                D = 0.0001f * Mathf.Sign(D == 0 ? 1f : D);
            }

            VnNorm = Pn.transform.localPosition / D;
            PtOnVnProjection = Vector3.Dot(Pt.transform.localPosition, VnNorm);
            Pl.transform.localPosition = PtOnVnProjection * VnNorm;
            Pon.transform.localPosition = Pt.transform.localPosition - (PtOnVnProjection - D) * VnNorm;
        }
        else if (DefinePlaneP0P1P2){
            TurnOnOffVectorsAndPoints(true);
            Vector3 v1 = P1.transform.localPosition - P0.transform.localPosition;
            Vector3 v2 = P2.transform.localPosition - P0.transform.localPosition;
            Vector3 P1P2Normal = -Vector3.Cross(v1, v2);
            VnNorm = P1P2Normal.normalized;
            Vector3 P1XNormalNormal = -Vector3.Cross(v1, VnNorm);
            D = Vector3.Dot(VnNorm, P0.transform.localPosition);
            Pn.transform.localPosition = D * VnNorm;
            PtOnVnProjection = Vector3.Dot(Pt.transform.localPosition, VnNorm);
            Pl.transform.localPosition = PtOnVnProjection * VnNorm;
            Pon.transform.localPosition = Pt.transform.localPosition - (PtOnVnProjection - D) * VnNorm;
            ShowP1.Direction = v1;
            ShowP1.Magnitude = v1.magnitude;
            ShowP1.VectorFromTo(P0.transform.localPosition, P1.transform.localPosition);
            ShowP2.Direction = v2;
            ShowP2.Magnitude = v2.magnitude;
            ShowP2.VectorFromTo(P0.transform.localPosition, P2.transform.localPosition);
            ShowP1P2Normal.Direction = P1P2Normal;
            ShowP1P2Normal.VectorAtDirLength(P0.transform.localPosition, P1P2Normal, P1P2Normal.magnitude);
            ShowP1NormalNormal.Direction = P1XNormalNormal;
            ShowP1NormalNormal.VectorAtDirLength(P0.transform.localPosition, P1XNormalNormal, P1XNormalNormal.magnitude);
            Debug.Log("V1 Length:" + v1.magnitude + ". P1NormalNormal Length:" + P1XNormalNormal.magnitude);

            Vector3 P0Pon = Pon.transform.localPosition - P0.transform.localPosition;
            float P0PonProjOnP1 = Vector3.Dot(P0Pon, v1.normalized);
            float P0PonProjOnP1NormalNormal = Vector3.Dot(P0Pon, P1XNormalNormal.normalized);
            bool boundInsideOutside = (P0PonProjOnP1 <= v1.magnitude && P0PonProjOnP1 >= 0) && (P0PonProjOnP1NormalNormal <= P1XNormalNormal.magnitude && P0PonProjOnP1NormalNormal >= 0);;
            if (boundInsideOutside){
                Debug.Log("P0PonProjOnP1:" + P0PonProjOnP1 + ". P0PonProjOnP1NormalNormal:" + P0PonProjOnP1NormalNormal + ". Pon INSIDE 2D bound");;
            }
        }


        float angleDeg = Mathf.Acos(Vector3.Dot(VnNorm, Pt.transform.localPosition.normalized)) * Mathf.Rad2Deg;
        Debug.Log("Angle between Vn and Pt:" + angleDeg + "Deg");

        float pTPOnMagnitude = Mathf.Abs(PtOnVnProjection - D);
        bool moreThanHalf = (angleDeg < 90f);
        Pon.SetActive(moreThanHalf);

        float scaleFactor = 1 + pTPOnMagnitude / 10;
        Pon.transform.localScale = Vector3.Max(_initialScale, _initialScale * scaleFactor);

        bool ptOnTopOfThePlane = Pl.transform.localPosition.magnitude >= D;
        if (!ptOnTopOfThePlane){
            Pon.SetActive(false);
        }

        #region For visualizing the vectors

        AxisFrame.ShowAxisFrame = ShowAxisFrame;
        ShowPtOnPlane.DrawVector = ShowProjections;
        ShowPtOnN.DrawVector = ShowProjections;
        ShowPt.DrawVector = ShowProjections;

        ShowNormal.Direction = VnNorm;

        float offset = 1.5f;
        float Dsize = Mathf.Abs(D) + offset;
        Vector3 from = Vector3.zero;

        if (D < 0) {
            from = D * VnNorm;
            Dsize = Mathf.Abs(D) + offset;
        }

        float useD = Mathf.Max(PtOnVnProjection, Vector3.Dot(VnNorm, (Pt.transform.localPosition - Pn.transform.localPosition)));
        float useSize = Dsize;

        // now consider d
        if ((useD + offset) > Dsize) {
            useSize = useD + offset;
        }
        else if (useD < 0) {
            Vector3 toFrom = Pl.transform.localPosition - from;
            if (Vector3.Dot(toFrom, VnNorm) < 0f) {
                from = Pl.transform.localPosition;
                useSize = Dsize + Pl.transform.localPosition.magnitude;
            }
        }

        ShowNormal.VectorAt = from;
        ShowNormal.Magnitude = useSize;

        Vector3 ptTon = PtOnVnProjection * VnNorm - Pt.transform.localPosition;
        ShowPtOnN.Direction = ptTon;
        ShowPtOnN.Magnitude = ptTon.magnitude;
        ShowPtOnN.VectorAt = Pt.transform.localPosition;

        ShowPt.VectorFromTo(Vector3.zero, Pt.transform.localPosition);

        Vector3 von = Pon.transform.localPosition - Pn.transform.localPosition;
        float s = von.magnitude * 1.2f;
        if (s < 2f)
            s = 2f;

        ShowPtOnPlane.VectorFromTo(Pon.transform.localPosition, Pt.transform.localPosition);
        Pon.transform.localRotation = Quaternion.FromToRotation(Vector3.up, VnNorm);

        ShowPlane.PlaneNormal = -VnNorm;
        ShowPlane.Center = Pn.transform.localPosition;
        ShowPlane.XSize = ShowPlane.ZSize = s;

        #endregion
    }

    private void TurnOnOffVectorsAndPoints(bool switcher){
        P0.SetActive(switcher);
        P1.SetActive(switcher);
        P2.SetActive(switcher);
        ShowP1.DrawVector = switcher;
        ShowP2.DrawVector = switcher;
        ShowP1P2Normal.DrawVector = switcher;
        ShowP1NormalNormal.DrawVector = switcher;
    }
}
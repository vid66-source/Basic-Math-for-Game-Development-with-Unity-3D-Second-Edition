using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_6_3_MyScript : MonoBehaviour {
    #region Identical to EX_6_2

    public Vector3 Direction;

    // Defines two vectors: V1 = P1 - P0, V2 = P2 - P0
    public GameObject P0 = null; // The three positions
    public GameObject P1 = null; //
    public GameObject P2 = null; //
    public GameObject P3 = null; //
    public GameObject MyVectorIntersect = null; //

    // Plane equation:   P dot vn = D
    public GameObject Ds; // To show the D-value
    public GameObject Pn; // Where Vn crosses the plane

    public bool ShowPointOnPlane = true; // Hide or Show Pt
    public GameObject Pt; // Point to adjust
    public GameObject Pon; // Point in the Plane, in the Pt direction

    #endregion

    public GameObject P2p; // The perpendicular version of P2
    public GameObject PVnBound = null; //
    public float VnSize = 3f;


    #region For visualizing the vectors

    private MyVector ShowV1, ShowV2, ShowV3, ShowV4;
    private MyVector ShowVn;
    private MyVector ShowNormal; // Vn
    private MyXZPlane ShowPlane; // Plane where XZ lies
    private MyLineSegment ShowPtLine; // Line to Pt

    #endregion

    // Start is called before the first frame update
    void Start() {
        #region Identical to EX_6_2

        Debug.Assert(P0 != null); // Verify proper setting in the editor
        Debug.Assert(P1 != null);
        Debug.Assert(P2 != null);
        Debug.Assert(Ds != null);
        Debug.Assert(Pn != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Pon != null);

        #endregion

        Debug.Assert(P2p != null);

        #region For visualizing the vectors

        // To support visualizing the vectors
        ShowV1 = new MyVector {
            VectorColor = Color.cyan
        };
        ShowV2 = new MyVector {
            VectorColor = Color.magenta
        };
        ShowV3 = new MyVector {
            VectorColor = Color.green
        };
        ShowV4 = new MyVector {
            VectorColor = Color.blue
        };
        ShowNormal = new MyVector {
            VectorColor = Color.white
        };
        ShowVn = new MyVector {
            VectorColor = Color.black
        };
        ShowPlane = new MyXZPlane {
            PlaneColor = new Color(0.8f, 0.3f, 0.3f, 1.0f),
            XSize = 0.5f,
            YSize = 0.5f,
            ZSize = 0.5f
        };
        ShowPtLine = new MyLineSegment {
            VectorColor = Color.black,
            LineWidth = 0.05f
        };
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Ds, true);
        sv.DisablePicking(Pn, true);
        sv.DisablePicking(Pon, true);
        sv.DisablePicking(P2p, true);
        MyVectorIntersect.SetActive(false);

        Direction = Vector3.up;

        #endregion
    }

    // Update is called once per frame
    void Update() {
        #region Identical to EX_6_2

        // Computes V1 and V2
        Vector3 v1 = P1.transform.localPosition - P0.transform.localPosition;
        Vector3 v2 = P2.transform.localPosition - P0.transform.localPosition;
        Vector3 myVector = Direction.normalized;
        if ((v1.magnitude < float.Epsilon) || (v2.magnitude < float.Epsilon))
            return;

        // Plane equation parameters
        Vector3 vn = Vector3.Cross(v1, v2);
        vn.Normalize(); // keep this vector normalized

        float D = Vector3.Dot(vn, P0.transform.localPosition);

        // Showing the plane equation is consistent
        Pn.transform.localPosition = D * vn;
        Ds.transform.localScale = new Vector3(D * 2f, D * 2f, D * 2f); // sphere expects diameter

        // Set up for displaying Pt and Pon
        Pt.SetActive(ShowPointOnPlane);
        Pon.SetActive(ShowPointOnPlane);
        float t = 0;
        bool almostParallel = false;
        if (ShowPointOnPlane) {
            float d = Vector3.Dot(vn, Pt.transform.localPosition); // distance
            almostParallel = (Mathf.Abs(d) < float.Epsilon);
            Pon.SetActive(!almostParallel);
            if (!almostParallel) {
                t = D / d;
                Pon.transform.localPosition = t * Pt.transform.localPosition;
            }
        }

        #endregion

        bool showIntersection = false;
        float denom = Vector3.Dot(vn, myVector);
        bool linesNotParalel = Mathf.Abs(denom) > float.Epsilon;
        if (linesNotParalel){
            float d = (D - Vector3.Dot(vn, P3.transform.localPosition)) / denom;

            if (d >= 0){
                MyVectorIntersect.transform.localPosition = P3.transform.localPosition + d * myVector;

                float checkD = Vector3.Dot(MyVectorIntersect.transform.localPosition, vn);
                showIntersection = Mathf.Abs(checkD - D) < 0.001f;
            }

            MyVectorIntersect.SetActive(showIntersection);


            float l1 = v1.magnitude;
            float l2 = v2.magnitude;
            float l3 = VnSize;
            Vector3 v2p = l2 * Vector3.Cross(vn, v1).normalized;
            Vector3 v1v2NormalScaled = l3 * Vector3.Cross(v1, v2).normalized;
            P2p.transform.localPosition = P0.transform.localPosition + v2p;
            PVnBound.transform.localPosition = P0.transform.localPosition + v1v2NormalScaled;

            bool inside = false;
            bool ptInside = false;
            if (!almostParallel){
                Vector3 von = Pon.transform.localPosition - P0.transform.localPosition;
                float d1 = Vector3.Dot(von, v1.normalized);
                float d2 = Vector3.Dot(von, v2p.normalized);

                inside = ((d1 >= 0) && (d1 <= l1)) && ((d2 >= 0) && (d2 <= l2));
                if (inside)
                    Debug.Log("Inside: Pon is inside of the region defined by V1 and V2");
                else
                    Debug.Log("Outside: Pon is outside of the region defined by V1 and V2");

                Vector3 vt = Pt.transform.localPosition - P0.transform.localPosition;
                float td1 = Vector3.Dot(vt, v1.normalized);
                float td2 = Vector3.Dot(vt, v2p.normalized);
                float td3 = Vector3.Dot(vt, vn.normalized);

                ptInside = ((td1 >= 0) && (td1 <= l1)) && ((td2 >= 0) && (td2 <= l2)) && ((td3 >= 0) && (td3 <= l3));

                if (ptInside)
                    Debug.Log("Inside: Pt is inside of the region defined by V1, V2 and Vn Size");
            }

            Vector3[] axes = {v1, v2p, v1v2NormalScaled };
            PlaneInfo[] planes = new PlaneInfo[6];
            for (int i = 0; i < planes.Length; i++){
                int axisIndex = i / 2;
                bool shifted = (i % 2) == 1;

                Vector3 offset = shifted ? axes[axisIndex] : Vector3.zero;

                Vector3 vec1, vec2;
                switch (axisIndex){
                    case 0:
                        vec1 = v2p;
                        vec2 = v1v2NormalScaled;
                        break;
                    case 1:
                        vec1 = v1;
                        vec2 = v1v2NormalScaled;
                        break;
                    case 2:
                        vec1 = v1;
                        vec2 = v2p;
                        break;
                    default:
                        vec1 = vec2 = Vector3.zero;
                        break;
                }

                Vector3 p0 = P0.transform.localPosition + offset;
                Vector3 p1 = p0 + vec1;
                Vector3 p2 = p0 + vec2;

                planes[i] = new PlaneInfo(p0, p1, p2);
            }

            foreach (var plane in planes){
                float planeDenom = Vector3.Dot(Direction,  plane.Normal);
                float vectorScale = (plane.D - Vector3.Dot(P3.transform.localPosition, plane.Normal)) / planeDenom;
                Vector3 pointOnPlane = P3.transform.localPosition + Direction * vectorScale;
                Vector3 pointInBoundingBox = pointOnPlane - plane.P0;
                float v1length = plane.V1.magnitude;
                float v2length = plane.V2.magnitude;
                float myVectorOnV1 = Vector3.Dot(pointInBoundingBox, plane.V1.normalized);
                float myVectorOnV2 = Vector3.Dot(pointInBoundingBox, plane.V2.normalized);
                bool isInside = (myVectorOnV1 >= 0) && (myVectorOnV1 <= v1length) &&  (myVectorOnV2 >= 0) && (myVectorOnV2 <= v2length);
                if (isInside){
                    Debug.Log("Inside: MyVector is inside of the region defined by V1 and V2");
                }
            }


            #region For visualizing the vectors

            ShowV1.VectorFromTo(P0.transform.localPosition, P1.transform.localPosition);
            ShowV2.VectorFromTo(P0.transform.localPosition, P2.transform.localPosition);
            ShowV3.VectorFromTo(P0.transform.localPosition, P2p.transform.localPosition);
            ShowV4.VectorAtDirLength(P3.transform.localPosition, myVector, 10f);

            ShowVn.VectorAt = P0.transform.localPosition;
            ShowVn.Direction = v1v2NormalScaled;
            ShowVn.Magnitude = v1v2NormalScaled.magnitude;

            ShowNormal.VectorAt = Vector3.zero;
            ShowNormal.Magnitude = Mathf.Abs(D) + 2f;
            ShowNormal.Direction = vn;

            ShowPlane.PlaneNormal = -vn;
            Vector3 at = P0.transform.localPosition + P1.transform.localPosition + P2.transform.localPosition +
                         Pn.transform.localPosition;
            int c = 4;

            float scale = 1.0f;
            ShowPtLine.DrawVector = ShowPointOnPlane;
            float da = v1.magnitude * scale;
            float db = v2.magnitude * scale;
            float du = Mathf.Max(da, db);

            if (ShowPointOnPlane && (!almostParallel)) {
                Pon.GetComponent<Renderer>().material.color = Color.white;
                float don = (Pon.transform.localPosition - P0.transform.localPosition).magnitude * scale;
                at += Pon.transform.localPosition;
                c++;
                du = Mathf.Max(du, don);

                // Now the line
                ShowPtLine.VectorColor = Color.black;
                if (Vector3.Dot(Pon.transform.localPosition, Pt.transform.localPosition) < 0) {
                    ShowPtLine.VectorColor = Color.red;
                    ShowPtLine.VectorFromTo(Pt.transform.localPosition, Pon.transform.localPosition);
                }
                else {
                    if (Pon.transform.localPosition.magnitude > Pt.transform.localPosition.magnitude)
                        ShowPtLine.VectorFromTo(Vector3.zero, Pon.transform.localPosition);
                    else
                        ShowPtLine.VectorFromTo(Vector3.zero, Pt.transform.localPosition);
                }

                if (!inside)
                    Pon.GetComponent<Renderer>().material.color = Color.red;
            }

            if (du < 0.5f)
                du = 0.5f;

            ShowPlane.XSize = ShowPlane.ZSize = du;
            ShowPlane.Center = at / c;

            #endregion
        }
    }
}
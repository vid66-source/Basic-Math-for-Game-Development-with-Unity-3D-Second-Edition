using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EX_6_6_MyScript : MonoBehaviour{
    #region identical to EX_6_5

    public bool ShowAxisFrame = true;

    // Plane Equation: P dot Vn = D
    private Vector3 Vn = Vector3.up;
    private float D = 2f;
    public GameObject Pn = null; // The point where plane normal passes

    public GameObject P0 = null, P1 = null; // The line segment
    public GameObject Pon = null; // The intersection position
    public GameObject Pa = null; // The intersection position
    public GameObject Pb = null; // The intersection position
    public GameObject Pc = null; // The intersection position

    #endregion

    public GameObject Pl = null; // Projection of P0 on Vn
    public GameObject Pr = null; // reflected position of P0

    #region For visualizing the vectors

    private MyVector ShowNormal, ShowNormalAtPon, ShowNormalAtPa, ShowNormalForBBTest; //
    private MyXZPlane ShowPlane; // Plane where XZ lies
    private MyLineSegment ShowLine;
    private MyLineSegment ShowRestOfLine;
    private MyLineSegment ShowReflect;
    private MyVector ShowM;
    private MyVector ShowVb;
    private MyVector ShowVc;

    #endregion

    // Start is called before the first frame update
    void Start(){
        # region identical to EX_6_5

        Debug.Assert(Pn != null); // Verify proper setting in the editor
        Debug.Assert(P0 != null);
        Debug.Assert(P1 != null);
        Debug.Assert(Pon != null);

        #endregion

        Debug.Assert(Pl != null);
        Debug.Assert(Pr != null);


        #region For visualizing the vectors

        // To support visualizing the vectors
        ShowNormal = new MyVector{
            VectorColor = Color.white
        };
        ShowNormalAtPa = new MyVector{
            VectorColor = Color.lightSalmon
        };
        ShowNormalForBBTest = new MyVector{
            VectorColor = Color.darkSlateGray
        };
        ShowM = new MyVector{
            VectorColor = Color.green
        };
        ShowVb = new MyVector{
            VectorColor = Color.darkOrange
        };
        ShowVc = new MyVector{
            VectorColor = Color.navyBlue
        };
        ShowNormalAtPon = new MyVector{
            VectorColor = Color.white
        };
        ShowPlane = new MyXZPlane{
            PlaneColor = new Color(0.8f, 0.3f, 0.3f, 1.0f),
            XSize = 0.5f,
            YSize = 0.5f,
            ZSize = 0.5f
        };
        ShowLine = new MyLineSegment{
            VectorColor = Color.black,
            LineWidth = 0.05f
        };
        ShowRestOfLine = new MyLineSegment{
            VectorColor = Color.red,
            LineWidth = 0.05f
        };
        ShowReflect = new MyLineSegment{
            VectorColor = Color.red,
            LineWidth = 0.05f
        };
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pn, true);
        sv.DisablePicking(Pon, true);
        sv.DisablePicking(Pl, true);
        sv.DisablePicking(Pr, true);

        #endregion
    }

    // Update is called once per frame
    void Update(){

        Vector3 vb = Pb.transform.localPosition - Pa.transform.localPosition;
        Vector3 vc = Pc.transform.localPosition - Pa.transform.localPosition;

        Vn = -Vector3.Cross(vb, vc);
        Vn.Normalize();
        D = Vector3.Dot(Pa.transform.localPosition, Vn);

        Pn.transform.localPosition = D * Vn;

        // Compute the line segment direction
        Vector3 v1 = P1.transform.localPosition - P0.transform.localPosition;
        if (v1.magnitude < float.Epsilon){
            Debug.Log("Ill defined line (magnitude of zero). Not processed");
            return;
        }

        float denom = Vector3.Dot(Vn, v1);
        bool lineNotParallelPlane = (Mathf.Abs(denom) > float.Epsilon); // Vn is not perpendicular with V1
        float d = 0;

        Pon.SetActive(lineNotParallelPlane);
        if (lineNotParallelPlane){
            d = (D - (Vector3.Dot(Vn, P0.transform.localPosition))) / denom;
            Pon.transform.localPosition = P0.transform.localPosition + d * v1;
            Debug.Log("Interesection pt at:" + Pon + "Distant from P0 d=" + d);
        }
        else{
            Debug.Log("Line is almost parallel to the plane, no intersection!");
        }


        float h = 0;
        Vector3 von, vr;
        Pr.SetActive(lineNotParallelPlane);
        Vector3 vbCrossVn = Vector3.Cross(vb, Vn);
        float length1 = vb.magnitude;
        float length2 = vc.magnitude;
        Vector3 vForBB = Pon.transform.localPosition -  Pa.transform.localPosition;
        float vForBBOnVb = Vector3.Dot(vForBB, vb.normalized);
        float vForBBOnVc = Vector3.Dot(vForBB, vc.normalized);
        bool showReflect = vForBBOnVb >= 0 && vForBBOnVb <= length1 &&
                           vForBBOnVc >= 0 && vForBBOnVc <= length2;
        if (lineNotParallelPlane){
            von = P0.transform.localPosition - Pon.transform.localPosition;
            h = Vector3.Dot(von, Vn);
            vr = 2 * h * Vn - von;
            float p0onVn = Vector3.Dot(P0.transform.localPosition, Vn);
            float p1onVn = Vector3.Dot(P1.transform.localPosition, Vn);
            if ((p0onVn > D) && (p1onVn < D) && showReflect){
                Pr.transform.localPosition = Pon.transform.localPosition + vr;
                Debug.Log("Incoming object position P0:" + P0.transform.localPosition + " Reflected Position Pr:" +
                          Pr.transform.localPosition);
            }
            else{
                Pr.transform.localPosition = Pon.transform.localPosition;
                Debug.Log("P0 is under the Plane or P1 is above Plane");
            }
        }
        else{
            Debug.Log("Line is almost parallel to the plane, no reflection!");
        }

        #region For visualizing the vectors

        AxisFrame.ShowAxisFrame = ShowAxisFrame;

        float offset = 1.5f;
        float size = Mathf.Abs(D) + offset;
        Vector3 from = Vector3.zero;

        if (D < 0){
            from = D * Vn;
            size = Mathf.Abs(D) + offset;
        }

        ShowNormal.VectorAt = from;
        ShowNormal.Direction = Vn;
        ShowNormal.Magnitude = size;

        ShowLine.VectorFromTo(P0.transform.localPosition, P1.transform.localPosition);
        ShowVb.VectorFromTo(Pa.transform.localPosition, Pb.transform.localPosition);
        ShowVc.VectorFromTo(Pa.transform.localPosition, Pc.transform.localPosition);
        ShowRestOfLine.DrawVector = false;
        if (lineNotParallelPlane && ((d < 0f) || (d > 1f))){
            ShowRestOfLine.DrawVector = true;
            if (d < 0f){
                ShowRestOfLine.VectorFromTo(Pon.transform.localPosition, P0.transform.localPosition);
            }
            else{
                ShowRestOfLine.VectorFromTo(Pon.transform.localPosition, P1.transform.localPosition);
            }
        }

        // only update when there is a proper projection
        float s = 2f;
        if (lineNotParallelPlane){
            Vector3 vpn = Pon.transform.localPosition - Pn.transform.localPosition;
            s = vpn.magnitude * 1.2f;
            if (s < 2f)
                s = 2f;
        }
        else{
            Pon.transform.localPosition = Pn.transform.localPosition;
        }

        ShowPlane.PlaneNormal = -Vn;
        ShowPlane.Center = 0.5f * (Pn.transform.localPosition + Pon.transform.localPosition);
        ShowPlane.XSize = ShowPlane.ZSize = s;

        Pon.transform.localRotation = Quaternion.FromToRotation(Vector3.up, Vn);


        ShowReflect.DrawVector = lineNotParallelPlane;
        ShowNormalAtPon.DrawVector = lineNotParallelPlane;
        ShowNormalAtPa.VectorAt = Pa.transform.localPosition;
        ShowNormalAtPa.Direction = Vn;
        ShowNormalAtPa.Magnitude = h + 1f;
        ShowNormalForBBTest.VectorAt = Pa.transform.localPosition;
        ShowNormalForBBTest.Direction = vbCrossVn;
        ShowNormalForBBTest.Magnitude = h + 1f;

        if (ShowReflect.DrawVector){
            ShowReflect.VectorFromTo(Pon.transform.localPosition, Pr.transform.localPosition);
            ShowNormalAtPon.VectorAt = Pon.transform.localPosition;
            ShowNormalAtPon.Direction = Vn;
            ShowNormalAtPon.Magnitude = h + 1f;

            ShowM.VectorFromTo(Pl.transform.localPosition, P0.transform.localPosition);
        }

        #endregion
    }
}
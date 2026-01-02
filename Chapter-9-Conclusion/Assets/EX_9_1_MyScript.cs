using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class EX_9_1_MyScript : MonoBehaviour {
    // Aim System
    public GameObject Pb = null;
    public GameObject Pc = null;
    public float Aspeed = 2.0f; // Agend Speed

    // Agent Support
    public bool MoveAgent = false;
    public float AgentSentInterval = 4f; // Every so many seconds will re-send
    public GameObject Pa = null;
    private Vector3 Adir = Vector3.zero;
    private float AgentSinceTime = 100f; // Keep track on when to send again

    // Hero
    public GameObject Ph = null;
    public bool HeroXMotion = true;
    public bool HeroYMotion = true;
    private Vector3 Vh = Vector3.zero;
    private float HeroSpeed = 0.5f;
    private const float kHeroZMotionRange = 1f;
    private bool hasReflected = false;

    //  Plane
    public bool ShowAxisFrame = false;
    public float D = -6.7f; // The distance to the plane
    public Vector3 Vn; // Normal vector of reflection plane
    public GameObject Pn; // Location where the plane center is

    // Shadow
    public bool CastShadow = true;
    public GameObject Ps; // Location of Shadow of Agent

    // Reflection
    public bool DoReflection = true;
    public GameObject Pon; // Collision point of Agent
    public GameObject Pr; // Reflection of current Agent position

    // Treasure Collision
    public bool CollideTreasure = true;
    public GameObject Pt; // Treasure position
    public float Tr = 2f; // Treasure radius


    public bool ShowDebugLines = true;

    #region For visualization

    // AimSystem
    private MyVector ShowAim;

    MyVector ShowVrN;

    // MyLineSegment ShowProj, ShowToPn, ShowFromPn;
    MyXZPlane ShowReflectionPlane;

    #endregion

    // Start is called before the first frame update
    void Start() {
        Debug.Assert(Pa != null); // Verify proper setting in the editor
        Debug.Assert(Pb != null);
        Debug.Assert(Pc != null);
        Debug.Assert(Pn != null);
        Debug.Assert(Ps != null);
        Debug.Assert(Pon != null);
        Debug.Assert(Pr != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Ph != null);

        #region For visualization

        // To support visualizing the vectors
        ShowAim = new MyVector {
            VectorColor = new Color(1.0f, 0f, 0f, 1.0f)
        };

        ShowVrN = new MyVector {
            VectorColor = Color.black
        };
        ShowReflectionPlane = new MyXZPlane {
            XSize = 2f,
            ZSize = 2f,
            PlaneColor = new Color(0.8f, 1.0f, 0.8f, 1.0f)
        };
        /*
        ShowProj = new MyLineSegment
        {
            VectorColor = Color.black,
            LineWidth = 0.02f
        };
        ShowToPn = new MyLineSegment
        {
            VectorColor = Color.red,
            LineWidth = 0.02f
        };
        ShowFromPn = new MyLineSegment
        {
            VectorColor = Color.red,
            LineWidth = 0.02f
        }; */
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pa, true);
        sv.DisablePicking(Pb, true);
        sv.DisablePicking(Pn, true);
        sv.DisablePicking(Ps, true);
        sv.DisablePicking(Pon, true);
        sv.DisablePicking(Pr, true);
        sv.DisablePicking(Ph, true);

        #endregion
    }

    // Update is called once per frame
    void Update() {
        #region Step 0: Initial error checking

        Debug.Assert((Pc.transform.localPosition - Pb.transform.localPosition).magnitude > float.Epsilon);
        Debug.Assert(Vn.magnitude > float.Epsilon);
        Debug.Assert(Aspeed > float.Epsilon);
        Debug.Assert(Tr > float.Epsilon);
        // recoveries from the errors
        if ((Pc.transform.localPosition - Pb.transform.localPosition).magnitude < float.Epsilon)
            Pc.transform.localPosition = Pb.transform.localPosition - Vector3.forward;
        if (Vn.magnitude < float.Epsilon)
            Vn = Vector3.forward;
        if (Aspeed < float.Epsilon)
            Aspeed = 0.01f;
        if (Tr < float.Epsilon)
            Tr = 0.01f;

        #endregion

        #region Step 1: The Aiming System

        Vector3 aDir = Pc.transform.localPosition - Pb.transform.localPosition;
        aDir.Normalize(); // assuming the two are not located at the same point
        Pc.transform.localPosition = Pb.transform.localPosition + Aspeed * aDir;
        if (!MoveAgent) {
            // only affect the agent if it is not moving
            Pa.transform.localPosition = Pb.transform.localPosition + 2 * Aspeed * aDir;
            Pa.transform.localRotation = Quaternion.LookRotation(aDir, Vector3.up);
            Adir = aDir;
            hasReflected = false;
        }

        #endregion

        #region Step 2: The Agent

        if (MoveAgent) {
            Pa.transform.localPosition += Aspeed * Time.deltaTime * Adir;
            AgentSinceTime += Time.deltaTime;
            if (AgentSinceTime > AgentSentInterval) {
                // Time to re-send the agent from base
                Pa.transform.localPosition = Pc.transform.localPosition;
                Adir = aDir;
                // Adir is the front direction (z)
                // Up is always Vector3.up (0, 1, 0)
                Pa.transform.localRotation = Quaternion.LookRotation(Adir, Vector3.up);
                AgentSinceTime = 0f;
                hasReflected = false;
            }
        }

        if (ShowDebugLines)
            Debug.DrawLine(Pa.transform.localPosition, Pa.transform.localPosition + 20f * Adir, Color.red);

        #endregion

        #region Step 3: The Hero motion

        // Hero follows agent (Pa) axis frame
        Vector3 po = Pa.transform.localPosition;
        Vector3 vx = Pa.transform.right;
        Vector3 vy = Pa.transform.up;
        Vector3 vz = Pa.transform.forward; //

        Vh.z += HeroSpeed * Time.deltaTime; // moved
        if (Mathf.Abs(Vh.z) > kHeroZMotionRange) {
            Vh.z = (Vh.z > 0f) ? 1f : -1f;
            HeroSpeed = -HeroSpeed;
        }

        if (HeroYMotion)
            Vh.y = Mathf.Abs(Mathf.Cos(Mathf.PI * Vh.z));

        if (HeroXMotion)
            Vh.x = Mathf.Sin(Mathf.PI * Vh.z);

        Vector3 vhc = Vh.x * vx + Vh.y * vy + Vh.z * vz;
        Ph.transform.localPosition = po + vhc;
        Ph.transform.localRotation = Pa.transform.localRotation;

        #endregion

        #region Step 4: The Plane and  in-front/parallel checks

        // Plane equation
        Vn.Normalize();
        Pn.transform.localPosition = D * Vn;

        // agent position checks
        float paDotVn = Vector3.Dot(Pa.transform.localPosition, Vn);
        bool infrontOfPlane = (paDotVn > D);

        // Agent motion direction checks
        float aDirDotVn = Vector3.Dot(Adir, Vn);
        bool isApproaching = (aDirDotVn < 0f);
        bool notParallel = (Mathf.Abs(aDirDotVn) > float.Epsilon);

        #endregion

        #region Step 5: The Shadow

        Ps.SetActive(CastShadow && infrontOfPlane);
        if (CastShadow && infrontOfPlane) {
            float h = Vector3.Dot(Pa.transform.localPosition, Vn);
            Ps.transform.localPosition = Pa.transform.localPosition - (h - D) * Vn;

            if (ShowDebugLines)
                Debug.DrawLine(Pa.transform.localPosition, Ps.transform.localPosition, Color.black);
        }

        #endregion

        #region Step 6: The Reflection

        Pon.SetActive(DoReflection && notParallel && infrontOfPlane && isApproaching);
        Pr.SetActive(DoReflection && notParallel && infrontOfPlane && isApproaching);
        Vector3 vr = Vector3.up; // Reflection vector
        bool vrIsValid = false;
        if (DoReflection && notParallel && isApproaching) {
            if (infrontOfPlane) {
                float d = (D - Vector3.Dot(Pa.transform.localPosition, Vn)) / aDirDotVn;
                Pon.transform.localPosition = Pa.transform.localPosition + d * Adir;
                Vector3 von = Pa.transform.localPosition - Pon.transform.localPosition; // von is simply -d*Adir
                Vector3 m = (Vector3.Dot(von, Vn) * Vn) - von;
                vr = 2 * m + von;
                Pr.transform.localPosition = Pon.transform.localPosition + vr;

                Vector3 Pnow = Pa.transform.localPosition;
                Vector3 Pnext = Pnow + Adir * Aspeed * Time.deltaTime;

                bool nowInFront = Vector3.Dot(Pnow, Vn) > D;
                bool nextInFront = Vector3.Dot(Pnext, Vn) > D;

                if (ShowDebugLines) {
                    Debug.DrawLine(Pa.transform.localPosition, Pon.transform.localPosition, Color.red);
                    Debug.DrawLine(Pon.transform.localPosition, Pr.transform.localPosition, Color.red);
                }

                // Vector3 half = Pa.transform.localScale * 0.5f;//half from local
                //
                // float r = Mathf.Abs(Vector3.Dot(Vn,vx) * half.x) + Mathf.Abs(Vector3.Dot(Vn,vy) * half.y) + Mathf.Abs(Vector3.Dot(Vn,vz) * half.z);
                //
                // float dist = Vector3.Dot(Pa.transform.position, Vn) - D;

                // if (dist <= r)
                // {
                //     Pa.transform.position = Pon.transform.position;
                //     Adir = vr.normalized;
                //     // Pa.transform.localRotation = Quaternion.LookRotation(Adir, Vector3.up);
                //     Vector3 forward = Adir.normalized;
                //     Vector3 up = Vector3.up.normalized;
                //     Vector3 right = Vector3.Cross(up, forward).normalized;
                //     up = Vector3.Cross(forward, right).normalized;
                //     Vector4 qZ = QAlignVectors(Vector3.forward, forward);
                //     Vector3 currentUp = QRotation(qZ, Vector3.up);
                //     Vector4 qUp = QAlignVectors(currentUp, up);
                //     Vector4 finalQ = QMultiplication(qUp, qZ);
                //     Pa.transform.localRotation = V4ToQ(finalQ);
                // }

                if (nowInFront && !nextInFront) {
                    Pa.transform.localPosition = Pon.transform.localPosition;
                    Adir = vr.normalized;
                    Pa.transform.localRotation = Quaternion.LookRotation(Adir, Vector3.up);
                    hasReflected = true;
                }

                // if (von.magnitude < float.Epsilon) What will happen if you do this?
                // if (von.magnitude < 0.1f)
                // {
                //     // collision!
                //     Adir = vr.normalized;
                //     Pa.transform.localRotation = Quaternion.LookRotation(Adir, Vector3.up);
                // }
            }
            else {
                Debug.Log("Potential problem!: high speed Agent, missing the plane collision?");
                // What can you do?
            }
        }

        #endregion

        #region Step 7: The collision with treasure

        Pt.SetActive(DoReflection && CollideTreasure);
        Pt.transform.localScale = new Vector3(2 * Tr, 2 * Tr, 2 * Tr); // this is the diameter
        Pt.GetComponent<Renderer>().material.color = MyDrawObject.NoCollisionColor;
        if (DoReflection && CollideTreasure && hasReflected) {
            Vector3 Pnow = Pa.transform.localPosition;
            Vector3 Pnext = Pnow + Adir * Aspeed * Time.deltaTime;
            Vector3 VforMeasure = Pnext - Pnow;
            Vector3 moveDir = VforMeasure.normalized;
            Vector3 half = Pa.transform.localScale * 0.5f; //half from local
            float r = Mathf.Abs(Vector3.Dot(moveDir, vx) * half.x) + Mathf.Abs(Vector3.Dot(moveDir, vy) * half.y) +
                      Mathf.Abs(Vector3.Dot(moveDir, vz) * half.z);
            Vector3 vt = Pt.transform.localPosition - Pnow;
            float dt = Vector3.Dot(vt, moveDir);
            Vector3 pdt;

            if (dt < 0) {
                pdt = Pnow;
            }
            else if (dt > VforMeasure.magnitude) {
                pdt = Pnext;
            }
            else {
                pdt = Pt.transform.localPosition - (Pnow + dt * VforMeasure.normalized);
            }

            float distanceToTreasure = (Pt.transform.localPosition - pdt).magnitude;

            if (distanceToTreasure <= Tr + r)
                Pt.GetComponent<Renderer>().material.color = MyDrawObject.CollisionColor;
        }

        #endregion

        #region For visualization

        AxisFrame.ShowAxisFrame = ShowAxisFrame;
        if (ShowAxisFrame && ShowDebugLines)
            Debug.DrawLine(Vector3.zero, Pn.transform.localPosition, Color.white);

        // Aiming
        ShowAim.VectorFromTo(Pb.transform.localPosition, Pc.transform.localPosition);

        // Refleciton plane
        ShowVrN.VectorAt = Pn.transform.localPosition;
        ShowVrN.Magnitude = 2.0f;
        ShowVrN.Direction = Vn;

        ShowReflectionPlane.Center = Pn.transform.localPosition; //  + new Vector3(-4f, 0f, 0f);
        ShowReflectionPlane.PlaneNormal = Vn;
        ShowReflectionPlane.XSize = ShowReflectionPlane.ZSize = 2f;
        /*
         * Do not adjust the plane size, it is confusing.
        const float S = 3f;
        float s = S;
        if (CastShadow || DoReflection)
        {
            if (CastShadow) {
                s = (Ps.transform.localPosition - Pn.transform.localPosition).magnitude * 0.3f;
                // ShowProj.VectorFromTo(Pa.transform.localPosition, Ps.transform.localPosition);
            }
            if (DoReflection)
            {
                float t = (Pon.transform.localPosition - Pn.transform.localPosition).magnitude * 0.3f;
                if (t > s)
                    s *= 1.02f;
                else if (t < s)
                        s *= 0.98f;
                // ShowToPn.VectorFromTo(Pa.transform.localPosition, Pon.transform.localPosition);
                // ShowFromPn.VectorFromTo(Pon.transform.localPosition, Pr.transform.localPosition);
            }
        }
        ShowReflectionPlane.XSize = ShowReflectionPlane.ZSize = s;
        */

        #endregion
    }

    #region Quaternion functions

    Quaternion V4ToQ(Vector4 q) {
        return new Quaternion(q.x, q.y, q.z, q.w);
    }

    Vector4 QAlignVectors(Vector3 from, Vector3 to) {
        from.Normalize();
        to.Normalize();
        float dot = Mathf.Clamp(Vector3.Dot(from, to), -1f, 1f);
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        Vector4 q = new Vector4(0, 0, 0, 1); // Quaternion identity
        Vector3 axis = Vector3.Cross(from, to);
        q = QFromAngleAxis(theta, axis);
        return q;
    }

    // axis: should be a normalized vector
    // angle: rotation (in degrees)
    Vector4 QFromAngleAxis(float angle, Vector3 axis) {
        float useTheta = angle * Mathf.Deg2Rad * 0.5f;
        float sinTheta = Mathf.Sin(useTheta);
        float cosTheta = Mathf.Cos(useTheta);
        axis.Normalize();
        return new Vector4(sinTheta * axis.x, sinTheta * axis.y, sinTheta * axis.z, cosTheta);
    }

    // Computes quaternion product of q1 q2
    Vector4 QMultiplication(Vector4 q1, Vector4 q2) {
        Vector4 r;
        r.x = q1.w * q2.x + q1.x * q2.w + q1.y * q2.z - q1.z * q2.y;
        r.y = q1.w * q2.y - q1.x * q2.z + q1.y * q2.w + q1.z * q2.x;
        r.z = q1.w * q2.z + q1.x * q2.y - q1.y * q2.x + q1.z * q2.w;
        r.w = q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z;
        return r;
    }

    // Rotate p based on the quaternion q
    Vector3 QRotation(Vector4 qr, Vector3 p) {
        Vector4 pq = new Vector4(p.x, p.y, p.z, 0);
        Vector4 qr_inv = new Vector4(-qr.x, -qr.y, -qr.z, qr.w);
        // q-inv: is rotate by the same axis by -theta OR
        //        =rotate by the -axis by theta
        // in either case: it is the above;

        pq = QMultiplication(qr, pq);
        pq = QMultiplication(pq, qr_inv);
        return new Vector3(pq.x, pq.y, pq.z);
    }

    #endregion
}
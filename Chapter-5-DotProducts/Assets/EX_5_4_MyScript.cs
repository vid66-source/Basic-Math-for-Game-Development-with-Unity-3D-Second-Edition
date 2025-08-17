using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_5_4_MyScript : MonoBehaviour
{
    // Positions: to deine the interval, the test, and projected
    public GameObject P0 = null;  // Position P0
    public GameObject P1 = null;  // Position P1
    public GameObject Pt = null;  // Position for distance computation
    public GameObject Pon = null; // closest point on line
    [SerializeField] private Treasure _treasure = null;
    [SerializeField] private Agent _agent = null;
    [SerializeField] private float _agentSpeed = 1.0f;
    [SerializeField] private float _agentBSRadius = 1.0f;
    [SerializeField] private float _ptBSRadius = 1.0f;

    #region For visualizing the line
    private MyVector ShowV1;
    private MyLineSegment ShowLine, ShowVc;
    private float kScaleFactor = 0.5f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Debug.Assert(P0 != null);   // Verify proper setting in the editor
        Debug.Assert(P1 != null);
        Debug.Assert(Pt != null);
        Debug.Assert(Pon != null);
        _agent.AgentSphereColor = Color.red;
        _agent.transform.position = P0.transform.position;
        _treasure.TreasureSphereColor = Color.blue;
        _treasure.TreasurePosition = Pt.transform.position;

        #region For visualizing the lines
        // To support visualizing the lines
        ShowLine = new MyLineSegment
        {
            VectorColor = MyDrawObject.NoCollisionColor,
            LineWidth = 0.6f
        };
        ShowVc = new MyLineSegment
        {
            VectorColor = Color.black,
            LineWidth = 0.05f
        };
        ShowV1 = new MyVector
        {
            VectorColor = Color.green
        };
        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(Pon, true);
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        _agent.AgentRadius = _agentBSRadius;
        _agent.AgentPosition = _agent.transform.position;
        _treasure.TreasurePosition = _treasure.transform.position;
        _treasure.TreasureRadius = _ptBSRadius;


        float distance = 0; // closest distance
        Vector3 v1 = P1.transform.localPosition - P0.transform.localPosition;
        float v1Len = v1.magnitude;
        _agent.transform.position += v1.normalized * _agentSpeed;
        // _agentVisual.transform.localPosition += v1.normalized * _agentSpeed;
        // if ((_agentVisual.transform.position - P0.transform.position).sqrMagnitude > (P1.transform.position - P0.transform.position).sqrMagnitude)
        //     _agentVisual.transform.position = P0.transform.position;

        if (v1Len > float.Epsilon)
        {
            Vector3 vt = Pt.transform.localPosition - P0.transform.localPosition;
            Vector3 v1n = (1f / v1Len) * v1; // <<-- what is going on here?
            float d = Vector3.Dot(vt, v1n);
            if (d < 0)
            {
                Pon.transform.localPosition = P0.transform.localPosition;
                distance = vt.magnitude;
            }
            else if (d > v1Len)
            {
                Pon.transform.localPosition = P1.transform.localPosition;
                distance = (Pt.transform.localPosition - P1.transform.localPosition).magnitude;
            }
            else
            {
                Pon.transform.localPosition = P0.transform.localPosition + d * v1n;
                Vector3 von = Pon.transform.localPosition - Pt.transform.localPosition;
                distance = von.magnitude;
            }
            float s = distance * kScaleFactor;
            Pon.transform.localScale = new Vector3(s, s, s);
            Debug.Log("v1Len=" + v1Len + " d=" + d + " Distance=" + distance);
        }

        #region  For visualizing the lines
        bool visible = v1Len > float.Epsilon;
        ShowVc.DrawVector = visible;
        ShowLine.DrawVector = visible;
        ShowV1.DrawVector = visible;
        if (v1Len > float.Epsilon)
        {
            Vector3 vt = Pt.transform.localPosition - P0.transform.localPosition;
            Vector3 v1n = (1f / v1Len) * v1; // <<-- what am I doing here?
            float d = Vector3.Dot(v1n, vt);

            ShowLine.VectorFromTo(P0.transform.localPosition, P1.transform.localPosition);
            ShowVc.VectorFromTo(Pt.transform.localPosition, Pon.transform.localPosition);
            float after = 0.45f;
            float before = 0.15f;
            Vector3 pv0 = P0.transform.localPosition - before * v1; ;
            Vector3 pv1 = P1.transform.localPosition + after * v1;

            ShowV1.VectorFromTo(pv0, pv1);
        }
        #endregion

    }

    private void PosManager(GameObject gameObject, SphereVisualizer sphereVisualizer, BoundingSphere boundingSphere, float radius) {
        boundingSphere.position = sphereVisualizer.Center = gameObject.transform.position;
        boundingSphere.radius = sphereVisualizer.Radius = radius;
    }

}



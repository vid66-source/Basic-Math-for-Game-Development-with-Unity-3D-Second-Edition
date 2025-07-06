using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_4_3_MyScript : MonoBehaviour {
    // Drawing control
    public bool DrawVelocity = true;
    public bool BeginExplore = false;

    public bool ShowRedTargetBoundingSphere = false;
    public bool ShowCheckeredExplorerBoundingSphere = false;
    public float RedTargetBoundingSphereRadius = 1f;
    public float CheckeredExplorerBoundingSphereRadius = 1f;

    public GameObject CheckeredExplorer = null; // Support CheckeredExplorer
    public float ExplorerSpeed = 0.05f; // units per second

    public GameObject GreenAgent = null; // Support the GreenAgent
    public float AgentSpeed = 1.0f; // units per second
    // public float AgentDistance = 3.0f; // Distance to explore before returning to base

    public GameObject RedTarget = null; // The RedTarget
    private MyVector ShowVelocity = null; // Visualizing Explorer Velocity

    private const float kSpeedScaleForDrawing = 15f;

    public Color GizmoColor = Color.cyan;


    // Start is called before the first frame update
    void Start() {
        Debug.Assert(CheckeredExplorer != null);
        Debug.Assert(RedTarget != null);
        Debug.Assert(GreenAgent != null);

        ShowVelocity = new MyVector() {
            VectorColor = Color.green
        };

        // initially Agent is resting insdie the Explorer
        GreenAgent.transform.localPosition = CheckeredExplorer.transform.localPosition;

        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(GreenAgent, true);
    }

    // Update is called once per frame
    void Update() {
        Vector3 vET = RedTarget.transform.localPosition - CheckeredExplorer.transform.localPosition;

        ShowVelocity.VectorAt = CheckeredExplorer.transform.localPosition;
        ShowVelocity.Magnitude = ExplorerSpeed * kSpeedScaleForDrawing;
        ShowVelocity.Direction = vET;
        ShowVelocity.DrawVector = DrawVelocity;


        if (BeginExplore) {
            float dToTarget = vET.magnitude; // Distance to target
            if (dToTarget < float.Epsilon)
                return; // Avoid normalizing a zero vector
            Vector3 vETn = vET.normalized;

            #region Process the Explorer (checkered sphere)

            if (!Intersects(CheckeredExplorer, RedTarget, RedTargetBoundingSphereRadius)) {
                Vector3 explorerVelocity = ExplorerSpeed * vETn;
                CheckeredExplorer.transform.localPosition += explorerVelocity * Time.deltaTime;
            }

            #endregion

            #region Process the Agent (small green sphere)

            Vector3 vAT = RedTarget.transform.localPosition - GreenAgent.transform.localPosition;
            Vector3 vAE = CheckeredExplorer.transform.localPosition - GreenAgent.transform.localPosition;
            Vector3 vAEn = vAE.normalized;

            Vector3 agentVelocity = AgentSpeed * vETn; // define velocity
            Vector3 reverseAgentVelocity = AgentSpeed * vAEn; // define velocity
                GreenAgent.transform.localPosition += agentVelocity * Time.deltaTime; // update position

            #endregion
        }
    }

    public bool Intersects(GameObject targetObject, GameObject objectWithBoundingSphere, float radius) {
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 centerPosition = objectWithBoundingSphere.transform.position;
        float sqrDistance = (targetPosition - centerPosition).sqrMagnitude;
        float sqrRadius = radius * radius;
        return sqrDistance <= sqrRadius;
    }

    void OnDrawGizmos() {
        if (ShowRedTargetBoundingSphere)
            DrawBoundingSphere(RedTarget, RedTargetBoundingSphereRadius);

        if (ShowCheckeredExplorerBoundingSphere)
            DrawBoundingSphere(CheckeredExplorer, CheckeredExplorerBoundingSphereRadius);
    }

    void DrawBoundingSphere(GameObject centerObject, float radius) {
        if (centerObject == null)
            return;

        Vector3 center = centerObject.transform.position;
        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(center, radius);
    }
}
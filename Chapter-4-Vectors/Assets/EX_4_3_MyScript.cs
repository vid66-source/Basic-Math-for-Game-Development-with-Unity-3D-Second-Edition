﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_4_3_MyScript : MonoBehaviour {
    // Drawing control
    public bool DrawVelocity = true;
    public bool BeginExplore = false;

    private BoundingSphere _boundingSphere;
    public bool ShowBoundingSphere = false;
    public float BoundingSphereRadius = 1f;

    public GameObject CheckeredExplorer = null; // Support CheckeredExplorer
    public float ExplorerSpeed = 0.05f; // units per second

    public GameObject GreenAgent = null; // Support the GreenAgent
    public float AgentSpeed = 1.0f; // units per second
    public float AgentDistance = 3.0f; // Distance to explore before returning to base

    public GameObject RedTarget = null; // The RedTarget
    private MyVector ShowVelocity = null; // Visualizing Explorer Velocity

    private const float kSpeedScaleForDrawing = 15f;


    // Start is called before the first frame update
    void Start() {
        Debug.Assert(CheckeredExplorer != null);
        Debug.Assert(RedTarget != null);
        Debug.Assert(GreenAgent != null);

        ShowVelocity = new MyVector() {
            VectorColor = Color.green
        };
        _boundingSphere = new BoundingSphere(RedTarget, CheckeredExplorer);
        _boundingSphere.Radius = BoundingSphereRadius;
        _boundingSphere.ShowGizmo = ShowBoundingSphere;

        // initially Agent is resting insdie the Explorer
        GreenAgent.transform.localPosition = CheckeredExplorer.transform.localPosition;

        var sv = UnityEditor.SceneVisibilityManager.instance;
        sv.DisablePicking(GreenAgent, true);
    }

    // Update is called once per frame
    void Update() {
        _boundingSphere.Radius = BoundingSphereRadius;
        _boundingSphere.ShowGizmo = ShowBoundingSphere;

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

            if (!_boundingSphere.Intersects(CheckeredExplorer)) {
                Vector3 explorerVelocity = ExplorerSpeed * vETn;
                CheckeredExplorer.transform.localPosition += explorerVelocity * Time.deltaTime;
            }

            #endregion

            #region Process the Agent (small green sphere)

            Vector3 agentVelocity = AgentSpeed * vETn; // define velocity
            GreenAgent.transform.localPosition += agentVelocity * Time.deltaTime; // update position
            Vector3 vEA = GreenAgent.transform.localPosition - CheckeredExplorer.transform.localPosition;
            if (_boundingSphere.Intersects(GreenAgent))
                GreenAgent.transform.localPosition = CheckeredExplorer.transform.localPosition;

            #endregion
        }
    }

    void OnDrawGizmos() {
        if (_boundingSphere != null) {
            _boundingSphere.DrawSphere();
        }
    }
}
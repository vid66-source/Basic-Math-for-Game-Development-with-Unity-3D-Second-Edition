using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_4_1_MyScript : MonoBehaviour{
    // For visualizing the two vectors
    public bool DrawAxisFrame = true; // Draw or Hide The AxisFrame
    public bool DrawVectorPoints = true;
    public bool DrawVectors = true;
    public bool CreateWithPositionVector = false;
    private bool lastPickingState = false;

    private MyVector ShowVd; // From Origin to Pd
    private MyVector ShowVdAtP1; // Show Vd at P1

    // Support position Pd as a vector from P1 to P2
    public GameObject P1 = null; // Position P1
    public GameObject P2 = null; // Position P2
    public GameObject Pd = null; // Position vector: Pd


    // Start is called before the first frame update
    void Start(){
        Debug.Assert(P1 != null); // Verify proper setting in the editor
        Debug.Assert(P2 != null);
        Debug.Assert(Pd != null);


        // To support visualizing the vectors
        ShowVd = new MyVector{
            VectorColor = Color.black,
            VectorAt = Vector3.zero // Always draw Vd from the origin
        };
        ShowVdAtP1 = new MyVector{
            VectorColor = new Color(0.9f, 0.9f, 0.9f)
        };
    }

    // Update is called once per frame
    void Update(){
        #region Visualization on/off: show or hide to avoid cluttering

        var sv = UnityEditor.SceneVisibilityManager.instance;

        if (lastPickingState != CreateWithPositionVector)
        {
            sv.DisablePicking(P2, CreateWithPositionVector);
            lastPickingState = CreateWithPositionVector;
        }
        AxisFrame.ShowAxisFrame = DrawAxisFrame; // Draw or Hide Axis Frame
        P1.SetActive(DrawVectorPoints); // objects that support position as vector
        P2.SetActive(DrawVectorPoints);
        Pd.SetActive(DrawVectorPoints);

        ShowVdAtP1.DrawVector = DrawVectors; // Display or hide the vectors
        ShowVd.DrawVector = DrawVectors;

        #endregion


        #region Enable manipulating Pd to create the vector and show the vector at P1

        if (CreateWithPositionVector){
            // Use position of Pd as position vector
            Vector3 vectorVd = Pd.transform.localPosition;

            // Step 1: take care of visualization
            //         for Vd
            ShowVd.Direction = vectorVd;
            ShowVd.Magnitude = vectorVd.magnitude;

            //         apply Vd at P1
            ShowVdAtP1.VectorAt = P1.transform.localPosition; // Always draw at P1
            ShowVdAtP1.Magnitude = vectorVd.magnitude; // get from vectorVd
            ShowVdAtP1.Direction = vectorVd;

            // Step 2: demonstrate P2 is indeed Vd away from P1
            P2.transform.localPosition = P1.transform.localPosition + vectorVd;
        }

        if (!CreateWithPositionVector){
            Vector3 vectorVd = P2.transform.localPosition - P1.transform.localPosition;

            // Step 1: Take care of visualization
            //         for Ve: from Pi to Pj
            ShowVdAtP1.VectorFromTo(P1.transform.localPosition, P2.transform.localPosition);
            //         Show as Ve at the origin
            ShowVd.Direction = vectorVd;
            ShowVd.Magnitude = vectorVd.magnitude;

            // Step 2: demonstrate Pe is indeed Ve away from the origin
            Pd.transform.localPosition = vectorVd;
        }

        #endregion
    }
}
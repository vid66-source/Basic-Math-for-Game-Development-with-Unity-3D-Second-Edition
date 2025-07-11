﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EX_4_4_MyScript : MonoBehaviour{
    public GameObject P0, P1, P2, P3; // V1=P1-P0 and V2=P2-p1

    private MyVector ShowV1atP0,
        ShowV2atV1, // Show V1 at P0 and V2 at head of V1
        ShowV2atP0, ShowV1atV2, // Show V2 at P0 and V1 at head of V2
        ShowV3atV2, // Show V3 at head of V2
        ShowSumV12, ShowSumV21, // V1+V2, and V2+V1
        ShowSumV123, // V1+V2+V3
        ShowSubV12,
        ShowSubV1V2AndV3, ShowSubV1AndV2V3, // V1-V2
        ShowNegV2,  // -V2
        ShowSumV1V2AndV3, ShowSumV1AndV2V3;

    private MyVector PosV1, PosV2, PosV3, PosSum, PosSumV123, PosSub, PosNegV2; // Show as position vectors

    public bool DrawAxisFrame = true;
    public bool DrawV12 = false, DrawV21 = false, DrawV32 = false;
    public bool DrawSum = false, DrawSum123 = false, DrawSumV1V2AndV3, DrawSumV1AndV2V3;
    public bool DrawSub = false, DrawSubV1V2AndV3, DrawSubV1AndV2V3, DrawNegV2 = false;
    public bool DrawPosVec = false;

    // Start is called before the first frame update
    void Start(){
        Debug.Assert(P0 != null);
        Debug.Assert(P1 != null);
        Debug.Assert(P2 != null);
        Debug.Assert(P3 != null);

        ShowV1atP0 = new MyVector(){
            VectorColor = Color.red
        };
        ShowV1atV2 = new MyVector(){
            VectorColor = Color.red
        };
        PosV1 = new MyVector(){
            VectorAt = Vector3.zero, // always show at the origin
            VectorColor = Color.red
        };

        ShowV2atP0 = new MyVector(){
            VectorColor = Color.blue
        };
        ShowV2atV1 = new MyVector(){
            VectorColor = Color.blue
        };
        PosV2 = new MyVector(){
            VectorAt = Vector3.zero,
            VectorColor = Color.blue
        };

        ShowV3atV2 = new MyVector(){
            VectorColor = Color.magenta
        };
        PosV3 = new MyVector(){
            VectorAt = Vector3.zero,
            VectorColor = Color.magenta
        };

        ShowSumV12 = new MyVector(){
            VectorColor = Color.green
        };
        ShowSumV21 = new MyVector(){
            VectorColor = Color.green
        };
        PosSum = new MyVector(){
            VectorAt = Vector3.zero,
            VectorColor = Color.green
        };

        ShowSumV123 = new MyVector(){
            VectorColor = Color.cyan
        };
        PosSumV123 = new MyVector(){
            VectorAt = Vector3.zero,
            VectorColor = Color.cyan
        };

        ShowSubV12 = new MyVector(){
            VectorColor = Color.gray
        };
        PosSub = new MyVector(){
            VectorAt = Vector3.zero,
            VectorColor = Color.gray
        };

        ShowSumV1V2AndV3 = new MyVector(){
            VectorColor = new Color(1f, 0.5f, 0f, 1f)
        };
        ShowSumV1AndV2V3 = new MyVector(){
            VectorColor = new Color(0.5f, 0f, 1f, 1f)
        };

        ShowSubV1V2AndV3 = new MyVector(){
            VectorColor = new Color(0f, 0.8f, 0.8f, 1f)
        };
        ShowSubV1AndV2V3 = new MyVector(){
            VectorColor = new Color(0.8f, 0f, 0.4f, 1f)
        };

        ShowNegV2 = new MyVector(){
            VectorColor = new Color(0.9f, 0.9f, 0.2f, 1.0f)
        };
        PosNegV2 = new MyVector(){
            VectorAt = Vector3.zero,
            VectorColor = new Color(0.9f, 0.9f, 0.2f, 1.0f)
        };
    }

    // Update is called once per frame
    void Update(){
        Vector3 V1 = P1.transform.localPosition - P0.transform.localPosition;
        Vector3 V2 = P2.transform.localPosition - P1.transform.localPosition;
        Vector3 V3 = P3.transform.localPosition - P2.transform.localPosition;
        Vector3 sumV12 = V1 + V2;
        Vector3 sumV21 = V2 + V1;
        Vector3 sumV123 = V3 + V2 + V1;
        Vector3 negV2 = -V2;
        Vector3 subV12 = V1 + negV2;
        Vector3 sumV1AndV2V3 = V1 + (V2 + V3);
        Vector3 sumV1V2AndV3 = (V1 + V2) + V3;
        Vector3 subV1V2AndV3 = (V1 - V2) - V3;
        Vector3 subV1AndV2V3 = V1 - (V2 - V3);

        #region Draw control: switch on/off what to show

        AxisFrame.ShowAxisFrame = DrawAxisFrame; // Draw or Hide Axis Frame
        ShowV1atP0.DrawVector = DrawV12;
        ShowV2atV1.DrawVector = DrawV12;
        ShowSumV12.DrawVector = DrawSum;
        ShowV2atP0.DrawVector = DrawV21;
        ShowV1atV2.DrawVector = DrawV21;
        ShowSumV21.DrawVector = DrawSum;
        ShowSubV12.DrawVector = DrawSub;
        ShowNegV2.DrawVector = DrawNegV2;
        ShowV3atV2.DrawVector = DrawV32;
        ShowSumV123.DrawVector = DrawSum123;
        PosV3.DrawVector = DrawPosVec && DrawV32;
        PosSumV123.DrawVector = DrawPosVec && DrawSum123;
        PosV1.DrawVector = DrawPosVec && (DrawV12 || DrawV21);
        PosV2.DrawVector = DrawPosVec && (DrawV12 || DrawV21);
        PosSum.DrawVector = DrawPosVec && DrawSum;
        PosSub.DrawVector = DrawPosVec && DrawSub;
        PosNegV2.DrawVector = DrawPosVec && DrawNegV2;
        ShowSubV1AndV2V3.DrawVector = DrawSubV1AndV2V3;
        ShowSubV1V2AndV3.DrawVector = DrawSubV1V2AndV3;
        ShowSumV1AndV2V3.DrawVector = DrawSumV1AndV2V3;
        ShowSumV1V2AndV3.DrawVector = DrawSumV1V2AndV3;

        #endregion

        #region V1: Show V1 at P0 and head of V2

        ShowV1atP0.VectorAt = P0.transform.localPosition;
        ShowV1atP0.Direction = V1;
        ShowV1atP0.Magnitude = V1.magnitude;

        ShowV1atV2.VectorAt = P0.transform.localPosition + V2;
        ShowV1atV2.Direction = V1;
        ShowV1atV2.Magnitude = V1.magnitude;

        PosV1.Direction = V1;
        PosV1.Magnitude = V1.magnitude;

        #endregion

        #region V2: show V2 at P0 and head of V1

        ShowV2atP0.VectorAt = P0.transform.localPosition;
        ShowV2atP0.Direction = V2;
        ShowV2atP0.Magnitude = V2.magnitude;

        ShowV2atV1.VectorAt = P0.transform.localPosition + V1;
        ShowV2atV1.Direction = V2;
        ShowV2atV1.Magnitude = V2.magnitude;

        PosV2.Direction = V2;
        PosV2.Magnitude = V2.magnitude;

        #endregion

        #region V3: show V3 at head of V2

        ShowV3atV2.VectorAt = P0.transform.localPosition + V2 + V1;
        ShowV3atV2.Direction = V3;
        ShowV3atV2.Magnitude = V3.magnitude;

        PosV3.Direction = V3;
        PosV3.Magnitude = V3.magnitude;

        #endregion

        #region Sum: show V1+V2 and V2+V1

        ShowSumV12.VectorAt = P0.transform.localPosition;
        ShowSumV12.Direction = sumV12;
        ShowSumV12.Magnitude = sumV12.magnitude;

        ShowSumV21.VectorAt = P0.transform.localPosition;
        ShowSumV21.Direction = sumV21;
        ShowSumV21.Magnitude = sumV21.magnitude;

        PosSum.Direction = sumV12;
        PosSum.Magnitude = sumV12.magnitude;

        #endregion

        #region Sum: show V1+V2+V3

        ShowSumV123.VectorAt = P0.transform.localPosition;
        ShowSumV123.Direction = sumV123;
        ShowSumV123.Magnitude = sumV123.magnitude;

        PosSumV123.Direction = sumV123;
        PosSumV123.Magnitude = sumV123.magnitude;

        #endregion

        #region Sum: show (V1+V2)+V3

        ShowSumV1V2AndV3.VectorAt = P0.transform.localPosition;
        ShowSumV1V2AndV3.Direction = sumV1V2AndV3;
        ShowSumV1V2AndV3.Magnitude = sumV1V2AndV3.magnitude;

        #endregion

        #region Sum: show V1+(V2+V3)

        ShowSumV1AndV2V3.VectorAt = P0.transform.localPosition;
        ShowSumV1AndV2V3.Direction = sumV1AndV2V3;
        ShowSumV1AndV2V3.Magnitude = sumV1AndV2V3.magnitude;

        #endregion

        #region Sum: show V1-(V2-V3)

        ShowSubV1AndV2V3.VectorAt = P0.transform.localPosition;
        ShowSubV1AndV2V3.Direction = subV1AndV2V3;
        ShowSubV1AndV2V3.Magnitude = subV1AndV2V3.magnitude;

        #endregion

        #region Sum: show (V1-V2)-V3

        ShowSubV1V2AndV3.VectorAt = P0.transform.localPosition;
        ShowSubV1V2AndV3.Direction = subV1V2AndV3;
        ShowSubV1V2AndV3.Magnitude = subV1V2AndV3.magnitude;

        #endregion

        #region Sub: show V1-V2

        ShowSubV12.VectorAt = P0.transform.localPosition;
        ShowSubV12.Direction = subV12;
        ShowSubV12.Magnitude = subV12.magnitude;

        PosSub.Direction = subV12;
        PosSub.Magnitude = subV12.magnitude;

        #endregion

        #region Negative vectors: -V2

        ShowNegV2.VectorAt = ShowV2atV1.VectorAt;
        ShowNegV2.Direction = negV2;
        ShowNegV2.Magnitude = negV2.magnitude;

        PosNegV2.Direction = negV2;
        PosNegV2.Magnitude = negV2.magnitude;

        #endregion
    }
}
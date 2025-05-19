using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EX_3_1_MyScript : MonoBehaviour
{
    #region Public Variables
    public GameObject Checker = null;         // The spheres to work with
    public GameObject Stripe = null;
    public GameObject AnotherPoint = null;


    public Vector3 CheckerPosition = Vector3.zero;
    public Vector3 StripePosition = Vector3.zero;
    public Vector3 AnotherPointPosition = Vector3.zero;

    public float DistanceBetween = 0.0f;
    public float MagnitudeOfVector = 0.0f;
    public float DistanceBetweenCA = 0.0f;
    public float MagnitudeOfVectorCA = 0.0f;
    public float DistanceBetweenSA = 0.0f;
    public float MagnitudeOfVectorSA = 0.0f;
    #endregion
    private Color myColor = new Color(5, 50, 18, 100);

    // Start is called before the first frame update
    void Start()
    {
		Debug.Assert(Checker!= null);	// Make sure proper editor setup
        Debug.Assert(Stripe != null);   // Make sure proper editor setup
        Debug.Assert(AnotherPoint != null);   // Make sure proper editor setup
    }

    // Update is called once per frame
    void Update()
    {
        // Update the sphere positions
        Checker.transform.localPosition = CheckerPosition;
        Stripe.transform.localPosition = StripePosition;
        AnotherPoint.transform.localPosition = AnotherPointPosition;

        // Apply Pythagorean Theorem to compute distance
        float dx = StripePosition.x - CheckerPosition.x;
        float dy = StripePosition.y - CheckerPosition.y;
        float dz = StripePosition.z - CheckerPosition.z;
        float dxCA = AnotherPointPosition.x - CheckerPosition.x;
        float dyCA = AnotherPointPosition.y - CheckerPosition.y;
        float dzCA = AnotherPointPosition.z - CheckerPosition.z;
        float dxSA = AnotherPointPosition.x - StripePosition.x;
        float dySA = AnotherPointPosition.y - StripePosition.y;
        float dzSA = AnotherPointPosition.z - StripePosition.z;

        DistanceBetween = Mathf.Sqrt(dx*dx + dy*dy + dz*dz);
        DistanceBetweenCA = Mathf.Sqrt(dxCA*dxCA + dyCA*dyCA + dzCA*dzCA);
        DistanceBetweenSA = Mathf.Sqrt(dxSA*dxSA + dySA*dySA + dzSA*dzSA);

        // Compute the magnitude of a Vector3
        Vector3 diff = StripePosition - CheckerPosition;
        Vector3 diffCA = AnotherPointPosition - CheckerPosition;
        Vector3 diffSA = AnotherPointPosition - StripePosition;
        MagnitudeOfVector = diff.magnitude;
        MagnitudeOfVectorCA = diffCA.magnitude;
        MagnitudeOfVectorSA = diffSA.magnitude;

        #region Display the dx, dy, and dz
        Vector3 posB = CheckerPosition + new Vector3(dx, 0f, 0f);  // Position B of Figure 3.1
        Vector3 posC = posB + new Vector3(0f, dy, 0f);             // Position C of Figure 3.1
        Vector3 posD = posC + new Vector3(0f, 0f, dz);             // Position D of Figure 3.1
        Debug.DrawLine(CheckerPosition, posB, Color.red);          // Connect the positions with
        Debug.DrawLine(posB, posC, Color.green);                   // Color lines
        Debug.DrawLine(posC, posD, Color.blue);
        Debug.DrawLine(CheckerPosition, posD, Color.black);

        // Vector3 posBCA = CheckerPosition + new Vector3(dxCA, 0f, 0f);  // Position B of Figure 3.1
        // Vector3 posCCA = posBCA + new Vector3(0f, dyCA, 0f);             // Position C of Figure 3.1
        // Vector3 posDCA = posCCA + new Vector3(0f, 0f, dzCA);             // Position D of Figure 3.1
        // Debug.DrawLine(CheckerPosition, posBCA, Color.white);          // Connect the positions with
        // Debug.DrawLine(posBCA, posCCA, Color.yellow);                   // Color lines
        // Debug.DrawLine(posCCA, posDCA, Color.magenta);
        // Debug.DrawLine(CheckerPosition, posDCA, Color.black);
        //
        // Vector3 posBSA = StripePosition + new Vector3(dxSA, 0f, 0f);  // Position B of Figure 3.1
        // Vector3 posCSA = posBSA + new Vector3(0f, dySA, 0f);             // Position C of Figure 3.1
        // Vector3 posDSA = posCSA + new Vector3(0f, 0f, dzSA);             // Position D of Figure 3.1
        // Debug.DrawLine(StripePosition, posBSA, Color.cyan);          // Connect the positions with
        // Debug.DrawLine(posBSA, posCSA, Color.grey);                   // Color lines
        // Debug.DrawLine(posCSA, posDSA, myColor);
        // Debug.DrawLine(StripePosition, posDSA, Color.black);
        #endregion
    }
}

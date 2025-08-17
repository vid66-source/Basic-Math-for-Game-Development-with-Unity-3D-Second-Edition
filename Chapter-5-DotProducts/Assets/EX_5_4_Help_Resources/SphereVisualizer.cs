using UnityEngine;

public class SphereVisualizer : MonoBehaviour {
    private Vector3 _center;
    private float _radius;
    private Color _color;

    public Vector3 Center {
        get { return _center; }
        set { _center = value; }
    } // центр сфери

    public float Radius {
        get { return _radius; }
        set { _radius = value; }
    } // радіус сфери

    public Color Color {
        get { return _color; }
        set { _color = value; }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color; // колір сфери
        Gizmos.DrawWireSphere(Center, Radius); // намалювати каркас сфери
    }
}
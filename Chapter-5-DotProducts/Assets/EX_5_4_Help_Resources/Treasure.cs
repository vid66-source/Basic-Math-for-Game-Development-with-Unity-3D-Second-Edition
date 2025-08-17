using UnityEngine;

public class Treasure : MonoBehaviour {
    [SerializeField] private SphereVisualizer _treasureSphereVisualizer = null;
    private BoundingSphere _treasureBoundingSphere = new BoundingSphere();
    private float _treasureRadius;
    private Vector3 _treasurePosition;
    private Color _treasureSphereColor;

    public Vector3 TreasurePosition {
        get { return transform.position; }
        set {
            _treasurePosition = value;
             transform.position = _treasurePosition;
            _treasureSphereVisualizer.Center = _treasureBoundingSphere.position = _treasurePosition;
        }
    }

    public Color TreasureSphereColor {
        get { return _treasureSphereColor; }
        set {
            _treasureSphereColor = value;
            _treasureSphereVisualizer.Color = _treasureSphereColor;
        }
    }

    public float TreasureRadius {
        get { return _treasureRadius; }
        set {
            _treasureRadius = value;
            _treasureBoundingSphere.radius = _treasureSphereVisualizer.Radius = _treasureRadius;
        }
    }
}
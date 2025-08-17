using UnityEngine;

public class Agent : MonoBehaviour {
    [SerializeField] private GameObject _agentPrefab = null;
    [SerializeField] private SphereVisualizer _agentSphereVisualizer = null;
    private BoundingSphere _agentBoundingSphere = new BoundingSphere();
    private float _agentRadius;
    private Color _agentSphereColor;

    public float AgentRadius {
        get { return _agentRadius; }
        set {
            _agentRadius = value;
            _agentSphereVisualizer.Radius = _agentBoundingSphere.radius = _agentRadius;
        }
    }

    public Color AgentSphereColor {
        get { return _agentSphereColor; }

        set {
            _agentSphereColor = value;
            _agentSphereVisualizer.Color = _agentSphereColor;
        }
    }

    public Vector3 AgentPosition {
        get { return transform.position; }
        set {
            transform.position = value;
            _agentSphereVisualizer.Center = _agentBoundingSphere.position = transform.position;
        }
    }
}
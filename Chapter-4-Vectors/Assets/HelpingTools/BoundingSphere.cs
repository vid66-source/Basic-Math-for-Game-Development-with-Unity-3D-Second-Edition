using UnityEngine;

public class BoundingSphere {
    public Color GizmoColor = Color.cyan;
    public bool ShowGizmo = true;
    public float Radius = 0f;
    public Vector3 Center = Vector3.zero;
    public Vector3 ObjectToInteractCenter = Vector3.zero;
    public GameObject ShpereCenter = null;
    public GameObject ObjectToInteractWith = null;

    public BoundingSphere(GameObject centerObject, GameObject objectToInteract) {
        ShpereCenter = centerObject;
        ObjectToInteractWith = objectToInteract;
        ShowGizmo = false;
        Center = ShpereCenter.transform.position;
    }

    public void DrawSphere() {
        if (!ShowGizmo)
            return;
        Center = ShpereCenter.transform.position;
        Gizmos.color = GizmoColor;
        Gizmos.DrawWireSphere(Center, Radius);
    }

    public bool Intersects(GameObject targetObject) {
        Vector3 targetPosition = targetObject.transform.position;
        Vector3 centerPosition = ShpereCenter.transform.position;
        float sqrDistance = (targetPosition - centerPosition).sqrMagnitude;
        float sqrRadius = Radius * Radius;
        return sqrDistance <= sqrRadius;
    }
}
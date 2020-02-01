using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Mouse : MonoBehaviour
{

    public LayerMask layerMask;

    [ReadOnly] public BuildableObject target;

    private TargetJoint2D joint;
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition).SetZ(0);
        if (joint) { joint.target = transform.position; }

        if (Input.GetMouseButtonDown(0)) { Pickup(); }
        else if (Input.GetMouseButtonUp(0)) { Drop(); }
    }

    private void Pickup()
    {
        if (joint) { Drop(); }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, layerMask);

        if (!hit) { return; }

        target = hit.collider.gameObject.GetComponent<BuildableObject>();

        if (!target) { return; }

        target.Pickup();

        joint = target.gameObject.AddComponent<TargetJoint2D>();
        joint.connectedBody = rb2d;
        // joint.distance = 0.05f;
        joint.autoConfigureTarget = false;
        joint.anchor = target.transform.InverseTransformPoint(transform.position);
    }

    private void Drop()
    {
        if (target)
        {
            Destroy(joint);
            target.Drop();
            target = null;
        }
    }

}
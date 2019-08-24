using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour
{
    public Vector3 dir;
    public bool finished = false;
    public Walker parent;
    public int chainPos = 0;
    public int chainLength = 0;

    Rigidbody rb;
    public MeshRenderer meshrenderer;
    public LineRenderer line;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        meshrenderer = GetComponent<MeshRenderer>();
        line = GetComponent<LineRenderer>();
    }

    public void Init(bool isRoot)
    {
        finished = false;
        transform.position = isRoot ? Vector3.zero : Random.insideUnitSphere.normalized * DLA.instance.SpawnSphereSize;
        if (DLA.instance.hideProcess) meshrenderer.enabled = false;
        if (isRoot) Stick(gameObject);
    }
    public void Init()
    {
        Init(false);
    }

    private void Stick(GameObject collider)
    {
        if (!finished)
        {
            rb.isKinematic = true;
            rb.velocity = Vector3.zero;
            finished = true;
            gameObject.layer = 9;
            meshrenderer.enabled = true;
            meshrenderer.material = DLA.instance.clusterMat;
            parent = collider.GetComponent<Walker>();
            line.widthMultiplier = 0.01f;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, parent.transform.position);
            if (transform.position != Vector3.zero)
            {
                chainPos = chainLength = parent.chainPos + 1;
                UpdateParentChain();
            }
            DLA.instance.FinishWalk(this);

        }
    }

    //recursively update the max chain length in everyhting
    private void UpdateParentChain()
    {
        parent.chainLength = chainPos;
        if (parent.chainPos != 0) parent.UpdateParentChain();
    }

    private void FixedUpdate()
    {
        if (!finished)
        {
            dir = Random.insideUnitSphere.normalized;
            Vector3 bias = (Vector3.zero - transform.position).normalized * DLA.instance.biasTowardCenter;
            dir += bias;
            if (transform.position.y > (DLA.instance.biasHorizontal / DLA.instance.SpawnSphereSize)) dir.y -= DLA.instance.biasHorizontal;
            else if (transform.position.y < -(DLA.instance.biasHorizontal / DLA.instance.SpawnSphereSize)) dir.y += DLA.instance.biasHorizontal;
            rb.velocity = dir * DLA.instance.WalkSpeed;
            //rb.MovePosition(dir); // * DLA.instance.WalkSpeed);
            if (Vector3.Distance(transform.position, Vector3.zero) > DLA.instance.SpawnSphereSize + 1) Init();
        }    
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 9) Stick(collision.gameObject);
    }
}

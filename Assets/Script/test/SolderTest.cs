using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolderTest : MonoBehaviour
{
    Rigidbody body;
    CapsuleCollider _collider;

    Vector3 movePos;
    bool hasTarget = false;

    public int camp;

    Vector3 porce = Vector3.zero;
    Vector3 continueForce = Vector3.zero;
    int continueTimes = 0;

    void Start()
    {
        body = GetComponent<Rigidbody>();

        _collider = GetComponent<CapsuleCollider>();

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveTo(Vector3 movePos)
    {
        this.movePos = movePos;
        hasTarget = true;
    }

    private void FixedUpdate()
    {
        if(hasTarget)
        {
            Vector3 dis = (movePos - transform.localPosition);
            porce += dis;
        }

        continueTimes--;
        if(continueTimes <= 0)
        {
            ClearContinueForce();
        }
        porce += continueForce;

        float force = porce.magnitude;
        force = Mathf.Min(force, 30);

        Vector3 addForce = porce.normalized * force;
        body.AddForce(addForce);
        float vel = body.velocity.magnitude;
        vel = Mathf.Min(vel, 10);

        body.velocity = body.velocity.normalized * vel;

        porce.Set(0, 0, 0);
    }

    public void AddForce(Vector3 force)
    {
        //body.AddForce(force);
        porce += force;
    }

    
    public void AddContinueForce(Vector3 force, int times)
    {
        continueForce += force;
        continueTimes = times;
    }

    public void ClearContinueForce()
    {
        continueForce.Set(0, 0, 0);
    }

    // 碰撞开始
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.contactCount);
        Debug.Log(collision.collider.name);

        ContactPoint contactPoint = collision.GetContact(0);

        SolderTest other = collision.collider.GetComponent<SolderTest>();

        if (other != null)
        {
            Debug.Log("lalalalal");
            Vector3 v = contactPoint.point - transform.localPosition;

            if (camp == other.camp)
            {
                Debug.Log("xxxxxxxx");
                Debug.Log(v);
                //collision.collider.material.bounciness = 0f;

                float x = v.x;
                v.x = v.z;
                v.z = -x;

                Debug.Log(v);

                AddContinueForce(v.normalized * 15, 30);
            }
            else
            {
            }
        }
    }

    // 碰撞结束
    void OnCollisionExit(Collision collision)
    {
        SolderTest other = collision.collider.GetComponent<SolderTest>();

        if (other != null)
        {
            if (camp == other.camp)
            {
                // ClearContinueForce();
            }
            else
            {
                
            }
        }

    }

    // 碰撞持续中
    void OnCollisionStay(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);

        SolderTest other = collision.collider.GetComponent<SolderTest>();

        if (other != null)
        {
            Vector3 v = contactPoint.point - transform.localPosition;

            if (camp == other.camp)
            {
                float x = v.x;
                v.x = v.z;
                v.z = -v.x;

                // AddForce(v.normalized * 0);
            }
            else
            {
                AddForce(v.normalized * 8);
            }
        }
    }
}

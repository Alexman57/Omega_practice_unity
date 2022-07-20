using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    RoadGenerator rg;
    Rigidbody rb;
    Animator animator;
    Coroutine movingCoroutine;

    Vector3 startGamePosition;
    Quaternion startGameRotation;

    bool isMoving = false;
    bool isJumping = false;

    public float laneChangeSpeed = 15;
    public float jumpPower = 15;
    public float jumpGravity = -40;

    float laneOffest = 2.5f;
    float realGravity = -9.8f;
    float pointStart;
    float pointFinish;
    float lastVectorX;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        startGamePosition = transform.position;
        startGameRotation = transform.rotation;
        SwipeManager.instance.MoveEvent += MovePlayer;
    }

    void MovePlayer(bool[] swipes)
    {
        if (swipes[(int)SwipeManager.Direction.Left] && pointFinish > -laneOffest)
        {
            MoveHorizontaly(-laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Right] && pointFinish < laneOffest)
        {
            MoveHorizontaly(laneChangeSpeed);
        }
        if (swipes[(int)SwipeManager.Direction.Up] && isJumping == false) {
            Jump();
        }
    }

    void Jump() 
    {
        isJumping = true;
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        Physics.gravity = new Vector3(0, jumpGravity, 0);
        StartCoroutine(StopJumpCoroutine());

    }

    IEnumerator StopJumpCoroutine() {
        do
        {
            yield return new WaitForSeconds(0.02f);
        } while (rb.velocity.y != 0);
        isJumping = false;
        Physics.gravity = new Vector3(0, realGravity, 0);
    }

    void MoveHorizontaly(float speed)
    {
        pointStart = -pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffest;
        if (isMoving) { StopCoroutine(movingCoroutine); isMoving = false; }
        movingCoroutine = StartCoroutine(MoveCoroutine(speed));
    }


    IEnumerator MoveCoroutine(float vectorX)
    {
        isMoving = true;
        while (Mathf.Abs(pointStart - transform.position.x) < laneOffest)
        {
            yield return new WaitForFixedUpdate();
            rb.velocity = new Vector3(vectorX, rb.velocity.y, 0);
            lastVectorX = vectorX;
            float x = Mathf.Clamp(transform.position.x, Mathf.Min(pointStart, pointFinish), Mathf.Max(pointStart, pointFinish));
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
        rb.velocity = Vector3.zero;
        transform.position = new Vector3(pointFinish, transform.position.y, transform.position.z);
        if (transform.position.y > 1){
            rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
        }
        isMoving = false;
    }
    
    public void StartGame() { animator.SetTrigger("Run"); }

    public void StartLevel()
    {
        animator.applyRootMotion = false;
        rg = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        rg.StartLevel();
    }

    public void ResetGame()
    {
        rb.velocity = Vector3.zero;
        pointStart = 0;
        pointFinish = 0;
        animator.applyRootMotion = true;
        animator.SetTrigger("Idle");
        transform.position = startGamePosition;
        transform.rotation = startGameRotation;
        rg = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        rg.ResetLevel();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ramp")
        {
            rb.constraints |= RigidbodyConstraints.FreezePositionZ;
        }
        if (other.gameObject.tag == "Lose")
        {
            ResetGame();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ramp")
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
        if (collision.gameObject.tag == "NotLose")
        {
            MoveHorizontaly(-lastVectorX);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == " RampPlane")
        {
            if(rb.velocity.x == 0 && isJumping == false)
            {
                rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
            }
        }
    }

}

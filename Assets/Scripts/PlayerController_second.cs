using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_second : MonoBehaviour
{
    RoadGenerator rg;
    Rigidbody rb;

    Coroutine movingCoroutine;

    public float laneChangeSpeed = 15;

    float laneOffest = 2.5f;
    float pointStart;
    float pointFinish;
    float lastVectorX;

    bool isMoving = true;

    Vector3 targetPos;

    void Start()
    {
        targetPos = transform.position;
        SwipeManager.instance.MoveEvent += MovePlayer;
    }

    // Update is called once per frame
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
        transform.position = Vector3.MoveTowards(transform.position, targetPos, laneChangeSpeed * Time.deltaTime);
    }

    void MoveHorizontaly(float speed)
    {
        pointStart = -pointFinish;
        pointFinish += Mathf.Sign(speed) * laneOffest;
        if (isMoving)   
        { 
            StopCoroutine(movingCoroutine); 
            isMoving = false;
        }
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
        if (transform.position.y > 1)
        {
            rb.velocity = new Vector3(rb.velocity.x, -10, rb.velocity.z);
        }
        isMoving = false;
    }



    public void StartLevel()
    {
       // animator.applyRootMotion = false;
        rg = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        rg.StartLevel();
    }

    public void ResetGame()
    {

        //   rb.velocity = Vector3.zero;
        // pointStart = 0;
        //  pointFinish = 0;
        //  animator.applyRootMotion = true;
        //  animator.SetTrigger("Idle");
        //   transform.position = startGamePosition;
        //   transform.rotation = startGameRotation;
        rg = GameObject.Find("RoadGenerator").GetComponent<RoadGenerator>();
        rg.ResetLevel();
    }


}

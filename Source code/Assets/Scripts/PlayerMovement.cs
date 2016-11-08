using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayerMovement : MonoBehaviour
{

    public enum Facing { right, left, up, down };
    public float speed;
    public float collideDistance;
    public float collideRadius;

    private Facing facing;
    private Facing wantToChange;

    public float avaliableTimeToSwitchFacing;
    private float counter;
    private float xChange;
    private float yChange;

    private bool isChanging;
    public bool debug;

    private bool canMove;
    private Collider2D[] colliders;
    private Rigidbody2D rb;
    public NetworkView nView;

    private float lastSynchronizationTime = 0f;
    private float syncDelay = 0f;
    private float syncTime = 0f;
    private Vector3 syncStartPosition = Vector3.zero;
    private Vector3 syncEndPosition = Vector3.zero;

    [HideInInspector]
    public bool alive = true;

    void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        nView = GetComponent<NetworkView>();
        wantToChange = Facing.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (!nView.isMine)
        {
            return;
        }

        if (!alive)
            return;

        if (isChanging)
        {
            changeFacingFromInput(xChange, yChange);
            counter -= Time.deltaTime;
            if (counter <= 0)
            {
                counter = -1;
                isChanging = false;
            }
        }

        move();

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            xChange = Input.GetAxisRaw("Horizontal");
            yChange = Input.GetAxisRaw("Vertical");
            isChanging = true;
            counter = avaliableTimeToSwitchFacing;
        }
    }

    private void SyncedMovement()
    {
        syncTime += Time.deltaTime;
        rb.position = Vector3.Lerp(syncStartPosition, syncEndPosition, syncTime / syncDelay);
    }

    void changeFacingFromInput(float x, float y)
    {
        if (x > 0)
        {
            isAvaliableToMove(Facing.right);
            return;
        }
        if (x < 0)
        {
            isAvaliableToMove(Facing.left);
            return;
        }
        if (y < 0)
        {
            isAvaliableToMove(Facing.down);
            return;
        }
        if (y > 0)
        {
            isAvaliableToMove(Facing.up);
            return;
        }
    }


    bool isAvaliableToMove(Facing face)
    {

        wantToChange = face;

        colliders = Physics2D.OverlapBoxAll(transform.position + facingToWorldScale(wantToChange) * collideDistance, new Vector3(1, 1, 1) * collideRadius, 0);
        canMove = true;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
                canMove = false;
        }
        if (canMove)
        {
            facing = wantToChange;
            isChanging = false;
            counter = -1;
            return true;
        }
        return false;
    }

    Vector3 facingToWorldScale(Facing face)
    {
        switch (face)
        {
            case Facing.right: return new Vector3(1, 0, 0);
            case Facing.left: return new Vector3(-1, 0, 0);
            case Facing.up: return new Vector3(0, 1, 0);
            case Facing.down: return new Vector3(0, -1, 0);

            default: Debug.Log("NO FACE DETECTED for facingToWorldScale!!!"); return new Vector3(0, 0, 0);
        }
    }

    void move()
    {
        rb.velocity = facingToWorldScale(facing) * speed * Time.deltaTime * 100;
        distortFace();
    }

    void distortFace()
    {
        switch (facing)
        {
            case Facing.right: transform.localScale = new Vector3(1, 1, 1); transform.rotation = Quaternion.Euler(0, 0, 0); break;
            case Facing.left: transform.localScale = new Vector3(-1, 1, 1); transform.rotation = Quaternion.Euler(0, 0, 0); break;
            case Facing.up: transform.localScale = new Vector3(1, 1, 1); transform.rotation = Quaternion.Euler(0, 0, 90); break;
            case Facing.down: transform.localScale = new Vector3(1, 1, 1); transform.rotation = Quaternion.Euler(0, 0, -90); break;

            default: Debug.Log("NO FACE DETECTED for DISTORTION!!!"); break;
        }
    }

    void OnDrawGizmos()
    {
        if (debug)
        {
            Gizmos.color = new Color(0, 0, 1, 0.4f);
            if (isChanging)
                Gizmos.color = new Color(1, 0, 1, 0.4f);
            Gizmos.DrawCube(transform.position + facingToWorldScale(wantToChange) * collideDistance, new Vector3(1, 1, 1) * collideRadius);
            //if (isChanging) {
            //    Gizmos.color = new Color(1, 0, 0, 0.4f);
            //    Gizmos.DrawCube(transform.position + facingToWorldScale(wantToChange) * collideDistance, new Vector3(1, 1, 1) * collideRadius);
            //}
        }
    }
}
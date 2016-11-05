using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private Rigidbody2D rb;
    public NetworkView nView;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        nView = GetComponent<NetworkView>();

    }

	// Update is called once per frame
	void Update () {
        if(nView.isMine) {
            InputMovement();
        }
    }

    void InputMovement() {
        float xspeed, yspeed;
        xspeed = Input.GetAxis("Horizontal");
        if (xspeed != 0) {
            rb.velocity = new Vector2(xspeed * 1 * Time.deltaTime * 100, rb.velocity.y);
        }
        yspeed = Input.GetAxis("Vertical");
        if (yspeed != 0) {
            rb.velocity = new Vector2(rb.velocity.x, yspeed * 1 * Time.deltaTime * 100);
        }
    }
}

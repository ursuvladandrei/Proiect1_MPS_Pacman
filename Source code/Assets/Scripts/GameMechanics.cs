using UnityEngine;
using System.Collections;

public class GameMechanics : MonoBehaviour {

	public NetworkView nView;
    public GameObject pacmanSprite;
    public GameObject ghostSprite;
    public bool ghost=false;
	private float respawn_eta=0.0f;//cat mai dureaza pana i se face respawn
	public int respawn_cooldown=5;// si cat se asteapta de obicei
	public int score=0;
	//server only
	private int ghost_id=-1;
	private float ghost_eta = 0.0f;
	public int ghost_cooldown=10;

	// Use this for initialization
	void Start () {
		nView = GetComponent<NetworkView>();
	}
	
	// Update is called once per frame
	void Update () {
        if(ghost) {
            pacmanSprite.SetActive(false);
            ghostSprite.SetActive(true);
        } else {
            pacmanSprite.SetActive(true);
            ghostSprite.SetActive(false);
        }
		GameObject[] players=GameObject.FindGameObjectsWithTag ("Player");

		if (Network.isServer && nView.isMine) {//se ocupa numai obiectul care corespunde serverului cu hotararea a cine ghost-ul la un moment dat

			if (players.Length >= 2) {
				if (ghost_id == -1) {//la inceput nu e nimeni ghost, asa ca se stabileste random cine sa fie prima oara
					ghost_id = Random.Range (0, players.Length);
					Debug.Log ("first ghost is: " + ghost_id);
					ghost_eta = ghost_cooldown;
					GameMechanics script;
					for (int i = 0; i < players.Length; i+=1) {
						script = players[i].GetComponent<GameMechanics> ();
						script.ghost_id = ghost_id;
					}
				}
				else {
					ghost_eta -= Time.deltaTime;
					if (ghost_eta < 0) {
						float dist,closest_player_distance=1000;
						int closest_player_id = -1;
						for (int i = 0; i < players.Length; i += 1) {
							if (i!=ghost_id){//------------------------vezi daca e viu cu script.respawn_eta
								dist = Mathf.Abs (players [ghost_id].transform.position.x - players [i].transform.position.x) + Mathf.Abs (players [ghost_id].transform.position.y - players [i].transform.position.y);
								if (dist < closest_player_distance) {
									closest_player_distance = dist;
									closest_player_id = i;
								}
							}
						}
						if (closest_player_id != -1) {
							ghost_id = closest_player_id;
							ghost_eta = ghost_cooldown;
							Debug.Log ("new ghost is: " + ghost_id);
							//comunica si celorlalte obiecte de pe server noul id
							GameMechanics script;
							for (int i = 0; i < players.Length; i+=1) {
								script = players [i].GetComponent<GameMechanics> ();
								script.ghost_id = ghost_id;
							}
						}
					}
				}
			}
		}

		//daca sunt macar 2 jucatori si s-a stabilit un ghost_id atunci spune
		//clonei sale din fiecare instanta ca ea este noul ghost
		//(adica trebuie sa apeleze rpc DOAR obiectul care este acum ghost)
		if (players.Length>=2 && ghost_id!=-1 && players[ghost_id].Equals (nView.gameObject)){
			Debug.Log ("sending shit");
			nView.RPC ("changeGhost", RPCMode.All);
		}
		if (Network.isServer) Debug.Log ("ghost_id: " + ghost_id);

		if (respawn_eta > 0) {
			respawn_eta -= Time.deltaTime;
			return;
		}
		if (respawn_eta < 0) {
			gameObject.GetComponent<CircleCollider2D> ().enabled = true;
			gameObject.GetComponentInChildren < SpriteRenderer> ().enabled = true;
			respawn_eta = 0;
			PlayerMovement script;
			script = gameObject.GetComponent<PlayerMovement> ();
			script.alive = true;
			return;
		}
	}

	[RPC]
	public void changeGhost(){
		GameObject[] players=GameObject.FindGameObjectsWithTag ("Player");
		GameMechanics script;
		Debug.Log ("new ghost");
		//seteaza la ceilalti playeri din instanta din care face parte ghost ca fiind false
		for (int i = 0; i < players.Length; i += 1) {
			script = players[i].GetComponent<GameMechanics> ();
			script.ghost = false;
		}
		//si il seteaza la true pe al lui
		ghost = true;
    }

	void OnCollisionEnter2D(Collision2D col){
		
		if (col.gameObject.tag == "Player" && ghost) {//cand ghost-ul prinde pe cineva
			score += 100;
			col.gameObject.GetComponent<CircleCollider2D>().enabled=false;
			col.gameObject.GetComponentInChildren < SpriteRenderer> ().enabled = false;
			col.gameObject.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
			//se reseteaza timerul pentru respawn
			GameMechanics script1;
			script1 = col.gameObject.GetComponent<GameMechanics> ();
			script1.respawn_eta = respawn_cooldown;
			//se comunica si scriptului de PlayerMovement sa nu mai faca nimic
			PlayerMovement script2;
			script2 = col.gameObject.GetComponent<PlayerMovement> ();
			script2.alive = false;
		}
	}
}

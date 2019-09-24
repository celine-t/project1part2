using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	#region movement_variables
	public float movespeed;
	float x_input;
	float y_input;
	#endregion

	#region attack_variables
	public float damage;
	public float attackspeed;
	float attackTimer;
	public float hitboxTiming;
	public float endAnimationTiming;
	bool isAttacking;
	Vector2 currDirection;
	#endregion

	#region health_variables
	public float maxHealth;
	float currHealth;
	public Slider hpSlider;
	#endregion

	#region animation_components
	Animator anim;
	#endregion

	#region physics_components
	Rigidbody2D playerRB;
	#endregion

	#region Unity_functions
	//Called once on creation
	private void Awake()
	{
		playerRB = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		attackTimer = 0;
		currHealth = maxHealth;
		hpSlider.value = currHealth / maxHealth;
	}
	//Called every frame
	private void Update()
	{
		if (isAttacking) {
			return;
		}

		//access our input values
		x_input = Input.GetAxisRaw("Horizontal");
		y_input = Input.GetAxisRaw("Vertical");
		Move();

		if (Input.GetKeyDown(KeyCode.J) && attackTimer <= 0)
		{
			Attack();
		}
		else
		{
			attackTimer -= Time.deltaTime;
		}

		if (Input.GetKeyDown(KeyCode.L)) {
			Interact();
		}
	}
	#endregion

	#region movement_functions
	//Moves the player based on WASD inputs and 'movespeed'
	private void Move()
	{
		anim.SetBool("Moving", true);
		//If player is pressing 'D'
		if (x_input > 0)
		{
			playerRB.velocity = Vector2.right * movespeed;
			currDirection = Vector2.right;
		}
		//If player is pressing 'A'
		else if (x_input < 0)
		{
			playerRB.velocity = Vector2.left * movespeed;
			currDirection = Vector2.left;
		}
		//If player is pressing 'W'
		else if (y_input > 0)
		{
			playerRB.velocity = Vector2.up * movespeed;
			currDirection = Vector2.up;
		}
		//If player is pressing 'S'
		else if (y_input < 0)
		{
			playerRB.velocity = Vector2.down * movespeed;
			currDirection = Vector2.down;
		}
		else
		{
			playerRB.velocity = Vector2.zero;
			anim.SetBool("Moving", false);
		}
		//Set Animator Directional Values
		anim.SetFloat("DirX", currDirection.x);
		anim.SetFloat("DirY", currDirection.y);
	}
	#endregion

	#region attack_functions
	private void Attack()
	{
		Debug.Log("Attacking now");

		attackTimer = attackspeed;

		//Handles all attack animations and calculates hitboxes
		StartCoroutine(AttackRoutine());
	}

	//Handle animations and hitboxes for the attack mechanism
	IEnumerator AttackRoutine() {

		//Pause movement and freeze player for the dureciton of the attack
		isAttacking = true;
		playerRB.velocity = Vector2.zero;

		//Start Animation
		anim.SetTrigger("Attack");

		//Start Sound Effect
		FindObjectOfType<AudioManager>().Play("PlayerAttack");

		//Brief pause before we calculate the hitbox
		yield return new WaitForSeconds(hitboxTiming);

		Debug.Log("Casting hitbox now");

		//Create hitbox
		RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + currDirection, Vector2.one, 0f, Vector2.zero, 0);
		foreach (RaycastHit2D hit in hits)
		{
			Debug.Log(hit.transform.name);
			if (hit.transform.CompareTag("Enemy"))
			{
				Debug.Log("tons of damage");
				hit.transform.GetComponent<Enemy>().TakeDamage(damage);
			}

		}

		yield return new WaitForSeconds(endAnimationTiming);

		isAttacking = false;
	}
	#endregion

	#region health_functions
	//Take damage based on 'value' parameter, which is passed in by caller
	public void TakeDamage(float value)
	{
		currHealth -= value;
		Debug.Log("Health is now " + currHealth.ToString());

		//Start Sound Effect
		FindObjectOfType<AudioManager>().Play("PlayerHurt");

		//Change UI
		hpSlider.value = currHealth / maxHealth;

		//Check for death
		if (currHealth <= 0)
		{
			Die();
		}
	}

	// Heals player to hp based on 'value' parameter, which is passed in by caller
	public void Heal(float value)
	{
		//Increment Health
		currHealth += value;
		currHealth = Mathf.Min(currHealth, maxHealth);
		Debug.Log("Health is now " + currHealth.ToString());

		//Change UI
		hpSlider.value = currHealth / maxHealth;
	}

	//Destroys player object and triggers end scene stuff
	private void Die()
	{
		//Start Sound Effect
		FindObjectOfType<AudioManager>().Play("PlayerDeath");

		//Destory GameObject
		Destroy(this.gameObject);

		//Trigger anything we need to end the game, find game manager and lose game

		GameObject gm = GameObject.FindWithTag("GameController");
		gm.GetComponent<GameManager>().LoseGame();
	}
	#endregion


	#region interact_functions
	void Interact() {
		RaycastHit2D[] hits = Physics2D.BoxCastAll(playerRB.position + currDirection, new Vector2(0.5f, 0.5f), 0f, Vector2.zero, 0);
		foreach (RaycastHit2D hit in hits) {
			if (hit.transform.CompareTag("Chest"))
			{
				hit.transform.GetComponent<Chest>().Interact();
			}
		}
	}
	#endregion
}

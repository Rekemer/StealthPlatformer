using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Controller_2D))]

public class Player : MonoBehaviour
{
	[SerializeField] public int damage = 1; // 1 is normal , 3 is fat
	//public float maxJumpHeight = 4;
	public float minJumpHeight = 1;

	public float targetVelocityX;
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;
	float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;
	public float moveSpeed = 6;
	float gravity;

	
	public Vector3 velocity;
	float velocityXSmoothing;

	public float maxJumpVelocity;
	public float minJumpVelocity;

	
	[HideInInspector]
	public Controller_2D controller;
	
	public bool disabled = false;
	//public Nut_Script nut;


	public Vector2 input { get; private set; } // it means that we can get outside, but change only in this class
	public void UnDisableAfterTime(float time)
	{
		Invoke("UnDisable", time);
	}
	void Awake()
	{
		
		controller = GetComponent<Controller_2D>();
	
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);

		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
		
	}

	
	


	void Update()
	{

		
		
			input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
			if ( controller.collisions.below)
			{
				targetVelocityX = input.x * moveSpeed ;
			}
			// else if (!jump.HasJumped)
			// {
			// 	targetVelocityX = input.x * moveSpeed;
			// }



			velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
			velocity.y += gravity * Time.deltaTime;
			if (Input.GetKeyDown("space") && controller.collisions.below )

			{
				velocity.y = maxJumpVelocity;
			}
			
			controller.Move(velocity * Time.deltaTime, input);
			if (controller.collisions.above || controller.collisions.below)
			{
				velocity.y = 0;
			}
			if (Input.GetKeyUp(KeyCode.Space))
			{
				if (velocity.y > minJumpVelocity )
				{
					velocity.y = minJumpVelocity;
				}
			}


		}
		
	}



	

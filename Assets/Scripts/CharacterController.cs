using UnityEngine;

public class CharacterController : MonoBehaviour
{
	public Transform cameraTransform;
	public Transform driverSeat;
	public float maxCameraAngle;
	public float carTurningSpeed = 1f;
	public float maxDeviation = 10f;

	private float carLife = 100f;
	public float carSpeed = 0f;

	public GameObject[] smokeParticles;
	public GameObject[] heartPieces;
	public AudioSource crashSound;

	//Read-only property that returns true 
	//if the car's life is below or equal to zero
	public bool IsDead
	{
		get
		{
			return carLife <= 0f;
		}
	}

	/// <summary>
	/// Called every frame
	/// </summary>
    void Update()
    {
		//If the car is dead, do not run
		if (IsDead)
			return;

		//Get the angle of the camera transform's z axis
		float angle = cameraTransform.eulerAngles.z;
		//Ensure that angles above 180 degrees become negative
		float convertedAngle = ConvertToNegative(angle);
		//Limit angle's range between camera's max angle deviations
		convertedAngle = Mathf.Clamp(convertedAngle, -maxCameraAngle, maxCameraAngle);

		//Get car's current rotation
		Vector3 rotation = transform.rotation.eulerAngles;
		//Tilt the car's heading towards that direction
		rotation.y = -convertedAngle;
		transform.eulerAngles = rotation;

		//Get car's current position
		Vector3 carPosition = transform.position;
		//Modify car's position on the x axis based on the camera's tilt
		carPosition.x -= convertedAngle * Time.deltaTime * carTurningSpeed;
		//Clamp movement within the road's limits
		carPosition.x = Mathf.Clamp(carPosition.x, -maxDeviation, maxDeviation);
		//Set the car's position
		transform.position = carPosition;
		//Affect the animation speed of the car's wheels
		gameObject.GetComponent<Animator>().speed = carSpeed * 0.1f;
    }

	/// <summary>
	/// Called every frame after Update
	/// </summary>
	private void LateUpdate()
	{
		//Moves camera's position to driver's seat every frame
		cameraTransform.parent.position = driverSeat.position;
	}

	/// <summary>
	/// Called every physics update
	/// </summary>
	private void FixedUpdate()
	{
		//Due to the game being an infinite runner, the car's rigidbody
		//goes to sleep after some time. I wake it up to force it to 
		//detect collisions.
		GetComponent<Rigidbody>().WakeUp();
	}

	//Gets euler angle and converts values greater than 180 to negative
	//to provide negative vector steering on the x axis
	private float ConvertToNegative(float angle)
	{
		if(angle > 180f)
		{
			angle -= 360f;			
		}
		return angle;
	}

	//Called by others to modify the car's life total
	//Manipulates smoke particle effects and health UI based on life total
	public void ModifyCarLife(float modifier)
	{
		//Modifies the car's life by modifier and clamps it to 0~100
		carLife = Mathf.Clamp(carLife + modifier, 0f, 100f);

		//Activates smoke particles when car's life total is less than the current step
		float step = 100f / (smokeParticles.Length + 1);
		float total = 100f;
		for(int i = 0; i < smokeParticles.Length; i++)
		{
			total -= step;
			if(carLife > total)
			{
				smokeParticles[i].SetActive(false);
			}
			else
			{
				smokeParticles[i].SetActive(true);
			}
		}

		//Activates heart pieces when car's life total is greater than the current step
		step = 10f;
		total = 0f;
		for(int i = 0; i < 10; i++)
		{
			total += 10f;
			if(carLife >= total)
			{
				heartPieces[i].SetActive(true);
			}
			else
			{
				heartPieces[i].SetActive(false);
			}
		}		
	}

	/// <summary>
	/// When car collides with an entity, 
	/// get its EntityController and call Hit() on it
	/// </summary>	
	private void OnCollisionEnter(Collision collision)
	{
		var entityController = collision.transform.GetComponent<EntityController>();
		if (entityController == null)
			return;
		entityController.Hit();
	}
}

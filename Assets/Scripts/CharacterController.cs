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

	public bool IsDead
	{
		get
		{
			return carLife <= 0f;
		}
	}

    void Update()
    {
		if (IsDead)
			return;
		float angle = cameraTransform.eulerAngles.z;
		float convertedAngle = ConvertToNegative(angle);
		convertedAngle = Mathf.Clamp(convertedAngle, -maxCameraAngle, maxCameraAngle);

		Vector3 rotation = transform.rotation.eulerAngles;
		rotation.y = -convertedAngle;
		transform.eulerAngles = rotation;

		Vector3 carPosition = transform.position;
		carPosition.x -= convertedAngle * Time.deltaTime * carTurningSpeed;
		carPosition.x = Mathf.Clamp(carPosition.x, -maxDeviation, maxDeviation);
		transform.position = carPosition;
		gameObject.GetComponent<Animator>().speed = carSpeed * 0.1f;
    }

	private void LateUpdate()
	{
		cameraTransform.parent.position = driverSeat.position;
	}

	private void FixedUpdate()
	{
		GetComponent<Rigidbody>().WakeUp();
	}

	private float ConvertToNegative(float angle)
	{
		if(angle > 180f)
		{
			angle -= 360f;			
		}
		return angle;
	}

	public void ModifyCarLife(float modifier)
	{
		carLife = Mathf.Clamp(carLife + modifier, 0f, 100f);
		float segment = 100f / (smokeParticles.Length + 1);
		float total = 100f;
		for(int i = 0; i < smokeParticles.Length; i++)
		{
			total -= segment;
			if(carLife > total)
			{
				smokeParticles[i].SetActive(false);
			}
			else
			{
				smokeParticles[i].SetActive(true);
			}
		}

		segment = 10f;
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

	private void OnCollisionEnter(Collision collision)
	{
		var entityController = collision.transform.GetComponent<EntityController>();
		if (entityController == null)
			return;
		entityController.Hit();
	}
}

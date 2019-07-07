using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Main controller class used to control 
/// the flow of the game.
/// </summary>
public class LevelController : MonoBehaviour
{
	public List<Transform> segments;

	public CharacterController characterController;
	private Transform[] segmentPrefabs;

	private float distanceTraveled = 0f;
	public Text distanceMeter;

	public bool hasStarted = false;
	private float targetSpeed = 0f;
	private int currentDifficulty = 0;

	public AudioSource backgroundMusic;

	public GameObject logo;

	/// <summary>
	/// Happens at the beginning of the game, 
	/// used to initialize properties.
	/// </summary>
    void Start()
    {
		//Load all segments from the resources folder
		segmentPrefabs = Resources.LoadAll<Transform>("Segments");

		//Setup each segment
		foreach(var segment in segments)
		{
			SetupSegmentEntities(segment);
		}
    }

	/// <summary>
	/// Passes refererences of LevelController and CharacterController
	/// to every entity in the segment.
	/// </summary>
	/// <param name="segment">Segment's Transform component</param>
	private void SetupSegmentEntities(Transform segment)
	{
		var entities = segment.GetComponentsInChildren<EntityController>();
		foreach(var entity in entities)
		{
			entity.Setup(this, characterController);
		}
	}	

	/// <summary>
	/// Called each frame
	/// </summary>
    void Update()
    {
		//If the game hasn't started, do not run level 
		if (hasStarted == false)
			return;

		//If the player is dead, stop the current level
		if(characterController.IsDead)
		{
			StopLevel();
		}

		//Speed with which car accelerates up to target speed
		float accelerationRate = targetSpeed / 3f;

		//Car's speed clamped between 0 and target speed
		characterController.carSpeed = Mathf.Clamp(characterController.carSpeed + (Time.deltaTime * accelerationRate), 0f, targetSpeed);

		//Calculate distance segment is displaced
		float distance = characterController.carSpeed * Time.deltaTime;

		//Move segments opposite to player's facing
		for (int i = 0; i < segments.Count; i++)
		{
			Transform segment = segments[i];
			Vector3 position = segment.position;
			
			//Displace segment backwards the calculated distance
			position.z -= distance;
			//Perform the movement
			segment.position = position;			
		}

		//Calculate total distance traveled
		//and clamp it between 0 and 999999999.99
		distanceTraveled = Mathf.Clamp(distanceTraveled + (distance / 1000f), 0f, 999999999.99f);
		//Display total distance traveled to player's car UI
		DisplayDistanceMeter();

		Transform centerSegment = segments[1];
		//If center segment is displaced half its size, 
		//we are at its end; swap segments.
		if(centerSegment.position.z <= -110)
		{
			SwapSegments();
		}

		int difficulty = (int)Mathf.Sqrt(distanceTraveled * 9f) + 1;
		//If we reached new difficulty level, switch the level's difficulty
		if(difficulty != currentDifficulty)
		{
			SetDifficulty(difficulty);
			Debug.Log("Setting difficulty to " + difficulty);
		}
    }

	void SwapSegments()
	{
		Transform segmentA = segments[0];
		Transform segmentB = segments[1];
		Transform segmentC = segments[2];

		Vector3 segmentAPosition = segmentA.position;
		Vector3 segmentBPosition = segmentB.position;
		Vector3 segmentCPosition = segmentC.position;

		//Destroy further back segment
		Destroy(segmentC.gameObject);
		//Get new segment in its place
		segmentC = CreateRandomSegment();
		//Setup new segment
		SetupSegmentEntities(segmentC);

		//Swap segments
		segments[0] = segmentC;
		segmentC.position = segmentAPosition;

		segments[1] = segmentA;
		segmentA.position = segmentBPosition;

		segments[2] = segmentB;
		segmentB.position = segmentCPosition;

		//Move all three segments 220 units back
		for (int i = 0; i < segments.Count; i++)
		{
			Transform segment = segments[i];
			Vector3 position = segment.position;
			position.z += 220;
			segment.position = position;
		}
	}

	/// <summary>
	/// Picks a random segment prefab 
	/// and creates an instance of it
	/// </summary>
	/// <returns>Instantiated segment</returns>
	private Transform CreateRandomSegment()
	{
		int index = Random.Range(0, segmentPrefabs.Length);
		var prefab = segmentPrefabs[index];
		return Instantiate(prefab);
	}

	/// <summary>
	/// Uses difficulty value to modify scene's properties
	/// </summary>
	/// <param name="difficulty">difficulty level</param>
	public void SetDifficulty(int difficulty)
	{
		currentDifficulty = difficulty;
		targetSpeed = difficulty * 5f;
		backgroundMusic.pitch = 0.5f + ((difficulty - 1f) * 0.05f);
	}

	/// <summary>
	/// Begins new run
	/// </summary>
	public void StartLevel()
	{		
		StopCoroutine("KillMusic");

		for(int i = 0; i < 3; i++)
		{
			Destroy(segments[i].gameObject);
			segments[i] = CreateRandomSegment();
			SetupSegmentEntities(segments[i]);
			Vector3 pos = new Vector3(0f, 0f, (1f - i) * 220f);
			segments[i].position = pos;			
		}
		logo.SetActive(false);
		characterController.gameObject.SetActive(true);
		//Reset car's position to the center of the middle segment
		var carPos = characterController.transform.position;
		carPos = new Vector3(0f, 0.68f, 0f);
		characterController.transform.position = carPos;		
		//Reset car's rotation to world forward
		characterController.transform.eulerAngles = Vector3.zero;
		characterController.carSpeed = 0f;
		distanceTraveled = 0f;
		characterController.ModifyCarLife(100f);
		characterController.enabled = true;
		SetDifficulty(1);
		//Reset music track to beginning
		backgroundMusic.time = 0f;
		backgroundMusic.Play();
		hasStarted = true;
	}
		
	/// <summary>
	/// Stops current run
	/// </summary>
	public void StopLevel()
	{		
		hasStarted = false;
		//Destroy the record
		StartCoroutine("KillMusic");
	}	

	/// <summary>
	/// Lowers background music's pitch gradually
	/// with linear interpolation
	/// </summary>	
	private IEnumerator KillMusic()
	{
		//Starting pitch
		float start = backgroundMusic.pitch;
		//Target pitch
		float end = 0f;
		//Rate of interpolation
		float rate = 0.75f;
		//Interpolation progress (0-1)
		float delta = 0f;

		while (delta < 1f)
		{
			delta += Time.deltaTime * rate;
			//Interpolate pitch from start to target pitch at delta percentage
			backgroundMusic.pitch = Mathf.Lerp(start, end, delta);
			//Wait for next frame
			yield return null;
		}
		backgroundMusic.Stop();
		characterController.enabled = false;
		logo.SetActive(true);
	}

	/// <summary>
	/// Displays distance traveled to car's UI
	/// </summary>
	private void DisplayDistanceMeter()
	{
		string display = distanceTraveled.ToString("########0.00") + " KM";
		distanceMeter.text = display;
	}
}

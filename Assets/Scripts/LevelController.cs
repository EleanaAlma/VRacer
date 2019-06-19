using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

    void Start()
    {
		segmentPrefabs = Resources.LoadAll<Transform>("Segments");
		foreach(var segment in segments)
		{
			SetupSegmentEntities(segment);
		}
    }

	private void SetupSegmentEntities(Transform segment)
	{
		var entities = segment.GetComponentsInChildren<EntityController>();
		foreach(var entity in entities)
		{
			entity.Setup(this, characterController);
		}
	}

	private Transform CreateRandomSegment()
	{
		int index = Random.Range(0, segmentPrefabs.Length);
		var prefab = segmentPrefabs[index];
		return Instantiate(prefab);
	}

    void Update()
    {
		if (!hasStarted)
			return;
		if(characterController.IsDead)
		{
			StopLevel();
		}
		float accelerationRate = targetSpeed / 3f;
		characterController.carSpeed = Mathf.Clamp(characterController.carSpeed + (Time.deltaTime * accelerationRate), 0f, targetSpeed);
        for(int i = 0; i < segments.Count; i++)
		{
			Transform segment = segments[i];
			Vector3 position = segment.position;
			var distance = characterController.carSpeed * Time.deltaTime;
			position.z -= distance;
			segment.position = position;

			distanceTraveled = Mathf.Clamp(distanceTraveled + (distance / 1000f), 0f, 999999999.99f);
			DisplayDistanceMeter();
		}

		Transform centerSegment = segments[1];
		if(centerSegment.position.z <= -110)
		{
			SwapSegments();
		}
		int difficulty = (int)Mathf.Sqrt(distanceTraveled * 3f) + 1;
		if(difficulty != currentDifficulty)
		{
			SetDifficulty(difficulty);
			Debug.Log("Setting difficulty to " + difficulty);
		}
    }

	private void DisplayDistanceMeter()
	{
		string display = distanceTraveled.ToString("########0.00") + " KM";
		distanceMeter.text = display;
	}

	void SwapSegments()
	{
		Transform segmentA = segments[0];
		Transform segmentB = segments[1];
		Transform segmentC = segments[2];

		Vector3 segmentAPosition = segmentA.position;
		Vector3 segmentBPosition = segmentB.position;
		Vector3 segmentCPosition = segmentC.position;

		Destroy(segmentC.gameObject);
		segmentC = CreateRandomSegment();
		SetupSegmentEntities(segmentC);

		segments[0] = segmentC;
		segmentC.position = segmentAPosition;

		segments[1] = segmentA;
		segmentA.position = segmentBPosition;

		segments[2] = segmentB;
		segmentB.position = segmentCPosition;

		for (int i = 0; i < segments.Count; i++)
		{
			Transform segment = segments[i];
			Vector3 position = segment.position;
			position.z += 220;
			segment.position = position;
		}
	}

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
		var carPos = characterController.transform.position;
		carPos = new Vector3(0f, 0.68f, 0f);
		characterController.transform.position = carPos;		
		characterController.transform.eulerAngles = Vector3.zero;
		characterController.carSpeed = 0f;
		distanceTraveled = 0f;
		characterController.ModifyCarLife(100f);
		characterController.enabled = true;
		SetDifficulty(1);
		backgroundMusic.time = 0f;
		backgroundMusic.Play();
		hasStarted = true;
	}

	public void StopLevel()
	{		
		hasStarted = false;
		StartCoroutine("KillMusic");
	}

	public void SetDifficulty(int level)
	{
		currentDifficulty = level;
		targetSpeed = level * 5f;
		backgroundMusic.pitch = 0.5f + ((level - 1f) * 0.05f);
	}

	private IEnumerator KillMusic()
	{
		float start = backgroundMusic.pitch;
		float end = 0f;
		float rate = 0.75f;
		float delta = 0f;
		while (delta < 1f)
		{
			delta += Time.deltaTime * rate;
			backgroundMusic.pitch = Mathf.Lerp(start, end, delta);
			yield return null;
		}
		backgroundMusic.Stop();
		characterController.enabled = false;
		logo.SetActive(true);
	}
}

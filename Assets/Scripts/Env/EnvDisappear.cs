using UnityEngine;
using System.Collections;

public class EnvDisappear : MonoBehaviour
{

	[Tooltip("Set to false to completely disable the timing, then use Activate(), Deactivate(), and SetActive(bool) to set its state")]
	public bool autoTimer = true;

	[Tooltip("Use this to offset the initial cycle")]
	public float initialTimer = 0f;

	[Tooltip("Stay active for 3 seconds")]
	public float visibleTime = 3f;

	[Tooltip("Then disappear for 5 seconds, total cycle time will be 8 seconds")]
	public float invisibleTime = 5f;

	//Returns visibleTime + invisibleTime
	public float totalCycleTime
	{
		get
		{
			return visibleTime + invisibleTime;
		}
	}

	[Tooltip("Time it plays a flashing effect to show that its about to disappear")]
	public float transitionTime = 1f;

	[Tooltip("Rate it flashes at the start of the transition")]
	public float transitionSpeedInitial = 0.1f;

	[Tooltip("Rate it flashes at the end of the transition")]
    public float transitionSpeedEnd = 0.025f;

	[Tooltip("Set to false to make it cycle once")]
	public bool looping = true;

	[Tooltip("If not looping: this states whether it goes visible at the end of the cycle or not")]
	public bool endVisibility = false;

	[Tooltip("The components that will be enabled and disabled when the platform appears and disappears")]
	public Behaviour[] componentsToSetEnabled;

	[Tooltip("The renderers that will be enabled and disabled when the platform appears, disappears, and flashes")]
	public Renderer[] renderersToSetEnabled;


	//Whether or not it is currently active
	public bool isActive { get; private set; }

	//Time it will toggle its active status
	private float nextTime;

	//Whether or not the cycle is paused
	private bool isPaused = false;

	//The time the cycle was paused
	private float pauseTime;

	//The time the last transition flash occured
	private float lastFlashTime;

	//If looping is false, this states whether or not is is currently cycling
	public bool isInCycle { get; private set; }

	public void StartCycle()
	{
		if(!autoTimer)
		{
			Debug.LogError("Cant StartCycle on a EnvDisappear that has the autoTimer disabled. Use Activate(), Deactivate() or enable autoTimer");
			return;
		}

		if(isInCycle)
		{
			Debug.LogError("EnvDisappear is already cycling");
			return;
		}

		//Debug.Log("StartCycle");

		isPaused = false;
		isInCycle = true;
		nextTime = Time.time + visibleTime;
		SetActivated();
	}

	public void PauseCycle()
	{
		if(isPaused)
		{
			Debug.LogError("EnvDisappear is not paused!");
			return;
		}
		isPaused = true;
		pauseTime = Time.time;
	}

	public void UnpauseCycle()
	{
		if(!isPaused)
		{
			Debug.LogError("EnvDisappear is not paused!");
			return;
		}
		isPaused = false;
		nextTime += Time.time - pauseTime; //Increase the time by the total time we were paused
	}

	public void StopCycle()
	{
		//Debug.Log("StopCycle");

		isPaused = true;
		isInCycle = false;
	}

	/// <summary>
	/// Does NOT stop the cycle!
	/// </summary>
	public void ResetCycle()
	{
		SetActivated();
		nextTime = Time.time + visibleTime;
	}

	public void Activate()
	{
		if(autoTimer)
		{
			Debug.LogError("Cant use Activate when the auto timer is enabled. Use StartCycle(), PauseCycle(), StopCycle(), ResetCycle() instead.");
			return;
		}

		SetActivated();
	}

	public void Deactivate()
	{
		if(autoTimer)
		{
			Debug.LogError("Cant use Deactivate when the auto timer is enabled. Use StartCycle(), PauseCycle(), StopCycle(), ResetCycle() instead.");
			return;
		}

		SetDeactivated();
	}

	public void SetActive(bool active)
	{
		if(autoTimer)
		{
			Debug.LogError("Cant use SetActive when the auto timer is enabled. Use StartCycle(), PauseCycle(), StopCycle(), ResetCycle() instead.");
			return;
		}

		if(active)
		{
			SetActivated();
		}
		else
		{
			SetDeactivated();
		}
	}

	void Awake()
	{
		if(autoTimer)
		{
			initialTimer = Mathf.Repeat(initialTimer, totalCycleTime);

			if(initialTimer > visibleTime)
			{
				//It starts invisible
				SetDeactivated();

				nextTime = invisibleTime - (initialTimer - visibleTime);
			}
			else
			{
				//It starts visible
				SetActivated();

				nextTime = visibleTime - initialTimer;
			}
		}
	}

	void Update()
	{
		if(autoTimer)
		{
			if(!isPaused)
			{
				if(Time.time > nextTime)
				{
					if(isActive)
					{
						SetDeactivated();

						if(!looping)
						{
							if(!endVisibility)
							{
								StopCycle();
							}
						}

						nextTime += invisibleTime;
					}
					else
					{
						if(looping)
						{
							SetActivated();
						}
						else
						{
							if(endVisibility)
							{
								SetActivated();
							}

							//Stop the cycle
							StopCycle();
						}

						nextTime += visibleTime;
					}
				}

				if(isActive)
				{
					float timeLeft = nextTime - Time.time;

					if(timeLeft < transitionTime)
					{
						float flashSpeed = Mathf.Lerp(transitionSpeedEnd, transitionSpeedInitial, timeLeft / transitionTime);

						if(Time.time > lastFlashTime + flashSpeed)
						{
							//Do a flash
							foreach(Renderer renderer in renderersToSetEnabled)
							{
								renderer.enabled = !renderer.enabled;
							}

							lastFlashTime += flashSpeed;
                        }
					}
				}
			}
		}
	}

	private void SetActivated()
	{
		//Debug.Log("Set Activated");

		isActive = true;
		foreach(Behaviour component in componentsToSetEnabled)
		{
			component.enabled = true;
		}
		foreach(Renderer renderer in renderersToSetEnabled)
		{
			renderer.enabled = true;
		}
	}

	private void SetDeactivated()
	{
		//Debug.Log("Set Deactivated");

		isActive = false;
		foreach(Behaviour component in componentsToSetEnabled)
		{
			component.enabled = false;
		}
		foreach(Renderer renderer in renderersToSetEnabled)
		{
			renderer.enabled = false;
		}
	}
}

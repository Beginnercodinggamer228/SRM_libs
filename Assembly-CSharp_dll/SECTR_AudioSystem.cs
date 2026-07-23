using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200007E RID: 126
[RequireComponent(typeof(AudioListener))]
[ExecuteInEditMode]
[AddComponentMenu("SECTR/Audio/SECTR Audio System")]
public class SECTR_AudioSystem : MonoBehaviour
{
	// Token: 0x0600026C RID: 620 RVA: 0x0000F564 File Offset: 0x0000D764
	private static int GetInstancesCount(SECTR_AudioCue cue)
	{
		List<SECTR_AudioSystem.Instance> list;
		SECTR_AudioSystem.maxInstancesTable.TryGetValue(cue, out list);
		if (list == null)
		{
			return 0;
		}
		return list.Count;
	}

	// Token: 0x1700004C RID: 76
	// (get) Token: 0x0600026D RID: 621 RVA: 0x0000F58A File Offset: 0x0000D78A
	public static bool Initialized
	{
		get
		{
			return SECTR_AudioSystem.system != null;
		}
	}

	// Token: 0x1700004D RID: 77
	// (get) Token: 0x0600026E RID: 622 RVA: 0x0000F597 File Offset: 0x0000D797
	public static SECTR_AudioSystem System
	{
		get
		{
			return SECTR_AudioSystem.system;
		}
	}

	// Token: 0x1700004E RID: 78
	// (get) Token: 0x0600026F RID: 623 RVA: 0x0000F59E File Offset: 0x0000D79E
	public static Transform Listener
	{
		get
		{
			return SECTR_AudioSystem.system.transform;
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0000F5AC File Offset: 0x0000D7AC
	public static SECTR_AudioCueInstance Play(SECTR_AudioCue audioCue, Vector3 position, bool loop)
	{
		return SECTR_AudioSystem.Play(audioCue, null, position, loop, null, false);
	}

	// Token: 0x06000271 RID: 625 RVA: 0x0000F5CC File Offset: 0x0000D7CC
	public static SECTR_AudioCueInstance Play(SECTR_AudioCue audioCue, Transform parent, Vector3 localPosition, bool loop, int? clipIndex = null, bool hasPriority = false)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			return default(SECTR_AudioCueInstance);
		}
		if (SECTR_AudioSystem.system.MasterBus == null)
		{
			Debug.LogWarning("SECTR_AudioSystem needs a Master Bus before you can play sounds.");
			return default(SECTR_AudioCueInstance);
		}
		if (SECTR_AudioSystem.activeInstances.Count >= SECTR_AudioSystem.system.MaxInstances)
		{
			Debug.LogWarning("Global max audio instances exceeded.");
			if (SECTR_AudioSystem.firstMaxInstanceWarning)
			{
				SECTR_AudioSystem.firstMaxInstanceWarning = false;
				foreach (SECTR_AudioSystem.Instance instance in SECTR_AudioSystem.activeInstances)
				{
					Debug.LogWarning("Instance: " + instance.Cue.name);
				}
			}
			return default(SECTR_AudioCueInstance);
		}
		if (audioCue == null)
		{
			return default(SECTR_AudioCueInstance);
		}
		if (!SECTR_AudioSystem._CheckInstances(audioCue, false))
		{
			if (!hasPriority || !SECTR_AudioSystem.maxInstancesTable.ContainsKey(audioCue))
			{
				return default(SECTR_AudioCueInstance);
			}
			SECTR_AudioSystem.Instance instance2 = SECTR_AudioSystem.maxInstancesTable[audioCue].FirstOrDefault((SECTR_AudioSystem.Instance i) => !i.HasPriority);
			if (instance2 == null)
			{
				return default(SECTR_AudioCueInstance);
			}
			instance2.Stop(true);
		}
		else if (audioCue.AudioClips.Count == 0)
		{
			Debug.LogWarning("Cannot play a clipless Audio Cues.");
			return default(SECTR_AudioCueInstance);
		}
		SECTR_AudioCue sourceCue = audioCue.SourceCue;
		if (UnityEngine.Random.value <= sourceCue.PlayProbability)
		{
			bool flag = sourceCue.IsLocal || SECTR_AudioSystem._CheckProximity(audioCue, parent, localPosition, null);
			loop |= sourceCue.Loops;
			if (flag || loop)
			{
				SECTR_AudioSystem.Instance instance3 = SECTR_AudioSystem.instancePool.Pop();
				if (instance3 != null)
				{
					instance3.Init(audioCue, parent, localPosition, loop, clipIndex, hasPriority);
					if (flag)
					{
						instance3.Play();
					}
					SECTR_AudioSystem.activeInstances.Add(instance3);
					return new SECTR_AudioCueInstance(instance3, instance3.Generation, loop);
				}
			}
		}
		return default(SECTR_AudioCueInstance);
	}

	// Token: 0x06000272 RID: 626 RVA: 0x0000F7CC File Offset: 0x0000D9CC
	public static SECTR_AudioCueInstance Clone(SECTR_AudioCueInstance instance, Vector3 newPosition)
	{
		if (instance)
		{
			SECTR_AudioSystem.Instance instance2 = SECTR_AudioSystem.instancePool.Pop();
			instance2.Clone((SECTR_AudioSystem.Instance)instance.GetInternalInstance(), newPosition);
			return new SECTR_AudioCueInstance(instance2, instance2.Generation, instance2.Loops);
		}
		return default(SECTR_AudioCueInstance);
	}

	// Token: 0x06000273 RID: 627 RVA: 0x0000F81C File Offset: 0x0000DA1C
	public static void PlayMusic(SECTR_AudioCue musicCue)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot play music before Audio System is initialized.");
			return;
		}
		if (musicCue != null)
		{
			if (musicCue.Is3D)
			{
				Debug.LogWarning("Music Cue " + musicCue.name + "is 3Dm but music should be Simple 2D.");
			}
			SECTR_AudioSystem.musicLoop.Stop(false);
			SECTR_AudioSystem.currentMusic = musicCue;
			SECTR_AudioSystem.musicLoop = SECTR_AudioSystem.Play(SECTR_AudioSystem.currentMusic, SECTR_AudioSystem.Listener, Vector3.zero, true, null, false);
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x0000F89B File Offset: 0x0000DA9B
	public static void StopMusic(bool stopImmediate)
	{
		if (SECTR_AudioSystem.Initialized)
		{
			SECTR_AudioSystem.musicLoop.Stop(stopImmediate);
			SECTR_AudioSystem.currentMusic = null;
		}
	}

	// Token: 0x06000275 RID: 629 RVA: 0x0000F8B5 File Offset: 0x0000DAB5
	public static void PushAmbience(SECTR_AudioAmbience ambience)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot activate an ambience before audio system is initialzied.");
			return;
		}
		if (ambience != null)
		{
			SECTR_AudioSystem.ambienceStack.Add(ambience);
		}
	}

	// Token: 0x06000276 RID: 630 RVA: 0x0000F8D7 File Offset: 0x0000DAD7
	public static void RemoveAmbience(SECTR_AudioAmbience ambience)
	{
		if (SECTR_AudioSystem.Initialized && ambience != null)
		{
			SECTR_AudioSystem.ambienceStack.Remove(ambience);
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x0000F8EF File Offset: 0x0000DAEF
	public static float GetBusVolume(string busName)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot get bus volume before Audio System is initialzied.");
		}
		else if (!string.IsNullOrEmpty(busName))
		{
			return SECTR_AudioSystem.GetBusVolume(SECTR_AudioSystem._FindBus(SECTR_AudioSystem.system.MasterBus, busName));
		}
		return 0f;
	}

	// Token: 0x06000278 RID: 632 RVA: 0x0000F927 File Offset: 0x0000DB27
	public static float GetBusVolume(SECTR_AudioBus bus)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot get bus volume before Audio System is initialzied.");
		}
		else if (bus)
		{
			return bus.UserVolume;
		}
		return 0f;
	}

	// Token: 0x06000279 RID: 633 RVA: 0x0000F950 File Offset: 0x0000DB50
	public static void SetBusVolume(string busName, float volume)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot activate an ambience before audio system is initialzied.");
			return;
		}
		if (!string.IsNullOrEmpty(busName))
		{
			SECTR_AudioSystem.SetBusVolume(SECTR_AudioSystem._FindBus(SECTR_AudioSystem.system.MasterBus, busName), volume);
		}
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000F982 File Offset: 0x0000DB82
	public static void SetBusVolume(SECTR_AudioBus bus, float volume)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot set bus volume before Audio System is initialzied.");
			return;
		}
		if (bus)
		{
			bus.UserVolume = volume;
		}
	}

	// Token: 0x0600027B RID: 635 RVA: 0x0000F9A5 File Offset: 0x0000DBA5
	public static void MuteBus(string busName, bool mute)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot mute bus before Audio System is initialzied.");
			return;
		}
		if (!string.IsNullOrEmpty(busName))
		{
			SECTR_AudioSystem.MuteBus(SECTR_AudioSystem._FindBus(SECTR_AudioSystem.system.MasterBus, busName), mute);
		}
	}

	// Token: 0x0600027C RID: 636 RVA: 0x0000F9D7 File Offset: 0x0000DBD7
	public static void MuteBus(SECTR_AudioBus bus, bool mute)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot mute bus before Audio System is initialzied.");
			return;
		}
		if (bus)
		{
			bus.Muted = mute;
		}
	}

	// Token: 0x0600027D RID: 637 RVA: 0x0000F9FA File Offset: 0x0000DBFA
	public static void PauseBus(string busName, bool paused)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot pause bus before Audio System is initialzied.");
			return;
		}
		if (!string.IsNullOrEmpty(busName))
		{
			SECTR_AudioSystem.PauseBus(SECTR_AudioSystem._FindBus(SECTR_AudioSystem.system.MasterBus, busName), paused);
		}
	}

	// Token: 0x0600027E RID: 638 RVA: 0x0000FA2C File Offset: 0x0000DC2C
	public static void PauseBus(SECTR_AudioBus bus, bool paused)
	{
		if (!SECTR_AudioSystem.Initialized)
		{
			Debug.LogWarning("Cannot pause bus before Audio System is initialzied.");
			return;
		}
		if (bus)
		{
			bool paused2 = bus.Paused;
			bus.Pause(paused);
			if (paused2 != bus.Paused)
			{
				int count = SECTR_AudioSystem.activeInstances.Count;
				for (int i = 0; i < count; i++)
				{
					SECTR_AudioSystem.Instance instance = SECTR_AudioSystem.activeInstances[i];
					if (bus.IsAncestorOf(instance.Bus))
					{
						instance.Pause(paused);
					}
				}
			}
		}
	}

	// Token: 0x0600027F RID: 639 RVA: 0x0000FAA0 File Offset: 0x0000DCA0
	public static bool IsOccluded(Vector3 worldSpacePosition, SECTR_AudioSystem.OcclusionModes occlusionFlags)
	{
		bool flag = false;
		Vector3 position = SECTR_AudioSystem.Listener.position;
		Vector3 direction = position - worldSpacePosition;
		float sqrMagnitude = direction.sqrMagnitude;
		if (!flag && (occlusionFlags & SECTR_AudioSystem.OcclusionModes.Distance) != (SECTR_AudioSystem.OcclusionModes)0)
		{
			flag = (sqrMagnitude >= SECTR_AudioSystem.system.OcclusionDistance * SECTR_AudioSystem.system.OcclusionDistance);
		}
		if (!flag && (occlusionFlags & SECTR_AudioSystem.OcclusionModes.Raycast) != (SECTR_AudioSystem.OcclusionModes)0)
		{
			float maxDistance = Mathf.Sqrt(sqrMagnitude);
			RaycastHit raycastHit;
			flag = (Physics.Raycast(worldSpacePosition, direction, out raycastHit, maxDistance, SECTR_AudioSystem.system.RaycastLayers) && raycastHit.transform != SECTR_AudioSystem.Listener);
		}
		if (!flag && (occlusionFlags & SECTR_AudioSystem.OcclusionModes.Graph) != (SECTR_AudioSystem.OcclusionModes)0)
		{
			SECTR_Graph.FindShortestPath(ref SECTR_AudioSystem.occlusionPath, worldSpacePosition, position, (SECTR_Portal.PortalFlags)0);
			int count = SECTR_AudioSystem.occlusionPath.Count;
			int num = 0;
			while (num < count && !flag)
			{
				SECTR_Graph.Node node = SECTR_AudioSystem.occlusionPath[num];
				if (node.Portal && (node.Portal.Flags & SECTR_Portal.PortalFlags.Closed) != (SECTR_Portal.PortalFlags)0)
				{
					flag = true;
				}
				num++;
			}
		}
		return flag;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x0000FB93 File Offset: 0x0000DD93
	private static IEnumerable<SECTR_AudioBus> FindNonUISFX()
	{
		if (SECTR_AudioSystem.System != null && SECTR_AudioSystem.System.MasterBus != null)
		{
			SECTR_AudioBus sectr_AudioBus = SECTR_AudioSystem._FindBus(SECTR_AudioSystem.System.MasterBus, "SFX");
			if (sectr_AudioBus == null)
			{
				yield break;
			}
			foreach (SECTR_AudioBus sectr_AudioBus2 in sectr_AudioBus.Children)
			{
				if (sectr_AudioBus2.name != "UI" && sectr_AudioBus2.name != "Pause Transition")
				{
					yield return sectr_AudioBus2;
				}
			}
			List<SECTR_AudioBus>.Enumerator enumerator = default(List<SECTR_AudioBus>.Enumerator);
		}
		yield break;
		yield break;
	}

	// Token: 0x06000281 RID: 641 RVA: 0x0000FB9C File Offset: 0x0000DD9C
	public static void PauseNonUISFX(bool pause)
	{
		foreach (SECTR_AudioBus bus in SECTR_AudioSystem.FindNonUISFX())
		{
			SECTR_AudioSystem.PauseBus(bus, pause);
		}
	}

	// Token: 0x06000282 RID: 642 RVA: 0x0000FBE8 File Offset: 0x0000DDE8
	public static void MuteNonUISFX(bool mute)
	{
		foreach (SECTR_AudioBus bus in SECTR_AudioSystem.FindNonUISFX())
		{
			SECTR_AudioSystem.MuteBus(bus, mute);
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x0000FC34 File Offset: 0x0000DE34
	private void OnEnable()
	{
		if (SECTR_AudioSystem.system && SECTR_AudioSystem.system != this)
		{
			Log.Error("Found duplicate SECTR_AudioSystem singleton instance.", Array.Empty<object>());
			Destroyer.Destroy(this, "SECTR_AudioSystem.OnEnable");
			return;
		}
		if (SECTR_AudioSystem.system == null)
		{
			SECTR_AudioSystem.system = this;
			SECTR_AudioSystem.instancePool = new Stack<SECTR_AudioSystem.Instance>(this.MaxInstances);
			for (int i = 0; i < this.MaxInstances; i++)
			{
				SECTR_AudioSystem.instancePool.Push(new SECTR_AudioSystem.Instance());
			}
			int num = SECTR_Modules.HasPro() ? Mathf.Max(0, this.MaxInstances - this.LowpassInstances) : this.MaxInstances;
			int num2 = this.MaxInstances - num;
			SECTR_AudioSystem.simpleSourcePool = new Stack<AudioSource>(num);
			SECTR_AudioSystem.lowpassSourcePool = (SECTR_Modules.HasPro() ? new Stack<AudioSource>(num2) : null);
			HideFlags hideFlags = HideFlags.HideAndDontSave;
			SECTR_AudioSystem.sourcePoolParent = new GameObject("SourcePool")
			{
				hideFlags = hideFlags
			}.transform;
			SECTR_AudioSystem.sourcePoolParent.transform.parent = SECTR_AudioSystem.sourcePoolParent;
			for (int j = 0; j < num; j++)
			{
				GameObject gameObject = new GameObject("SimpleInstance" + j);
				gameObject.hideFlags = hideFlags;
				gameObject.transform.parent = SECTR_AudioSystem.sourcePoolParent.transform;
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.playOnAwake = false;
				gameObject.SetActive(false);
				SECTR_AudioSystem.simpleSourcePool.Push(audioSource);
			}
			for (int k = 0; k < num2; k++)
			{
				GameObject gameObject2 = new GameObject("LowpassInstance" + k);
				gameObject2.hideFlags = hideFlags;
				gameObject2.transform.parent = SECTR_AudioSystem.sourcePoolParent.transform;
				AudioSource audioSource2 = gameObject2.AddComponent<AudioSource>();
				audioSource2.playOnAwake = false;
				gameObject2.AddComponent<AudioLowPassFilter>().enabled = false;
				gameObject2.SetActive(false);
				SECTR_AudioSystem.lowpassSourcePool.Push(audioSource2);
			}
			SECTR_AudioSystem.ambienceStack = new List<SECTR_AudioAmbience>(32);
			SECTR_AudioSystem.activeInstances = new List<SECTR_AudioSystem.Instance>(this.MaxInstances);
			SECTR_AudioSystem.maxInstancesTable = new Dictionary<SECTR_AudioCue, List<SECTR_AudioSystem.Instance>>(this.MaxInstances / 8);
			SECTR_AudioSystem.proximityTable = new Dictionary<SECTR_AudioCue, List<SECTR_AudioSystem.Instance>>(this.MaxInstances / 8);
			SECTR_AudioSystem._UpdateTime();
			SECTR_AudioSystem.windowHDRMax = this.HDRBaseLoudness;
			SECTR_AudioSystem.windowHDRMin = SECTR_AudioSystem.windowHDRMax - this.HDRWindowSize;
			SECTR_AudioSystem.occlusionPath = new List<SECTR_Graph.Node>(32);
			if (this.MasterBus != null)
			{
				this.MasterBus.ResetUserVolume();
				SECTR_AudioSystem._UpdateBusPitchVolume(this.MasterBus, 1f, 1f);
			}
			else
			{
				Debug.LogWarning("SECTR AudioSystem has no MasterBus. Game sounds will not play.");
			}
			this.MasterBus.ResetPauseState();
		}
	}

	// Token: 0x06000284 RID: 644 RVA: 0x0000FEC0 File Offset: 0x0000E0C0
	private void OnDisable()
	{
		if (SECTR_AudioSystem.system == this)
		{
			int count = SECTR_AudioSystem.activeInstances.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_AudioSystem.activeInstances[i].Stop(true);
			}
			this.MasterBus.ResetPauseState();
			if (SECTR_AudioSystem.sourcePoolParent)
			{
				Destroyer.Destroy(SECTR_AudioSystem.sourcePoolParent.gameObject, "SECTR_AudioSystem.OnDisable#1");
				SECTR_AudioSystem.sourcePoolParent = null;
			}
			SECTR_AudioSystem.system = null;
			SECTR_AudioSystem.activeInstances = null;
			SECTR_AudioSystem.maxInstancesTable = null;
			SECTR_AudioSystem.proximityTable = null;
			SECTR_AudioSystem.instancePool = null;
			SECTR_AudioSystem.simpleSourcePool = null;
			SECTR_AudioSystem.lowpassSourcePool = null;
			SECTR_AudioSystem.currentTime = 0f;
			SECTR_AudioSystem.ambienceStack = null;
			SECTR_AudioSystem.currentAmbience = null;
			SECTR_AudioSystem.nextAmbienceOneShotTime = 0f;
			SECTR_AudioSystem.currentMusic = null;
			SECTR_AudioSystem.occlusionPath = null;
		}
	}

	// Token: 0x06000285 RID: 645 RVA: 0x0000FF8C File Offset: 0x0000E18C
	private void LateUpdate()
	{
		if (SECTR_AudioSystem.system == this && !AudioListener.pause && this.MasterBus)
		{
			float num = SECTR_AudioSystem._UpdateTime();
			SECTR_AudioSystem._UpdateBusPitchVolume(this.MasterBus, 1f, 1f);
			SECTR_AudioSystem._UpdateAmbience();
			SECTR_AudioSystem.windowHDRMax = Mathf.Max(this.HDRBaseLoudness, SECTR_AudioSystem.windowHDRMax - this.HDRDecay * num);
			SECTR_AudioSystem.windowHDRMin = SECTR_AudioSystem.windowHDRMax - this.HDRWindowSize;
			SECTR_AudioSystem.currentLoudness = 0f;
			int num2 = SECTR_AudioSystem.activeInstances.Count;
			int i = 0;
			while (i < num2)
			{
				SECTR_AudioSystem.Instance instance = SECTR_AudioSystem.activeInstances[i];
				instance.Update(num, false);
				if (!instance.Active && !instance.FadingOut)
				{
					instance.Uninit();
					SECTR_AudioSystem.activeInstances.RemoveAt(i);
					SECTR_AudioSystem.instancePool.Push(instance);
					num2--;
				}
				else
				{
					i++;
				}
			}
			SECTR_AudioSystem.currentLoudness = 10f * Mathf.Log10(SECTR_AudioSystem.currentLoudness);
			SECTR_AudioSystem.windowHDRMax = Mathf.Max(SECTR_AudioSystem.currentLoudness, SECTR_AudioSystem.windowHDRMax);
		}
	}

	// Token: 0x06000286 RID: 646 RVA: 0x000100A4 File Offset: 0x0000E2A4
	private static bool _CheckInstances(SECTR_AudioCue audioCue, bool isPlaying)
	{
		int num = audioCue.SourceCue.MaxInstances;
		if (isPlaying)
		{
			num++;
		}
		return num <= 0 || SECTR_AudioSystem.GetInstancesCount(audioCue) < num;
	}

	// Token: 0x06000287 RID: 647 RVA: 0x000100D4 File Offset: 0x0000E2D4
	private static bool _CheckProximity(SECTR_AudioCue audioCue, Transform parent, Vector3 position, SECTR_AudioSystem.Instance testInstance)
	{
		if (parent)
		{
			position = parent.localToWorldMatrix.MultiplyPoint3x4(position);
		}
		SECTR_AudioCue sourceCue = audioCue.SourceCue;
		float num = sourceCue.MaxDistance + SECTR_AudioSystem.system.CullingBuffer;
		if (Vector3.SqrMagnitude(position - SECTR_AudioSystem.Listener.position) <= num * num)
		{
			int proximityLimit = sourceCue.ProximityLimit;
			List<SECTR_AudioSystem.Instance> list;
			if (proximityLimit > 0 && SECTR_AudioSystem.proximityTable.TryGetValue(audioCue, out list))
			{
				int count = list.Count;
				if (count > proximityLimit)
				{
					float num2 = sourceCue.MaxDistance + sourceCue.MaxDistance;
					int num3 = 0;
					for (int i = 0; i < count; i++)
					{
						SECTR_AudioSystem.Instance instance = list[i];
						if (instance != testInstance && Vector3.SqrMagnitude(position - instance.Position) < num2 && ++num3 >= proximityLimit)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000288 RID: 648 RVA: 0x000101B0 File Offset: 0x0000E3B0
	private static float _UpdateTime()
	{
		float num = (float)AudioSettings.dspTime;
		float result = num - SECTR_AudioSystem.currentTime;
		SECTR_AudioSystem.currentTime = num;
		return result;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x000101D4 File Offset: 0x0000E3D4
	private static void _UpdateBusPitchVolume(SECTR_AudioBus bus, float effectiveVolume, float effectivePitch)
	{
		if (bus)
		{
			bus.EffectiveVolume = effectiveVolume;
			bus.EffectivePitch = effectivePitch;
			int count = bus.Children.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_AudioSystem._UpdateBusPitchVolume(bus.Children[i], bus.EffectiveVolume, bus.EffectivePitch);
			}
		}
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0001022C File Offset: 0x0000E42C
	private static void _UpdateAmbience()
	{
		SECTR_AudioAmbience sectr_AudioAmbience = (SECTR_AudioSystem.ambienceStack.Count > 0) ? SECTR_AudioSystem.ambienceStack[SECTR_AudioSystem.ambienceStack.Count - 1] : SECTR_AudioSystem.system.DefaultAmbience;
		if (sectr_AudioAmbience != SECTR_AudioSystem.currentAmbience)
		{
			SECTR_AudioSystem.ambienceLoop.Stop(false);
			SECTR_AudioSystem.ambienceOneShot.Stop(false);
			SECTR_AudioSystem.currentAmbience = sectr_AudioAmbience;
			if (SECTR_AudioSystem.currentAmbience != null)
			{
				if (SECTR_AudioSystem.currentAmbience.OneShots.Count > 0)
				{
					SECTR_AudioSystem.nextAmbienceOneShotTime = SECTR_AudioSystem.currentTime + UnityEngine.Random.Range(SECTR_AudioSystem.currentAmbience.OneShotInterval.x, SECTR_AudioSystem.currentAmbience.OneShotInterval.y);
				}
				if (SECTR_AudioSystem.currentAmbience.BackgroundLoop)
				{
					if (SECTR_AudioSystem.currentAmbience.BackgroundLoop.Spatialization == SECTR_AudioCue.Spatializations.Infinite3D)
					{
						SECTR_AudioSystem.ambienceLoop = SECTR_AudioSystem.Play(SECTR_AudioSystem.currentAmbience.BackgroundLoop, SECTR_AudioSystem.Listener, UnityEngine.Random.onUnitSphere, true, null, false);
					}
					else
					{
						SECTR_AudioSystem.ambienceLoop = SECTR_AudioSystem.Play(SECTR_AudioSystem.currentAmbience.BackgroundLoop, SECTR_AudioSystem.Listener, Vector3.zero, true, null, false);
					}
				}
			}
		}
		if (SECTR_AudioSystem.currentAmbience != null)
		{
			if (SECTR_AudioSystem.currentAmbience.OneShots.Count > 0 && SECTR_AudioSystem.currentTime >= SECTR_AudioSystem.nextAmbienceOneShotTime)
			{
				SECTR_AudioCue sectr_AudioCue = SECTR_AudioSystem.currentAmbience.OneShots[UnityEngine.Random.Range(0, SECTR_AudioSystem.currentAmbience.OneShots.Count)];
				if (sectr_AudioCue != null)
				{
					if (sectr_AudioCue.SourceCue.Loops)
					{
						Debug.LogWarning("Cannot play ambient one shot " + sectr_AudioCue.name + ". It is set to loop.");
					}
					else
					{
						if (!sectr_AudioCue.IsLocal)
						{
							Debug.LogWarning("Ambient one shot " + sectr_AudioCue.name + "should be 2D or Infinite 3D.");
						}
						SECTR_AudioSystem.ambienceOneShot = SECTR_AudioSystem.Play(sectr_AudioCue, SECTR_AudioSystem.Listener, UnityEngine.Random.onUnitSphere, false, null, false);
					}
				}
				SECTR_AudioSystem.nextAmbienceOneShotTime = SECTR_AudioSystem.currentTime + UnityEngine.Random.Range(SECTR_AudioSystem.currentAmbience.OneShotInterval.x, SECTR_AudioSystem.currentAmbience.OneShotInterval.y);
			}
			if (SECTR_AudioSystem.ambienceLoop)
			{
				SECTR_AudioSystem.ambienceLoop.Volume = SECTR_AudioSystem.currentAmbience.Volume;
			}
			if (SECTR_AudioSystem.ambienceOneShot)
			{
				SECTR_AudioSystem.ambienceOneShot.Volume = SECTR_AudioSystem.currentAmbience.Volume;
			}
		}
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00010488 File Offset: 0x0000E688
	private static SECTR_AudioBus _FindBus(SECTR_AudioBus bus, string busName)
	{
		if (bus)
		{
			if (bus.name == busName)
			{
				return bus;
			}
			int count = bus.Children.Count;
			for (int i = 0; i < count; i++)
			{
				SECTR_AudioBus sectr_AudioBus = SECTR_AudioSystem._FindBus(bus.Children[i], busName);
				if (sectr_AudioBus)
				{
					return sectr_AudioBus;
				}
			}
		}
		return null;
	}

	// Token: 0x0400029D RID: 669
	private static SECTR_AudioSystem system = null;

	// Token: 0x0400029E RID: 670
	private static Stack<SECTR_AudioSystem.Instance> instancePool = null;

	// Token: 0x0400029F RID: 671
	private static Stack<AudioSource> simpleSourcePool = null;

	// Token: 0x040002A0 RID: 672
	private static Stack<AudioSource> lowpassSourcePool = null;

	// Token: 0x040002A1 RID: 673
	private static Transform sourcePoolParent = null;

	// Token: 0x040002A2 RID: 674
	private static List<SECTR_AudioSystem.Instance> activeInstances = null;

	// Token: 0x040002A3 RID: 675
	private static Dictionary<SECTR_AudioCue, List<SECTR_AudioSystem.Instance>> maxInstancesTable = null;

	// Token: 0x040002A4 RID: 676
	private static Dictionary<SECTR_AudioCue, List<SECTR_AudioSystem.Instance>> proximityTable = null;

	// Token: 0x040002A5 RID: 677
	private static float currentTime = 0f;

	// Token: 0x040002A6 RID: 678
	private static List<SECTR_AudioAmbience> ambienceStack;

	// Token: 0x040002A7 RID: 679
	private static SECTR_AudioAmbience currentAmbience = null;

	// Token: 0x040002A8 RID: 680
	private static SECTR_AudioCueInstance ambienceLoop;

	// Token: 0x040002A9 RID: 681
	private static SECTR_AudioCueInstance ambienceOneShot;

	// Token: 0x040002AA RID: 682
	private static float nextAmbienceOneShotTime = 0f;

	// Token: 0x040002AB RID: 683
	private static SECTR_AudioCue currentMusic = null;

	// Token: 0x040002AC RID: 684
	private static SECTR_AudioCueInstance musicLoop;

	// Token: 0x040002AD RID: 685
	private static float windowHDRMax = 0f;

	// Token: 0x040002AE RID: 686
	private static float windowHDRMin = 0f;

	// Token: 0x040002AF RID: 687
	private static float currentLoudness = 0f;

	// Token: 0x040002B0 RID: 688
	private static List<SECTR_Graph.Node> occlusionPath;

	// Token: 0x040002B1 RID: 689
	private const float EPSILON = 0.001f;

	// Token: 0x040002B2 RID: 690
	[SECTR_ToolTip("The maximum number of instances that can be active at once. Inaudible sounds do not count against this limit.")]
	public int MaxInstances = 128;

	// Token: 0x040002B3 RID: 691
	[SECTR_ToolTip("The number of instances to allocate with lowpass effects (for occlusion and the like).")]
	public int LowpassInstances = 32;

	// Token: 0x040002B4 RID: 692
	[SECTR_ToolTip("The Bus at the top of the mixing heirarchy. Required to play sounds.", null, false)]
	public SECTR_AudioBus MasterBus;

	// Token: 0x040002B5 RID: 693
	[SECTR_ToolTip("The baseline settings for any environmental audio. Will be audible when no other ambiences are active.")]
	public SECTR_AudioAmbience DefaultAmbience = new SECTR_AudioAmbience();

	// Token: 0x040002B6 RID: 694
	[SECTR_ToolTip("Minimum Loudness for the HDR mixer. Current Loudness will never drop below this.", 0f, 200f)]
	public float HDRBaseLoudness = 50f;

	// Token: 0x040002B7 RID: 695
	[SECTR_ToolTip("The maximum difference between the loudest sound and the softest sound before sounds are simply culled out.", 0f, 200f)]
	public float HDRWindowSize = 50f;

	// Token: 0x040002B8 RID: 696
	[SECTR_ToolTip("Speed at which HDR window decays after a loud sound is played.", 0f, 100f)]
	public float HDRDecay = 1f;

	// Token: 0x040002B9 RID: 697
	[SECTR_ToolTip("Should sounds close to the listener be blended into 2D (to avoid harsh stereo switching).")]
	public bool BlendNearbySounds = true;

	// Token: 0x040002BA RID: 698
	[SECTR_ToolTip("Objects close to the listener will be blended into 2D, as a kind of fake HRTF. This determines the start and end of that blend.", "BlendNearbySounds")]
	public Vector2 NearBlendRange = new Vector2(0.25f, 0.75f);

	// Token: 0x040002BB RID: 699
	[SECTR_ToolTip("Determines what kind of logic to use for computing sound occlusion.", null, typeof(SECTR_AudioSystem.OcclusionModes))]
	public SECTR_AudioSystem.OcclusionModes OcclusionFlags;

	// Token: 0x040002BC RID: 700
	[SECTR_ToolTip("The distance beyond which sounds will be considered occluded, if Distance occlusion is enabled.", "OcclusionFlags")]
	public float OcclusionDistance = 100f;

	// Token: 0x040002BD RID: 701
	[SECTR_ToolTip("The layers to test against when raycasting for occlusion.", "OcclusionFlags")]
	public LayerMask RaycastLayers = -5;

	// Token: 0x040002BE RID: 702
	[SECTR_ToolTip("The amount by which to decrease the volume of occluded sounds.", "OcclusionFlags", 0f, 1f)]
	public float OcclusionVolume = 0.5f;

	// Token: 0x040002BF RID: 703
	[SECTR_ToolTip("The frequency cutoff of the lowpass filter for occluded sounds.", "OcclusionFlags", 10f, 22000f)]
	public float OcclusionCutoff = 2200f;

	// Token: 0x040002C0 RID: 704
	[SECTR_ToolTip("The resonance Q of the lowpass filter for occluded sounds.", "OcclusionFlags", 1f, 10f)]
	public float OcclusionResonanceQ = 1f;

	// Token: 0x040002C1 RID: 705
	[SECTR_ToolTip("The amount of time between tests to see if looping sounds should start or stop running.")]
	public Vector2 RetestInterval = new Vector2(0.5f, 1f);

	// Token: 0x040002C2 RID: 706
	[SECTR_ToolTip("The amount of buffer to give before culling distant sounds.")]
	public float CullingBuffer = 10f;

	// Token: 0x040002C3 RID: 707
	[SECTR_ToolTip("Enable or disable of the in-game audio HUD.", true)]
	public bool ShowAudioHUD;

	// Token: 0x040002C4 RID: 708
	[SECTR_ToolTip("In the editor only, puts the listener at the AudioSystem, not at the Scene Camera.", true)]
	public bool Debugging;

	// Token: 0x040002C5 RID: 709
	private static bool firstMaxInstanceWarning = true;

	// Token: 0x0200007F RID: 127
	private class Instance : SECTR_IAudioInstance
	{
		// Token: 0x1700004F RID: 79
		// (get) Token: 0x0600028E RID: 654 RVA: 0x0001062D File Offset: 0x0000E82D
		public int Generation
		{
			get
			{
				return this.generation;
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x0600028F RID: 655 RVA: 0x00010638 File Offset: 0x0000E838
		public bool Active
		{
			get
			{
				return (this.Loops || this.Delayed || (this.source && (this.source.isPlaying || this.Paused || AudioListener.pause))) && !this.FadingOut && (this.source == null || this.source.enabled);
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000290 RID: 656 RVA: 0x000106A4 File Offset: 0x0000E8A4
		// (set) Token: 0x06000291 RID: 657 RVA: 0x00010704 File Offset: 0x0000E904
		public Vector3 Position
		{
			get
			{
				Vector3 vector = this.localPosition;
				if (this.parent)
				{
					if (this.ThreeD && this.Local)
					{
						vector += this.parent.transform.position;
					}
					else
					{
						vector = this.parent.localToWorldMatrix.MultiplyPoint3x4(vector);
					}
				}
				return vector;
			}
			set
			{
				if (this.parent)
				{
					if (this.ThreeD && this.Local)
					{
						this.localPosition = value - this.parent.transform.position;
					}
					else
					{
						this.localPosition = this.parent.worldToLocalMatrix.MultiplyPoint3x4(value);
					}
				}
				else
				{
					this.localPosition = value;
				}
				if (this.source)
				{
					this.source.transform.position = value;
				}
			}
		}

		// Token: 0x17000052 RID: 82
		// (get) Token: 0x06000292 RID: 658 RVA: 0x0001078D File Offset: 0x0000E98D
		// (set) Token: 0x06000293 RID: 659 RVA: 0x00010795 File Offset: 0x0000E995
		public Vector3 LocalPosition
		{
			get
			{
				return this.localPosition;
			}
			set
			{
				this.localPosition = value;
				if (this.source)
				{
					this.source.transform.position = this.Position;
				}
			}
		}

		// Token: 0x17000053 RID: 83
		// (get) Token: 0x06000294 RID: 660 RVA: 0x000107C1 File Offset: 0x0000E9C1
		// (set) Token: 0x06000295 RID: 661 RVA: 0x000107C9 File Offset: 0x0000E9C9
		public float Volume
		{
			get
			{
				return this.userVolume;
			}
			set
			{
				if (this.userVolume != value)
				{
					this.userVolume = Mathf.Clamp01(value);
					this.Update(0f, true);
				}
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x06000296 RID: 662 RVA: 0x000107EC File Offset: 0x0000E9EC
		// (set) Token: 0x06000297 RID: 663 RVA: 0x000107F4 File Offset: 0x0000E9F4
		public float Pitch
		{
			get
			{
				return this.userPitch;
			}
			set
			{
				if (this.userPitch != value)
				{
					this.userPitch = Mathf.Clamp(value, 0f, 2f);
					this.Update(0f, true);
				}
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x06000298 RID: 664 RVA: 0x00010821 File Offset: 0x0000EA21
		// (set) Token: 0x06000299 RID: 665 RVA: 0x00010829 File Offset: 0x0000EA29
		public bool Mute
		{
			get
			{
				return this.Mute;
			}
			set
			{
				if (this.Muted != value)
				{
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.Muted, value);
					if (this.source)
					{
						this.source.mute = value;
					}
				}
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x0600029A RID: 666 RVA: 0x00010855 File Offset: 0x0000EA55
		// (set) Token: 0x0600029B RID: 667 RVA: 0x00010876 File Offset: 0x0000EA76
		public float TimeSeconds
		{
			get
			{
				if (!(this.source != null))
				{
					return 0f;
				}
				return this.source.time;
			}
			set
			{
				if (this.source)
				{
					this.source.time = value;
				}
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x0600029C RID: 668 RVA: 0x00010891 File Offset: 0x0000EA91
		// (set) Token: 0x0600029D RID: 669 RVA: 0x000108AE File Offset: 0x0000EAAE
		public int TimeSamples
		{
			get
			{
				if (!(this.source != null))
				{
					return 0;
				}
				return this.source.timeSamples;
			}
			set
			{
				if (this.source)
				{
					this.source.timeSamples = value;
				}
			}
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x0600029E RID: 670 RVA: 0x000108C9 File Offset: 0x0000EAC9
		public bool HasPriority
		{
			get
			{
				return this.hasPriority;
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x000108D4 File Offset: 0x0000EAD4
		public void ForceInfinite()
		{
			this._SetFlag(SECTR_AudioSystem.Instance.Flags.ForcedInfinite, true);
			this._SetFlag(SECTR_AudioSystem.Instance.Flags.Local, true);
			this._SetFlag(SECTR_AudioSystem.Instance.Flags.ThreeD, true);
			this.occlusionAlpha = 1f;
			if (this.source)
			{
				this.source.rolloffMode = AudioRolloffMode.Linear;
				this.source.maxDistance = 1000000f;
				this.source.minDistance = this.source.maxDistance - 0.001f;
				this.source.dopplerLevel = 0f;
			}
			this.Update(0f, true);
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0001096B File Offset: 0x0000EB6B
		public void ForceOcclusion(bool occluded)
		{
			if (this.audioCue && this.audioCue.SourceCue.Spatialization == SECTR_AudioCue.Spatializations.Occludable3D)
			{
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.Occluded, occluded);
			}
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060002A1 RID: 673 RVA: 0x00010999 File Offset: 0x0000EB99
		public bool Loops
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Loops) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060002A2 RID: 674 RVA: 0x000109A6 File Offset: 0x0000EBA6
		public bool Local
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Local) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x1700005B RID: 91
		// (get) Token: 0x060002A3 RID: 675 RVA: 0x000109B4 File Offset: 0x0000EBB4
		public bool ThreeD
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.ThreeD) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x060002A4 RID: 676 RVA: 0x000109C2 File Offset: 0x0000EBC2
		public bool FadingIn
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.FadingIn) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x060002A5 RID: 677 RVA: 0x000109CF File Offset: 0x0000EBCF
		public bool FadingOut
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.FadingOut) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x060002A6 RID: 678 RVA: 0x000109DC File Offset: 0x0000EBDC
		public bool Muted
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Muted) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x1700005F RID: 95
		// (get) Token: 0x060002A7 RID: 679 RVA: 0x000109E9 File Offset: 0x0000EBE9
		public bool Paused
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Paused) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002A8 RID: 680 RVA: 0x000109F7 File Offset: 0x0000EBF7
		public bool HDR
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.HDR) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x060002A9 RID: 681 RVA: 0x00010A08 File Offset: 0x0000EC08
		public bool Occludable
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Occludable) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x060002AA RID: 682 RVA: 0x00010A19 File Offset: 0x0000EC19
		public bool Occluded
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Occluded) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x060002AB RID: 683 RVA: 0x00010A2A File Offset: 0x0000EC2A
		public bool ForcedInfinite
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.ForcedInfinite) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x060002AC RID: 684 RVA: 0x00010A3B File Offset: 0x0000EC3B
		public bool Delayed
		{
			get
			{
				return (this.flags & SECTR_AudioSystem.Instance.Flags.Delayed) > (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x17000065 RID: 101
		// (get) Token: 0x060002AD RID: 685 RVA: 0x00010A4C File Offset: 0x0000EC4C
		public SECTR_AudioBus Bus
		{
			get
			{
				if (!(this.audioCue != null))
				{
					return null;
				}
				return this.audioCue.Bus;
			}
		}

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x060002AE RID: 686 RVA: 0x00010A69 File Offset: 0x0000EC69
		public SECTR_AudioCue Cue
		{
			get
			{
				return this.audioCue;
			}
		}

		// Token: 0x060002AF RID: 687 RVA: 0x00010A74 File Offset: 0x0000EC74
		public void Init(SECTR_AudioCue audioCue, Transform parent, Vector3 localPosition, bool loops, int? clipIndex, bool hasPriority)
		{
			if (this.audioCue == null)
			{
				this.generation++;
				this.audioCue = audioCue;
				this.clipIndex = clipIndex;
				this.hasPriority = hasPriority;
				SECTR_AudioCue sourceCue = audioCue.SourceCue;
				this.flags = (SECTR_AudioSystem.Instance.Flags)0;
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.Loops, loops);
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.Local, sourceCue.IsLocal);
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.ThreeD, sourceCue.Is3D);
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.HDR, sourceCue.HDR);
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.Occludable, SECTR_AudioSystem.system.OcclusionFlags != (SECTR_AudioSystem.OcclusionModes)0 && sourceCue.Spatialization == SECTR_AudioCue.Spatializations.Occludable3D);
				this.userVolume = 1f;
				this.userPitch = 1f;
				if (this.Local)
				{
					this.parent = SECTR_AudioSystem.Listener;
				}
				else
				{
					this.parent = parent;
				}
				this.localPosition = localPosition;
				this._AddProximityInstance(sourceCue);
				this._ScheduleNextTest();
			}
		}

		// Token: 0x060002B0 RID: 688 RVA: 0x00010B64 File Offset: 0x0000ED64
		public void Clone(SECTR_AudioSystem.Instance instance, Vector3 newPosition)
		{
			if (instance.Active)
			{
				this.generation++;
				this.audioCue = instance.audioCue;
				this.flags = instance.flags;
				this.fadeStarTime = instance.fadeStarTime;
				this.basePitch = instance.basePitch;
				this.baseVolumeLoudness = instance.baseVolumeLoudness;
				this.userVolume = instance.userVolume;
				this.userPitch = instance.userPitch;
				this.occlusionAlpha = instance.occlusionAlpha;
				this.hdrCurve = instance.hdrCurve;
				this.parent = instance.parent;
				this.Position = newPosition;
				this._AddProximityInstance(this.audioCue.SourceCue);
				this._ScheduleNextTest();
				if (this._AcquireSource())
				{
					this.Update(0f, true);
					if (this.source)
					{
						this._SetFlag(SECTR_AudioSystem.Instance.Flags.Paused, false);
						this.source.clip = instance.source.clip;
						this.source.timeSamples = instance.source.timeSamples;
						this.PlaySource();
					}
				}
			}
		}

		// Token: 0x060002B1 RID: 689 RVA: 0x00010C7C File Offset: 0x0000EE7C
		public void Uninit()
		{
			if (this.audioCue != null)
			{
				List<SECTR_AudioSystem.Instance> list;
				if (this.audioCue.SourceCue.ProximityLimit > 0 && SECTR_AudioSystem.proximityTable.TryGetValue(this.audioCue, out list))
				{
					list.Remove(this);
				}
				this._ReleaseSource();
				this.audioCue = null;
				this.parent = null;
				this.flags = (SECTR_AudioSystem.Instance.Flags)0;
			}
		}

		// Token: 0x060002B2 RID: 690 RVA: 0x00010CE4 File Offset: 0x0000EEE4
		public void Play()
		{
			SECTR_AudioCue.ClipData clipData = (this.clipIndex != null) ? this.audioCue.AudioClips[this.clipIndex.Value] : this.audioCue.GetNextClip();
			if (clipData != null && clipData.Clip != null && this._AcquireSource())
			{
				if (clipData.Clip.loadState == AudioDataLoadState.Unloaded)
				{
					clipData.Clip.LoadAudioData();
				}
				SECTR_AudioCue sourceCue = this.audioCue.SourceCue;
				if (sourceCue.FadeInTime > 0f)
				{
					this.fadeStarTime = SECTR_AudioSystem.currentTime;
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingIn, true);
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingOut, false);
				}
				if (this.Occludable && !this.ForcedInfinite)
				{
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.Occluded, SECTR_AudioSystem.IsOccluded(this.Position, SECTR_AudioSystem.system.OcclusionFlags));
					this.occlusionAlpha = (this.Occluded ? 1f : 0f);
				}
				if (this.HDR)
				{
					this.baseVolumeLoudness = UnityEngine.Random.Range(sourceCue.Loudness.x, sourceCue.Loudness.y);
				}
				else
				{
					this.baseVolumeLoudness = UnityEngine.Random.Range(sourceCue.Volume.x, sourceCue.Volume.y);
				}
				this.baseVolumeLoudness *= clipData.Volume;
				if (this.HDR)
				{
					if (clipData.HDRCurve != null && clipData.HDRCurve.length > 0)
					{
						this.hdrCurve = clipData.HDRCurve;
					}
					else
					{
						Debug.LogWarning("Playing " + this.audioCue.name + " without HDR keys. Bake HDR keys for higher quality audio.");
					}
				}
				this.Update(0f, true);
				if (this.source)
				{
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.Paused, false);
					this.source.clip = clipData.Clip;
					if (sourceCue.Delay.y > 0f)
					{
						this._SetFlag(SECTR_AudioSystem.Instance.Flags.Delayed, true);
						this.nextTestTime = SECTR_AudioSystem.currentTime + UnityEngine.Random.Range(sourceCue.Delay.x, sourceCue.Delay.y);
						return;
					}
					this.PlaySource();
				}
			}
		}

		// Token: 0x060002B3 RID: 691 RVA: 0x00010F0C File Offset: 0x0000F10C
		public void Pause(bool paused)
		{
			if (paused)
			{
				this.pauseCount++;
			}
			else
			{
				this.pauseCount = Math.Max(0, this.pauseCount - 1);
			}
			paused = (this.pauseCount > 0);
			this._SetFlag(SECTR_AudioSystem.Instance.Flags.Paused, paused);
			if (this.source)
			{
				if (paused)
				{
					this.source.Pause();
					return;
				}
				if (!this.source.isPlaying)
				{
					this.PlaySource();
				}
			}
		}

		// Token: 0x060002B4 RID: 692 RVA: 0x00010F84 File Offset: 0x0000F184
		public void PlaySource()
		{
			if (this.source != null && this.Bus != null)
			{
				if (!this.Bus.Paused)
				{
					this.source.Play();
					return;
				}
				if (this.Loops || this.source.loop)
				{
					this.source.Play();
					this.source.Pause();
					return;
				}
			}
			else if (this.source != null)
			{
				this.source.Play();
			}
		}

		// Token: 0x060002B5 RID: 693 RVA: 0x0001100B File Offset: 0x0000F20B
		public void Stop(bool stopImmediately)
		{
			this._SetFlag(SECTR_AudioSystem.Instance.Flags.Loops, false);
			this._Stop(stopImmediately);
		}

		// Token: 0x060002B6 RID: 694 RVA: 0x0001101C File Offset: 0x0000F21C
		public void SkipFadeIn()
		{
			this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingIn, false);
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x00011028 File Offset: 0x0000F228
		public void Update(float deltaTime, bool volumeOnly)
		{
			if (this.Delayed)
			{
				if (SECTR_AudioSystem.currentTime < this.nextTestTime)
				{
					return;
				}
				if (this.source != null)
				{
					this.PlaySource();
				}
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.Delayed, false);
				this._ScheduleNextTest();
			}
			Vector3 position;
			if (this.ThreeD)
			{
				position = this.Position;
				if (this.source)
				{
					this.source.transform.position = position;
				}
			}
			else
			{
				position = SECTR_AudioSystem.Listener.position;
			}
			float num = 1f;
			if (this.FadingIn)
			{
				num = Mathf.Clamp01((SECTR_AudioSystem.currentTime - this.fadeStarTime) / this.audioCue.SourceCue.FadeInTime);
				if (num >= 1f)
				{
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingIn, false);
				}
			}
			else if (this.FadingOut)
			{
				float num2 = SECTR_AudioSystem.currentTime - this.fadeStarTime;
				num = Mathf.Clamp01(1f - num2 / this.audioCue.SourceCue.FadeOutTime);
				if (num <= 0f)
				{
					this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingOut, false);
					this._Stop(true);
				}
			}
			if (this.source && (this.source.isPlaying || this.Paused || volumeOnly) && !this.Muted)
			{
				float num3 = this.audioCue.Bus ? this.audioCue.Bus.EffectiveVolume : SECTR_AudioSystem.system.MasterBus.EffectiveVolume;
				float num4 = this.audioCue.Bus ? this.audioCue.Bus.EffectivePitch : SECTR_AudioSystem.system.MasterBus.Pitch;
				float num10;
				if (this.HDR)
				{
					SECTR_AudioCue sourceCue = this.audioCue.SourceCue;
					float num5 = 1f;
					if (!this.Local)
					{
						float maxDistance = sourceCue.MaxDistance;
						float minDistance = sourceCue.MinDistance;
						Vector3 position2 = SECTR_AudioSystem.Listener.transform.position;
						float num6 = Vector3.SqrMagnitude(position - position2);
						if (num6 > maxDistance * maxDistance)
						{
							num5 = 0f;
						}
						else if (num6 > minDistance * minDistance)
						{
							float num7 = Mathf.Sqrt(num6);
							SECTR_AudioCue.FalloffTypes falloff = this.audioCue.SourceCue.Falloff;
							if (falloff != SECTR_AudioCue.FalloffTypes.Linear)
							{
								if (falloff == SECTR_AudioCue.FalloffTypes.Logrithmic)
								{
									num5 = Mathf.Clamp01(1f / Mathf.Max(num7 - minDistance - 1f, 0.001f));
								}
							}
							else
							{
								num5 = 1f - Mathf.Clamp01((num7 - minDistance) / (maxDistance - minDistance));
							}
						}
					}
					float num8 = this.baseVolumeLoudness;
					if (this.hdrCurve != null)
					{
						float num9 = this.hdrCurve.Evaluate(this.source.time);
						num8 += num9;
					}
					num8 += 20f * Mathf.Log10(Mathf.Max(this.userVolume * num * num5, 0.001f));
					if (num8 < SECTR_AudioSystem.windowHDRMin && (volumeOnly || (this.baseVolumeLoudness - SECTR_AudioSystem.windowHDRMin) / SECTR_AudioSystem.system.HDRDecay > this.source.time - this.source.clip.length))
					{
						this._Stop(false);
						return;
					}
					SECTR_AudioSystem.currentLoudness += Mathf.Pow(10f, num8 * 0.1f);
					num10 = Mathf.Clamp01(Mathf.Pow(10f, (num8 - SECTR_AudioSystem.windowHDRMax) * 0.05f));
				}
				else
				{
					num10 = this.baseVolumeLoudness * num * this.userVolume;
				}
				if (this.Occludable)
				{
					float num11 = 1f;
					this.occlusionAlpha += deltaTime * (this.Occluded ? num11 : (-num11));
					this.occlusionAlpha = Mathf.Clamp01(this.occlusionAlpha);
					float t = this.occlusionAlpha * this.audioCue.SourceCue.OcclusionScale;
					num10 *= Mathf.Lerp(1f, SECTR_AudioSystem.system.OcclusionVolume, t);
					if (this.lowpass)
					{
						this.lowpass.enabled = (this.occlusionAlpha > 0f);
						if (this.lowpass.enabled)
						{
							this.lowpass.cutoffFrequency = Mathf.Lerp(22000f, SECTR_AudioSystem.system.OcclusionCutoff, t);
							this.lowpass.lowpassResonanceQ = Mathf.Lerp(1f, SECTR_AudioSystem.system.OcclusionResonanceQ, t);
						}
					}
				}
				this.source.volume = Mathf.Clamp01(num10 * num3);
				this.source.pitch = Mathf.Clamp(this.userPitch * this.basePitch * num4, 0f, 2f);
			}
			if (volumeOnly)
			{
				return;
			}
			if (this.source && (this.source.isPlaying || this.Paused) && !this.Local && SECTR_AudioSystem.system.BlendNearbySounds)
			{
				float num12 = Vector3.SqrMagnitude(SECTR_AudioSystem.Listener.position - position);
				float spatialBlend;
				if (num12 <= SECTR_AudioSystem.system.NearBlendRange.x * SECTR_AudioSystem.system.NearBlendRange.x)
				{
					spatialBlend = 0f;
				}
				else if (num12 <= SECTR_AudioSystem.system.NearBlendRange.y * SECTR_AudioSystem.system.NearBlendRange.y)
				{
					spatialBlend = Mathf.Clamp01((Mathf.Sqrt(num12) - SECTR_AudioSystem.system.NearBlendRange.x) / (SECTR_AudioSystem.system.NearBlendRange.y - SECTR_AudioSystem.system.NearBlendRange.x));
				}
				else
				{
					spatialBlend = 1f;
				}
				this.source.spatialBlend = spatialBlend;
			}
			if (this.Loops && !this.Paused)
			{
				bool flag = this.source != null && this.source.isPlaying;
				bool flag2 = !flag && (!this.HDR || this.baseVolumeLoudness >= SECTR_AudioSystem.windowHDRMin);
				if (this.Local)
				{
					if (!flag && flag2 && SECTR_AudioSystem._CheckInstances(this.audioCue, flag))
					{
						this.Play();
						return;
					}
				}
				else if (SECTR_AudioSystem.currentTime >= this.nextTestTime)
				{
					bool flag3 = SECTR_AudioSystem._CheckProximity(this.audioCue, this.parent, this.localPosition, this);
					if (flag3 && !flag && flag2 && SECTR_AudioSystem._CheckInstances(this.audioCue, flag))
					{
						this.Play();
					}
					else if (!flag3 && flag)
					{
						this._Stop(true);
					}
					else if (this.Occludable && !this.ForcedInfinite)
					{
						this._SetFlag(SECTR_AudioSystem.Instance.Flags.Occluded, SECTR_AudioSystem.IsOccluded(position, SECTR_AudioSystem.system.OcclusionFlags));
					}
					this._ScheduleNextTest();
				}
			}
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x000116F1 File Offset: 0x0000F8F1
		private void _SetFlag(SECTR_AudioSystem.Instance.Flags flag, bool on)
		{
			if (on)
			{
				this.flags |= flag;
				return;
			}
			this.flags &= ~flag;
		}

		// Token: 0x060002B9 RID: 697 RVA: 0x00011714 File Offset: 0x0000F914
		private bool _AcquireSource()
		{
			if (!this.source)
			{
				SECTR_AudioCue sourceCue = this.audioCue.SourceCue;
				bool flag = this.Occludable && !sourceCue.BypassEffects && SECTR_Modules.HasPro() && SECTR_AudioSystem.lowpassSourcePool.Count > 0;
				this.source = (flag ? SECTR_AudioSystem.lowpassSourcePool.Pop() : SECTR_AudioSystem.simpleSourcePool.Pop());
				if (this.source)
				{
					if (flag)
					{
						this.lowpass = this.source.GetComponent<AudioLowPassFilter>();
						this.lowpass.enabled = false;
					}
					this.source.time = 0f;
					this.source.timeSamples = 0;
					this.source.priority = sourceCue.Priority;
					this.source.bypassEffects = sourceCue.BypassEffects;
					this.source.loop = sourceCue.Loops;
					this.source.spread = sourceCue.Spread;
					this.source.mute = this.Muted;
					this.basePitch = UnityEngine.Random.Range(sourceCue.Pitch.x, sourceCue.Pitch.y);
					if (sourceCue.MaxInstances > 0)
					{
						if (!SECTR_AudioSystem.maxInstancesTable.ContainsKey(this.audioCue))
						{
							SECTR_AudioSystem.maxInstancesTable[this.audioCue] = new List<SECTR_AudioSystem.Instance>();
						}
						SECTR_AudioSystem.maxInstancesTable[this.audioCue].Add(this);
					}
					this.source.panStereo = 0f;
					this.source.spatialBlend = 1f;
					this.source.bypassReverbZones = this.Local;
					if (this.Local)
					{
						if (this.ThreeD)
						{
							this.source.rolloffMode = AudioRolloffMode.Linear;
							this.source.maxDistance = 1000000f;
							this.source.minDistance = this.source.maxDistance - 0.001f;
						}
						else
						{
							this.source.panStereo = sourceCue.Pan2D;
							this.source.spatialBlend = 0f;
						}
						this.source.dopplerLevel = 0f;
						if ((SECTR_AudioSystem.currentAmbience != null && SECTR_AudioSystem.currentAmbience.BackgroundLoop == this.audioCue) || (SECTR_AudioSystem.currentMusic != null && SECTR_AudioSystem.currentMusic == this.audioCue))
						{
							this.source.priority = 0;
						}
					}
					else
					{
						if (this.HDR)
						{
							this.source.rolloffMode = AudioRolloffMode.Linear;
							this.source.minDistance = 1000000f;
							this.source.maxDistance = this.source.minDistance + 0.001f;
						}
						else
						{
							SECTR_AudioCue.FalloffTypes falloff = sourceCue.Falloff;
							if (falloff != SECTR_AudioCue.FalloffTypes.Linear && falloff == SECTR_AudioCue.FalloffTypes.Logrithmic)
							{
								this.source.rolloffMode = AudioRolloffMode.Logarithmic;
							}
							else
							{
								this.source.rolloffMode = AudioRolloffMode.Linear;
							}
							this.source.minDistance = sourceCue.MinDistance;
							this.source.maxDistance = Mathf.Max(sourceCue.MaxDistance, sourceCue.MinDistance + 0.001f);
						}
						this.source.dopplerLevel = sourceCue.DopplerLevel;
						this.source.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
					}
					this.source.transform.position = this.Position;
					this.source.gameObject.SetActive(true);
				}
			}
			return this.source != null;
		}

		// Token: 0x060002BA RID: 698 RVA: 0x00011A7C File Offset: 0x0000FC7C
		private void _ReleaseSource()
		{
			if (this.source != null)
			{
				if (this.audioCue.MaxInstances > 0 && SECTR_AudioSystem.maxInstancesTable.ContainsKey(this.audioCue) && SECTR_AudioSystem.maxInstancesTable[this.audioCue].Remove(this) && SECTR_AudioSystem.maxInstancesTable[this.audioCue].Count == 0)
				{
					SECTR_AudioSystem.maxInstancesTable.Remove(this.audioCue);
				}
				this.source.Stop();
				this.source.gameObject.SetActive(false);
				if (this.lowpass)
				{
					this.lowpass.enabled = false;
					SECTR_AudioSystem.lowpassSourcePool.Push(this.source);
				}
				else
				{
					SECTR_AudioSystem.simpleSourcePool.Push(this.source);
				}
				this.source = null;
				this.lowpass = null;
				this.hdrCurve = null;
			}
		}

		// Token: 0x060002BB RID: 699 RVA: 0x00011B68 File Offset: 0x0000FD68
		private void _AddProximityInstance(SECTR_AudioCue srcCue)
		{
			int proximityLimit = srcCue.ProximityLimit;
			if (proximityLimit > 0)
			{
				List<SECTR_AudioSystem.Instance> list;
				if (!SECTR_AudioSystem.proximityTable.TryGetValue(this.audioCue, out list))
				{
					list = new List<SECTR_AudioSystem.Instance>(proximityLimit * 2);
					SECTR_AudioSystem.proximityTable[this.audioCue] = list;
				}
				list.Add(this);
			}
		}

		// Token: 0x060002BC RID: 700 RVA: 0x00011BB5 File Offset: 0x0000FDB5
		private void _ScheduleNextTest()
		{
			this.nextTestTime = SECTR_AudioSystem.currentTime + UnityEngine.Random.Range(SECTR_AudioSystem.system.RetestInterval.x, SECTR_AudioSystem.system.RetestInterval.y);
		}

		// Token: 0x060002BD RID: 701 RVA: 0x00011BE8 File Offset: 0x0000FDE8
		private void _Stop(bool stopImmediately)
		{
			if (!stopImmediately && this.source && this.source.isPlaying && this.audioCue && this.audioCue.SourceCue.FadeOutTime > 0f)
			{
				if (this.FadingIn)
				{
					float num = 1f - Mathf.Clamp01((SECTR_AudioSystem.currentTime - this.fadeStarTime) / this.audioCue.SourceCue.FadeInTime);
					this.fadeStarTime = SECTR_AudioSystem.currentTime - num * this.audioCue.SourceCue.FadeOutTime;
				}
				else
				{
					this.fadeStarTime = SECTR_AudioSystem.currentTime;
				}
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingOut, true);
				this._SetFlag(SECTR_AudioSystem.Instance.Flags.FadingIn, false);
				return;
			}
			this._ReleaseSource();
		}

		// Token: 0x040002C6 RID: 710
		private int? clipIndex;

		// Token: 0x040002C7 RID: 711
		private bool hasPriority;

		// Token: 0x040002C8 RID: 712
		private int pauseCount;

		// Token: 0x040002C9 RID: 713
		private int generation;

		// Token: 0x040002CA RID: 714
		private AudioSource source;

		// Token: 0x040002CB RID: 715
		private AudioLowPassFilter lowpass;

		// Token: 0x040002CC RID: 716
		private SECTR_AudioCue audioCue;

		// Token: 0x040002CD RID: 717
		private Transform parent;

		// Token: 0x040002CE RID: 718
		private Vector3 localPosition = Vector3.zero;

		// Token: 0x040002CF RID: 719
		private SECTR_AudioSystem.Instance.Flags flags;

		// Token: 0x040002D0 RID: 720
		private float nextTestTime;

		// Token: 0x040002D1 RID: 721
		private float fadeStarTime;

		// Token: 0x040002D2 RID: 722
		private float basePitch = 1f;

		// Token: 0x040002D3 RID: 723
		private float baseVolumeLoudness = 1f;

		// Token: 0x040002D4 RID: 724
		private float userVolume = 1f;

		// Token: 0x040002D5 RID: 725
		private float userPitch = 1f;

		// Token: 0x040002D6 RID: 726
		private float occlusionAlpha = 1f;

		// Token: 0x040002D7 RID: 727
		private AnimationCurve hdrCurve;

		// Token: 0x02000080 RID: 128
		[Flags]
		private enum Flags
		{
			// Token: 0x040002D9 RID: 729
			Loops = 1,
			// Token: 0x040002DA RID: 730
			FadingIn = 2,
			// Token: 0x040002DB RID: 731
			FadingOut = 4,
			// Token: 0x040002DC RID: 732
			Muted = 8,
			// Token: 0x040002DD RID: 733
			Local = 16,
			// Token: 0x040002DE RID: 734
			ThreeD = 32,
			// Token: 0x040002DF RID: 735
			Paused = 64,
			// Token: 0x040002E0 RID: 736
			HDR = 128,
			// Token: 0x040002E1 RID: 737
			Occludable = 256,
			// Token: 0x040002E2 RID: 738
			Occluded = 512,
			// Token: 0x040002E3 RID: 739
			ForcedInfinite = 1024,
			// Token: 0x040002E4 RID: 740
			Delayed = 2048
		}
	}

	// Token: 0x02000081 RID: 129
	[Flags]
	public enum OcclusionModes
	{
		// Token: 0x040002E6 RID: 742
		Graph = 1,
		// Token: 0x040002E7 RID: 743
		Raycast = 2,
		// Token: 0x040002E8 RID: 744
		Distance = 4
	}
}

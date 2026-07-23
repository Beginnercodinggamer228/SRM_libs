using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020006E0 RID: 1760
public abstract class DirectedActorSpawner : SRBehaviour
{
	// Token: 0x060024B6 RID: 9398 RVA: 0x0008D549 File Offset: 0x0008B749
	public virtual void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
	}

	// Token: 0x060024B7 RID: 9399 RVA: 0x0008D55B File Offset: 0x0008B75B
	public virtual void Start()
	{
		this.Register(base.GetComponentInParent<CellDirector>());
		this.region = base.GetComponentInParent<Region>();
	}

	// Token: 0x060024B8 RID: 9400
	protected abstract void Register(CellDirector cellDir);

	// Token: 0x060024B9 RID: 9401 RVA: 0x00017787 File Offset: 0x00015987
	protected virtual GameObject MaybeReplacePrefab(GameObject prefab)
	{
		return prefab;
	}

	// Token: 0x060024BA RID: 9402 RVA: 0x0008D575 File Offset: 0x0008B775
	public virtual bool CanSpawn(float? forHour = null)
	{
		return !this.region.Hibernated && this.GetEligibleConstraints(forHour).Count > 0;
	}

	// Token: 0x060024BB RID: 9403 RVA: 0x00013CC5 File Offset: 0x00011EC5
	protected virtual bool CanContinueSpawning()
	{
		return true;
	}

	// Token: 0x060024BC RID: 9404 RVA: 0x0008D598 File Offset: 0x0008B798
	public bool CanSpawnSomething()
	{
		float currHour = this.timeDir.CurrHour();
		DirectedActorSpawner.SpawnConstraint[] array = this.constraints;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].InWindow(currHour))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060024BD RID: 9405 RVA: 0x0008D5D4 File Offset: 0x0008B7D4
	public virtual IEnumerator Spawn(int count, Randoms rand)
	{
		Dictionary<DirectedActorSpawner.SpawnConstraint, float> dictionary = new Dictionary<DirectedActorSpawner.SpawnConstraint, float>();
		float currHour = this.timeDir.CurrHour();
		foreach (DirectedActorSpawner.SpawnConstraint spawnConstraint in this.constraints)
		{
			if (spawnConstraint.InWindow(currHour))
			{
				dictionary[spawnConstraint] = spawnConstraint.weight;
			}
		}
		if (dictionary.Count > 0)
		{
			DirectedActorSpawner.SpawnConstraint spawnConstraint2 = rand.Pick<DirectedActorSpawner.SpawnConstraint>(dictionary, null);
			SlimeSet slimeset = spawnConstraint2.slimeset;
			bool feral = spawnConstraint2.feral;
			bool maxAgitation = spawnConstraint2.maxAgitation;
			int ii = 0;
			while (ii < count && this.CanContinueSpawning())
			{
				Dictionary<GameObject, float> dictionary2 = new Dictionary<GameObject, float>();
				foreach (SlimeSet.Member member in slimeset.members)
				{
					dictionary2[member.prefab] = member.weight;
				}
				GameObject gameObject = rand.Pick<GameObject>(dictionary2, null);
				if (gameObject == null)
				{
					Log.Warning("Spawner slimeset select with no choices? Skipping.", Array.Empty<object>());
					break;
				}
				gameObject = this.MaybeReplacePrefab(gameObject);
				GameObject gameObject2 = (this.spawnLocs == null || this.spawnLocs.Length == 0) ? null : rand.Pick<GameObject>(this.spawnLocs);
				Vector3 vector = (gameObject2 == null) ? (base.transform.position + base.transform.rotation * this.GetSpawnOffset(rand) * this.radius) : gameObject2.transform.position;
				Quaternion rotation = (gameObject2 == null) ? base.transform.rotation : gameObject2.transform.rotation;
				GameObject gameObject3 = SRBehaviour.InstantiateActor(gameObject, this.region.setId, vector, rotation, false);
				if (feral)
				{
					SlimeFeral component = gameObject3.GetComponent<SlimeFeral>();
					if (component != null)
					{
						Vacuumable component2 = gameObject3.GetComponent<Vacuumable>();
						if (component2 != null && component2.size != Vacuumable.Size.NORMAL)
						{
							component.SetFeral();
						}
						else
						{
							Log.Warning("Normal sized slimes cannot be made feral, but trying to mark as such.", Array.Empty<object>());
						}
					}
					else
					{
						Log.Warning("Slime has no feral behavior, but trying to mark as such.", Array.Empty<object>());
					}
				}
				if (maxAgitation)
				{
					SlimeEmotions component3 = gameObject3.GetComponent<SlimeEmotions>();
					if (component3 != null)
					{
						component3.Adjust(SlimeEmotions.Emotion.AGITATION, 1f);
					}
				}
				if (this.enableToteming)
				{
					TotemLinker componentInChildren = gameObject3.GetComponentInChildren<TotemLinker>();
					if (componentInChildren != null)
					{
						componentInChildren.SetStackReceptive(true);
					}
				}
				SpawnListener[] componentsInChildren = gameObject3.GetComponentsInChildren<SpawnListener>(true);
				int i;
				for (i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].DidSpawn();
				}
				this.SpawnFX(gameObject3, vector);
				Vector3 force = rotation * Vector3.up * 10f;
				Vector3 torque = rotation * Vector3.up * rand.GetInRange(-20f, 20f);
				Rigidbody component4 = gameObject3.GetComponent<Rigidbody>();
				component4.AddForce(force, ForceMode.VelocityChange);
				component4.AddTorque(torque, ForceMode.VelocityChange);
				this.OnActorSpawned(gameObject3);
				yield return new WaitForSeconds(rand.GetInRange(0.1f, 0.3f) * this.spawnDelayFactor);
				i = ii + 1;
				ii = i;
			}
			slimeset = null;
		}
		yield break;
	}

	// Token: 0x060024BE RID: 9406 RVA: 0x00003296 File Offset: 0x00001496
	protected virtual void OnActorSpawned(GameObject spawnedObj)
	{
	}

	// Token: 0x060024BF RID: 9407 RVA: 0x0008D5F1 File Offset: 0x0008B7F1
	protected virtual void SpawnFX(GameObject spawnedObj, Vector3 pos)
	{
		if (this.spawnFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.spawnFX, pos, Quaternion.identity);
		}
	}

	// Token: 0x060024C0 RID: 9408 RVA: 0x0008D614 File Offset: 0x0008B814
	private Vector3 GetSpawnOffset(Randoms rand)
	{
		UnityEngine.Random.InitState(rand.GetInt());
		Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
		return new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
	}

	// Token: 0x060024C1 RID: 9409 RVA: 0x0008D648 File Offset: 0x0008B848
	public List<DirectedActorSpawner.SpawnConstraint> GetEligibleConstraints(float? forHour)
	{
		List<DirectedActorSpawner.SpawnConstraint> list = new List<DirectedActorSpawner.SpawnConstraint>();
		foreach (DirectedActorSpawner.SpawnConstraint spawnConstraint in this.constraints)
		{
			if (spawnConstraint.InWindow(forHour ?? this.timeDir.CurrHour()))
			{
				list.Add(spawnConstraint);
			}
		}
		return list;
	}

	// Token: 0x040023A2 RID: 9122
	[Tooltip("An effect to play along with each spawn.")]
	public GameObject spawnFX;

	// Token: 0x040023A3 RID: 9123
	[Tooltip("The size of the area in which to do the spawning.")]
	public float radius = 5f;

	// Token: 0x040023A4 RID: 9124
	[Tooltip("Adjusts how much time between the actors being spawned.")]
	public float spawnDelayFactor = 1f;

	// Token: 0x040023A5 RID: 9125
	[Tooltip("Whether we should immediately enable toteming of spawned actors.")]
	public bool enableToteming;

	// Token: 0x040023A6 RID: 9126
	public DirectedActorSpawner.SpawnConstraint[] constraints;

	// Token: 0x040023A7 RID: 9127
	public GameObject[] spawnLocs;

	// Token: 0x040023A8 RID: 9128
	public bool allowDirectedSpawns = true;

	// Token: 0x040023A9 RID: 9129
	public float directedSpawnWeight = 1f;

	// Token: 0x040023AA RID: 9130
	protected TimeDirector timeDir;

	// Token: 0x040023AB RID: 9131
	private Region region;

	// Token: 0x040023AC RID: 9132
	protected const float POP_FORCE = 10f;

	// Token: 0x040023AD RID: 9133
	protected const float POP_ROTATE_MAX = 20f;

	// Token: 0x020006E1 RID: 1761
	public enum TimeMode
	{
		// Token: 0x040023AF RID: 9135
		ANY,
		// Token: 0x040023B0 RID: 9136
		DAY,
		// Token: 0x040023B1 RID: 9137
		NIGHT,
		// Token: 0x040023B2 RID: 9138
		CUSTOM
	}

	// Token: 0x020006E2 RID: 1762
	[Serializable]
	public class TimeWindow
	{
		// Token: 0x060024C3 RID: 9411 RVA: 0x0008D6D4 File Offset: 0x0008B8D4
		public float Start()
		{
			switch (this.timeMode)
			{
			case DirectedActorSpawner.TimeMode.ANY:
				return 0f;
			case DirectedActorSpawner.TimeMode.DAY:
				return 6f;
			case DirectedActorSpawner.TimeMode.NIGHT:
				return 18f;
			default:
				return this.startHour;
			}
		}

		// Token: 0x060024C4 RID: 9412 RVA: 0x0008D714 File Offset: 0x0008B914
		public float End()
		{
			switch (this.timeMode)
			{
			case DirectedActorSpawner.TimeMode.ANY:
				return 24f;
			case DirectedActorSpawner.TimeMode.DAY:
				return 18f;
			case DirectedActorSpawner.TimeMode.NIGHT:
				return 6f;
			default:
				return this.endHour;
			}
		}

		// Token: 0x040023B3 RID: 9139
		public DirectedActorSpawner.TimeMode timeMode;

		// Token: 0x040023B4 RID: 9140
		public float startHour;

		// Token: 0x040023B5 RID: 9141
		public float endHour = 24f;
	}

	// Token: 0x020006E3 RID: 1763
	[Serializable]
	public class SpawnConstraint
	{
		// Token: 0x060024C6 RID: 9414 RVA: 0x0008D768 File Offset: 0x0008B968
		public bool InWindow(float currHour)
		{
			float num = this.window.Start();
			float num2 = this.window.End();
			return (num2 >= num && num <= currHour && currHour <= num2) || (num2 < num && (currHour <= num2 || currHour >= num));
		}

		// Token: 0x040023B6 RID: 9142
		public SlimeSet slimeset;

		// Token: 0x040023B7 RID: 9143
		public float weight = 1f;

		// Token: 0x040023B8 RID: 9144
		public DirectedActorSpawner.TimeWindow window;

		// Token: 0x040023B9 RID: 9145
		public bool feral;

		// Token: 0x040023BA RID: 9146
		public bool maxAgitation;
	}
}

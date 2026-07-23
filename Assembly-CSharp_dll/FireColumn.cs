using System;
using System.Collections;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000701 RID: 1793
public class FireColumn : SRBehaviour
{
	// Token: 0x06002568 RID: 9576 RVA: 0x0008FA38 File Offset: 0x0008DC38
	public void Awake()
	{
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.region = base.GetComponentInParent<Region>();
		for (int i = 0; i < this.fireballs.Length; i++)
		{
			this.fireballEntryIdxWeightMap.Add(i, this.fireballs[i].weight);
			RegionMember component = this.fireballs[i].prefab.GetComponent<RegionMember>();
			if (component == null || !component.canHibernate)
			{
				this.hibernatingFireballEntryIdxWeightMap.Add(i, this.fireballs[i].weight);
			}
		}
		this.columnCollider = base.GetComponent<Collider>();
		this.columnCollider.enabled = false;
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x0008FAE4 File Offset: 0x0008DCE4
	public void OnTriggerEnter(Collider col)
	{
		if (this.fireActive)
		{
			Ignitable component = col.GetComponent<Ignitable>();
			if (component != null)
			{
				component.Ignite(base.gameObject);
			}
		}
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x0008FB10 File Offset: 0x0008DD10
	public void ActivateFire()
	{
		if (base.isActiveAndEnabled && !this.fireActive && !this.deactivating)
		{
			SECTR_AudioSystem.Play(this.columnSpawnCue, base.transform.position, false);
			this.columnFireLookCueInstance = SECTR_AudioSystem.Play(this.columnFireLoopCue, base.transform.position, true);
			this.fireActive = true;
			this.fireEffectObj.SetActive(true);
			this.nextFireballTime = this.timeDir.WorldTime() + (double)Randoms.SHARED.GetInRange(0f, this.minFireballDelay);
			this.endOfLifeTime = this.timeDir.HoursFromNow(this.lifetimeHrs);
			this.columnCollider.enabled = true;
		}
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x0008FBD4 File Offset: 0x0008DDD4
	public void DeactivateFire()
	{
		if (base.isActiveAndEnabled && this.fireActive)
		{
			this.deactivating = true;
			this.fireActive = false;
			base.StartCoroutine(this.FizzleThenDeactivateFireParticles());
			this.columnFireLookCueInstance.Stop(true);
			this.columnCollider.enabled = false;
		}
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x0008FC24 File Offset: 0x0008DE24
	public void OnDisable()
	{
		if (this.deactivating)
		{
			this.FinishDeactivate();
		}
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x0008FC34 File Offset: 0x0008DE34
	private IEnumerator FizzleThenDeactivateFireParticles()
	{
		ParticleSystem[] componentsInChildren = this.fireEffectObj.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Stop();
		}
		yield return new WaitForSeconds(2f);
		this.FinishDeactivate();
		yield break;
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x0008FC44 File Offset: 0x0008DE44
	private void FinishDeactivate()
	{
		this.fireEffectObj.SetActive(false);
		ParticleSystem[] componentsInChildren = this.fireEffectObj.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].Play();
		}
		this.deactivating = false;
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x0008FC86 File Offset: 0x0008DE86
	public bool IsFireActive()
	{
		return this.fireActive;
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x0008FC90 File Offset: 0x0008DE90
	public void Update()
	{
		if (this.fireActive)
		{
			if (this.timeDir.HasReached(this.nextFireballTime))
			{
				this.SpawnFireball();
				this.nextFireballTime = this.timeDir.WorldTime() + (double)Randoms.SHARED.GetInRange(this.minFireballDelay, this.maxFireballDelay);
			}
			if (this.timeDir.HasReached(this.endOfLifeTime))
			{
				this.DeactivateFire();
			}
		}
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x0008FD00 File Offset: 0x0008DF00
	private void SpawnFireball()
	{
		Dictionary<int, float> dictionary = this.IsHibernating() ? this.hibernatingFireballEntryIdxWeightMap : this.fireballEntryIdxWeightMap;
		if (dictionary.Count <= 0)
		{
			return;
		}
		int num = Randoms.SHARED.Pick<int>(dictionary, -1);
		if (num < 0)
		{
			return;
		}
		FireColumn.FireballEntry fireballEntry = this.fireballs[num];
		GameObject gameObject;
		if (fireballEntry.prefab.GetComponent<Identifiable>() == null)
		{
			gameObject = SRBehaviour.InstantiatePooledDynamic(fireballEntry.prefab, base.transform.position + base.transform.up * Randoms.SHARED.GetInRange(fireballEntry.minBallHeight, fireballEntry.maxBallHeight), Quaternion.identity);
		}
		else
		{
			gameObject = SRBehaviour.InstantiateActor(fireballEntry.prefab, this.region.setId, base.transform.position + base.transform.up * Randoms.SHARED.GetInRange(fireballEntry.minBallHeight, fireballEntry.maxBallHeight), Quaternion.identity, false);
		}
		gameObject.GetComponent<Rigidbody>().AddForce(UnityEngine.Random.onUnitSphere * Randoms.SHARED.GetInRange(fireballEntry.minBallEjectForce, fireballEntry.maxBallEjectForce));
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x0008FE23 File Offset: 0x0008E023
	public void NoteInOasis()
	{
		this.isInOasis = true;
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x0008FE2C File Offset: 0x0008E02C
	public bool IsInOasis()
	{
		return this.isInOasis;
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x0008FE34 File Offset: 0x0008E034
	private bool IsHibernating()
	{
		return this.region != null && this.region.Hibernated;
	}

	// Token: 0x04002453 RID: 9299
	public FireColumn.FireballEntry[] fireballs;

	// Token: 0x04002454 RID: 9300
	public Transform fireballParent;

	// Token: 0x04002455 RID: 9301
	public GameObject fireEffectObj;

	// Token: 0x04002456 RID: 9302
	public float minFireballDelay = 10f;

	// Token: 0x04002457 RID: 9303
	public float maxFireballDelay = 20f;

	// Token: 0x04002458 RID: 9304
	public float lifetimeHrs = 0.5f;

	// Token: 0x04002459 RID: 9305
	public SECTR_AudioCue columnSpawnCue;

	// Token: 0x0400245A RID: 9306
	public SECTR_AudioCue columnFireLoopCue;

	// Token: 0x0400245B RID: 9307
	private SECTR_AudioCueInstance columnFireLookCueInstance;

	// Token: 0x0400245C RID: 9308
	private bool fireActive;

	// Token: 0x0400245D RID: 9309
	private double nextFireballTime;

	// Token: 0x0400245E RID: 9310
	private double endOfLifeTime;

	// Token: 0x0400245F RID: 9311
	private Dictionary<int, float> fireballEntryIdxWeightMap = new Dictionary<int, float>();

	// Token: 0x04002460 RID: 9312
	private Dictionary<int, float> hibernatingFireballEntryIdxWeightMap = new Dictionary<int, float>();

	// Token: 0x04002461 RID: 9313
	private TimeDirector timeDir;

	// Token: 0x04002462 RID: 9314
	private Region region;

	// Token: 0x04002463 RID: 9315
	private Collider columnCollider;

	// Token: 0x04002464 RID: 9316
	private bool isInOasis;

	// Token: 0x04002465 RID: 9317
	private bool deactivating;

	// Token: 0x04002466 RID: 9318
	private const float FIZZLE_TIME = 2f;

	// Token: 0x02000702 RID: 1794
	[Serializable]
	public class FireballEntry
	{
		// Token: 0x04002467 RID: 9319
		public GameObject prefab;

		// Token: 0x04002468 RID: 9320
		public float weight;

		// Token: 0x04002469 RID: 9321
		public float minBallHeight;

		// Token: 0x0400246A RID: 9322
		public float maxBallHeight;

		// Token: 0x0400246B RID: 9323
		public float minBallEjectForce;

		// Token: 0x0400246C RID: 9324
		public float maxBallEjectForce;
	}
}

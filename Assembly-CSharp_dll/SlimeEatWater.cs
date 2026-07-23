using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000475 RID: 1141
public class SlimeEatWater : SRBehaviour
{
	// Token: 0x06001792 RID: 6034 RVA: 0x0005B888 File Offset: 0x00059A88
	public void Awake()
	{
		this.emotions = base.GetComponent<SlimeEmotions>();
		this.slimeEat = base.GetComponent<SlimeEat>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.faceAnim = base.GetComponent<SlimeFaceAnimator>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.ResetEatClock();
	}

	// Token: 0x06001793 RID: 6035 RVA: 0x0005B8D8 File Offset: 0x00059AD8
	public void Update()
	{
		if (Time.time >= this.nextDensityCheck)
		{
			this.tooDenseToProducePlort = this.IsSlimeDensityTooHigh();
			this.faceAnim.SetShouldBlush(this.tooDenseToProducePlort);
			if (!this.tooDenseToProducePlort)
			{
				this.tooDenseToProducePlort = this.IsPlortDensityTooHigh();
			}
			this.nextDensityCheck = Time.time + 2f;
		}
		if (this.waters.Count > 0 && Time.time >= this.nextChompTime && this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER) > this.slimeEat.minDriveToEat)
		{
			LiquidSource liquidSource = Randoms.SHARED.Pick<LiquidSource>(this.waters, null);
			liquidSource.ConsumeLiquid();
			if (!this.tooDenseToProducePlort)
			{
				base.StartCoroutine(this.ProduceAfterDelay(1, this.plort, 2f));
			}
			this.OnEat(SlimeEmotions.Emotion.HUNGER, liquidSource.liquidId);
		}
	}

	// Token: 0x06001794 RID: 6036 RVA: 0x0005B9B0 File Offset: 0x00059BB0
	private bool IsPlortDensityTooHigh()
	{
		int num = 0;
		float num2 = this.plortDensityDistance * this.plortDensityDistance;
		int num3 = this.CalcMaximumPlortDensity();
		this.objectsInCell.Clear();
		CellDirector.Get(Identifiable.Id.PUDDLE_PLORT, this.regionMember, this.objectsInCell);
		int count = this.objectsInCell.Count;
		int num4 = 0;
		while (num4 < count && num <= num3)
		{
			if ((this.objectsInCell[num4].transform.position - base.transform.position).sqrMagnitude <= num2)
			{
				num++;
			}
			num4++;
		}
		this.objectsInCell.Clear();
		return num > num3;
	}

	// Token: 0x06001795 RID: 6037 RVA: 0x0005BA58 File Offset: 0x00059C58
	private bool IsSlimeDensityTooHigh()
	{
		int num = 0;
		float num2 = this.slimeDensityDistance * this.slimeDensityDistance;
		int num3 = this.CalcMaximumSlimeDensity();
		this.objectsInCell.Clear();
		CellDirector.GetSlimes(this.regionMember, this.objectsInCell);
		int count = this.objectsInCell.Count;
		int num4 = 0;
		while (num4 < count && num <= num3)
		{
			if ((this.objectsInCell[num4].transform.position - base.transform.position).sqrMagnitude <= num2)
			{
				num++;
			}
			num4++;
		}
		this.objectsInCell.Clear();
		return num > num3;
	}

	// Token: 0x06001796 RID: 6038 RVA: 0x0005BAFE File Offset: 0x00059CFE
	public int CalcMaximumSlimeDensity()
	{
		if (this.nearbyFavoriteToys > 0)
		{
			return this.maxSlimeDensity + 1;
		}
		return this.maxSlimeDensity;
	}

	// Token: 0x06001797 RID: 6039 RVA: 0x0005BB18 File Offset: 0x00059D18
	public int CalcMaximumPlortDensity()
	{
		return this.maxPlortDensity;
	}

	// Token: 0x06001798 RID: 6040 RVA: 0x0005BB20 File Offset: 0x00059D20
	public void OnTriggerEnter(Collider col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.waters.Add(component);
		}
	}

	// Token: 0x06001799 RID: 6041 RVA: 0x0005BB5C File Offset: 0x00059D5C
	public void OnTriggerExit(Collider col)
	{
		LiquidSource component = col.gameObject.GetComponent<LiquidSource>();
		if (component != null && Identifiable.IsWater(component.liquidId))
		{
			this.waters.Remove(component);
		}
	}

	// Token: 0x0600179A RID: 6042 RVA: 0x0005BB98 File Offset: 0x00059D98
	public void ResetEatClock()
	{
		this.nextChompTime = Time.time + this.eatRate;
	}

	// Token: 0x0600179B RID: 6043 RVA: 0x0005BBAC File Offset: 0x00059DAC
	public void EnterToyProximity()
	{
		this.nearbyFavoriteToys++;
	}

	// Token: 0x0600179C RID: 6044 RVA: 0x0005BBBC File Offset: 0x00059DBC
	public void ExitToyProximity()
	{
		this.nearbyFavoriteToys--;
	}

	// Token: 0x0600179D RID: 6045 RVA: 0x0005BBCC File Offset: 0x00059DCC
	private void OnEat(SlimeEmotions.Emotion driver, Identifiable.Id otherId)
	{
		this.ResetEatClock();
		this.emotions.Adjust(driver, -this.slimeEat.drivePerEat);
		if (otherId != Identifiable.Id.PLAYER)
		{
			this.emotions.Adjust(SlimeEmotions.Emotion.AGITATION, -this.slimeEat.agitationPerEat);
		}
	}

	// Token: 0x0600179E RID: 6046 RVA: 0x0005BC0B File Offset: 0x00059E0B
	private IEnumerator ProduceAfterDelay(int count, GameObject produces, float delay)
	{
		yield return new WaitForSeconds(delay);
		this.Produce(count, produces, false);
		yield break;
	}

	// Token: 0x0600179F RID: 6047 RVA: 0x0005BC30 File Offset: 0x00059E30
	private void Produce(int count, GameObject produces, bool immediate)
	{
		if (base.gameObject != null)
		{
			for (int i = 0; i < count; i++)
			{
				Vector3 position = base.transform.TransformPoint(SlimeEatWater.LOCAL_PRODUCE_LOC);
				Vector3 velocity = base.transform.TransformVector(SlimeEatWater.LOCAL_PRODUCE_VEL);
				GameObject gameObject = SRBehaviour.InstantiateActor(produces, this.regionMember.setId, position, base.transform.rotation, false);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					component.velocity = velocity;
				}
				PlortInvulnerability component2 = gameObject.GetComponent<PlortInvulnerability>();
				if (component2 != null)
				{
					component2.GoInvulnerable();
				}
				gameObject.transform.DOScale(gameObject.transform.localScale, 0.5f).From(0.001f, true);
				if (!immediate && this.produceFX != null)
				{
					SRBehaviour.SpawnAndPlayFX(this.produceFX, position, base.transform.rotation);
				}
			}
			this.slimeAudio.Play(this.slimeAudio.slimeSounds.plortCue);
		}
	}

	// Token: 0x060017A0 RID: 6048 RVA: 0x0005BD3F File Offset: 0x00059F3F
	public bool WillNowEat(Identifiable.Id id)
	{
		return Identifiable.IsWater(id) && this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER) > this.slimeEat.minDriveToEat;
	}

	// Token: 0x060017A1 RID: 6049 RVA: 0x0005BD64 File Offset: 0x00059F64
	public List<Identifiable.Id> GetProducedIds(Identifiable.Id id, List<Identifiable.Id> produced)
	{
		produced.Clear();
		if (Identifiable.IsWater(id))
		{
			produced.Add(Identifiable.Id.PUDDLE_PLORT);
		}
		return produced;
	}

	// Token: 0x060017A2 RID: 6050 RVA: 0x0005BD80 File Offset: 0x00059F80
	public void EatImmediate(GameObject target, Identifiable.Id id, List<Identifiable.Id> produced, List<Identifiable.Id> collected, bool skipDelays)
	{
		LiquidSource component = target.GetComponent<LiquidSource>();
		component.ConsumeLiquid();
		if (!this.tooDenseToProducePlort)
		{
			this.Produce(1, this.plort, true);
		}
		this.OnEat(SlimeEmotions.Emotion.HUNGER, component.liquidId);
	}

	// Token: 0x040016BE RID: 5822
	[Tooltip("The plort to produce when we eat.")]
	public GameObject plort;

	// Token: 0x040016BF RID: 5823
	public GameObject produceFX;

	// Token: 0x040016C0 RID: 5824
	public float slimeDensityDistance = 10f;

	// Token: 0x040016C1 RID: 5825
	public int maxSlimeDensity;

	// Token: 0x040016C2 RID: 5826
	public float plortDensityDistance = 10f;

	// Token: 0x040016C3 RID: 5827
	public int maxPlortDensity = 8;

	// Token: 0x040016C4 RID: 5828
	public float eatRate = 3f;

	// Token: 0x040016C5 RID: 5829
	private SlimeEmotions emotions;

	// Token: 0x040016C6 RID: 5830
	private SlimeEat slimeEat;

	// Token: 0x040016C7 RID: 5831
	private float nextChompTime;

	// Token: 0x040016C8 RID: 5832
	private HashSet<LiquidSource> waters = new HashSet<LiquidSource>();

	// Token: 0x040016C9 RID: 5833
	private SlimeAudio slimeAudio;

	// Token: 0x040016CA RID: 5834
	private RegionMember regionMember;

	// Token: 0x040016CB RID: 5835
	private SlimeFaceAnimator faceAnim;

	// Token: 0x040016CC RID: 5836
	private bool tooDenseToProducePlort;

	// Token: 0x040016CD RID: 5837
	private float nextDensityCheck;

	// Token: 0x040016CE RID: 5838
	private static readonly Vector3 LOCAL_PRODUCE_LOC = new Vector3(0f, 0.5f, 0f);

	// Token: 0x040016CF RID: 5839
	private static readonly Vector3 LOCAL_PRODUCE_VEL = new Vector3(0f, 1f, 0f);

	// Token: 0x040016D0 RID: 5840
	private const float DENSITY_CHECK_PERIOD = 2f;

	// Token: 0x040016D1 RID: 5841
	private const float PRODUCE_SCALE_UP_TIME = 0.5f;

	// Token: 0x040016D2 RID: 5842
	private int nearbyFavoriteToys;

	// Token: 0x040016D3 RID: 5843
	private List<GameObject> objectsInCell = new List<GameObject>();
}

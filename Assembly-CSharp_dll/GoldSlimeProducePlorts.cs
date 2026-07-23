using System;
using System.Collections.Generic;
using MonomiPark.SlimeRancher.DataModel;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x020003E8 RID: 1000
public class GoldSlimeProducePlorts : CollidableActorBehaviour, Collidable, ActorModel.Participant
{
	// Token: 0x060014D5 RID: 5333 RVA: 0x00003296 File Offset: 0x00001496
	public void InitModel(ActorModel model)
	{
	}

	// Token: 0x060014D6 RID: 5334 RVA: 0x000510C9 File Offset: 0x0004F2C9
	public void SetModel(ActorModel model)
	{
		this.model = (SlimeModel)model;
	}

	// Token: 0x060014D7 RID: 5335 RVA: 0x000510D7 File Offset: 0x0004F2D7
	public override void Start()
	{
		base.Start();
		this.flee = base.GetComponent<GoldSlimeFlee>();
		this.regionMember = base.GetComponent<RegionMember>();
		this.slimeAudio = base.GetComponent<SlimeAudio>();
		this.playerState = SRSingleton<SceneContext>.Instance.PlayerState;
	}

	// Token: 0x060014D8 RID: 5336 RVA: 0x00051113 File Offset: 0x0004F313
	private bool IdentCausesPlorts(Identifiable.Id id)
	{
		return id != Identifiable.Id.GINGER_VEGGIE && id != Identifiable.Id.GOLD_PLORT && (Identifiable.IsFood(id) || Identifiable.IsChick(id) || Identifiable.IsPlort(id));
	}

	// Token: 0x060014D9 RID: 5337 RVA: 0x0005113C File Offset: 0x0004F33C
	public void ProcessCollisionEnter(Collision collision)
	{
		Identifiable component = collision.gameObject.GetComponent<Identifiable>();
		if (component != null)
		{
			Identifiable.Id id = component.id;
			if (this.IdentCausesPlorts(id) && !this.colliders.Contains(collision.gameObject))
			{
				PlortInvulnerability component2 = collision.gameObject.GetComponent<PlortInvulnerability>();
				if (component2 == null || !component2.enabled)
				{
					float num = float.NegativeInfinity;
					foreach (ContactPoint contactPoint in collision.contacts)
					{
						float num2 = Vector3.Dot(contactPoint.normal, collision.relativeVelocity);
						if (num2 > num)
						{
							num = num2;
						}
					}
					if (num > 0.02f)
					{
						this.ProducePlort();
						if (this.flee != null)
						{
							this.flee.StartFleeing(SRSingleton<SceneContext>.Instance.Player);
						}
						this.colliders.Add(collision.gameObject);
					}
				}
			}
		}
	}

	// Token: 0x060014DA RID: 5338 RVA: 0x00003296 File Offset: 0x00001496
	public void ProcessCollisionExit(Collision col)
	{
	}

	// Token: 0x060014DB RID: 5339 RVA: 0x00051238 File Offset: 0x0004F438
	private void ProduceAt(Vector3 dropAt)
	{
		PlortInvulnerability component = SRBehaviour.InstantiateActor(this.plortPrefab, this.regionMember.setId, dropAt, base.transform.rotation, false).GetComponent<PlortInvulnerability>();
		if (component != null)
		{
			component.GoInvulnerable();
		}
		this.plortsProduced++;
		if (this.plortsProduced == 3)
		{
			SRSingleton<SceneContext>.Instance.AchievementsDirector.AddToStat(AchievementsDirector.IntStat.GOLD_SLIME_TRIPLE_PLORT, 1);
		}
	}

	// Token: 0x060014DC RID: 5340 RVA: 0x000512A8 File Offset: 0x0004F4A8
	private void ProducePlort()
	{
		if (this.model.isGlitch)
		{
			return;
		}
		Vector3 vector = base.transform.position - base.transform.forward;
		this.ProduceAt(vector);
		if (this.playerState.HasUpgrade(PlayerState.Upgrade.GOLDEN_SURESHOT))
		{
			this.ProduceAt(vector - base.transform.forward * 0.25f);
			this.ProduceAt(vector - base.transform.forward * 0.5f);
		}
		base.GetComponent<Rigidbody>().AddForce(Vector3.up * 400f);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.jumpCue);
		this.slimeAudio.Play(this.slimeAudio.slimeSounds.voiceJumpCue);
		if (this.plortFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.plortFX, base.transform.position, base.transform.rotation);
		}
	}

	// Token: 0x040013B2 RID: 5042
	public GameObject plortPrefab;

	// Token: 0x040013B3 RID: 5043
	public GameObject plortFX;

	// Token: 0x040013B4 RID: 5044
	private GoldSlimeFlee flee;

	// Token: 0x040013B5 RID: 5045
	private RegionMember regionMember;

	// Token: 0x040013B6 RID: 5046
	private HashSet<GameObject> colliders = new HashSet<GameObject>();

	// Token: 0x040013B7 RID: 5047
	private SlimeAudio slimeAudio;

	// Token: 0x040013B8 RID: 5048
	private int plortsProduced;

	// Token: 0x040013B9 RID: 5049
	private PlayerState playerState;

	// Token: 0x040013BA RID: 5050
	private SlimeModel model;

	// Token: 0x040013BB RID: 5051
	private const float PLORT_THRESHOLD = 0.02f;

	// Token: 0x040013BC RID: 5052
	private const float JUMP_ON_HIT_FORCE = 400f;
}

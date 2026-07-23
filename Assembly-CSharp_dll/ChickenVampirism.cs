using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003A5 RID: 933
public class ChickenVampirism : FindConsumable, ControllerCollisionListener
{
	// Token: 0x06001378 RID: 4984 RVA: 0x0004BCDB File Offset: 0x00049EDB
	public override void Awake()
	{
		base.Awake();
		this.timeDir = SRSingleton<SceneContext>.Instance.TimeDirector;
		this.modDir = SRSingleton<SceneContext>.Instance.ModDirector;
		this.modDir.RegisterModsListener(new ModDirector.ModsListener(this.SetEnabled));
	}

	// Token: 0x06001379 RID: 4985 RVA: 0x0004BD1A File Offset: 0x00049F1A
	public override void OnDestroy()
	{
		base.OnDestroy();
		this.modDir.UnregisterModsListener(new ModDirector.ModsListener(this.SetEnabled));
	}

	// Token: 0x0600137A RID: 4986 RVA: 0x0004BD39 File Offset: 0x00049F39
	private void SetEnabled()
	{
		base.enabled = this.modDir.VampiricChickens();
		if (!base.enabled && this.activeFX != null)
		{
			Destroyer.Destroy(this.activeFX, "ChickenVampirism.SetEnabled");
		}
	}

	// Token: 0x0600137B RID: 4987 RVA: 0x0004BD72 File Offset: 0x00049F72
	public override void OnEnable()
	{
		base.OnEnable();
		if (!this.modDir.VampiricChickens())
		{
			base.enabled = false;
		}
	}

	// Token: 0x0600137C RID: 4988 RVA: 0x0004BD8E File Offset: 0x00049F8E
	public override void OnDisable()
	{
		base.OnDisable();
		if (this.activeFX != null)
		{
			Destroyer.Destroy(this.activeFX, "ChickenVampirism.OnDisable");
		}
	}

	// Token: 0x0600137D RID: 4989 RVA: 0x0004BDB4 File Offset: 0x00049FB4
	public void Update()
	{
		float num = this.timeDir.CurrDayFraction();
		this.isNight = (num < 0.25f || num > 0.75f);
		if (this.isNight && this.activeFX == null)
		{
			this.activeFX = UnityEngine.Object.Instantiate<GameObject>(this.fx);
			this.activeFX.transform.SetParent(base.transform, false);
			return;
		}
		if (!this.isNight && this.activeFX != null)
		{
			Destroyer.Destroy(this.activeFX, "ChickenVampirism.Update");
		}
	}

	// Token: 0x0600137E RID: 4990 RVA: 0x0004BE4C File Offset: 0x0004A04C
	public void OnControllerCollision(GameObject gameObj)
	{
		if (base.enabled && this.isNight && Time.time >= this.nextTime)
		{
			if (gameObj.GetInterfaceComponent<Damageable>().Damage(this.damagePerTouch, base.gameObject))
			{
				DeathHandler.Kill(gameObj, DeathHandler.Source.CHICKEN_VAMPIRISM, base.gameObject, "ChickenVampirism.OnControllerCollision");
			}
			this.nextTime = Time.time + this.repeatTime;
		}
	}

	// Token: 0x0600137F RID: 4991 RVA: 0x0004BEB3 File Offset: 0x0004A0B3
	protected override Dictionary<Identifiable.Id, DriveCalculator> GetSearchIds()
	{
		Dictionary<Identifiable.Id, DriveCalculator> dictionary = new Dictionary<Identifiable.Id, DriveCalculator>(Identifiable.idComparer);
		dictionary[Identifiable.Id.PLAYER] = new ChickenVampirism.ChickenDriveCalculator();
		return dictionary;
	}

	// Token: 0x06001380 RID: 4992 RVA: 0x0004BECC File Offset: 0x0004A0CC
	public override float Relevancy(bool isGrounded)
	{
		if (!base.enabled || !this.isNight)
		{
			return 0f;
		}
		float num;
		this.target = base.FindNearestConsumable(out num);
		if (!(this.target == null))
		{
			return 0.99f;
		}
		return 0f;
	}

	// Token: 0x06001381 RID: 4993 RVA: 0x00003296 File Offset: 0x00001496
	public override void Selected()
	{
	}

	// Token: 0x06001382 RID: 4994 RVA: 0x0004BF16 File Offset: 0x0004A116
	public override void Action()
	{
		if (this.target != null)
		{
			base.MoveTowards(SlimeSubbehaviour.GetGotoPos(this.target), base.IsBlocked(this.target, 0, false), ref this.nextLeapAvail, this.maxJump);
		}
	}

	// Token: 0x0400123E RID: 4670
	public GameObject fx;

	// Token: 0x0400123F RID: 4671
	public int damagePerTouch = 10;

	// Token: 0x04001240 RID: 4672
	public float repeatTime = 1f;

	// Token: 0x04001241 RID: 4673
	public float maxJump = 12f;

	// Token: 0x04001242 RID: 4674
	private GameObject activeFX;

	// Token: 0x04001243 RID: 4675
	private GameObject target;

	// Token: 0x04001244 RID: 4676
	private float nextTime;

	// Token: 0x04001245 RID: 4677
	private bool isNight;

	// Token: 0x04001246 RID: 4678
	private TimeDirector timeDir;

	// Token: 0x04001247 RID: 4679
	private ModDirector modDir;

	// Token: 0x04001248 RID: 4680
	private float nextLeapAvail;

	// Token: 0x04001249 RID: 4681
	private const float INIT_NO_DAMAGE_WINDOW = 0.1f;

	// Token: 0x020003A6 RID: 934
	private class ChickenDriveCalculator : DriveCalculator
	{
		// Token: 0x06001384 RID: 4996 RVA: 0x0004BF77 File Offset: 0x0004A177
		public ChickenDriveCalculator() : base(SlimeEmotions.Emotion.NONE, 0f, 0f)
		{
		}

		// Token: 0x06001385 RID: 4997 RVA: 0x00027ECC File Offset: 0x000260CC
		public override float Drive(SlimeEmotions emotions, Identifiable.Id id)
		{
			return 1f;
		}
	}
}

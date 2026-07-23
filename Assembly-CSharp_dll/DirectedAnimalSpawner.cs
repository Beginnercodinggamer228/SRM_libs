using System;
using System.Collections;
using MonomiPark.SlimeRancher.DataModel;

// Token: 0x020006E5 RID: 1765
public class DirectedAnimalSpawner : DirectedActorSpawner, DirectedAnimalSpawnerModel.Participant
{
	// Token: 0x060024CE RID: 9422 RVA: 0x0008DB7C File Offset: 0x0008BD7C
	public override void Awake()
	{
		base.Awake();
		this.oasis = base.GetComponentInParent<Oasis>();
		SRSingleton<SceneContext>.Instance.GameModel.RegisterAnimalSpawner(this);
	}

	// Token: 0x060024CF RID: 9423 RVA: 0x0006868D File Offset: 0x0006688D
	public void InitModel(DirectedAnimalSpawnerModel model)
	{
		model.pos = base.transform.position;
	}

	// Token: 0x060024D0 RID: 9424 RVA: 0x0008DBA0 File Offset: 0x0008BDA0
	public void SetModel(DirectedAnimalSpawnerModel model)
	{
		this.model = model;
	}

	// Token: 0x060024D1 RID: 9425 RVA: 0x0008DBA9 File Offset: 0x0008BDA9
	public override bool CanSpawn(float? forHour = null)
	{
		return base.CanSpawn(forHour) && this.timeDir.HasReached(this.model.nextSpawnTime) && !this.IsOasisFull();
	}

	// Token: 0x060024D2 RID: 9426 RVA: 0x0008DBD7 File Offset: 0x0008BDD7
	public override IEnumerator Spawn(int count, Randoms rand)
	{
		this.model.nextSpawnTime = this.timeDir.HoursFromNowOrStart(Randoms.SHARED.GetInRange(this.minSpawnIntervalGameHours, this.maxSpawnIntervalGameHours));
		yield return base.Spawn(count, rand);
		yield break;
	}

	// Token: 0x060024D3 RID: 9427 RVA: 0x0008DBF4 File Offset: 0x0008BDF4
	public double GetNextSpawnTime()
	{
		return this.model.nextSpawnTime;
	}

	// Token: 0x060024D4 RID: 9428 RVA: 0x0008DC01 File Offset: 0x0008BE01
	public void SetNextSpawnTime(double time)
	{
		this.model.nextSpawnTime = time;
	}

	// Token: 0x060024D5 RID: 9429 RVA: 0x0008DC0F File Offset: 0x0008BE0F
	protected override void Register(CellDirector cellDir)
	{
		cellDir.Register(this);
	}

	// Token: 0x060024D6 RID: 9430 RVA: 0x0008DC18 File Offset: 0x0008BE18
	private bool IsOasisFull()
	{
		return this.oasis != null && this.oasis.NeedsMoreAnimals();
	}

	// Token: 0x040023C4 RID: 9156
	public float minSpawnIntervalGameHours = 12f;

	// Token: 0x040023C5 RID: 9157
	public float maxSpawnIntervalGameHours = 18f;

	// Token: 0x040023C6 RID: 9158
	private DirectedAnimalSpawnerModel model;

	// Token: 0x040023C7 RID: 9159
	private Oasis oasis;
}

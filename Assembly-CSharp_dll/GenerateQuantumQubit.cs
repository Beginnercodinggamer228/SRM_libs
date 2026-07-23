using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x020003DE RID: 990
public class GenerateQuantumQubit : SRBehaviour
{
	// Token: 0x0600149A RID: 5274 RVA: 0x00050084 File Offset: 0x0004E284
	private void Awake()
	{
		this.plexer = base.gameObject.GetComponent<SlimeSubbehaviourPlexer>();
		this.slimeEmotions = base.gameObject.GetComponent<SlimeEmotions>();
		this.calmedByWaterSpray = base.GetComponent<CalmedByWaterSpray>();
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x000500B4 File Offset: 0x0004E2B4
	public QubitWander GetRandomQubit()
	{
		return Randoms.SHARED.Pick<QubitWander>(from q in this.qubits
		where q.HasArrived() && Physics.OverlapSphere(q.transform.position, 0.6f, -5).Length == 0
		select q, null);
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x000500EB File Offset: 0x0004E2EB
	public int GetQubitCount()
	{
		return this.qubits.Count;
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x000500F8 File Offset: 0x0004E2F8
	public void ClearQubits()
	{
		this.DestroyQubits(true);
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x00050101 File Offset: 0x0004E301
	public void DissipateQubit(QubitWander qubit)
	{
		this.qubits.Remove(qubit);
		this.DestroyQubit(qubit.gameObject, true);
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x00050120 File Offset: 0x0004E320
	public bool ReadyForSuperposition()
	{
		if (this.qubits.Count == 0)
		{
			return false;
		}
		for (int i = 0; i < this.qubits.Count; i++)
		{
			if (!this.qubits[i].HasArrived())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060014A0 RID: 5280 RVA: 0x00050168 File Offset: 0x0004E368
	private float GetGenerationDelay()
	{
		return Mathf.Lerp(this.MaxGenerationDelay, this.MinGenerationDelay, this.slimeEmotions.GetCurr(SlimeEmotions.Emotion.AGITATION));
	}

	// Token: 0x060014A1 RID: 5281 RVA: 0x00050187 File Offset: 0x0004E387
	private bool ReadyToGenerate()
	{
		return this.nextQubitGenerationTime <= Time.time && !this.plexer.IsGrounded();
	}

	// Token: 0x060014A2 RID: 5282 RVA: 0x000501A6 File Offset: 0x0004E3A6
	private void Start()
	{
		this.UpdateGenerationTime();
		this.generationAttempts = 5;
	}

	// Token: 0x060014A3 RID: 5283 RVA: 0x000501B5 File Offset: 0x0004E3B5
	private void UpdateGenerationTime()
	{
		this.nextQubitGenerationTime = Time.time + this.GetGenerationDelay();
	}

	// Token: 0x060014A4 RID: 5284 RVA: 0x000501CC File Offset: 0x0004E3CC
	private void Update()
	{
		if (this.calmedByWaterSpray.IsCalmed())
		{
			this.DestroyQubits(true);
			this.UpdateGenerationTime();
		}
		if (this.ReadyToGenerate() && this.generationAttempts >= 5)
		{
			this.generationAttempts = 0;
		}
		if (this.generationAttempts < 5 && this.plexer.IsGrounded() && !this.plexer.IsCaptive())
		{
			Vector3 position;
			if (this.FindValidQubitLocation(out position))
			{
				this.GenerateQubit(position);
				if (this.qubits.Count > this.MaxQubits)
				{
					this.DissipateQubit(this.qubits[0]);
				}
				this.generationAttempts = 5;
			}
			else
			{
				this.generationAttempts++;
			}
			if (this.generationAttempts >= 5)
			{
				this.UpdateGenerationTime();
			}
		}
	}

	// Token: 0x060014A5 RID: 5285 RVA: 0x0005028C File Offset: 0x0004E48C
	private void GenerateQubit(Vector3 position)
	{
		SlimeAppearance qubitAppearance = this.AppearanceApplicator.Appearance.QubitAppearance;
		if (qubitAppearance == null)
		{
			Log.Error("No qubit appearance found for slime.", new object[]
			{
				"name",
				base.gameObject.name
			});
		}
		GameObject gameObject = SRBehaviour.InstantiateDynamic(this.QubitPrefab, base.gameObject.transform.position, Quaternion.identity, false);
		SlimeAppearanceApplicator componentInChildren = gameObject.GetComponentInChildren<SlimeAppearanceApplicator>();
		componentInChildren.SlimeDefinition = this.AppearanceApplicator.SlimeDefinition;
		componentInChildren.Appearance = qubitAppearance;
		componentInChildren.ApplyAppearance();
		float prefabScale = this.AppearanceApplicator.SlimeDefinition.PrefabScale;
		gameObject.transform.localScale = new Vector3(prefabScale, prefabScale, prefabScale);
		QubitWander component = gameObject.GetComponent<QubitWander>();
		component.EndPosition = position;
		component.parentQuantumGenerator = this;
		this.qubits.Add(component);
	}

	// Token: 0x060014A6 RID: 5286 RVA: 0x00050364 File Offset: 0x0004E564
	private bool FindValidQubitLocation(out Vector3 position)
	{
		Vector2 vector = UnityEngine.Random.insideUnitCircle * this.QubitSearchRadius;
		Vector3 position2 = base.gameObject.transform.position;
		position2.x += vector.x;
		position2.z += vector.y;
		return GenerateQuantumQubit.GetAdjustedQubitLocation(position2, out position);
	}

	// Token: 0x060014A7 RID: 5287 RVA: 0x000503BC File Offset: 0x0004E5BC
	public static bool GetAdjustedQubitLocation(Vector3 castFrom, out Vector3 position)
	{
		position = Vector3.zero;
		castFrom.y += 1000f;
		RaycastHit[] array = Physics.SphereCastAll(castFrom, 0.6f, Vector3.down, float.PositiveInfinity, -539068421);
		GenerateQuantumQubit.collidersToReset.Clear();
		Vector3 zero = Vector3.zero;
		float num = float.MaxValue;
		bool flag = false;
		if (array.Length != 0)
		{
			float num2 = QuantumCeiling.AdjustMinDist(castFrom, 980f);
			for (int i = 0; i < array.Length; i++)
			{
				MeshCollider component = array[i].collider.GetComponent<MeshCollider>();
				if (component != null && !component.convex && array[i].collider.GetComponent<Rigidbody>() == null)
				{
					GenerateQuantumQubit.collidersToReset.Add(component);
					try
					{
						component.convex = true;
					}
					catch
					{
						Log.Error("Exception when changing to convex.", new object[]
						{
							"object name",
							component.name
						});
						throw;
					}
				}
				if (array[i].distance > num2 && array[i].distance < num && array[i].point.y >= 0f)
				{
					zero = new Vector3(array[i].point.x, array[i].point.y + 0.61f, array[i].point.z);
					num = array[i].distance;
					flag = true;
				}
			}
			if (flag && Physics.OverlapSphereNonAlloc(zero, 0.6f, GenerateQuantumQubit.Local_OverlapCount, -539068421) == 0 && !CorralRegion.IsWithin(zero))
			{
				flag = true;
				position = zero;
			}
			else
			{
				flag = false;
			}
			foreach (MeshCollider meshCollider in GenerateQuantumQubit.collidersToReset)
			{
				meshCollider.convex = false;
			}
		}
		return flag;
	}

	// Token: 0x060014A8 RID: 5288 RVA: 0x000505CC File Offset: 0x0004E7CC
	private void DestroyQubits(bool spawnFX)
	{
		foreach (QubitWander qubitWander in this.qubits)
		{
			if (qubitWander != null)
			{
				this.DestroyQubit(qubitWander.gameObject, spawnFX);
			}
		}
		this.qubits.Clear();
	}

	// Token: 0x060014A9 RID: 5289 RVA: 0x0005063C File Offset: 0x0004E83C
	private void DestroyQubit(GameObject qubit, bool spawnFX)
	{
		if (spawnFX)
		{
			SRBehaviour.SpawnAndPlayFX(this.DissipateFx, qubit.transform.position, Quaternion.identity);
		}
		Destroyer.Destroy(qubit.gameObject, "GenerateQuantumQubit.DestroyQubit");
	}

	// Token: 0x060014AA RID: 5290 RVA: 0x0005066D File Offset: 0x0004E86D
	private void OnDestroy()
	{
		if (SRSingleton<SceneContext>.Instance != null)
		{
			this.DestroyQubits(false);
		}
	}

	// Token: 0x0400136F RID: 4975
	public float QubitSearchRadius = 10f;

	// Token: 0x04001370 RID: 4976
	private const float POSITION_CHECK_SPHERECAST_START = 1000f;

	// Token: 0x04001371 RID: 4977
	private const float SPHERECAST_RADIUS = 0.6f;

	// Token: 0x04001372 RID: 4978
	private const float QUBIT_RADIUS = 0.61f;

	// Token: 0x04001373 RID: 4979
	private const int SPHERECAST_LAYER_MASK = -539068421;

	// Token: 0x04001374 RID: 4980
	private const float MAX_QUBIT_HEIGHT = 20f;

	// Token: 0x04001375 RID: 4981
	public float MaxQubitHeight = 10f;

	// Token: 0x04001376 RID: 4982
	private const int MAX_GENERATION_ATTEMPTS = 5;

	// Token: 0x04001377 RID: 4983
	public int MaxQubits = 5;

	// Token: 0x04001378 RID: 4984
	private float nextQubitGenerationTime;

	// Token: 0x04001379 RID: 4985
	private List<QubitWander> qubits = new List<QubitWander>();

	// Token: 0x0400137A RID: 4986
	private static List<MeshCollider> collidersToReset = new List<MeshCollider>();

	// Token: 0x0400137B RID: 4987
	private int generationAttempts;

	// Token: 0x0400137C RID: 4988
	private SlimeSubbehaviourPlexer plexer;

	// Token: 0x0400137D RID: 4989
	private SlimeEmotions slimeEmotions;

	// Token: 0x0400137E RID: 4990
	private CalmedByWaterSpray calmedByWaterSpray;

	// Token: 0x0400137F RID: 4991
	public float MinGenerationDelay = 5f;

	// Token: 0x04001380 RID: 4992
	public float MaxGenerationDelay = 20f;

	// Token: 0x04001381 RID: 4993
	public GameObject QubitPrefab;

	// Token: 0x04001382 RID: 4994
	public SlimeAppearanceApplicator AppearanceApplicator;

	// Token: 0x04001383 RID: 4995
	public GameObject DissipateFx;

	// Token: 0x04001384 RID: 4996
	private const float MIN_PLACEMENT_Y = 0f;

	// Token: 0x04001385 RID: 4997
	private static Collider[] Local_OverlapCount = new Collider[6];
}

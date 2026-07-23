using System;
using System.Collections;
using MonomiPark.SlimeRancher.Regions;
using UnityEngine;

// Token: 0x02000739 RID: 1849
public class Nutcracker : SRBehaviour
{
	// Token: 0x060026A2 RID: 9890 RVA: 0x0009354B File Offset: 0x0009174B
	public void Awake()
	{
		this.lookupDir = SRSingleton<GameContext>.Instance.LookupDirector;
		this.region = base.GetComponentInParent<Region>();
		this.anim = base.GetComponentInParent<Animator>();
		this.activateId = Animator.StringToHash("activate");
	}

	// Token: 0x060026A3 RID: 9891 RVA: 0x00093585 File Offset: 0x00091785
	public void OnTriggerEnter(Collider col)
	{
		if (col.isTrigger)
		{
			return;
		}
		if (Identifiable.GetId(col.gameObject) == this.convertFromId)
		{
			base.StartCoroutine(this.DoCrack(col.gameObject));
		}
	}

	// Token: 0x060026A4 RID: 9892 RVA: 0x000935B6 File Offset: 0x000917B6
	private IEnumerator DoCrack(GameObject toCrack)
	{
		toCrack.GetComponent<Rigidbody>().isKinematic = true;
		toCrack.GetComponent<Collider>().enabled = false;
		Renderer[] componentsInChildren = toCrack.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].enabled = false;
		}
		this.anim.SetTrigger(this.activateId);
		if (this.crackCue != null)
		{
			SECTR_AudioSystem.Play(this.crackCue, base.transform.position, false);
		}
		yield return new WaitForSeconds(3f);
		GameObject prefab = this.lookupDir.GetPrefab(this.convertToId);
		if (this.crackFX != null)
		{
			SRBehaviour.SpawnAndPlayFX(this.crackFX, toCrack.transform.position, toCrack.transform.rotation);
		}
		for (int j = 0; j < this.convertToCount; j++)
		{
			Vector3 position = base.transform.position + UnityEngine.Random.insideUnitSphere * 0.5f;
			SRBehaviour.InstantiateActor(prefab, this.region.setId, position, Quaternion.LookRotation(UnityEngine.Random.onUnitSphere), false);
		}
		Destroyer.DestroyActor(toCrack, "Nutcracker.DoCrack", false);
		yield break;
	}

	// Token: 0x040025D8 RID: 9688
	public Identifiable.Id convertFromId = Identifiable.Id.KOOKADOBA_BALL;

	// Token: 0x040025D9 RID: 9689
	public Identifiable.Id convertToId = Identifiable.Id.KOOKADOBA_FRUIT;

	// Token: 0x040025DA RID: 9690
	public int convertToCount = 5;

	// Token: 0x040025DB RID: 9691
	public GameObject crackFX;

	// Token: 0x040025DC RID: 9692
	public SECTR_AudioCue crackCue;

	// Token: 0x040025DD RID: 9693
	private LookupDirector lookupDir;

	// Token: 0x040025DE RID: 9694
	private Animator anim;

	// Token: 0x040025DF RID: 9695
	private Region region;

	// Token: 0x040025E0 RID: 9696
	private int activateId;

	// Token: 0x040025E1 RID: 9697
	private const float SPAWN_RAD = 0.5f;

	// Token: 0x040025E2 RID: 9698
	private const float TIME_BEFORE_CREATE = 3f;
}

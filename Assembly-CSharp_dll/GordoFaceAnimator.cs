using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003EC RID: 1004
public class GordoFaceAnimator : MonoBehaviour
{
	// Token: 0x06001503 RID: 5379 RVA: 0x00051B24 File Offset: 0x0004FD24
	public void Awake()
	{
		this.comps = base.GetComponentInParent<GordoFaceComponents>();
		this.slimeAudio = base.GetComponentInParent<SlimeAudio>();
		List<Renderer> list = new List<Renderer>();
		foreach (Renderer renderer in base.GetComponentsInChildren<Renderer>())
		{
			if (renderer.sharedMaterials.Length == 3)
			{
				list.Add(renderer);
			}
		}
		this.renderers = list.ToArray();
	}

	// Token: 0x06001504 RID: 5380 RVA: 0x00051B86 File Offset: 0x0004FD86
	public void Start()
	{
		this.InitStates();
		this.SetDefaultState();
	}

	// Token: 0x06001505 RID: 5381 RVA: 0x00051B94 File Offset: 0x0004FD94
	private void InitStates()
	{
		this.HAPPY = new GordoFaceAnimator.State(this, this.comps.blinkEyes, this.comps.happyMouth, null, delegate(GordoFaceAnimator.State state)
		{
			if (this.inVac)
			{
				this.SetState(this.HUNGRY);
			}
		});
		this.HUNGRY = new GordoFaceAnimator.State(this, this.comps.blinkEyes, this.comps.chompOpenMouth, null, delegate(GordoFaceAnimator.State state)
		{
			if (!this.inVac)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.STRAIN = new GordoFaceAnimator.State(this, this.comps.strainEyes, this.comps.strainMouth, null, delegate(GordoFaceAnimator.State state)
		{
		});
	}

	// Token: 0x06001506 RID: 5382 RVA: 0x00051C41 File Offset: 0x0004FE41
	public void Update()
	{
		this.currState.Update();
	}

	// Token: 0x06001507 RID: 5383 RVA: 0x00051C4E File Offset: 0x0004FE4E
	public void SetInVac(bool val)
	{
		this.inVac = val;
	}

	// Token: 0x06001508 RID: 5384 RVA: 0x00051C57 File Offset: 0x0004FE57
	public void SetTrigger(string trigger)
	{
		this.triggers.Add(trigger);
	}

	// Token: 0x06001509 RID: 5385 RVA: 0x00051C66 File Offset: 0x0004FE66
	public void SetDefaultState()
	{
		this.SetState(this.HAPPY);
	}

	// Token: 0x0600150A RID: 5386 RVA: 0x00051C74 File Offset: 0x0004FE74
	private void SetState(GordoFaceAnimator.State state)
	{
		if (state != this.currState)
		{
			this.triggers.Clear();
			this.currState = state;
			this.currState.Init();
			this.currState.Update();
		}
	}

	// Token: 0x040013D8 RID: 5080
	public const string STRAIN_TRIGGER = "Strain";

	// Token: 0x040013D9 RID: 5081
	private bool inVac;

	// Token: 0x040013DA RID: 5082
	private GordoFaceAnimator.State HAPPY;

	// Token: 0x040013DB RID: 5083
	private GordoFaceAnimator.State HUNGRY;

	// Token: 0x040013DC RID: 5084
	private GordoFaceAnimator.State STRAIN;

	// Token: 0x040013DD RID: 5085
	private GordoFaceAnimator.State currState;

	// Token: 0x040013DE RID: 5086
	private SlimeAudio slimeAudio;

	// Token: 0x040013DF RID: 5087
	private HashSet<string> triggers = new HashSet<string>();

	// Token: 0x040013E0 RID: 5088
	private GordoFaceComponents comps;

	// Token: 0x040013E1 RID: 5089
	private Renderer[] renderers;

	// Token: 0x020003ED RID: 1005
	private class State
	{
		// Token: 0x0600150E RID: 5390 RVA: 0x00051CE6 File Offset: 0x0004FEE6
		public override string ToString()
		{
			return this.eyes.name + ":" + this.mouth.name;
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00051D08 File Offset: 0x0004FF08
		public State(GordoFaceAnimator anim, Material eyes, Material mouth, SECTR_AudioCue cue, GordoFaceAnimator.State.UpdateDelegate update)
		{
			this.anim = anim;
			this.eyes = eyes;
			this.mouth = mouth;
			this.cue = cue;
			this.updateDel = update;
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00051D40 File Offset: 0x0004FF40
		public virtual void Init()
		{
			this.startTime = Time.fixedTime;
			this.ApplyMats(this.eyes, this.mouth);
			if (this.cue != null)
			{
				this.anim.slimeAudio.Play(this.cue);
			}
			this.AddReact("Strain", this.anim.STRAIN);
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00051DA4 File Offset: 0x0004FFA4
		public virtual void Update()
		{
			if (!this.React())
			{
				this.updateDel(this);
			}
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x00051DBC File Offset: 0x0004FFBC
		private bool React()
		{
			if (this.anim.triggers.Count > 0)
			{
				foreach (string text in this.anim.triggers)
				{
					if (this.reacts.ContainsKey(text))
					{
						this.anim.SetState(this.reacts[text]);
						this.anim.triggers.Remove(text);
						return true;
					}
				}
				return false;
			}
			return false;
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x00051E60 File Offset: 0x00050060
		public void AddReact(string trigger, GordoFaceAnimator.State state)
		{
			this.reacts[trigger] = state;
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x00051E70 File Offset: 0x00050070
		protected void ApplyMats(Material eyes, Material mouth)
		{
			foreach (Renderer renderer in this.anim.renderers)
			{
				Material[] sharedMaterials = renderer.sharedMaterials;
				sharedMaterials[1] = eyes;
				sharedMaterials[2] = mouth;
				renderer.sharedMaterials = sharedMaterials;
			}
		}

		// Token: 0x040013E2 RID: 5090
		public GordoFaceAnimator anim;

		// Token: 0x040013E3 RID: 5091
		protected Material eyes;

		// Token: 0x040013E4 RID: 5092
		protected Material mouth;

		// Token: 0x040013E5 RID: 5093
		protected SECTR_AudioCue cue;

		// Token: 0x040013E6 RID: 5094
		public float startTime;

		// Token: 0x040013E7 RID: 5095
		protected GordoFaceAnimator.State.UpdateDelegate updateDel;

		// Token: 0x040013E8 RID: 5096
		protected Dictionary<string, GordoFaceAnimator.State> reacts = new Dictionary<string, GordoFaceAnimator.State>();

		// Token: 0x020003EE RID: 1006
		// (Invoke) Token: 0x06001516 RID: 5398
		public delegate void UpdateDelegate(GordoFaceAnimator.State state);
	}

	// Token: 0x020003EF RID: 1007
	private class BlinkingState : GordoFaceAnimator.State
	{
		// Token: 0x06001519 RID: 5401 RVA: 0x00051EAF File Offset: 0x000500AF
		public BlinkingState(GordoFaceAnimator anim, Material eyes, Material mouth, SECTR_AudioCue cue, GordoFaceAnimator.State.UpdateDelegate del) : base(anim, eyes, mouth, cue, del)
		{
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x00051EEA File Offset: 0x000500EA
		public override void Init()
		{
			base.Init();
			this.blinkTime = Time.time + UnityEngine.Random.Range(this.MIN_BLINK_GAP, this.MAX_BLINK_GAP);
		}

		// Token: 0x0600151B RID: 5403 RVA: 0x00051F10 File Offset: 0x00050110
		public override void Update()
		{
			base.Update();
			if (Time.time >= this.unblinkTime)
			{
				base.ApplyMats(this.eyes, this.mouth);
				this.unblinkTime = float.PositiveInfinity;
				this.blinkTime = Time.time + UnityEngine.Random.Range(this.MIN_BLINK_GAP, this.MAX_BLINK_GAP);
				return;
			}
			if (Time.time >= this.blinkTime)
			{
				base.ApplyMats(this.anim.comps.blinkEyes, this.mouth);
				this.unblinkTime = Time.time + this.BLINK_TIME;
				this.blinkTime = float.PositiveInfinity;
			}
		}

		// Token: 0x040013E9 RID: 5097
		private float blinkTime;

		// Token: 0x040013EA RID: 5098
		private float unblinkTime = float.PositiveInfinity;

		// Token: 0x040013EB RID: 5099
		private float MIN_BLINK_GAP = 0.5f;

		// Token: 0x040013EC RID: 5100
		private float MAX_BLINK_GAP = 1f;

		// Token: 0x040013ED RID: 5101
		private float BLINK_TIME = 0.1f;
	}
}

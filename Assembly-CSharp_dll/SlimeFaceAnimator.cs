using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000481 RID: 1153
public class SlimeFaceAnimator : RegisteredActorBehaviour, RegistryUpdateable
{
	// Token: 0x060017DC RID: 6108 RVA: 0x0005C942 File Offset: 0x0005AB42
	public void Awake()
	{
		this.emotions = base.GetComponentInParent<SlimeEmotions>();
		this.slimeAudio = base.GetComponentInParent<SlimeAudio>();
	}

	// Token: 0x060017DD RID: 6109 RVA: 0x0005C95C File Offset: 0x0005AB5C
	public override void Start()
	{
		base.Start();
		this.InitStates();
		this.SetState(this.HAPPY);
	}

	// Token: 0x060017DE RID: 6110 RVA: 0x0005C978 File Offset: 0x0005AB78
	private void InitStates()
	{
		this.HAPPY = new SlimeFaceAnimator.BlinkingState(this, SlimeFace.SlimeExpression.Happy, null, delegate(SlimeFaceAnimator.State state)
		{
			float curr = this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION);
			float curr2 = this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER);
			float curr3 = this.emotions.GetCurr(SlimeEmotions.Emotion.FEAR);
			if (this.feral)
			{
				this.SetState(this.FERAL);
				return;
			}
			if (this.glitch)
			{
				this.SetState(this.GLITCH);
				return;
			}
			if (curr > 0.9f)
			{
				this.SetState(this.ANGRY);
				return;
			}
			if (curr2 > 0.666f && this.seekingFood)
			{
				this.SetState(this.HUNGRY);
				return;
			}
			if (curr2 > 0.99f)
			{
				this.SetState(this.STARVING);
				return;
			}
			if (curr3 > 0.6f)
			{
				this.SetState(this.SCARED);
				return;
			}
			if (curr < 0.01f && curr2 < 0.33f && curr3 < 0.01f)
			{
				this.SetState(this.ELATED);
			}
		});
		this.ANGRY = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Angry, null, delegate(SlimeFaceAnimator.State state)
		{
			if (this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) < 0.7f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.FERAL = new SlimeFaceAnimator.BlinkingState(this, SlimeFace.SlimeExpression.Feral, null, delegate(SlimeFaceAnimator.State state)
		{
			if (!this.feral)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.HUNGRY = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Hungry, null, delegate(SlimeFaceAnimator.State state)
		{
			float curr = this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER);
			if (this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) > 0.9f)
			{
				this.SetState(this.ANGRY);
				return;
			}
			if (curr > 0.99f)
			{
				this.SetState(this.STARVING);
				return;
			}
			if (curr < 0.4f || !this.seekingFood)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.STARVING = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Starving, null, delegate(SlimeFaceAnimator.State state)
		{
			float curr = this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER);
			if (this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION) > 0.9f)
			{
				this.SetState(this.ANGRY);
				return;
			}
			if (curr < 0.98f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.SCARED = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Scared, this.slimeAudio.slimeSounds.voiceFearCue, delegate(SlimeFaceAnimator.State state)
		{
			if (this.emotions.GetCurr(SlimeEmotions.Emotion.FEAR) < 0.4f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.ELATED = new SlimeFaceAnimator.BlinkingState(this, SlimeFace.SlimeExpression.Elated, this.slimeAudio.slimeSounds.voiceFunCue, delegate(SlimeFaceAnimator.State state)
		{
			float curr = this.emotions.GetCurr(SlimeEmotions.Emotion.AGITATION);
			float curr2 = this.emotions.GetCurr(SlimeEmotions.Emotion.HUNGER);
			float curr3 = this.emotions.GetCurr(SlimeEmotions.Emotion.FEAR);
			if (curr > 0.02f || curr2 > 0.35f || curr3 > 0.02f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.AWE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Awe, this.slimeAudio.slimeSounds.voiceAweCue, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 1f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.LONG_AWE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Awe, this.slimeAudio.slimeSounds.voiceAweCue, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 3f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.ALARM = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Alarm, this.slimeAudio.slimeSounds.voiceAlarmCue, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 1f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.WINCE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Wince, this.slimeAudio.slimeSounds.voiceDamageCue, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 1f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.SNEEZE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Wince, this.slimeAudio.slimeSounds.sneezeCue, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 1f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.MINOR_WINCE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Wince, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 0.2f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.ATTACK_TELEGRAPH = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.AttackTelegraph, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 1f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.CHOMP_OPEN = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.ChompOpen, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 1f)
			{
				this.SetState(this.CHOMP_CLOSED);
			}
		});
		this.CHOMP_OPEN_QUICK = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.ChompOpen, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 0.25f)
			{
				this.SetState(this.CHOMP_CLOSED);
			}
		});
		this.CHOMP_CLOSED = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.ChompClosed, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 2f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.INVOKE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Invoke, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + 3f)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.GRIMACE = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Grimace, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + BoomSlimeExplode.EXPLOSION_PREP_TIME)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.FRIED = new SlimeFaceAnimator.State(this, SlimeFace.SlimeExpression.Fried, null, delegate(SlimeFaceAnimator.State state)
		{
			if (Time.fixedTime > state.startTime + BoomSlimeExplode.EXPLOSION_RECOVERY_TIME)
			{
				this.SetState(this.HAPPY);
			}
		});
		this.GLITCH = new SlimeFaceAnimator.GlitchState(this, SlimeFace.SlimeExpression.Glitch);
		this.HAPPY.AddReact("triggerAwe", this.AWE);
		this.HUNGRY.AddReact("triggerAwe", this.AWE);
		this.HAPPY.AddReact("triggerLongAwe", this.LONG_AWE);
		this.HUNGRY.AddReact("triggerLongAwe", this.LONG_AWE);
	}

	// Token: 0x060017DF RID: 6111 RVA: 0x0005CC6A File Offset: 0x0005AE6A
	public void RegistryUpdate()
	{
		if (this.hasStarted)
		{
			this.currState.Update();
		}
	}

	// Token: 0x060017E0 RID: 6112 RVA: 0x0005CC7F File Offset: 0x0005AE7F
	public void SetGlitch()
	{
		this.glitch = true;
		this.SetState(this.GLITCH);
	}

	// Token: 0x060017E1 RID: 6113 RVA: 0x0005CC94 File Offset: 0x0005AE94
	public void SetFeral()
	{
		this.feral = true;
		this.triggers.Clear();
		if (this.currState is SlimeFaceAnimator.BlinkingState)
		{
			((SlimeFaceAnimator.BlinkingState)this.currState).ClearBlinkTime();
		}
	}

	// Token: 0x060017E2 RID: 6114 RVA: 0x0005CCC8 File Offset: 0x0005AEC8
	public void ClearFeral()
	{
		this.feral = false;
		if (this.currState != this.CHOMP_OPEN && this.currState != this.CHOMP_OPEN_QUICK)
		{
			this.triggers.Clear();
			if (this.currState is SlimeFaceAnimator.BlinkingState)
			{
				((SlimeFaceAnimator.BlinkingState)this.currState).ClearBlinkTime();
			}
		}
	}

	// Token: 0x060017E3 RID: 6115 RVA: 0x0005CD20 File Offset: 0x0005AF20
	public void SetSeekingFood(bool val)
	{
		this.seekingFood = val;
	}

	// Token: 0x060017E4 RID: 6116 RVA: 0x0005CD29 File Offset: 0x0005AF29
	public void SetShouldBlush(bool blush)
	{
		this.shouldBlush = blush;
	}

	// Token: 0x060017E5 RID: 6117 RVA: 0x0005CD32 File Offset: 0x0005AF32
	public void SetTrigger(string trigger)
	{
		this.triggers.Add(trigger);
	}

	// Token: 0x060017E6 RID: 6118 RVA: 0x0005CD41 File Offset: 0x0005AF41
	private void SetState(SlimeFaceAnimator.State state)
	{
		if (state != this.currState)
		{
			this.triggers.Clear();
			this.currState = state;
			this.currState.Init();
			this.currState.Update();
		}
	}

	// Token: 0x04001729 RID: 5929
	private bool feral;

	// Token: 0x0400172A RID: 5930
	private bool glitch;

	// Token: 0x0400172B RID: 5931
	private bool seekingFood;

	// Token: 0x0400172C RID: 5932
	private bool shouldBlush;

	// Token: 0x0400172D RID: 5933
	public SlimeAppearanceApplicator appearanceApplicator;

	// Token: 0x0400172E RID: 5934
	private SlimeFaceAnimator.State HAPPY;

	// Token: 0x0400172F RID: 5935
	private SlimeFaceAnimator.State ANGRY;

	// Token: 0x04001730 RID: 5936
	private SlimeFaceAnimator.State HUNGRY;

	// Token: 0x04001731 RID: 5937
	private SlimeFaceAnimator.State STARVING;

	// Token: 0x04001732 RID: 5938
	private SlimeFaceAnimator.State SCARED;

	// Token: 0x04001733 RID: 5939
	private SlimeFaceAnimator.State ELATED;

	// Token: 0x04001734 RID: 5940
	private SlimeFaceAnimator.State FERAL;

	// Token: 0x04001735 RID: 5941
	private SlimeFaceAnimator.State AWE;

	// Token: 0x04001736 RID: 5942
	private SlimeFaceAnimator.State LONG_AWE;

	// Token: 0x04001737 RID: 5943
	private SlimeFaceAnimator.State ALARM;

	// Token: 0x04001738 RID: 5944
	private SlimeFaceAnimator.State WINCE;

	// Token: 0x04001739 RID: 5945
	private SlimeFaceAnimator.State MINOR_WINCE;

	// Token: 0x0400173A RID: 5946
	private SlimeFaceAnimator.State ATTACK_TELEGRAPH;

	// Token: 0x0400173B RID: 5947
	private SlimeFaceAnimator.State CHOMP_OPEN;

	// Token: 0x0400173C RID: 5948
	private SlimeFaceAnimator.State CHOMP_OPEN_QUICK;

	// Token: 0x0400173D RID: 5949
	private SlimeFaceAnimator.State CHOMP_CLOSED;

	// Token: 0x0400173E RID: 5950
	private SlimeFaceAnimator.State INVOKE;

	// Token: 0x0400173F RID: 5951
	private SlimeFaceAnimator.State GRIMACE;

	// Token: 0x04001740 RID: 5952
	private SlimeFaceAnimator.State FRIED;

	// Token: 0x04001741 RID: 5953
	private SlimeFaceAnimator.State SNEEZE;

	// Token: 0x04001742 RID: 5954
	private SlimeFaceAnimator.State GLITCH;

	// Token: 0x04001743 RID: 5955
	private SlimeFaceAnimator.State currState;

	// Token: 0x04001744 RID: 5956
	private SlimeEmotions emotions;

	// Token: 0x04001745 RID: 5957
	private SlimeAudio slimeAudio;

	// Token: 0x04001746 RID: 5958
	private HashSet<string> triggers = new HashSet<string>();

	// Token: 0x02000482 RID: 1154
	private class State
	{
		// Token: 0x060017FC RID: 6140 RVA: 0x0005D0DA File Offset: 0x0005B2DA
		public override string ToString()
		{
			return this.facialExpression.ToString();
		}

		// Token: 0x060017FD RID: 6141 RVA: 0x0005D0ED File Offset: 0x0005B2ED
		public State(SlimeFaceAnimator anim, SlimeFace.SlimeExpression facialExpression, SECTR_AudioCue cue, SlimeFaceAnimator.State.UpdateDelegate update)
		{
			this.facialExpression = facialExpression;
			this.anim = anim;
			this.cue = cue;
			this.updateDel = update;
		}

		// Token: 0x060017FE RID: 6142 RVA: 0x0005D120 File Offset: 0x0005B320
		public virtual void Init()
		{
			this.startTime = Time.fixedTime;
			this.ApplyFacialExpression(this.facialExpression);
			if (this.cue != null)
			{
				this.anim.slimeAudio.Play(this.cue);
			}
			this.AddReact("triggerAlarm", this.anim.ALARM);
			this.AddReact("triggerAttackTelegraph", this.anim.ATTACK_TELEGRAPH);
			this.AddReact("triggerChompOpen", this.anim.CHOMP_OPEN);
			this.AddReact("triggerChompOpenQuick", this.anim.CHOMP_OPEN_QUICK);
			this.AddReact("triggerChompClosed", this.anim.CHOMP_CLOSED);
			this.AddReact("triggerWince", this.anim.WINCE);
			this.AddReact("triggerMinorWince", this.anim.MINOR_WINCE);
			this.AddReact("triggerConcentrate", this.anim.INVOKE);
			this.AddReact("triggerGrimace", this.anim.GRIMACE);
			this.AddReact("triggerFried", this.anim.FRIED);
			this.AddReact("triggerSneeze", this.anim.SNEEZE);
		}

		// Token: 0x060017FF RID: 6143 RVA: 0x0005D25A File Offset: 0x0005B45A
		public virtual void Update()
		{
			if (!this.React())
			{
				this.updateDel(this);
			}
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0005D270 File Offset: 0x0005B470
		protected virtual bool React()
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

		// Token: 0x06001801 RID: 6145 RVA: 0x0005D314 File Offset: 0x0005B514
		public void AddReact(string trigger, SlimeFaceAnimator.State state)
		{
			this.reacts[trigger] = state;
		}

		// Token: 0x06001802 RID: 6146 RVA: 0x0005D323 File Offset: 0x0005B523
		protected void ApplyFacialExpression(SlimeFace.SlimeExpression faceExpression)
		{
			this.anim.appearanceApplicator.SetExpression(faceExpression);
		}

		// Token: 0x04001747 RID: 5959
		public SlimeFaceAnimator anim;

		// Token: 0x04001748 RID: 5960
		protected SlimeFace.SlimeExpression facialExpression;

		// Token: 0x04001749 RID: 5961
		protected SECTR_AudioCue cue;

		// Token: 0x0400174A RID: 5962
		public float startTime;

		// Token: 0x0400174B RID: 5963
		protected SlimeFaceAnimator.State.UpdateDelegate updateDel;

		// Token: 0x0400174C RID: 5964
		protected Dictionary<string, SlimeFaceAnimator.State> reacts = new Dictionary<string, SlimeFaceAnimator.State>();

		// Token: 0x02000483 RID: 1155
		// (Invoke) Token: 0x06001804 RID: 6148
		public delegate void UpdateDelegate(SlimeFaceAnimator.State state);
	}

	// Token: 0x02000484 RID: 1156
	private class BlinkingState : SlimeFaceAnimator.State
	{
		// Token: 0x06001807 RID: 6151 RVA: 0x0005D336 File Offset: 0x0005B536
		public BlinkingState(SlimeFaceAnimator anim, SlimeFace.SlimeExpression facialExpression, SECTR_AudioCue cue, SlimeFaceAnimator.State.UpdateDelegate del) : base(anim, facialExpression, cue, del)
		{
		}

		// Token: 0x06001808 RID: 6152 RVA: 0x0005D34E File Offset: 0x0005B54E
		public override void Init()
		{
			base.Init();
			this.blinkTime = Time.time + UnityEngine.Random.Range(0.5f, 4.5f);
		}

		// Token: 0x06001809 RID: 6153 RVA: 0x0005D374 File Offset: 0x0005B574
		public override void Update()
		{
			base.Update();
			if (this.anim.currState != this)
			{
				return;
			}
			if (Time.time >= this.unblinkTime)
			{
				base.ApplyFacialExpression(this.facialExpression);
				if (this.anim.shouldBlush)
				{
					base.ApplyFacialExpression(SlimeFace.SlimeExpression.Blush);
				}
				this.unblinkTime = float.PositiveInfinity;
				this.blinkTime = Time.time + UnityEngine.Random.Range(0.5f, 4.5f);
				return;
			}
			if (Time.time >= this.blinkTime)
			{
				if (!this.anim.feral)
				{
					base.ApplyFacialExpression(this.facialExpression);
					base.ApplyFacialExpression(this.anim.shouldBlush ? SlimeFace.SlimeExpression.BlushBlink : SlimeFace.SlimeExpression.Blink);
				}
				this.unblinkTime = Time.time + 0.1f;
				this.blinkTime = float.PositiveInfinity;
			}
		}

		// Token: 0x0600180A RID: 6154 RVA: 0x0005D443 File Offset: 0x0005B643
		public void ClearBlinkTime()
		{
			this.unblinkTime = 0f;
			this.blinkTime = 0f;
		}

		// Token: 0x0400174D RID: 5965
		private float blinkTime;

		// Token: 0x0400174E RID: 5966
		private float unblinkTime = float.PositiveInfinity;

		// Token: 0x0400174F RID: 5967
		private const float MIN_BLINK_GAP = 0.5f;

		// Token: 0x04001750 RID: 5968
		private const float MAX_BLINK_GAP = 4.5f;

		// Token: 0x04001751 RID: 5969
		private const float BLINK_TIME = 0.1f;
	}

	// Token: 0x02000485 RID: 1157
	private class GlitchState : SlimeFaceAnimator.State
	{
		// Token: 0x0600180B RID: 6155 RVA: 0x0005D45B File Offset: 0x0005B65B
		public GlitchState(SlimeFaceAnimator anim, SlimeFace.SlimeExpression facialExpression) : base(anim, facialExpression, null, delegate(SlimeFaceAnimator.State s)
		{
		})
		{
		}

		// Token: 0x0600180C RID: 6156 RVA: 0x0001E8A5 File Offset: 0x0001CAA5
		protected override bool React()
		{
			return false;
		}
	}
}

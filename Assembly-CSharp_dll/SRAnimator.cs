using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200035D RID: 861
public abstract class SRAnimator : SRBehaviour
{
	// Token: 0x1700018F RID: 399
	// (get) Token: 0x060011EB RID: 4587 RVA: 0x000476A8 File Offset: 0x000458A8
	// (set) Token: 0x060011EC RID: 4588 RVA: 0x000476B0 File Offset: 0x000458B0
	public Animator animator { get; private set; }

	// Token: 0x060011ED RID: 4589 RVA: 0x000476B9 File Offset: 0x000458B9
	public virtual void Awake()
	{
		this.animator = base.GetRequiredComponent<Animator>();
	}

	// Token: 0x060011EE RID: 4590 RVA: 0x000476C8 File Offset: 0x000458C8
	public virtual void OnEnable()
	{
		if (this.hibernatingParameters != null)
		{
			this.hibernatingParameters.ForEach(delegate(SRAnimator.HibernatingParameter p)
			{
				p.Restore();
			});
			this.hibernatingParameters = null;
		}
		if (this.hibernatingStates != null)
		{
			for (int i = 0; i < this.hibernatingStates.Count; i++)
			{
				AnimatorStateInfo animatorStateInfo = this.hibernatingStates[i];
				this.animator.Play(animatorStateInfo.shortNameHash, i, animatorStateInfo.normalizedTime);
			}
			this.hibernatingStates = null;
		}
	}

	// Token: 0x060011EF RID: 4591 RVA: 0x0004775C File Offset: 0x0004595C
	public virtual void OnDisable()
	{
		this.hibernatingParameters = (from p in this.animator.parameters
		select new SRAnimator.HibernatingParameter(this.animator, p)).ToList<SRAnimator.HibernatingParameter>();
		this.hibernatingStates = (from ii in Enumerable.Range(0, this.animator.layerCount)
		select this.animator.GetCurrentAnimatorStateInfo(ii)).ToList<AnimatorStateInfo>();
	}

	// Token: 0x0400110F RID: 4367
	private List<SRAnimator.HibernatingParameter> hibernatingParameters;

	// Token: 0x04001110 RID: 4368
	private List<AnimatorStateInfo> hibernatingStates;

	// Token: 0x0200035E RID: 862
	private class HibernatingParameter
	{
		// Token: 0x060011F3 RID: 4595 RVA: 0x000477DC File Offset: 0x000459DC
		public HibernatingParameter(Animator animator, AnimatorControllerParameter parameter)
		{
			AnimatorControllerParameterType type = parameter.type;
			switch (type)
			{
			case AnimatorControllerParameterType.Float:
			{
				float current = animator.GetFloat(parameter.nameHash);
				this.Restore = delegate()
				{
					animator.SetFloat(parameter.nameHash, current);
				};
				return;
			}
			case (AnimatorControllerParameterType)2:
				break;
			case AnimatorControllerParameterType.Int:
			{
				int current = animator.GetInteger(parameter.nameHash);
				this.Restore = delegate()
				{
					animator.SetInteger(parameter.nameHash, current);
				};
				return;
			}
			case AnimatorControllerParameterType.Bool:
			{
				bool current = animator.GetBool(parameter.nameHash);
				this.Restore = delegate()
				{
					animator.SetBool(parameter.nameHash, current);
				};
				return;
			}
			default:
				if (type == AnimatorControllerParameterType.Trigger)
				{
					this.Restore = delegate()
					{
					};
					return;
				}
				break;
			}
			throw new NotImplementedException(string.Format("Failed to hibernate SRAnimator parameter. [animator={0}; parameter={1}; type={2}]", animator.name, parameter.name, parameter.type));
		}

		// Token: 0x04001111 RID: 4369
		public readonly Action Restore;
	}
}

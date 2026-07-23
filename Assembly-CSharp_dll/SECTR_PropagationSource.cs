using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200008D RID: 141
[ExecuteInEditMode]
[RequireComponent(typeof(SECTR_Member))]
[AddComponentMenu("SECTR/Audio/SECTR Propagation Source")]
public class SECTR_PropagationSource : SECTR_AudioSource
{
	// Token: 0x1700006B RID: 107
	// (get) Token: 0x060002F4 RID: 756 RVA: 0x00012948 File Offset: 0x00010B48
	public override bool IsPlaying
	{
		get
		{
			return this.playing || this.activeSounds.Count > 0;
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00012962 File Offset: 0x00010B62
	public override void Play()
	{
		this.playing = true;
		this.played = false;
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x00012974 File Offset: 0x00010B74
	public override void Stop(bool stopImmediately)
	{
		int count = this.activeSounds.Count;
		for (int i = 0; i < count; i++)
		{
			SECTR_PropagationSource.PathSound pathSound = this.activeSounds[i];
			if (pathSound != null)
			{
				pathSound.instance.Stop(stopImmediately);
			}
		}
		this.activeSounds.Clear();
		this.playing = false;
		this.played = false;
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x000129CE File Offset: 0x00010BCE
	protected override void OnEnable()
	{
		base.OnEnable();
		this.cachedMember = base.GetComponent<SECTR_Member>();
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x000129E2 File Offset: 0x00010BE2
	protected override void OnDisable()
	{
		base.OnDisable();
		this.cachedMember = null;
	}

	// Token: 0x060002F9 RID: 761 RVA: 0x000129F4 File Offset: 0x00010BF4
	private void Update()
	{
		if (this.playing && this.Cue != null && this.cachedMember.Sectors.Count > 0 && SECTR_AudioSystem.Initialized)
		{
			Vector3 position = SECTR_AudioSystem.Listener.position;
			Vector3 position2 = base.transform.position;
			this.directDistanceToListener = Vector3.Distance(position2, position);
			bool flag = this.Cue.SourceCue.Spatialization == SECTR_AudioCue.Spatializations.Occludable3D;
			int num = this.activeSounds.Count;
			if (this.played && !this.Loop && !this.Cue.SourceCue.Loops && num == 0)
			{
				this.Stop(false);
				return;
			}
			if (this.directDistanceToListener <= this.Cue.SourceCue.MaxDistance)
			{
				SECTR_PropagationSource.PathSound pathSound = null;
				SECTR_Graph.FindShortestPath(ref this.path, position, base.transform.position, (SECTR_Portal.PortalFlags)0);
				int count = this.path.Count;
				if (count > 0)
				{
					SECTR_Portal portal = this.path[0].Portal;
					SECTR_Portal sectr_Portal = (count > 1) ? this.path[1].Portal : null;
					bool flag2 = false;
					for (int i = 0; i < num; i++)
					{
						SECTR_PropagationSource.PathSound pathSound2 = this.activeSounds[i];
						if (portal == pathSound2.firstPortal || portal == pathSound2.secondPortal || sectr_Portal == pathSound2.firstPortal)
						{
							pathSound = pathSound2;
							break;
						}
					}
					if (pathSound == null)
					{
						pathSound = new SECTR_PropagationSource.PathSound();
						flag2 = true;
					}
					pathSound.firstPortal = portal;
					pathSound.secondPortal = sectr_Portal;
					pathSound.occluded = false;
					pathSound.firstDistance = 0f;
					pathSound.secondDistance = 0f;
					pathSound.distance = 0f;
					SECTR_AudioSystem.OcclusionModes occlusionModes = flag ? SECTR_AudioSystem.System.OcclusionFlags : ((SECTR_AudioSystem.OcclusionModes)0);
					bool flag3 = (occlusionModes & SECTR_AudioSystem.OcclusionModes.Graph) > (SECTR_AudioSystem.OcclusionModes)0;
					if (count == 1 && this.path[0].Portal == null)
					{
						pathSound.firstDistance = this.directDistanceToListener;
						pathSound.secondDistance = this.directDistanceToListener;
					}
					else
					{
						for (int j = 0; j < count; j++)
						{
							SECTR_Portal portal2 = this.path[j].Portal;
							SECTR_Portal sectr_Portal2 = (j < count - 1) ? this.path[j + 1].Portal : null;
							Vector3 center = portal2.Center;
							if (j == 0)
							{
								pathSound.firstDistance += Vector3.Distance(center, position);
							}
							else if (j == 1 && portal2)
							{
								pathSound.secondDistance += Vector3.Distance(center, position);
							}
							float num2;
							if (portal2 && sectr_Portal2)
							{
								num2 = Vector3.Distance(center, sectr_Portal2.Center);
							}
							else
							{
								num2 = Vector3.Distance(center, position2);
							}
							pathSound.firstDistance += num2;
							if (j >= 1)
							{
								pathSound.secondDistance += num2;
							}
							if (portal2 && flag3 && !pathSound.occluded && (portal2.Flags & SECTR_Portal.PortalFlags.Closed) != (SECTR_Portal.PortalFlags)0)
							{
								pathSound.occluded = true;
							}
						}
					}
					occlusionModes &= ~SECTR_AudioSystem.OcclusionModes.Graph;
					if (!pathSound.occluded && occlusionModes != (SECTR_AudioSystem.OcclusionModes)0)
					{
						pathSound.occluded = SECTR_AudioSystem.IsOccluded(position2, occlusionModes);
					}
					this._ComputeSoundSpatialization(position, this.directDistanceToListener, pathSound);
					if (!pathSound.instance)
					{
						if (this.activeSounds.Count > 0)
						{
							pathSound.instance = SECTR_AudioSystem.Clone(this.activeSounds[0].instance, pathSound.position);
						}
						else
						{
							pathSound.instance = SECTR_AudioSystem.Play(this.Cue, pathSound.position, this.Loop);
						}
						pathSound.instance.ForceInfinite();
						if (flag2)
						{
							this.activeSounds.Add(pathSound);
							num++;
						}
					}
					else
					{
						pathSound.instance.Position = pathSound.position;
					}
					pathSound.lastListenerPosition = position;
					this.played = true;
				}
				float num3 = 1f;
				int k;
				for (k = 0; k < num; k++)
				{
					SECTR_PropagationSource.PathSound pathSound3 = this.activeSounds[k];
					if (pathSound3 != pathSound)
					{
						this._ComputeSoundSpatialization(position, this.directDistanceToListener, pathSound3);
						pathSound3.weight = (pathSound3.instance ? (1f - Mathf.Clamp01(Vector3.Distance(pathSound3.lastListenerPosition, position) * 0.5f)) : 0f);
						num3 -= pathSound3.weight;
					}
				}
				if (pathSound != null)
				{
					pathSound.weight = Mathf.Max(0f, num3);
				}
				k = 0;
				float minDistance = this.Cue.SourceCue.MinDistance;
				float maxDistance = this.Cue.SourceCue.MaxDistance;
				float num4 = 1f / (maxDistance - minDistance);
				while (k < num)
				{
					SECTR_PropagationSource.PathSound pathSound4 = this.activeSounds[k];
					if (pathSound4.instance)
					{
						pathSound4.instance.Position = pathSound4.position;
						float num5 = 1f;
						SECTR_AudioCue.FalloffTypes falloff = this.Cue.SourceCue.Falloff;
						if (falloff != SECTR_AudioCue.FalloffTypes.Linear)
						{
							if (falloff == SECTR_AudioCue.FalloffTypes.Logrithmic)
							{
								num5 = Mathf.Clamp01(1f / Mathf.Max(pathSound4.distance - minDistance - 1f, 0.001f));
							}
						}
						else
						{
							num5 = 1f - Mathf.Clamp01((pathSound4.distance - minDistance) * num4);
						}
						pathSound4.instance.Volume = num5 * pathSound4.weight * this.volume;
						pathSound4.instance.Pitch = this.pitch;
						if (flag)
						{
							pathSound4.instance.ForceOcclusion(pathSound4.occluded);
						}
					}
					if (pathSound4.weight <= 0f || !pathSound4.instance)
					{
						pathSound4.instance.Stop(true);
						this.activeSounds.RemoveAt(k);
						num--;
					}
					else
					{
						k++;
					}
				}
				return;
			}
			for (int l = 0; l < num; l++)
			{
				SECTR_PropagationSource.PathSound pathSound5 = this.activeSounds[l];
				if (pathSound5 != null)
				{
					pathSound5.instance.Stop(false);
				}
			}
			this.activeSounds.Clear();
		}
	}

	// Token: 0x060002FA RID: 762 RVA: 0x00003296 File Offset: 0x00001496
	protected override void OnVolumePitchChanged()
	{
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0001304C File Offset: 0x0001124C
	private void _ComputeSoundSpatialization(Vector3 listenerPosition, float distanceToListener, SECTR_PropagationSource.PathSound pathSound)
	{
		if (pathSound.firstPortal != null)
		{
			Vector3 center = pathSound.firstPortal.Center;
			Vector3 a = pathSound.secondPortal ? pathSound.secondPortal.Center : base.transform.position;
			float num = pathSound.firstPortal.BoundingBox.SqrDistance(listenerPosition);
			Vector3 position;
			float distance;
			if (num >= this.InterpDistance * this.InterpDistance)
			{
				position = center;
				distance = pathSound.firstDistance;
			}
			else
			{
				float t = Mathf.Clamp01(Mathf.Sqrt(num) / this.InterpDistance);
				position = Vector3.Lerp(a, center, t);
				distance = Mathf.Lerp(pathSound.secondDistance, pathSound.firstDistance, t);
			}
			pathSound.position = position;
			pathSound.distance = distance;
			return;
		}
		pathSound.position = base.transform.position;
		pathSound.distance = distanceToListener;
	}

	// Token: 0x04000312 RID: 786
	private SECTR_Member cachedMember;

	// Token: 0x04000313 RID: 787
	private List<SECTR_Graph.Node> path = new List<SECTR_Graph.Node>(32);

	// Token: 0x04000314 RID: 788
	private List<SECTR_PropagationSource.PathSound> activeSounds = new List<SECTR_PropagationSource.PathSound>(4);

	// Token: 0x04000315 RID: 789
	private float directDistanceToListener;

	// Token: 0x04000316 RID: 790
	private bool playing;

	// Token: 0x04000317 RID: 791
	private bool played;

	// Token: 0x04000318 RID: 792
	[SECTR_ToolTip("When the listener gets within this distance of a portal, the sound direction will start to blend towards the next portal or source position.", 0f, -1f)]
	public float InterpDistance = 2f;

	// Token: 0x0200008E RID: 142
	private class PathSound
	{
		// Token: 0x04000319 RID: 793
		public SECTR_AudioCueInstance instance;

		// Token: 0x0400031A RID: 794
		public SECTR_Portal firstPortal;

		// Token: 0x0400031B RID: 795
		public SECTR_Portal secondPortal;

		// Token: 0x0400031C RID: 796
		public float firstDistance;

		// Token: 0x0400031D RID: 797
		public float secondDistance;

		// Token: 0x0400031E RID: 798
		public float distance;

		// Token: 0x0400031F RID: 799
		public Vector3 position;

		// Token: 0x04000320 RID: 800
		public Vector3 lastListenerPosition;

		// Token: 0x04000321 RID: 801
		public float weight = 1f;

		// Token: 0x04000322 RID: 802
		public bool occluded;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000B2 RID: 178
[AddComponentMenu("SECTR/Stream/SECTR Loading Door")]
public class SECTR_LoadingDoor : SECTR_Door
{
	// Token: 0x06000419 RID: 1049 RVA: 0x00018DC5 File Offset: 0x00016FC5
	protected override void OnEnable()
	{
		base.OnEnable();
		if (this.FadeBeforeLoad)
		{
			this.fadeTexture = new Texture2D(1, 1);
			this.fadeTexture.SetPixel(0, 0, this.FadeColor);
			this.fadeTexture.Apply();
		}
	}

	// Token: 0x0600041A RID: 1050 RVA: 0x00018E00 File Offset: 0x00017000
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		if (this.Portal && (this.LoadLayers & 1 << other.gameObject.layer) != 0)
		{
			SECTR_Chunk sectr_Chunk = this._GetOppositeChunk(other.transform.position);
			if (sectr_Chunk)
			{
				SECTR_Chunk sectr_Chunk2 = null;
				SECTR_LoadingDoor.LoadRequest loadRequest;
				if (this.loadRequests.TryGetValue(other, out loadRequest))
				{
					if (loadRequest.chunkToUnload)
					{
						sectr_Chunk2 = loadRequest.chunkToUnload;
						loadRequest.chunkToUnload = null;
					}
				}
				else
				{
					loadRequest = new SECTR_LoadingDoor.LoadRequest();
				}
				if (this.FadeBeforeLoad && !sectr_Chunk.IsLoaded())
				{
					loadRequest.fadeMode = SECTR_LoadingDoor.FadeMode.FadeOut;
				}
				loadRequest.enteredFront = (sectr_Chunk.Sector == this.Portal.BackSector);
				loadRequest.enteredBack = (sectr_Chunk.Sector == this.Portal.FrontSector);
				if (this.FadeBeforeLoad)
				{
					loadRequest.chunkToLoad = sectr_Chunk;
				}
				else
				{
					sectr_Chunk.AddReference();
					loadRequest.loadedChunk = sectr_Chunk;
				}
				this.loadRequests[other] = loadRequest;
				if (sectr_Chunk2)
				{
					sectr_Chunk2.RemoveReference();
				}
			}
		}
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00018F20 File Offset: 0x00017120
	protected override void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);
		if (this.Portal && (this.LoadLayers & 1 << other.gameObject.layer) != 0)
		{
			SECTR_Chunk sectr_Chunk = this._GetOppositeChunk(other.transform.position);
			if (sectr_Chunk)
			{
				SECTR_LoadingDoor.LoadRequest loadRequest = this.loadRequests[other];
				if (this.FadeBeforeLoad && loadRequest.fadeMode == SECTR_LoadingDoor.FadeMode.FadeOut)
				{
					loadRequest.fadeMode = SECTR_LoadingDoor.FadeMode.FadeIn;
				}
				bool flag = sectr_Chunk.Sector == this.Portal.FrontSector;
				bool flag2 = sectr_Chunk.Sector == this.Portal.BackSector;
				if (loadRequest.loadedChunk && ((loadRequest.enteredFront && flag2) || (loadRequest.enteredBack && flag)))
				{
					loadRequest.chunkToUnload = loadRequest.loadedChunk;
				}
				else if ((loadRequest.enteredFront && flag) || (loadRequest.enteredBack && flag2))
				{
					loadRequest.chunkToUnload = sectr_Chunk;
				}
				else
				{
					loadRequest.chunkToUnload = loadRequest.loadedChunk;
				}
				if (this.loadRequests.Count > 1 || base.IsClosed())
				{
					if (loadRequest.chunkToUnload)
					{
						loadRequest.chunkToUnload.RemoveReference();
					}
					this.loadRequests.Remove(other);
				}
			}
		}
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00019068 File Offset: 0x00017268
	private void OnGUI()
	{
		if (this.FadeBeforeLoad)
		{
			float num = Time.deltaTime / this.FadeTime;
			float num2 = 0f;
			foreach (SECTR_LoadingDoor.LoadRequest loadRequest in this.loadRequests.Values)
			{
				switch (loadRequest.fadeMode)
				{
				case SECTR_LoadingDoor.FadeMode.FadeIn:
					loadRequest.fadeAmount -= num;
					if (loadRequest.fadeAmount <= 0f)
					{
						loadRequest.fadeMode = SECTR_LoadingDoor.FadeMode.None;
					}
					break;
				case SECTR_LoadingDoor.FadeMode.FadeOut:
					loadRequest.fadeAmount += num;
					if (loadRequest.fadeAmount >= 1f)
					{
						if (loadRequest.chunkToLoad)
						{
							loadRequest.chunkToLoad.AddReference();
							loadRequest.loadedChunk = loadRequest.chunkToLoad;
							loadRequest.chunkToLoad = null;
						}
						loadRequest.fadeMode = SECTR_LoadingDoor.FadeMode.Hold;
						loadRequest.holdStart = Time.time;
					}
					break;
				case SECTR_LoadingDoor.FadeMode.Hold:
					if (!this.CanOpen())
					{
						loadRequest.holdStart = Time.time;
					}
					else if (Time.time >= loadRequest.holdStart + this.HoldTime)
					{
						loadRequest.fadeMode = SECTR_LoadingDoor.FadeMode.FadeIn;
					}
					break;
				}
				loadRequest.fadeAmount = Mathf.Clamp01(loadRequest.fadeAmount);
				num2 = Mathf.Max(num2, loadRequest.fadeAmount);
			}
			if (num2 > 0f)
			{
				GUI.color = new Color(1f, 1f, 1f, num2);
				GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.fadeTexture);
			}
		}
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00019224 File Offset: 0x00017424
	protected override bool CanOpen()
	{
		if (this.Portal)
		{
			if (!this._IsSectorLoaded(this.Portal.FrontSector))
			{
				return false;
			}
			if (!this._IsSectorLoaded(this.Portal.BackSector))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00019260 File Offset: 0x00017460
	private void OnClose()
	{
		if (this.loadRequests.Count == 1)
		{
			Dictionary<Collider, SECTR_LoadingDoor.LoadRequest>.Enumerator enumerator = this.loadRequests.GetEnumerator();
			enumerator.MoveNext();
			KeyValuePair<Collider, SECTR_LoadingDoor.LoadRequest> keyValuePair = enumerator.Current;
			SECTR_LoadingDoor.LoadRequest value = keyValuePair.Value;
			if (value.chunkToUnload)
			{
				value.chunkToUnload.RemoveReference();
				this.loadRequests.Clear();
			}
		}
	}

	// Token: 0x0600041F RID: 1055 RVA: 0x000192C4 File Offset: 0x000174C4
	private bool _IsSectorLoaded(SECTR_Sector sector)
	{
		if (sector && sector.Frozen)
		{
			SECTR_Chunk component = sector.GetComponent<SECTR_Chunk>();
			if (component && !component.IsLoaded())
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06000420 RID: 1056 RVA: 0x000192FC File Offset: 0x000174FC
	private SECTR_Chunk _GetOppositeChunk(Vector3 position)
	{
		if (this.Portal)
		{
			SECTR_Sector sectr_Sector = SECTR_Geometry.IsPointInFrontOfPlane(position, this.Portal.Center, this.Portal.Normal) ? this.Portal.BackSector : this.Portal.FrontSector;
			if (sectr_Sector)
			{
				return sectr_Sector.GetComponent<SECTR_Chunk>();
			}
		}
		return null;
	}

	// Token: 0x040003FE RID: 1022
	private Texture2D fadeTexture;

	// Token: 0x040003FF RID: 1023
	private Dictionary<Collider, SECTR_LoadingDoor.LoadRequest> loadRequests = new Dictionary<Collider, SECTR_LoadingDoor.LoadRequest>(4);

	// Token: 0x04000400 RID: 1024
	[SECTR_ToolTip("Specifies which layers are allow to cause loads (vs simply opening the door).")]
	public LayerMask LoadLayers = 16777215;

	// Token: 0x04000401 RID: 1025
	[SECTR_ToolTip("Should screen fade to black before loading.")]
	public bool FadeBeforeLoad;

	// Token: 0x04000402 RID: 1026
	[SECTR_ToolTip("How long to fade out before loading. Also, how long to fade back in.", "FadeBeforeLoad")]
	public float FadeTime = 1f;

	// Token: 0x04000403 RID: 1027
	[SECTR_ToolTip("How long to stay faded out. Helps cover pops right at the moment of loading.", "FadeBeforeLoad")]
	public float HoldTime = 0.1f;

	// Token: 0x04000404 RID: 1028
	[SECTR_ToolTip("The color to fade the screen to on load.", "FadeBeforeLoad")]
	public Color FadeColor = Color.black;

	// Token: 0x020000B3 RID: 179
	private enum FadeMode
	{
		// Token: 0x04000406 RID: 1030
		None,
		// Token: 0x04000407 RID: 1031
		FadeIn,
		// Token: 0x04000408 RID: 1032
		FadeOut,
		// Token: 0x04000409 RID: 1033
		Hold
	}

	// Token: 0x020000B4 RID: 180
	private class LoadRequest
	{
		// Token: 0x0400040A RID: 1034
		public SECTR_Chunk chunkToLoad;

		// Token: 0x0400040B RID: 1035
		public SECTR_Chunk chunkToUnload;

		// Token: 0x0400040C RID: 1036
		public SECTR_Chunk loadedChunk;

		// Token: 0x0400040D RID: 1037
		public bool enteredFront;

		// Token: 0x0400040E RID: 1038
		public bool enteredBack;

		// Token: 0x0400040F RID: 1039
		public SECTR_LoadingDoor.FadeMode fadeMode;

		// Token: 0x04000410 RID: 1040
		public float fadeAmount;

		// Token: 0x04000411 RID: 1041
		public float holdStart;
	}
}

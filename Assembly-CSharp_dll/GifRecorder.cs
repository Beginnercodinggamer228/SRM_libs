using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gif.Components;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000202 RID: 514
public class GifRecorder : MonoBehaviour
{
	// Token: 0x06000AEB RID: 2795 RVA: 0x0002E320 File Offset: 0x0002C520
	public void Update()
	{
		if (!SRSingleton<GameContext>.Instance.OptionsDirector.bufferForGif)
		{
			foreach (GifRecorder.Frame frame in this.frames)
			{
				Destroyer.Destroy(frame.tex, "GifRecorder.Update#1");
			}
			this.frames.Clear();
			if (this.hooks.Count > 0)
			{
				foreach (GifRecorder.CameraHook instance in this.hooks)
				{
					Destroyer.Destroy(instance, "GifRecorder.Update#2");
				}
				this.hooks.Clear();
			}
			return;
		}
		if (this.hooks.Count == 0)
		{
			this.frames.Clear();
			Camera[] allCameras = Camera.allCameras;
			for (int i = 0; i < allCameras.Length; i++)
			{
				GifRecorder.CameraHook cameraHook = allCameras[i].gameObject.AddComponent<GifRecorder.CameraHook>();
				cameraHook.recorder = this;
				this.hooks.Add(cameraHook);
			}
		}
		float time = Time.time;
		if (time >= this.nextFrameTime)
		{
			this.renderTex = new RenderTexture(GifRecorder.GIF_WIDTH, GifRecorder.GIF_HEIGHT, 24);
			float delay = (this.frames.Count == 0) ? GifRecorder.MIN_FRAME_DELAY : (time - this.lastFrameTime);
			this.frames.Enqueue(new GifRecorder.Frame(this.renderTex, time, delay));
			if (this.frames.Peek().time < time - GifRecorder.GIF_LENGTH)
			{
				Destroyer.Destroy(this.frames.Dequeue().tex, "GifRecorder.Update#3");
			}
			this.lastFrameTime = time;
			this.nextFrameTime = time + GifRecorder.MIN_FRAME_DELAY;
			return;
		}
		this.renderTex = null;
	}

	// Token: 0x06000AEC RID: 2796 RVA: 0x0002E4FC File Offset: 0x0002C6FC
	public void DoRenderImage(RenderTexture source)
	{
		if (this.renderTex != null)
		{
			Graphics.Blit(source, this.renderTex);
		}
	}

	// Token: 0x06000AED RID: 2797 RVA: 0x0002E518 File Offset: 0x0002C718
	public void MaybeSaveGif()
	{
		if (SRSingleton<GameContext>.Instance.OptionsDirector.bufferForGif)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.storeGifUI);
			StoreGifUI ui = gameObject.GetComponent<StoreGifUI>();
			ui.onConfirm = delegate()
			{
				this.StartCoroutine(this.SaveGif(delegate
				{
					ui.Close();
				}));
			};
			return;
		}
		UnityEngine.Object.Instantiate<GameObject>(this.cannotStoreGifUI);
	}

	// Token: 0x06000AEE RID: 2798 RVA: 0x0002E57F File Offset: 0x0002C77F
	private IEnumerator SaveGif(UnityAction onComplete)
	{
		Debug.Log("Starting saving Gif...");
		string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SlimeRancher-" + string.Format("{0:yyyy-MM-dd-hh-mm-ss-ff}", DateTime.Now) + ".gif");
		using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
		{
			AnimatedGifEncoder gifEncoder = new AnimatedGifEncoder();
			try
			{
				gifEncoder.SetRepeat(0);
				gifEncoder.Start(fileStream);
				int count = 0;
				foreach (GifRecorder.Frame frame in this.frames)
				{
					yield return new WaitForEndOfFrame();
					Texture2D texture2D = new Texture2D(GifRecorder.GIF_WIDTH, GifRecorder.GIF_HEIGHT, TextureFormat.RGB24, false);
					try
					{
						RenderTexture.active = frame.tex;
						texture2D.ReadPixels(new Rect(0f, 0f, (float)GifRecorder.GIF_WIDTH, (float)GifRecorder.GIF_HEIGHT), 0, 0);
					}
					finally
					{
						RenderTexture.active = null;
					}
					gifEncoder.AddFrame(texture2D.GetPixels32(), GifRecorder.GIF_WIDTH, GifRecorder.GIF_HEIGHT);
					gifEncoder.SetDelay(Mathf.RoundToInt(frame.delay * GifRecorder.MILLIS_PER_SEC));
					int num = count + 1;
					count = num;
					if (num > 1000)
					{
						Debug.Log("Too many frames in gif, ejecting...");
						break;
					}
					frame = null;
				}
				Queue<GifRecorder.Frame>.Enumerator enumerator = default(Queue<GifRecorder.Frame>.Enumerator);
			}
			finally
			{
				gifEncoder.Finish();
			}
			gifEncoder = null;
		}
		FileStream fileStream = null;
		Debug.Log("...finished Gif");
		if (onComplete != null)
		{
			onComplete();
		}
		yield break;
		yield break;
	}

	// Token: 0x040008D7 RID: 2263
	public GameObject storeGifUI;

	// Token: 0x040008D8 RID: 2264
	public GameObject cannotStoreGifUI;

	// Token: 0x040008D9 RID: 2265
	private Queue<GifRecorder.Frame> frames = new Queue<GifRecorder.Frame>();

	// Token: 0x040008DA RID: 2266
	private float nextFrameTime;

	// Token: 0x040008DB RID: 2267
	private float lastFrameTime;

	// Token: 0x040008DC RID: 2268
	private List<GifRecorder.CameraHook> hooks = new List<GifRecorder.CameraHook>();

	// Token: 0x040008DD RID: 2269
	private static float MIN_FRAME_DELAY = 0.075f;

	// Token: 0x040008DE RID: 2270
	private static float GIF_LENGTH = 3.5f;

	// Token: 0x040008DF RID: 2271
	private RenderTexture renderTex;

	// Token: 0x040008E0 RID: 2272
	private static int GIF_WIDTH = 560;

	// Token: 0x040008E1 RID: 2273
	private static int GIF_HEIGHT = 315;

	// Token: 0x040008E2 RID: 2274
	private static float MILLIS_PER_SEC = 1000f;

	// Token: 0x02000203 RID: 515
	private class Frame
	{
		// Token: 0x06000AF1 RID: 2801 RVA: 0x0002E5E7 File Offset: 0x0002C7E7
		public Frame(RenderTexture tex, float time, float delay)
		{
			this.tex = tex;
			this.time = time;
			this.delay = delay;
		}

		// Token: 0x040008E3 RID: 2275
		public RenderTexture tex;

		// Token: 0x040008E4 RID: 2276
		public float time;

		// Token: 0x040008E5 RID: 2277
		public float delay;
	}

	// Token: 0x02000204 RID: 516
	private class CameraHook : SRBehaviour
	{
		// Token: 0x06000AF2 RID: 2802 RVA: 0x0002E604 File Offset: 0x0002C804
		public void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			this.recorder.DoRenderImage(source);
			Graphics.Blit(source, destination);
		}

		// Token: 0x06000AF3 RID: 2803 RVA: 0x0002E619 File Offset: 0x0002C819
		public void OnDestroy()
		{
			this.recorder.hooks.Remove(this);
		}

		// Token: 0x040008E6 RID: 2278
		public GifRecorder recorder;
	}
}

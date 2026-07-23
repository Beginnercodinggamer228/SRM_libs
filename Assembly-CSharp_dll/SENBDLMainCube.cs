using System;
using UnityEngine;

// Token: 0x020007E8 RID: 2024
public class SENBDLMainCube : MonoBehaviour
{
	// Token: 0x06002A53 RID: 10835 RVA: 0x0009EBF8 File Offset: 0x0009CDF8
	private void Start()
	{
		this.glowColors[0] = new Color(1f, 0.47058824f, 0.050980393f);
		this.glowColors[2] = new Color(0.32941177f, 0.6392157f, 1f);
		this.glowColors[1] = new Color(0.60784316f, 1f, 0.11764706f);
		this.glowColors[3] = new Color(1f, 0.18431373f, 0f);
		this.currentColor = this.glowColors[0];
		SENBDLGlobal.sphereOfCubesRotation = Quaternion.identity;
		for (int i = 0; i < 150; i++)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.orbitingCube, Vector3.zero, Quaternion.identity);
		}
		for (int j = 0; j < 19; j++)
		{
			UnityEngine.Object.Instantiate<GameObject>(this.glowingOrbitingCube, Vector3.zero, Quaternion.identity);
		}
		Camera.main.backgroundColor = new Color(0.08f, 0.08f, 0.08f);
		SENBDLGlobal.mainCube = this;
		this.bloomShader = Camera.main.GetComponent<SENaturalBloomAndDirtyLens>();
	}

	// Token: 0x06002A54 RID: 10836 RVA: 0x00003296 File Offset: 0x00001496
	private void OnGUI()
	{
	}

	// Token: 0x06002A55 RID: 10837 RVA: 0x0009ED20 File Offset: 0x0009CF20
	private void Update()
	{
		this.deltaTime = Time.deltaTime / Time.timeScale;
		this.AnimateColor();
		this.RotateSphereOfCubes();
		float d = 40f;
		Vector3 vector = Vector3.up * d;
		vector = Quaternion.Euler(Vector3.right * Time.time * d * 0.5f) * vector;
		base.transform.Rotate(vector * Time.deltaTime);
		this.IncrementCounters();
		this.GetInput();
		this.UpdateShaderValues();
		this.SmoothFPSCounter();
	}

	// Token: 0x06002A56 RID: 10838 RVA: 0x0009EDB8 File Offset: 0x0009CFB8
	private void AnimateColor()
	{
		if (this.newColorCounter >= 8f)
		{
			this.newColorCounter = 0f;
			this.currentColorIndex = (this.currentColorIndex + 1) % this.glowColors.Length;
			this.previousColor = this.currentColor;
			this.currentColor = this.glowColors[this.currentColorIndex];
		}
		float t = Mathf.Clamp01(this.newColorCounter / 8f * 5f);
		this.glowColor = Color.Lerp(this.previousColor, this.currentColor, t);
		Color color = this.glowColor * Mathf.Pow(Mathf.Sin(Time.time) * 0.48f + 0.52f, 4f);
		this.cubeEmissivePart.GetComponent<Renderer>().material.SetColor("_EmissionColor", color);
		base.GetComponent<Light>().color = color;
		Color value = Color.Lerp(new Color
		{
			r = 1f - this.glowColor.r,
			g = 1f - this.glowColor.g,
			b = 1f - this.glowColor.b
		}, Color.white, 0.1f);
		this.particles.GetComponent<Renderer>().material.SetColor("_TintColor", value);
	}

	// Token: 0x06002A57 RID: 10839 RVA: 0x0009EF16 File Offset: 0x0009D116
	private void RotateSphereOfCubes()
	{
		SENBDLGlobal.sphereOfCubesRotation = Quaternion.Euler(Vector3.up * Time.time * 20f);
	}

	// Token: 0x06002A58 RID: 10840 RVA: 0x0009EF3B File Offset: 0x0009D13B
	private void IncrementCounters()
	{
		this.newColorCounter += Time.deltaTime;
	}

	// Token: 0x06002A59 RID: 10841 RVA: 0x0009EF50 File Offset: 0x0009D150
	private void GetInput()
	{
		if (Input.GetKey(KeyCode.RightArrow))
		{
			this.bloomAmount += 0.2f * this.deltaTime;
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			this.bloomAmount -= 0.2f * this.deltaTime;
		}
		if (Input.GetKey(KeyCode.UpArrow))
		{
			this.lensDirtAmount += 0.4f * this.deltaTime;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			this.lensDirtAmount -= 0.4f * this.deltaTime;
		}
		if (Input.GetKey(KeyCode.Period))
		{
			Time.timeScale += 0.5f * this.deltaTime;
		}
		if (Input.GetKey(KeyCode.Comma))
		{
			Time.timeScale -= 0.5f * this.deltaTime;
		}
		this.bloomAmount = Mathf.Clamp(this.bloomAmount, 0f, 0.4f);
		this.lensDirtAmount = Mathf.Clamp(this.lensDirtAmount, 0f, 0.95f);
		Time.timeScale = Mathf.Clamp(Time.timeScale, 0.1f, 1f);
		if (Input.GetKeyDown(KeyCode.Space))
		{
			this.bloomAmount = 0.05f;
			this.lensDirtAmount = 0.1f;
			Time.timeScale = 1f;
		}
	}

	// Token: 0x06002A5A RID: 10842 RVA: 0x0009F0A9 File Offset: 0x0009D2A9
	private void UpdateShaderValues()
	{
		this.bloomShader.bloomIntensity = this.bloomAmount;
		this.bloomShader.lensDirtIntensity = this.lensDirtAmount;
	}

	// Token: 0x06002A5B RID: 10843 RVA: 0x0009F0CD File Offset: 0x0009D2CD
	private void SmoothFPSCounter()
	{
		this.fps = Mathf.Lerp(this.fps, 1f / this.deltaTime, 5f * this.deltaTime);
	}

	// Token: 0x04002965 RID: 10597
	private Color[] glowColors = new Color[4];

	// Token: 0x04002966 RID: 10598
	public GameObject orbitingCube;

	// Token: 0x04002967 RID: 10599
	public GameObject glowingOrbitingCube;

	// Token: 0x04002968 RID: 10600
	public GameObject cubeEmissivePart;

	// Token: 0x04002969 RID: 10601
	public GameObject particles;

	// Token: 0x0400296A RID: 10602
	private const float newColorFrequency = 8f;

	// Token: 0x0400296B RID: 10603
	private float newColorCounter;

	// Token: 0x0400296C RID: 10604
	private Color currentColor;

	// Token: 0x0400296D RID: 10605
	private Color previousColor;

	// Token: 0x0400296E RID: 10606
	[HideInInspector]
	public Color glowColor;

	// Token: 0x0400296F RID: 10607
	private int currentColorIndex;

	// Token: 0x04002970 RID: 10608
	private float bloomAmount = 0.04f;

	// Token: 0x04002971 RID: 10609
	private float lensDirtAmount = 0.3f;

	// Token: 0x04002972 RID: 10610
	private float fps;

	// Token: 0x04002973 RID: 10611
	private float deltaTime;

	// Token: 0x04002974 RID: 10612
	private SENaturalBloomAndDirtyLens bloomShader;
}

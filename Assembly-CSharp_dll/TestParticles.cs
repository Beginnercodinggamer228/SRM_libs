using System;
using UnityEngine;

// Token: 0x0200005D RID: 93
public class TestParticles : MonoBehaviour
{
	// Token: 0x06000197 RID: 407 RVA: 0x0000BA4C File Offset: 0x00009C4C
	private void Start()
	{
		if (this.m_PrefabListFire.Length != 0 || this.m_PrefabListWind.Length != 0 || this.m_PrefabListWater.Length != 0 || this.m_PrefabListEarth.Length != 0 || this.m_PrefabListIce.Length != 0 || this.m_PrefabListThunder.Length != 0 || this.m_PrefabListLight.Length != 0 || this.m_PrefabListDarkness.Length != 0)
		{
			this.m_CurrentElementIndex = 0;
			this.m_CurrentParticleIndex = 0;
			this.ShowParticle();
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0000BAB8 File Offset: 0x00009CB8
	private void Update()
	{
		if (this.m_CurrentElementIndex != -1 && this.m_CurrentParticleIndex != -1)
		{
			if (Input.GetKeyUp(KeyCode.UpArrow))
			{
				this.m_CurrentElementIndex++;
				this.m_CurrentParticleIndex = 0;
				this.ShowParticle();
				return;
			}
			if (Input.GetKeyUp(KeyCode.DownArrow))
			{
				this.m_CurrentElementIndex--;
				this.m_CurrentParticleIndex = 0;
				this.ShowParticle();
				return;
			}
			if (Input.GetKeyUp(KeyCode.LeftArrow))
			{
				this.m_CurrentParticleIndex--;
				this.ShowParticle();
				return;
			}
			if (Input.GetKeyUp(KeyCode.RightArrow))
			{
				this.m_CurrentParticleIndex++;
				this.ShowParticle();
			}
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x0000BB70 File Offset: 0x00009D70
	private void OnGUI()
	{
		GUI.Window(1, new Rect((float)(Screen.width - 260), 5f, 250f, 80f), new GUI.WindowFunction(this.InfoWindow), "Info");
		GUI.Window(2, new Rect((float)(Screen.width - 260), (float)(Screen.height - 85), 250f, 80f), new GUI.WindowFunction(this.ParticleInformationWindow), "Help");
	}

	// Token: 0x0600019A RID: 410 RVA: 0x0000BBF4 File Offset: 0x00009DF4
	private void ShowParticle()
	{
		if (this.m_CurrentElementIndex > 7)
		{
			this.m_CurrentElementIndex = 0;
		}
		else if (this.m_CurrentElementIndex < 0)
		{
			this.m_CurrentElementIndex = 7;
		}
		if (this.m_CurrentElementIndex == 0)
		{
			this.m_CurrentElementList = this.m_PrefabListFire;
			this.m_ElementName = "FIRE";
		}
		else if (this.m_CurrentElementIndex == 1)
		{
			this.m_CurrentElementList = this.m_PrefabListWater;
			this.m_ElementName = "WATER";
		}
		else if (this.m_CurrentElementIndex == 2)
		{
			this.m_CurrentElementList = this.m_PrefabListWind;
			this.m_ElementName = "WIND";
		}
		else if (this.m_CurrentElementIndex == 3)
		{
			this.m_CurrentElementList = this.m_PrefabListEarth;
			this.m_ElementName = "EARTH";
		}
		else if (this.m_CurrentElementIndex == 4)
		{
			this.m_CurrentElementList = this.m_PrefabListThunder;
			this.m_ElementName = "THUNDER";
		}
		else if (this.m_CurrentElementIndex == 5)
		{
			this.m_CurrentElementList = this.m_PrefabListIce;
			this.m_ElementName = "ICE";
		}
		else if (this.m_CurrentElementIndex == 6)
		{
			this.m_CurrentElementList = this.m_PrefabListLight;
			this.m_ElementName = "LIGHT";
		}
		else if (this.m_CurrentElementIndex == 7)
		{
			this.m_CurrentElementList = this.m_PrefabListDarkness;
			this.m_ElementName = "DARKNESS";
		}
		if (this.m_CurrentParticleIndex >= this.m_CurrentElementList.Length)
		{
			this.m_CurrentParticleIndex = 0;
		}
		else if (this.m_CurrentParticleIndex < 0)
		{
			this.m_CurrentParticleIndex = this.m_CurrentElementList.Length - 1;
		}
		this.m_ParticleName = this.m_CurrentElementList[this.m_CurrentParticleIndex].name;
		if (this.m_CurrentParticle != null)
		{
			Destroyer.Destroy(this.m_CurrentParticle, "TestParticles.ShowParticle");
		}
		this.m_CurrentParticle = UnityEngine.Object.Instantiate<GameObject>(this.m_CurrentElementList[this.m_CurrentParticleIndex]);
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000BDBC File Offset: 0x00009FBC
	private void ParticleInformationWindow(int id)
	{
		GUI.Label(new Rect(12f, 25f, 280f, 20f), string.Concat(new object[]
		{
			"Up/Down: ",
			this.m_ElementName,
			" (",
			this.m_CurrentParticleIndex + 1,
			"/",
			this.m_CurrentElementList.Length,
			")"
		}));
		GUI.Label(new Rect(12f, 50f, 280f, 20f), "Left/Right: " + this.m_ParticleName.ToUpper());
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0000BE70 File Offset: 0x0000A070
	private void InfoWindow(int id)
	{
		GUI.Label(new Rect(15f, 25f, 240f, 20f), "Elementals 1.1.1");
		GUI.Label(new Rect(15f, 50f, 240f, 20f), "www.ge-team.com/pages");
	}

	// Token: 0x040001D2 RID: 466
	public GameObject[] m_PrefabListFire;

	// Token: 0x040001D3 RID: 467
	public GameObject[] m_PrefabListWind;

	// Token: 0x040001D4 RID: 468
	public GameObject[] m_PrefabListWater;

	// Token: 0x040001D5 RID: 469
	public GameObject[] m_PrefabListEarth;

	// Token: 0x040001D6 RID: 470
	public GameObject[] m_PrefabListIce;

	// Token: 0x040001D7 RID: 471
	public GameObject[] m_PrefabListThunder;

	// Token: 0x040001D8 RID: 472
	public GameObject[] m_PrefabListLight;

	// Token: 0x040001D9 RID: 473
	public GameObject[] m_PrefabListDarkness;

	// Token: 0x040001DA RID: 474
	private int m_CurrentElementIndex = -1;

	// Token: 0x040001DB RID: 475
	private int m_CurrentParticleIndex = -1;

	// Token: 0x040001DC RID: 476
	private string m_ElementName = "";

	// Token: 0x040001DD RID: 477
	private string m_ParticleName = "";

	// Token: 0x040001DE RID: 478
	private GameObject[] m_CurrentElementList;

	// Token: 0x040001DF RID: 479
	private GameObject m_CurrentParticle;
}

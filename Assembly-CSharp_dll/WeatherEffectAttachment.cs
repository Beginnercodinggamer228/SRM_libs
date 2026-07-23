using System;
using UnityEngine;

// Token: 0x020002EC RID: 748
public class WeatherEffectAttachment : MonoBehaviour
{
	// Token: 0x06001010 RID: 4112 RVA: 0x00040AB7 File Offset: 0x0003ECB7
	public void OnTriggerEnter(Collider col)
	{
		if (col.GetComponent<WeatherBlockingTrigger>() != null)
		{
			this.blockers++;
			this.UpdateBlocked();
		}
	}

	// Token: 0x06001011 RID: 4113 RVA: 0x00040ADB File Offset: 0x0003ECDB
	public void OnTriggerExit(Collider col)
	{
		if (col.GetComponent<WeatherBlockingTrigger>() != null)
		{
			this.blockers--;
			this.UpdateBlocked();
		}
	}

	// Token: 0x06001012 RID: 4114 RVA: 0x00040B00 File Offset: 0x0003ED00
	public void SetWeather(GameObject weatherPrefab)
	{
		if (this.currWeather != null)
		{
			Destroyer.Destroy(this.currWeather, "WeatherEffectAttachment.SetWeather");
		}
		if (weatherPrefab != null)
		{
			this.currWeather = UnityEngine.Object.Instantiate<GameObject>(weatherPrefab);
			this.currWeather.transform.SetParent(base.transform, false);
			this.UpdateBlocked();
		}
	}

	// Token: 0x06001013 RID: 4115 RVA: 0x00040B5D File Offset: 0x0003ED5D
	private void UpdateBlocked()
	{
		if (this.currWeather != null)
		{
			this.currWeather.SetActive(this.blockers == 0);
		}
	}

	// Token: 0x04000EEF RID: 3823
	private GameObject currWeather;

	// Token: 0x04000EF0 RID: 3824
	private int blockers;
}

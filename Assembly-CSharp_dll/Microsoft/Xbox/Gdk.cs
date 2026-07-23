using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Microsoft.Xbox
{
	// Token: 0x02000BD4 RID: 3028
	public class Gdk : MonoBehaviour
	{
		// Token: 0x1700049B RID: 1179
		// (get) Token: 0x060056A8 RID: 22184 RVA: 0x0010656C File Offset: 0x0010476C
		public static Gdk Helpers
		{
			get
			{
				if (Gdk._xboxHelpers == null)
				{
					Gdk[] array = UnityEngine.Object.FindObjectsOfType<Gdk>();
					if (array.Length != 0)
					{
						Gdk._xboxHelpers = array[0];
						Gdk._xboxHelpers._Initialize();
					}
					else
					{
						Gdk._LogError("Error: Could not find Xbox prefab. Make sure you have added the Xbox prefab to your scene.");
					}
				}
				return Gdk._xboxHelpers;
			}
		}

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x060056A9 RID: 22185 RVA: 0x001065B4 File Offset: 0x001047B4
		// (remove) Token: 0x060056AA RID: 22186 RVA: 0x001065EC File Offset: 0x001047EC
		public event Gdk.OnGameSaveLoadedHandler OnGameSaveLoaded;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x060056AB RID: 22187 RVA: 0x00106624 File Offset: 0x00104824
		// (remove) Token: 0x060056AC RID: 22188 RVA: 0x0010665C File Offset: 0x0010485C
		public event Gdk.OnErrorHandler OnError;

		// Token: 0x060056AD RID: 22189 RVA: 0x00106691 File Offset: 0x00104891
		private void Start()
		{
			this._Initialize();
		}

		// Token: 0x060056AE RID: 22190 RVA: 0x00106699 File Offset: 0x00104899
		private void _Initialize()
		{
			if (Gdk._initialized)
			{
				return;
			}
			Gdk._initialized = true;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		// Token: 0x060056AF RID: 22191 RVA: 0x001066B4 File Offset: 0x001048B4
		private void InitializeHresultToFriendlyErrorLookup()
		{
			Gdk._hresultToFriendlyErrorLookup.Add(-2143330041, "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, have you published it in Partner Center?");
		}

		// Token: 0x060056B0 RID: 22192 RVA: 0x00003296 File Offset: 0x00001496
		public void SignIn()
		{
		}

		// Token: 0x060056B1 RID: 22193 RVA: 0x00003296 File Offset: 0x00001496
		public void Save(byte[] data)
		{
		}

		// Token: 0x060056B2 RID: 22194 RVA: 0x00003296 File Offset: 0x00001496
		public void LoadSaveData()
		{
		}

		// Token: 0x060056B3 RID: 22195 RVA: 0x00003296 File Offset: 0x00001496
		public void UnlockAchievement(string achievementId)
		{
		}

		// Token: 0x060056B4 RID: 22196 RVA: 0x00003296 File Offset: 0x00001496
		private void Update()
		{
		}

		// Token: 0x060056B5 RID: 22197 RVA: 0x001066CC File Offset: 0x001048CC
		protected static bool Succeeded(int hresult, string operationFriendlyName)
		{
			bool result = false;
			if (HR.SUCCEEDED(hresult))
			{
				result = true;
			}
			else
			{
				string text = hresult.ToString("X8");
				string text2 = string.Empty;
				if (Gdk._hresultToFriendlyErrorLookup.ContainsKey(hresult))
				{
					text2 = Gdk._hresultToFriendlyErrorLookup[hresult];
				}
				else
				{
					text2 = operationFriendlyName + " failed.";
				}
				Gdk._LogError(string.Format("{0} Error code: hr=0x{1}", text2, text));
				if (Gdk.Helpers.OnError != null)
				{
					Gdk.Helpers.OnError(Gdk.Helpers, new ErrorEventArgs(text, text2));
				}
			}
			return result;
		}

		// Token: 0x060056B6 RID: 22198 RVA: 0x0010675A File Offset: 0x0010495A
		private static void _LogError(string message)
		{
			Debug.Log(message);
		}

		// Token: 0x0400430B RID: 17163
		[Header("You can find the value of the scid in your MicrosoftGame.config")]
		public string scid;

		// Token: 0x0400430C RID: 17164
		public Text gamertagLabel;

		// Token: 0x0400430D RID: 17165
		public bool signInOnStart = true;

		// Token: 0x0400430E RID: 17166
		private static Gdk _xboxHelpers;

		// Token: 0x0400430F RID: 17167
		private static bool _initialized;

		// Token: 0x04004310 RID: 17168
		private static Dictionary<int, string> _hresultToFriendlyErrorLookup;

		// Token: 0x04004311 RID: 17169
		private const int _100PercentAchievementProgress = 100;

		// Token: 0x04004312 RID: 17170
		private const string _GameSaveContainerName = "x_game_save_default_container";

		// Token: 0x04004313 RID: 17171
		private const string _GameSaveBlobName = "x_game_save_default_blob";

		// Token: 0x04004314 RID: 17172
		private const int _MaxAssociatedProductsToRetrieve = 25;

		// Token: 0x02000BD5 RID: 3029
		// (Invoke) Token: 0x060056B9 RID: 22201
		public delegate void OnGameSaveLoadedHandler(object sender, GameSaveLoadedArgs e);

		// Token: 0x02000BD6 RID: 3030
		// (Invoke) Token: 0x060056BD RID: 22205
		public delegate void OnErrorHandler(object sender, ErrorEventArgs e);
	}
}

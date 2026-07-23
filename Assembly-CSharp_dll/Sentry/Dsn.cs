using System;

namespace Sentry
{
	// Token: 0x020008AA RID: 2218
	public class Dsn
	{
		// Token: 0x06003057 RID: 12375 RVA: 0x000BE778 File Offset: 0x000BC978
		public Dsn(string dsn)
		{
			if (dsn == "")
			{
				throw new ArgumentException("invalid argument - DSN cannot be empty");
			}
			this._uri = new Uri(dsn);
			if (string.IsNullOrEmpty(this._uri.UserInfo))
			{
				throw new ArgumentException("Invalid DSN: No public key provided.");
			}
			string[] array = this._uri.UserInfo.Split(new char[]
			{
				':'
			});
			this.publicKey = array[0];
			if (string.IsNullOrEmpty(this.publicKey))
			{
				throw new ArgumentException("Invalid DSN: No public key provided.");
			}
			this.secretKey = null;
			if (array.Length > 1)
			{
				this.secretKey = array[1];
			}
			string arg = this._uri.AbsolutePath.Substring(0, this._uri.AbsolutePath.LastIndexOf('/'));
			string text = this._uri.AbsoluteUri.Substring(this._uri.AbsoluteUri.LastIndexOf('/') + 1);
			if (string.IsNullOrEmpty(text))
			{
				throw new ArgumentException("Invalid DSN: A Project Id is required.");
			}
			UriBuilder uriBuilder = new UriBuilder
			{
				Scheme = this._uri.Scheme,
				Host = this._uri.DnsSafeHost,
				Port = this._uri.Port,
				Path = string.Format("{0}/api/{1}/store/", arg, text)
			};
			this.callUri = uriBuilder.Uri;
		}

		// Token: 0x04002E79 RID: 11897
		private Uri _uri;

		// Token: 0x04002E7A RID: 11898
		public Uri callUri;

		// Token: 0x04002E7B RID: 11899
		public string secretKey;

		// Token: 0x04002E7C RID: 11900
		public string publicKey;
	}
}

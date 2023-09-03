using System;

namespace UnityObsoleteGui
{
	// Token: 0x02000018 RID: 24
	public class ToggleEventArgs : EventArgs
	{
		// Token: 0x06000139 RID: 313 RVA: 0x0000F527 File Offset: 0x0000E527
		public ToggleEventArgs(string name, bool b)
		{
			this.Name = name;
			this.Value = b;
		}

		// Token: 0x0400011C RID: 284
		public string Name;

		// Token: 0x0400011D RID: 285
		public bool Value;
	}
}

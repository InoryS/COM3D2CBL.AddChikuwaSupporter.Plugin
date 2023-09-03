using System;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x02000013 RID: 19
	public class Toggle : Element
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000F264 File Offset: 0x0000E264
		// (set) Token: 0x06000124 RID: 292 RVA: 0x0000F27C File Offset: 0x0000E27C
		public bool Value
		{
			get
			{
				return this.val;
			}
			set
			{
				this.val = value;
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000F288 File Offset: 0x0000E288
		// (set) Token: 0x06000126 RID: 294 RVA: 0x0000F2A5 File Offset: 0x0000E2A5
		public string Text
		{
			get
			{
				return this.Content.text;
			}
			set
			{
				this.Content.text = value;
			}
		}

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000127 RID: 295 RVA: 0x0000F2B5 File Offset: 0x0000E2B5
		// (remove) Token: 0x06000128 RID: 296 RVA: 0x0000F2CE File Offset: 0x0000E2CE
		public event EventHandler<ToggleEventArgs> OnChange;

		// Token: 0x06000129 RID: 297 RVA: 0x0000F2E7 File Offset: 0x0000E2E7
		public Toggle(string name, Rect rect, EventHandler<ToggleEventArgs> _OnChange) : this(name, rect, false, "", _OnChange)
		{
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000F2FB File Offset: 0x0000E2FB
		public Toggle(string name, Rect rect, bool def, EventHandler<ToggleEventArgs> _OnChange) : this(name, rect, def, "", _OnChange)
		{
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000F310 File Offset: 0x0000E310
		public Toggle(string name, Rect rect, string text, EventHandler<ToggleEventArgs> _OnChange) : this(name, rect, false, text, _OnChange)
		{
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000F324 File Offset: 0x0000E324
		public Toggle(string name, Rect rect, bool def, string text, EventHandler<ToggleEventArgs> _OnChange) : base(name, rect)
		{
			this.val = def;
			this.Content = new GUIContent(text);
			this.OnChange = (EventHandler<ToggleEventArgs>)Delegate.Combine(this.OnChange, _OnChange);
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000F378 File Offset: 0x0000E378
		public override void Draw(Rect outRect)
		{
			this.onChange(GUI.Toggle(outRect, this.Value, this.Content, this.Style));
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000F39C File Offset: 0x0000E39C
		private void onChange(bool newValue)
		{
			if (newValue != this.val)
			{
				this.OnChange(this, new ToggleEventArgs(this.name, newValue));
			}
			this.val = newValue;
		}

		// Token: 0x0400010E RID: 270
		private bool val;

		// Token: 0x0400010F RID: 271
		public GUIStyle Style = "toggle";

		// Token: 0x04000110 RID: 272
		public GUIContent Content;
	}
}

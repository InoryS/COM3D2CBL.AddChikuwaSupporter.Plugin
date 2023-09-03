using System;
using UnityEngine;

namespace UnityObsoleteGui
{
	// Token: 0x02000012 RID: 18
	public class HSlider : Element
	{
		// Token: 0x14000005 RID: 5
		// (add) Token: 0x0600011E RID: 286 RVA: 0x0000F158 File Offset: 0x0000E158
		// (remove) Token: 0x0600011F RID: 287 RVA: 0x0000F171 File Offset: 0x0000E171
		public event EventHandler<SliderEventArgs> OnChange;

		// Token: 0x06000120 RID: 288 RVA: 0x0000F18C File Offset: 0x0000E18C
		public HSlider(string name, Rect rect, float min, float max, float def, EventHandler<SliderEventArgs> _OnChange) : base(name, rect)
		{
			this.Value = def;
			this.Min = min;
			this.Max = max;
			this.OnChange = (EventHandler<SliderEventArgs>)Delegate.Combine(this.OnChange, _OnChange);
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000F1F3 File Offset: 0x0000E1F3
		public override void Draw(Rect outRect)
		{
			this.onChange(GUI.HorizontalSlider(outRect, this.Value, this.Min, this.Max, this.Style, this.ThumbStyle));
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000F224 File Offset: 0x0000E224
		private void onChange(float newValue)
		{
			if (newValue != this.Value)
			{
				this.OnChange(this, new SliderEventArgs(this.name, newValue));
				this.Value = newValue;
			}
		}

		// Token: 0x04000108 RID: 264
		public GUIStyle Style = "horizontalSlider";

		// Token: 0x04000109 RID: 265
		public GUIStyle ThumbStyle = "horizontalSliderThumb";

		// Token: 0x0400010A RID: 266
		public float Value;

		// Token: 0x0400010B RID: 267
		public float Min;

		// Token: 0x0400010C RID: 268
		public float Max;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using param;
using UnityEngine;
using UnityInjector;
using UnityInjector.Attributes;
using UnityObsoleteGui;

namespace COM3D2CBL.AddChikuwaSupporter.Plugin
{
	// Token: 0x02000002 RID: 2
	[PluginFilter("COM3D2OHVRx64")]
	[PluginFilter("COM3D2OHx86")]
	[PluginName(" AddChikuwaSupporter_feat_YotogiSlider")]
	[PluginVersion("0.2.0.15")]
	[PluginFilter("COM3D2OHx64")]
	public class AddChikuwaSupporter : PluginBase
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00001050
		private bool canStart
		{
			get
			{
				return this.bInitCompleted && this.bLoadBoneAnimetion && !this.bFadeInWait;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002080 File Offset: 0x00001080
		private int idxAheOrgasm
		{
			get
			{
				return (int)Math.Min(Math.Max(Math.Floor((double)((float)(this.iOrgasmCount - 1) / this.fOrgasmsPerAheLevel)), 0.0), 2.0);
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000003 RID: 3 RVA: 0x000020C8 File Offset: 0x000010C8
		private int iKupaMin
		{
			get
			{
				return Mathf.Max(this.iKupaDef, Mathf.Min(this.iKupaStart + this.iKupaIncrementPerOrgasm * this.iOrgasmCount, this.iKupaNormalMax));
			}
		}

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000004 RID: 4 RVA: 0x00002104 File Offset: 0x00001104
		private int iAnalKupaMin
		{
			get
			{
				return Mathf.Max(this.iAnalKupaDef, Mathf.Min(this.iAnalKupaStart + this.iAnalKupaIncrementPerOrgasm * this.iOrgasmCount, this.iAnalKupaNormalMax));
			}
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002140 File Offset: 0x00001140
		public void Awake()
		{
			this.pa["WIN.Load"] = new AddChikuwaSupporter.PlayAnime("WIN.Load", 2, 0f, 0.25f, AddChikuwaSupporter.PlayAnime.Formula.Quadratic);
			this.pa["AHE.継続.0"] = new AddChikuwaSupporter.PlayAnime("AHE.継続.0", 1, 0f, 0.75f);
			this.pa["AHE.絶頂.0"] = new AddChikuwaSupporter.PlayAnime("AHE.絶頂.0", 2, 6f, 9f);
			this.pa["AHE.痙攣.0"] = new AddChikuwaSupporter.PlayAnime("AHE.痙攣.0", 1, 0f, 9f, AddChikuwaSupporter.PlayAnime.Formula.Convulsion);
			this.pa["AHE.痙攣.1"] = new AddChikuwaSupporter.PlayAnime("AHE.痙攣.1", 1, 0f, 10f, AddChikuwaSupporter.PlayAnime.Formula.Convulsion);
			this.pa["AHE.痙攣.2"] = new AddChikuwaSupporter.PlayAnime("AHE.痙攣.2", 1, 0f, 11f, AddChikuwaSupporter.PlayAnime.Formula.Convulsion);
			this.pa["BOTE.絶頂"] = new AddChikuwaSupporter.PlayAnime("BOTE.絶頂", 1, 0f, 6f);
			this.pa["BOTE.止める"] = new AddChikuwaSupporter.PlayAnime("BOTE.止める", 1, 0f, 4f);
			this.pa["BOTE.流れ出る"] = new AddChikuwaSupporter.PlayAnime("BOTE.流れ出る", 1, 0f, 20f);
			this.pa["KUPA.挿入.0"] = new AddChikuwaSupporter.PlayAnime("KUPA.挿入.0", 1, 0.2f, 1f);
			this.pa["KUPA.挿入.1"] = new AddChikuwaSupporter.PlayAnime("KUPA.挿入.1", 1, 1.5f, 2.5f);
			this.pa["KUPA.止める"] = new AddChikuwaSupporter.PlayAnime("KUPA.止める", 1, 1f, 3f);
			this.pa["AKPA.挿入.0"] = new AddChikuwaSupporter.PlayAnime("AKPA.挿入.0", 1, 0.5f, 1.5f);
			this.pa["AKPA.挿入.1"] = new AddChikuwaSupporter.PlayAnime("AKPA.挿入.1", 1, 1.5f, 2.5f);
			this.pa["AKPA.止める"] = new AddChikuwaSupporter.PlayAnime("AKPA.止める", 1, 1f, 3f);
			this.pa["KUPACL.剥く.0"] = new AddChikuwaSupporter.PlayAnime("KUPACL.剥く.0", 1, 0f, 0.3f);
			this.pa["KUPACL.剥く.1"] = new AddChikuwaSupporter.PlayAnime("KUPACL.剥く.1", 1, 0.2f, 0.6f);
			this.pa["KUPACL.被る"] = new AddChikuwaSupporter.PlayAnime("KUPACL.被る", 1, 0f, 0.4f);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002400 File Offset: 0x00001400
		public void OnLevelWasLoaded(int level)
		{
			this.fPassedTimeOnLevel = 0f;
			if (level == 10)
			{
				base.StartCoroutine(this.initCoroutine(this.TimePerInit));
			}
			else
			{
				if ((level == 2 || level == 4 || level == 11) && this.bInitCompleted)
				{
					this.maid = GameMain.Instance.CharacterMgr.GetMaid(0);
					if (!this.maid)
					{
						return;
					}
					this.updateMaidEyePosY(0f);
					this.maid.ResetProp("wear");
					this.maid.ResetProp("Hara");
					this.maid.ResetAll();
					this.maid.SetProp("Hara", this.iDefHara, false);
					this.updateSlider("Slider:Hara", (float)this.iDefHara);
					this.VertexMorph_FromProcItem(this.maid.body0, "hara", (float)this.iDefHara);
					this.updateShapeKeyChikubiBokkiValue(this.iDefChikubiNae);
					this.updateShapeKeyChikubiTareValue(this.iDefChikubiTare);
					this.bInitCompleted = false;
				}
				this.bLoadBoneAnimetion = false;
			}
			this.sceneLevel = level;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002540 File Offset: 0x00001540
		public void Update()
		{
			this.fPassedTimeOnLevel += Time.deltaTime;
			if (this.sceneLevel == 10 && this.bInitCompleted)
			{
				switch (this.yotogiPlayManagerWithChubLip.fade_status)
				{
				case 1:
					if (!this.bFadeInWait)
					{
						this.bSyncMotionSpeed = false;
						this.bFadeInWait = true;
					}
					break;
				case 3:
					if (this.bFadeInWait)
					{
						this.detectSkillCbl();
						this.bFadeInWait = false;
					}
					else if (this.canStart)
					{
						if (this.chuBlipManager.GetFinish())
						{
							if (!this.isOnFinish)
							{
								this.isOnFinish = true;
								base.StartCoroutine(this.OnClickFinishButtonCoroutine(0.1f));
							}
						}
						this.checkAutoIKUOnUpdate();
						this.isDeviceIn = this.chuBlipManager.DeviceIn();
						if (this.isDeviceIn != this.isBeforeDeviceIn)
						{
							if (this.isDeviceIn)
							{
							}
							this.animateAutoKupa();
						}
						else if (this.isDeviceIn)
						{
							this.incrementValuesOnUpdate();
							this.overrideFaceBlendOnUpdate();
						}
						this.isBeforeDeviceIn = this.isDeviceIn;
						if (!this.goParameterUnit)
						{
							this.goParameterUnit = GameObject.Find(this.parameterUnitName);
						}
						if (AddChikuwaSupporter.IsActive(this.goParameterUnit))
						{
							if (this.slider.ContainsKey("Reason"))
							{
								this.slider["Reason"].Visible = true;
							}
							if (!this.bParameterViewerInitCompleted)
							{
								this.yotogiParamBasicBarWithChubLip = BaseMgr<YotogiParamBasicBarWithChubLip>.Instance;
								this.bParameterViewerInitCompleted = true;
							}
						}
						else
						{
							if (this.slider.ContainsKey("Reason"))
							{
								this.slider["Reason"].Visible = false;
							}
							this.bParameterViewerInitCompleted = false;
						}
						if (Input.GetKeyDown(this.ToggleWindowKey))
						{
							this.winAnimeRect = this.window.Rectangle;
							this.visible = !this.visible;
							this.playAnimeOnInputKeyDown(this.ToggleWindowKey);
						}
						if (this.fPassedTimeOnCommand >= 0f)
						{
							this.fPassedTimeOnCommand += Time.deltaTime;
						}
						this.updateAnimeOnUpdate();
					}
					if (this.panel["Status"].Enabled)
					{
						this.slider["PistonDepthShallow"].Value = this.chuBlipManager.GetInsertDepth() * 1000f;
						this.slider["PistonDepthDeep"].Value = this.chuBlipManager.GetInsertDepth() * 1000f;
						this.slider["InsertDepth"].Value = this.chuBlipManager.GetInsertDepth() * 1000f;
						this.slider["CurrentPistonSpeed"].Value = this.chuBlipManager.GetPistonSpeedDecision();
						this.slider["HoleSensitivity"].Value = GameMain.Instance.CMSystem.HoleSensitivity * 1000f;
						this.slider["HoleSync"].Value = GameMain.Instance.CMSystem.HoleSync * 1000f;
						this.slider["HoleAutoSpeed"].Value = (float)GameMain.Instance.CMSystem.HoleAutoSpeed;
						this.slider["HoleAutoInsertStartPos"].Value = (float)GameMain.Instance.CMSystem.HoleAutoInsertStartPos;
						this.slider["HoleAutoInsertEndPos"].Value = (float)GameMain.Instance.CMSystem.HoleAutoInsertEndPos;
					}
					break;
				}
			}
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002958 File Offset: 0x00001958
		public void OnGUI()
		{
			if ((this.sceneLevel == 14 || this.sceneLevel == 10) && this.canStart)
			{
				this.updateAnimeOnGUI();
				if (this.visible && !this.pa["WIN.Load"].NowPlaying)
				{
					this.updateCameraControl();
					this.window.Draw();
				}
			}
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000030E0 File Offset: 0x000020E0
		private IEnumerator OnClickFinishButtonCoroutine(float waitTime)
		{
			this.iLastExcite = (int)this.slider["Excite"].Value;
			this.fLastSliderSensitivity = this.slider["Sensitivity"].Value;
			this.iLastSliderFrustration = this.getSliderFrustration();
			this.fPassedTimeOnCommand = 0f;
			if (this.panel["Status"].Enabled)
			{
				this.updateMaidFrustration(this.iLastSliderFrustration);
			}
			this.initAnimeOnCommand();
			if (this.panel["AutoAHE"].Enabled)
			{
				float num = (float)this.maid.Param.status.cur_excite;
				int idxAheOrgasm = this.idxAheOrgasm;
				if (this.iLastExcite >= this.iAheExcite[idxAheOrgasm])
				{
					this.pa["AHE.継続.0"].Play(this.fAheLastEye, this.fAheOrgasmEyeMax[idxAheOrgasm]);
					float[] vform = new float[]
					{
						this.fAheOrgasmEyeMax[idxAheOrgasm],
						this.fAheOrgasmSpeed[idxAheOrgasm]
					};
					float[] vto = new float[]
					{
						this.fAheOrgasmEyeMin[idxAheOrgasm],
						100f * this.slider["AfterOrgasmDecrement"].Value / 100f
					};
					this.updateMotionSpeed(this.fAheOrgasmSpeed[idxAheOrgasm]);
					this.pa["AHE.絶頂.0"].Play(vform, vto);
					if (!this.panel["FaceAnime"].Enabled && this.pa["AHE.絶頂.0"].NowPlaying)
					{
						this.maid.FaceAnime(this.sAheOrgasmFace[this.idxAheOrgasm], 5f, 0);
						this.panel["FaceAnime"].HeaderUILabelText = this.sAheOrgasmFace[this.idxAheOrgasm];
					}
					if (this.toggle["Convulsion"].Value)
					{
						if (this.pa["AHE.痙攣." + idxAheOrgasm].NowPlaying)
						{
							this.iAheOrgasmChain++;
						}
						this.pa["AHE.痙攣." + idxAheOrgasm].Play(0f, this.fAheOrgasmConvulsion[idxAheOrgasm]);
					}
					this.iOrgasmCount++;
				}
			}
			if (this.panel["AutoBOTE"].Enabled)
			{
				float vform2 = (float)Mathf.Max(this.iCurrentHara, this.iDefHara);
				float num2 = (float)this.iDefHara;
				YotogiCBL.SkillData selectSkillData = YotogiPlayManagerWithChubLip.GetSelectSkillData();
				bool flag = false;
				foreach (string value in this.sNoBoteSkillNames)
				{
					if (selectSkillData.skill_name.StartsWith(value))
					{
						flag = true;
					}
				}
				if (!flag && (float)((int)this.slider["InsertDepth"].Value) >= 500f)
				{
					this.iBoteCount++;
					num2 = (float)Mathf.Min(this.iCurrentHara + this.iHaraIncrement, this.iBoteHaraMax);
					this.pa["BOTE.絶頂"].Play(vform2, num2);
				}
				else
				{
					if (this.toggle["SlowCreampie"].Value)
					{
						this.pa["BOTE.流れ出る"].Play(vform2, num2);
					}
					else
					{
						this.pa["BOTE.止める"].Play(vform2, num2);
					}
					this.iBoteCount = 0;
				}
				this.iCurrentHara = (int)num2;
			}
			this.animateAutoTun();
			this.animateAutoKupa();
			if (!this.panel["FaceAnime"].Enabled && this.pa["AHE.絶頂.0"].NowPlaying)
			{
				this.maid.FaceAnime(this.sAheOrgasmFace[this.idxAheOrgasm], 5f, 0);
				this.panel["FaceAnime"].HeaderUILabelText = this.sAheOrgasmFace[this.idxAheOrgasm];
			}
			while (this.chuBlipManager.GetFinish())
			{
				yield return new WaitForSeconds(waitTime);
			}
			this.isOnFinish = false;
			this.updateMaidExcite((int)(this.slider["OrgasmThreshold"].Value * (100f - this.slider["AfterOrgasmDecrement"].Value) / 100f));
			yield break;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00003108 File Offset: 0x00002108
		public void OnYotogiPlayManagerOnClickCommand(Yotogi.SkillData.Command.Data command_data)
		{
			this.iLastExcite = this.maid.Param.status.cur_excite;
			this.fLastSliderSensitivity = this.slider["Sensitivity"].Value;
			this.iLastSliderFrustration = this.getSliderFrustration();
			this.fPassedTimeOnCommand = 0f;
			if (this.panel["Status"].Enabled)
			{
				this.updateMaidFrustration(this.iLastSliderFrustration);
			}
			this.initAnimeOnCommand();
			this.orgOnClickCommand(command_data);
			this.playAnimeOnCommand(command_data.basic);
			this.syncSlidersOnClickCommand(command_data.status);
			if (command_data.basic.command_type == 4)
			{
				if (!this.panel["FaceAnime"].Enabled && this.pa["AHE.絶頂.0"].NowPlaying)
				{
					this.maid.FaceAnime(this.sAheOrgasmFace[this.idxAheOrgasm], 5f, 0);
					this.panel["FaceAnime"].HeaderUILabelText = this.sAheOrgasmFace[this.idxAheOrgasm];
				}
			}
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000324C File Offset: 0x0000224C
		public bool OnYotogiKagManagerTagFace(KagTagSupport tag_data)
		{
			bool result;
			if (this.panel["FaceAnime"].Enabled || this.pa["AHE.絶頂.0"].NowPlaying)
			{
				result = false;
			}
			else
			{
				this.panel["FaceAnime"].HeaderUILabelText = tag_data.GetTagProperty("name").AsString();
				result = this.orgTagFace(tag_data);
			}
			return result;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000032CC File Offset: 0x000022CC
		public bool OnYotogiKagManagerTagFaceBlend(KagTagSupport tag_data)
		{
			bool result;
			if (this.panel["FaceBlend"].Enabled || this.pa["AHE.絶頂.0"].NowPlaying)
			{
				result = false;
			}
			else
			{
				this.panel["FaceBlend"].HeaderUILabelText = tag_data.GetTagProperty("name").AsString();
				result = this.orgTagFaceBlend(tag_data);
			}
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000334C File Offset: 0x0000234C
		public void OnChangeSliderExcite(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateMaidExcite((int)args.Value);
			}
		}

		// Token: 0x0600000E RID: 14 RVA: 0x00003384 File Offset: 0x00002384
		public void OnChangeSliderMind(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateMaidMind((int)args.Value);
			}
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000033BC File Offset: 0x000023BC
		public void OnChangeSliderPistonDepthShallow(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updatePistonDepthShallow((int)args.Value);
			}
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000033F4 File Offset: 0x000023F4
		public void OnChangeSliderPistonDepthDeep(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updatePistonDepthDeep((int)args.Value);
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x0000342C File Offset: 0x0000242C
		public void OnChangeSliderInsertDepth(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateInsertDepth((int)args.Value);
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00003464 File Offset: 0x00002464
		public void OnChangeSliderCurrentPistonSpeed(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateCurrentPistonSpeed((int)args.Value);
			}
		}

		// Token: 0x06000013 RID: 19 RVA: 0x0000349C File Offset: 0x0000249C
		public void OnChangeSliderReason(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateMaidReason((int)args.Value);
			}
		}

		// Token: 0x06000014 RID: 20 RVA: 0x000034D4 File Offset: 0x000024D4
		public void OnChangeSliderHoleSensitivity(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateHoleSensitivity((int)args.Value);
			}
		}

		// Token: 0x06000015 RID: 21 RVA: 0x0000350C File Offset: 0x0000250C
		public void OnChangeSliderHoleSync(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateHoleSync((int)args.Value);
			}
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00003544 File Offset: 0x00002544
		public void OnChangeSliderHoleAutoSpeed(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateHoleAutoSpeed((int)args.Value);
			}
		}

		// Token: 0x06000017 RID: 23 RVA: 0x0000357C File Offset: 0x0000257C
		public void OnChangeSliderHoleAutoInsertStartPos(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateHoleAutoInsertStartPos((int)args.Value);
			}
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000035B4 File Offset: 0x000025B4
		public void OnChangeSliderHoleAutoInsertEndPos(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateHoleAutoInsertEndPos((int)args.Value);
			}
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000035EC File Offset: 0x000025EC
		public void OnChangeSliderSensitivity(object ys, SliderEventArgs args)
		{
			this.setExIni<float>("Status", "Sensitivity", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600001A RID: 26 RVA: 0x00003610 File Offset: 0x00002610
		public void OnChangeSliderMotionSpeed(object ys, SliderEventArgs args)
		{
			if (this.panel["Status"].Enabled)
			{
				this.updateMotionSpeed(args.Value);
			}
		}

		// Token: 0x0600001B RID: 27 RVA: 0x00003647 File Offset: 0x00002647
		public void OnChangeToggleCheckPistonSpeed(object tgl, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoIKU", "CheckPistonSpeedEnabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600001C RID: 28 RVA: 0x00003668 File Offset: 0x00002668
		public void OnChangeToggleSupportCreamPie(object tgl, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoIKU", "SupportCreamPieEnabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600001D RID: 29 RVA: 0x00003689 File Offset: 0x00002689
		public void OnChangeToggleContinueInsert(object tgl, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoIKU", "ContinueInsertEnabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600001E RID: 30 RVA: 0x000036AC File Offset: 0x000026AC
		public void OnChangeSliderOrgasmThreshold(object ys, SliderEventArgs args)
		{
			if (this.panel["AutoIKU"].Enabled)
			{
				this.setExIni<float>("AutoIKU", "OrgasmThreshold", args.Value);
				base.SaveConfig();
				this.updateOrgasmThreshold(args.Value);
			}
		}

		// Token: 0x0600001F RID: 31 RVA: 0x00003704 File Offset: 0x00002704
		public void OnChangeSliderOrgasmStartSecond(object ys, SliderEventArgs args)
		{
			if (this.panel["AutoIKU"].Enabled)
			{
				this.setExIni<float>("AutoIKU", "OrgasmStartSecond", args.Value);
				base.SaveConfig();
				this.updateOrgasmStartSecond(args.Value);
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x0000375C File Offset: 0x0000275C
		public void OnChangeSliderVaginalFornixReachCount(object ys, SliderEventArgs args)
		{
			if (this.panel["AutoIKU"].Enabled)
			{
				this.setExIni<float>("AutoIKU", "VaginalFornixReachCount", args.Value);
				base.SaveConfig();
				this.updateVaginalFornixReachCount(args.Value);
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000037B4 File Offset: 0x000027B4
		public void OnChangeSliderAfterOrgasmDecrement(object ys, SliderEventArgs args)
		{
			if (this.panel["AutoIKU"].Enabled)
			{
				this.setExIni<float>("AutoIKU", "AfterOrgasmDecrement", args.Value);
				base.SaveConfig();
				this.updateAfterOrgasmDecrement(args.Value);
			}
		}

		// Token: 0x06000022 RID: 34 RVA: 0x0000380B File Offset: 0x0000280B
		public void OnChangeSliderEyeY(object ys, SliderEventArgs args)
		{
			this.updateMaidEyePosY(args.Value);
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000381B File Offset: 0x0000281B
		public void OnChangeSliderAHEEyeMax(object ys, SliderEventArgs args)
		{
			this.setExIni<float>("AutoAHE", "AHEEyeMax", args.Value);
			base.SaveConfig();
			this.updateAHEEyeMax(args.Value);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x0000384C File Offset: 0x0000284C
		public void OnChangeSliderChikubiScale(object ys, SliderEventArgs args)
		{
			float value = this.slider["ChikubiBokki"].Value * this.slider["ChikubiScale"].Value / 100f;
			this.updateShapeKeyChikubiBokkiValue(value);
			this.setExIni<float>("AutoTUN", "ChikubiScale", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000025 RID: 37 RVA: 0x000038B2 File Offset: 0x000028B2
		public void OnChangeSliderChikubiNae(object ys, SliderEventArgs args)
		{
			this.updateChikubiNaeValue(args.Value);
			this.setExIni<float>("AutoTUN", "ChikubiNae", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000038E0 File Offset: 0x000028E0
		public void OnChangeSliderChikubiBokki(object ys, SliderEventArgs args)
		{
			float value = args.Value * this.slider["ChikubiScale"].Value / 100f;
			this.updateShapeKeyChikubiBokkiValue(value);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x00003919 File Offset: 0x00002919
		public void OnChangeSliderChikubiTare(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyChikubiTareValue(args.Value);
			this.setExIni<float>("AutoTUN", "ChikubiTare", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000028 RID: 40 RVA: 0x00003947 File Offset: 0x00002947
		public void OnChangeToggleSlowCreampie(object tgl, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoBOTE", "SlowCreampie", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000029 RID: 41 RVA: 0x00003968 File Offset: 0x00002968
		public void OnChangeSliderHara(object ys, SliderEventArgs args)
		{
			this.updateMaidHaraValue(args.Value);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00003978 File Offset: 0x00002978
		public void OnChangeSliderKupa(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyKupaValue(args.Value);
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00003988 File Offset: 0x00002988
		public void OnChangeToggleAutoIncrement(object tgl, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoAHE", "AutoIncrementEnabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600002C RID: 44 RVA: 0x000039A9 File Offset: 0x000029A9
		public void OnChangeSliderAnalKupa(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyAnalKupaValue(args.Value);
		}

		// Token: 0x0600002D RID: 45 RVA: 0x000039B9 File Offset: 0x000029B9
		public void OnChangeSliderKupaLevel(object ys, SliderEventArgs args)
		{
			this.setExIni<float>("AutoKUPA", "KupaLevel", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600002E RID: 46 RVA: 0x000039DA File Offset: 0x000029DA
		public void OnChangeSliderLabiaKupa(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyLabiaKupaValue(args.Value);
			this.setExIni<float>("AutoKUPA", "LabiaKupa", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00003A08 File Offset: 0x00002A08
		public void OnChangeSliderVaginaKupa(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyVaginaKupaValue(args.Value);
			this.setExIni<float>("AutoKUPA", "VaginaKupa", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003A36 File Offset: 0x00002A36
		public void OnChangeSliderNyodoKupa(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyNyodoKupaValue(args.Value);
			this.setExIni<float>("AutoKUPA", "NyodoKupa", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003A64 File Offset: 0x00002A64
		public void OnChangeSliderSuji(object ys, SliderEventArgs args)
		{
			this.updateShapeKeySujiValue(args.Value);
			this.setExIni<float>("AutoKUPA", "Suji", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003A92 File Offset: 0x00002A92
		public void OnChangeSliderClitoris(object ys, SliderEventArgs args)
		{
			this.updateShapeKeyClitorisValue(args.Value);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00003AA2 File Offset: 0x00002AA2
		public void OnChangeToggleLipsync(object tgl, ToggleEventArgs args)
		{
			this.updateMaidFoceKuchipakuSelfUpdateTime(args.Value);
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00003AB2 File Offset: 0x00002AB2
		public void OnChangeToggleConvulsion(object tgl, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoAHE", "ConvulsionEnabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000035 RID: 53 RVA: 0x00003AD3 File Offset: 0x00002AD3
		public void OnChangeEnabledAutoPiston(object panel, ToggleEventArgs args)
		{
			this.updateAutoPiston(args.Value);
			this.setExIni<bool>("AutoPiston", "Enabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003B01 File Offset: 0x00002B01
		public void OnChangeEnabledAutoIKU(object panel, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoIKU", "Enabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00003B22 File Offset: 0x00002B22
		public void OnChangeEnabledAutoAHE(object panel, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoAHE", "Enabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00003B43 File Offset: 0x00002B43
		public void OnChangeEnabledAutoTUN(object panel, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoTUN", "Enabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00003B64 File Offset: 0x00002B64
		public void OnChangeEnabledAutoBOTE(object panel, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoBOTE", "Enabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003B85 File Offset: 0x00002B85
		public void OnChangeEnabledAutoKUPA(object panel, ToggleEventArgs args)
		{
			this.setExIni<bool>("AutoKUPA", "Enabled", args.Value);
			base.SaveConfig();
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003BA8 File Offset: 0x00002BA8
		public void OnClickButtonFaceAnime(object ygb, ButtonEventArgs args)
		{
			if (this.panel["FaceAnime"].Enabled)
			{
				this.maid.FaceAnime(args.ButtonName, 1f, 0);
				this.panel["FaceAnime"].HeaderUILabelText = args.ButtonName;
			}
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003C08 File Offset: 0x00002C08
		public void OnClickButtonFaceBlend(object ysg, ButtonEventArgs args)
		{
			if (this.panel["FaceBlend"].Enabled)
			{
				this.maid.FaceBlend(args.ButtonName);
				this.panel["FaceBlend"].HeaderUILabelText = args.ButtonName;
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003C61 File Offset: 0x00002C61
		public void OnClickButtonStageSelect(object ysg, ButtonEventArgs args)
		{
			GameMain.Instance.BgMgr.ChangeBg(args.ButtonName);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003D74 File Offset: 0x00002D74
		private IEnumerator initCoroutine(float waitTime)
		{
			yield return new WaitForSeconds(this.WaitFirstInit);
			while (!(this.bInitCompleted = this.initialize()))
			{
				yield return new WaitForSeconds(waitTime);
			}
			yield break;
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003D9C File Offset: 0x00002D9C
		private bool initialize()
		{
			if (!this.goCommandUnit)
			{
				this.goCommandUnit = GameObject.Find(this.commandUnitName);
			}
			bool result;
			if (!AddChikuwaSupporter.IsActive(this.goCommandUnit))
			{
				result = false;
			}
			else
			{
				this.maid = GameMain.Instance.CharacterMgr.GetMaid(0);
				if (!this.maid)
				{
					result = false;
				}
				else
				{
					this.maidStatusInfo = AddChikuwaSupporter.getFieldInfo<MaidParam>("status_");
					if (AddChikuwaSupporter.IsNull<FieldInfo>(this.maidStatusInfo))
					{
						result = false;
					}
					else
					{
						this.maidFoceKuchipakuSelfUpdateTime = AddChikuwaSupporter.getFieldInfo<Maid>("m_bFoceKuchipakuSelfUpdateTime");
						if (AddChikuwaSupporter.IsNull<FieldInfo>(this.maidFoceKuchipakuSelfUpdateTime))
						{
							result = false;
						}
						else
						{
							this.yotogiParamBasicBar = AddChikuwaSupporter.getInstance<YotogiParamBasicBar>();
							this.chuBlipManager = GameMain.Instance.ChubLipMgr;
							if (!this.chuBlipManager)
							{
								result = false;
							}
							else
							{
								this.onaholeChuBlipDevice = AddChikuwaSupporter.getInstance<OnaholeChuBlipDevice>();
								if (!this.onaholeChuBlipDevice)
								{
									result = false;
								}
								else
								{
									this.onaholeMotion = AddChikuwaSupporter.getInstance<OnaholeMotion>();
									if (!this.onaholeMotion)
									{
										result = false;
									}
									else
									{
										this.yotogiPlayManagerWithChubLip = AddChikuwaSupporter.getInstance<YotogiPlayManagerWithChubLip>();
										YotogiKagManager yotogi_kag = GameMain.Instance.ScriptMgr.yotogi_kag;
										this.kagScript = AddChikuwaSupporter.getFieldValue<YotogiKagManager, KagScript>(yotogi_kag, "kag_");
										try
										{
											this.kagScript.RemoveTagCallBack("face");
											this.kagScript.AddTagCallBack("face", new KagScript.KagTagCallBack(this.OnYotogiKagManagerTagFace));
											this.kagScript.RemoveTagCallBack("faceblend");
											this.kagScript.AddTagCallBack("faceblend", new KagScript.KagTagCallBack(this.OnYotogiKagManagerTagFaceBlend));
											this.kagScriptCallbacksOverride = true;
										}
										catch (Exception ex)
										{
											AddChikuwaSupporter.LogError("kagScriptCallBack() : {0}", new object[]
											{
												ex
											});
										}
										this.orgTagFace = AddChikuwaSupporter.getMethodDelegate<YotogiKagManager, Func<KagTagSupport, bool>>(yotogi_kag, "TagFace");
										this.orgTagFaceBlend = AddChikuwaSupporter.getMethodDelegate<YotogiKagManager, Func<KagTagSupport, bool>>(yotogi_kag, "TagFaceBlend");
										foreach (KeyValuePair<Yotogi.Stage, Yotogi.StageData> keyValuePair in Yotogi.stage_data_list)
										{
											this.sStageNames.Add(keyValuePair.Value.prefab_name);
										}
										foreach (KeyValuePair<string, AddChikuwaSupporter.PlayAnime> keyValuePair2 in this.pa)
										{
											AddChikuwaSupporter.PlayAnime value = keyValuePair2.Value;
											if (!value.SetterExist)
											{
												if (value.Contains("WIN"))
												{
													value.SetSetter(new Action<float[]>(this.updateWindowAnime));
												}
												if (value.Contains("BOTE"))
												{
													value.SetSetter(new Action<float>(this.updateMaidHaraValue));
												}
												if (value.Contains("KUPA"))
												{
													value.SetSetter(new Action<float>(this.updateShapeKeyKupaValue));
												}
												if (value.Contains("AKPA"))
												{
													value.SetSetter(new Action<float>(this.updateShapeKeyAnalKupaValue));
												}
												if (value.Contains("KUPACL"))
												{
													value.SetSetter(new Action<float>(this.updateShapeKeyClitorisValue));
												}
												if (value.Contains("AHE"))
												{
													value.SetSetter(new Action<float>(this.updateOrgasmConvulsion));
												}
												if (value.Contains("AHE.継続"))
												{
													value.SetSetter(new Action<float>(this.updateMaidEyePosY));
												}
												if (value.Contains("AHE.絶頂"))
												{
													value.SetSetter(new Action<float[]>(this.updateAheOrgasm));
												}
											}
										}
										this.fAheDefEye = this.maid.body0.trsEyeL.localPosition.y * this.fEyePosToSliderMul;
										this.iDefHara = this.maid.GetProp("Hara").value;
										this.iBoteCount = 0;
										this.iOrgasmCount = 0;
										this.iAheOrgasmChain = 0;
										this.bKupaAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("kupa");
										this.bOrgasmAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("orgasm");
										this.bAnalKupaAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("analkupa");
										this.bLabiaKupaAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("labiakupa");
										this.bVaginaKupaAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("vaginakupa");
										this.bNyodoKupaAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("nyodokupa");
										this.bSujiAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("suji");
										this.bClitorisAvailable = this.maid.body0.goSlot[0].morph.hash.ContainsKey("clitoris");
										this.bBokkiChikubiAvailable = this.isExistVertexMorph(this.maid.body0, "chikubi_bokki");
										this.window = new Window(this.winRatioRect, "0.1.0.15", "Chikuwa Supporter");
										float num = (float)this.maid.Param.status.mind;
										float def = this.chuBlipManager.GetInsertDepth() * 1000f * 0.01f;
										float def2 = this.chuBlipManager.GetInsertDepth() * 1000f * 0.5f;
										float def3 = this.chuBlipManager.GetInsertDepth() * 1000f;
										float def4 = this.chuBlipManager.GetPistonSpeedDecision();
										float num2 = (float)this.maid.Param.status.reason;
										this.fCurrentHoleSensitivity = GameMain.Instance.CMSystem.HoleSensitivity * 1000f;
										float def5 = GameMain.Instance.CMSystem.HoleSync * 1000f;
										float def6 = (float)GameMain.Instance.CMSystem.HoleAutoSpeed;
										float def7 = (float)GameMain.Instance.CMSystem.HoleAutoInsertStartPos;
										float num3 = (float)GameMain.Instance.CMSystem.HoleAutoInsertEndPos;
										int def8 = this.sStageNames.IndexOf(YotogiStageSelectManager.StagePrefab);
										this.slider["PistonDepthShallow"] = new AddChikuwaSupporter.YotogiSlider("Slider:PistonDepthShallow", 11f, 500f, def, new EventHandler<SliderEventArgs>(this.OnChangeSliderPistonDepthShallow), this.sliderNameStatus[0], false);
										this.slider["PistonDepthDeep"] = new AddChikuwaSupporter.YotogiSlider("Slider:PistonDepthDeep", 501f, 1000f, def2, new EventHandler<SliderEventArgs>(this.OnChangeSliderPistonDepthDeep), this.sliderNameStatus[2], false);
										this.slider["InsertDepth"] = new AddChikuwaSupporter.YotogiSlider("Slider:InsertDepth", 0f, 1000f, def3, new EventHandler<SliderEventArgs>(this.OnChangeSliderInsertDepth), this.sliderNameStatus[1], false);
										this.slider["CurrentPistonSpeed"] = new AddChikuwaSupporter.YotogiSlider("Slider:CurrentPistonSpeed", 0f, 100f, def4, new EventHandler<SliderEventArgs>(this.OnChangeSliderCurrentPistonSpeed), this.sliderNameStatus[3], false);
										this.slider["Reason"] = new AddChikuwaSupporter.YotogiSlider("Slider:Reason", 0f, num2, num2, new EventHandler<SliderEventArgs>(this.OnChangeSliderReason), this.sliderNameStatus[4], true);
										this.slider["HoleSensitivity"] = new AddChikuwaSupporter.YotogiSlider("Slider:HoleSensitivity", 0f, 1000f, this.fCurrentHoleSensitivity, new EventHandler<SliderEventArgs>(this.OnChangeSliderHoleSensitivity), this.sliderNameStatus[5], true);
										this.slider["HoleSync"] = new AddChikuwaSupporter.YotogiSlider("Slider:HoleSync", 0f, 1000f, def5, new EventHandler<SliderEventArgs>(this.OnChangeSliderHoleSync), this.sliderNameStatus[6], true);
										this.slider["Excite"] = new AddChikuwaSupporter.YotogiSlider("Slider:Excite", -100f, 300f, 0f, new EventHandler<SliderEventArgs>(this.OnChangeSliderExcite), this.sliderNameStatus[7], true);
										this.slider["Sensitivity"] = new AddChikuwaSupporter.YotogiSlider("Slider:Sensitivity", -100f, 200f, this.fSensitivity, new EventHandler<SliderEventArgs>(this.OnChangeSliderSensitivity), this.sliderNameStatus[8], true);
										this.slider["HoleAutoSpeed"] = new AddChikuwaSupporter.YotogiSlider("Slider:HoleAutoSpeed", 0f, 1000f, def6, new EventHandler<SliderEventArgs>(this.OnChangeSliderHoleAutoSpeed), this.sliderNameAutoPiston[0], true);
										this.slider["HoleAutoInsertStartPos"] = new AddChikuwaSupporter.YotogiSlider("Slider:HoleAutoInsertStartPos", 0f, 1000f, def7, new EventHandler<SliderEventArgs>(this.OnChangeSliderHoleAutoInsertStartPos), this.sliderNameAutoPiston[1], true);
										this.slider["HoleAutoInsertEndPos"] = new AddChikuwaSupporter.YotogiSlider("Slider:HoleAutoInsertEndPos", 0f, 1000f, def7, new EventHandler<SliderEventArgs>(this.OnChangeSliderHoleAutoInsertEndPos), this.sliderNameAutoPiston[2], true);
										this.toggle["CheckPistonSpeed"] = new AddChikuwaSupporter.YotogiToggle("Toggle:CheckPistonSpeed", false, " Check piston speed", new EventHandler<ToggleEventArgs>(this.OnChangeToggleCheckPistonSpeed));
										this.toggle["SupportCreamPie"] = new AddChikuwaSupporter.YotogiToggle("Toggle:SupportCreamPie", false, " Support cream pie", new EventHandler<ToggleEventArgs>(this.OnChangeToggleSupportCreamPie));
										this.toggle["ContinueInsert"] = new AddChikuwaSupporter.YotogiToggle("Toggle:ContinueInsert", false, " Continue Insert", new EventHandler<ToggleEventArgs>(this.OnChangeToggleContinueInsert));
										this.slider["OrgasmThreshold"] = new AddChikuwaSupporter.YotogiSlider("Slider:OrgasmThreshold", 0f, 1000f, this.fOrgasmThreshold, new EventHandler<SliderEventArgs>(this.OnChangeSliderOrgasmThreshold), this.sliderNameAutoIKU[0], true);
										this.slider["OrgasmStartSecond"] = new AddChikuwaSupporter.YotogiSlider("Slider:OrgasmStartSecond", 0f, 10f, this.fOrgasmStartSecond, new EventHandler<SliderEventArgs>(this.OnChangeSliderOrgasmStartSecond), this.sliderNameAutoIKU[1], true);
										this.slider["VaginalFornixReachCount"] = new AddChikuwaSupporter.YotogiSlider("Slider:VaginalFornixReachCount", 0f, 20f, this.fVaginalFornixReachCount, new EventHandler<SliderEventArgs>(this.OnChangeSliderVaginalFornixReachCount), this.sliderNameAutoIKU[2], true);
										this.slider["AfterOrgasmDecrement"] = new AddChikuwaSupporter.YotogiSlider("Slider:AfterOrgasmDecrement", 100f, 0f, this.fAfterOrgasmDecrement, new EventHandler<SliderEventArgs>(this.OnChangeSliderAfterOrgasmDecrement), this.sliderNameAutoIKU[3], true);
										this.slider["EyeY"] = new AddChikuwaSupporter.YotogiSlider("Slider:EyeY", 0f, 100f, this.fAheDefEye, new EventHandler<SliderEventArgs>(this.OnChangeSliderEyeY), this.sliderNameAutoAHE[0], false);
										this.slider["AHEEyeMax"] = new AddChikuwaSupporter.YotogiSlider("Slider:AHEEyeMax", 0f, 100f, this.fAheEyeMax, new EventHandler<SliderEventArgs>(this.OnChangeSliderAHEEyeMax), this.sliderNameAutoAHE[1], false);
										this.slider["ChikubiScale"] = new AddChikuwaSupporter.YotogiSlider("Slider:ChikubiScale", 1f, 100f, this.fChikubiScale, new EventHandler<SliderEventArgs>(this.OnChangeSliderChikubiScale), this.sliderNameAutoTUN[0], true);
										this.slider["ChikubiNae"] = new AddChikuwaSupporter.YotogiSlider("Slider:ChikubiNae", -15f, 150f, this.fChikubiNae, new EventHandler<SliderEventArgs>(this.OnChangeSliderChikubiNae), this.sliderNameAutoTUN[1], true);
										this.slider["ChikubiBokki"] = new AddChikuwaSupporter.YotogiSlider("Slider:ChikubiBokki", -15f, 150f, this.fChikubiBokki, new EventHandler<SliderEventArgs>(this.OnChangeSliderChikubiBokki), this.sliderNameAutoTUN[2], true);
										this.slider["ChikubiTare"] = new AddChikuwaSupporter.YotogiSlider("Slider:ChikubiTare", 0f, 150f, this.fChikubiTare, new EventHandler<SliderEventArgs>(this.OnChangeSliderChikubiTare), this.sliderNameAutoTUN[3], true);
										this.toggle["SlowCreampie"] = new AddChikuwaSupporter.YotogiToggle("Toggle:SlowCreamPie", false, " Slow creampie", new EventHandler<ToggleEventArgs>(this.OnChangeToggleSlowCreampie));
										this.slider["Hara"] = new AddChikuwaSupporter.YotogiSlider("Slider:Hara", 0f, 150f, (float)this.iDefHara, new EventHandler<SliderEventArgs>(this.OnChangeSliderHara), this.sliderNameAutoBOTE[0], false);
										this.slider["Kupa"] = new AddChikuwaSupporter.YotogiSlider("Slider:Kupa", 0f, 150f, 0f, new EventHandler<SliderEventArgs>(this.OnChangeSliderKupa), this.sliderNameAutoKUPA[0], false);
										this.slider["AnalKupa"] = new AddChikuwaSupporter.YotogiSlider("Slider:AnalKupa", 0f, 150f, 0f, new EventHandler<SliderEventArgs>(this.OnChangeSliderAnalKupa), this.sliderNameAutoKUPA[1], false);
										this.slider["KupaLevel"] = new AddChikuwaSupporter.YotogiSlider("Slider:KupaLevel", 0f, 100f, this.fKupaLevel, new EventHandler<SliderEventArgs>(this.OnChangeSliderKupaLevel), this.sliderNameAutoKUPA[2], true);
										this.slider["LabiaKupa"] = new AddChikuwaSupporter.YotogiSlider("Slider:LabiaKupa", 0f, 150f, this.fLabiaKupa, new EventHandler<SliderEventArgs>(this.OnChangeSliderLabiaKupa), this.sliderNameAutoKUPA[3], true);
										this.slider["VaginaKupa"] = new AddChikuwaSupporter.YotogiSlider("Slider:VaginaKupa", 0f, 150f, this.fVaginaKupa, new EventHandler<SliderEventArgs>(this.OnChangeSliderVaginaKupa), this.sliderNameAutoKUPA[4], true);
										this.slider["NyodoKupa"] = new AddChikuwaSupporter.YotogiSlider("Slider:NyodoKupa", 0f, 150f, this.fNyodoKupa, new EventHandler<SliderEventArgs>(this.OnChangeSliderNyodoKupa), this.sliderNameAutoKUPA[5], true);
										this.slider["Suji"] = new AddChikuwaSupporter.YotogiSlider("Slider:Suji", 0f, 150f, this.fSuji, new EventHandler<SliderEventArgs>(this.OnChangeSliderSuji), this.sliderNameAutoKUPA[6], true);
										this.slider["Clitoris"] = new AddChikuwaSupporter.YotogiSlider("Slider:Clitoris", 0f, 150f, 0f, new EventHandler<SliderEventArgs>(this.OnChangeSliderClitoris), this.sliderNameAutoKUPA[7], false);
										this.toggle["AutoIncrement"] = new AddChikuwaSupporter.YotogiToggle("Toggle:AutoIncrement", false, " Auto increment", new EventHandler<ToggleEventArgs>(this.OnChangeToggleAutoIncrement));
										this.toggle["Lipsync"] = new AddChikuwaSupporter.YotogiToggle("Toggle:Lipsync", false, " Lipsync cancelling", new EventHandler<ToggleEventArgs>(this.OnChangeToggleLipsync));
										this.toggle["Convulsion"] = new AddChikuwaSupporter.YotogiToggle("Toggle:Convulsion", false, " Orgasm convulsion", new EventHandler<ToggleEventArgs>(this.OnChangeToggleConvulsion));
										this.grid["FaceAnime"] = new AddChikuwaSupporter.YotogiButtonGrid("GridButton:FaceAnime", this.sFaceNames, new EventHandler<ButtonEventArgs>(this.OnClickButtonFaceAnime), 6, false);
										this.grid["FaceBlend"] = new AddChikuwaSupporter.YotogiButtonGrid("GridButton:FaceBlend", this.sFaceNames, new EventHandler<ButtonEventArgs>(this.OnClickButtonFaceBlend), 6, true);
										this.lSelect["StageSelcet"] = new AddChikuwaSupporter.YotogiLineSelect("LineSelect:StageSelcet", "Stage : ", this.sStageNames.ToArray(), def8, new EventHandler<ButtonEventArgs>(this.OnClickButtonStageSelect));
										this.toggle["CheckPistonSpeed"].Visible = false;
										this.toggle["SupportCreamPie"].Visible = false;
										this.toggle["ContinueInsert"].Visible = false;
										this.slider["OrgasmThreshold"].Visible = false;
										this.slider["OrgasmStartSecond"].Visible = false;
										this.slider["VaginalFornixReachCount"].Visible = false;
										this.slider["AfterOrgasmDecrement"].Visible = false;
										this.slider["HoleAutoSpeed"].Visible = false;
										this.slider["HoleAutoInsertStartPos"].Visible = false;
										this.slider["HoleAutoInsertEndPos"].Visible = false;
										this.slider["EyeY"].Visible = false;
										this.slider["AHEEyeMax"].Visible = false;
										this.toggle["Convulsion"].Visible = false;
										this.toggle["AutoIncrement"].Visible = false;
										this.slider["ChikubiScale"].Visible = false;
										this.slider["ChikubiNae"].Visible = false;
										this.slider["ChikubiBokki"].Visible = false;
										this.slider["ChikubiTare"].Visible = false;
										this.toggle["SlowCreampie"].Visible = false;
										this.slider["Hara"].Visible = false;
										this.slider["Kupa"].Visible = false;
										this.slider["AnalKupa"].Visible = false;
										this.slider["KupaLevel"].Visible = false;
										this.slider["LabiaKupa"].Visible = false;
										this.slider["VaginaKupa"].Visible = false;
										this.slider["NyodoKupa"].Visible = false;
										this.slider["Suji"].Visible = false;
										this.slider["Clitoris"].Visible = false;
										this.toggle["Lipsync"].Visible = false;
										this.grid["FaceAnime"].Visible = false;
										this.grid["FaceBlend"].Visible = false;
										this.window.AddChild<AddChikuwaSupporter.YotogiLineSelect>(this.lSelect["StageSelcet"]);
										this.window.AddHorizontalSpacer();
										this.panel["Status"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:Status", "Status", AddChikuwaSupporter.YotogiPanel.HeaderUI.Slider));
										this.panel["Status"].Enabled = true;
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["PistonDepthShallow"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["InsertDepth"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["PistonDepthDeep"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["CurrentPistonSpeed"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Reason"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["HoleSensitivity"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["HoleSync"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Excite"]);
										this.panel["Status"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Sensitivity"]);
										this.window.AddHorizontalSpacer();
										this.panel["AutoPiston"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:AutoPiston", "AutoPiston", new EventHandler<ToggleEventArgs>(this.OnChangeEnabledAutoPiston)));
										this.panel["AutoPiston"].Enabled = false;
										this.panel["AutoPiston"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["HoleAutoSpeed"]);
										this.panel["AutoPiston"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["HoleAutoInsertStartPos"]);
										this.panel["AutoPiston"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["HoleAutoInsertEndPos"]);
										this.window.AddHorizontalSpacer();
										this.panel["AutoIKU"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:AutoIKU", "AutoIKU", new EventHandler<ToggleEventArgs>(this.OnChangeEnabledAutoIKU)));
										this.panel["AutoIKU"].Enabled = false;
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["CheckPistonSpeed"]);
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["SupportCreamPie"]);
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["ContinueInsert"]);
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["OrgasmThreshold"]);
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["OrgasmStartSecond"]);
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["VaginalFornixReachCount"]);
										this.panel["AutoIKU"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["AfterOrgasmDecrement"]);
										this.window.AddHorizontalSpacer();
										this.panel["AutoAHE"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:AutoAHE", "AutoAHE", new EventHandler<ToggleEventArgs>(this.OnChangeEnabledAutoAHE)));
										if (this.bOrgasmAvailable)
										{
											this.panel["AutoAHE"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["Convulsion"]);
										}
										this.panel["AutoAHE"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["AutoIncrement"]);
										this.panel["AutoAHE"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["EyeY"]);
										this.panel["AutoAHE"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["AHEEyeMax"]);
										this.window.AddHorizontalSpacer();
										this.panel["AutoTUN"] = new AddChikuwaSupporter.YotogiPanel("Panel:AutoTUN", "AutoTUN", new EventHandler<ToggleEventArgs>(this.OnChangeEnabledAutoTUN));
										if (this.bBokkiChikubiAvailable)
										{
											this.panel["AutoTUN"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(this.panel["AutoTUN"]);
											this.panel["AutoTUN"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["ChikubiScale"]);
											this.panel["AutoTUN"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["ChikubiNae"]);
											this.panel["AutoTUN"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["ChikubiBokki"]);
											this.panel["AutoTUN"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["ChikubiTare"]);
											this.window.AddHorizontalSpacer();
										}
										this.panel["AutoBOTE"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:AutoBOTE", "AutoBOTE", new EventHandler<ToggleEventArgs>(this.OnChangeEnabledAutoBOTE)));
										this.panel["AutoBOTE"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["SlowCreampie"]);
										this.panel["AutoBOTE"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Hara"]);
										this.window.AddHorizontalSpacer();
										this.panel["AutoKUPA"] = new AddChikuwaSupporter.YotogiPanel("Panel:AutoKUPA", "AutoKUPA", new EventHandler<ToggleEventArgs>(this.OnChangeEnabledAutoKUPA));
										if (this.bKupaAvailable || this.bAnalKupaAvailable)
										{
											this.panel["AutoKUPA"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(this.panel["AutoKUPA"]);
											if (this.bKupaAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Kupa"]);
											}
											if (this.bAnalKupaAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["AnalKupa"]);
											}
											if (this.bKupaAvailable || this.bAnalKupaAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["KupaLevel"]);
											}
											if (this.bLabiaKupaAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["LabiaKupa"]);
											}
											if (this.bVaginaKupaAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["VaginaKupa"]);
											}
											if (this.bNyodoKupaAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["NyodoKupa"]);
											}
											if (this.bSujiAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Suji"]);
											}
											if (this.bClitorisAvailable)
											{
												this.panel["AutoKUPA"].AddChild<AddChikuwaSupporter.YotogiSlider>(this.slider["Clitoris"]);
											}
											this.window.AddHorizontalSpacer();
										}
										this.panel["FaceAnime"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:FaceAnime", "FaceAnime", AddChikuwaSupporter.YotogiPanel.HeaderUI.Face));
										this.panel["FaceAnime"].AddChild<AddChikuwaSupporter.YotogiToggle>(this.toggle["Lipsync"]);
										this.panel["FaceAnime"].AddChild<AddChikuwaSupporter.YotogiButtonGrid>(this.grid["FaceAnime"]);
										this.window.AddHorizontalSpacer();
										this.panel["FaceBlend"] = this.window.AddChild<AddChikuwaSupporter.YotogiPanel>(new AddChikuwaSupporter.YotogiPanel("Panel:FaceBlend", "FaceBlend", AddChikuwaSupporter.YotogiPanel.HeaderUI.Face));
										this.panel["FaceBlend"].AddChild<AddChikuwaSupporter.YotogiButtonGrid>(this.grid["FaceBlend"]);
										base.ReloadConfig();
										this.ToggleWindowKey = this.parseExIni<KeyCode>("Keys", "ToggleWindow", this.ToggleWindowKey);
										this.slider["Sensitivity"].Value = this.parseExIni<float>("Status", "Sensitivity", this.fSensitivity);
										this.panel["AutoPiston"].Enabled = this.parseExIni<bool>("AutoPiston", "Enabled", this.panel["AutoPiston"].Enabled);
										this.updateAutoPiston(this.panel["AutoPiston"].Enabled);
										this.panel["AutoIKU"].Enabled = this.parseExIni<bool>("AutoIKU", "Enabled", this.panel["AutoIKU"].Enabled);
										this.toggle["CheckPistonSpeed"].Value = this.parseExIni<bool>("AutoIKU", "CheckPistonSpeedEnabled", this.toggle["CheckPistonSpeed"].Value);
										this.toggle["SupportCreamPie"].Value = this.parseExIni<bool>("AutoIKU", "SupportCreamPieEnabled", this.toggle["SupportCreamPie"].Value);
										this.toggle["ContinueInsert"].Value = this.parseExIni<bool>("AutoIKU", "ContinueInsertEnabled", this.toggle["ContinueInsert"].Value);
										this.slider["OrgasmThreshold"].Value = this.parseExIni<float>("AutoIKU", "OrgasmThreshold", this.fOrgasmThreshold);
										this.slider["OrgasmStartSecond"].Value = this.parseExIni<float>("AutoIKU", "OrgasmStartSecond", this.fOrgasmStartSecond);
										this.slider["VaginalFornixReachCount"].Value = this.parseExIni<float>("AutoIKU", "VaginalFornixReachCount", this.fVaginalFornixReachCount);
										this.slider["AfterOrgasmDecrement"].Value = this.parseExIni<float>("AutoIKU", "AfterOrgasmDecrement", this.fAfterOrgasmDecrement);
										this.panel["AutoAHE"].Enabled = this.parseExIni<bool>("AutoAHE", "Enabled", this.panel["AutoAHE"].Enabled);
										this.toggle["Convulsion"].Value = this.parseExIni<bool>("AutoAHE", "ConvulsionEnabled", this.toggle["Convulsion"].Value);
										this.toggle["AutoIncrement"].Value = this.parseExIni<bool>("AutoAHE", "AutoIncrementEnabled", this.toggle["AutoIncrement"].Value);
										this.slider["AHEEyeMax"].Value = this.parseExIni<float>("AutoAHE", "AHEEyeMax", this.fAheEyeMax);
										this.fOrgasmsPerAheLevel = this.parseExIni<float>("AutoAHE", "OrgasmsPerLevel", this.fOrgasmsPerAheLevel);
										this.fAheEyeDecrement = this.parseExIni<float>("AutoAHE", "EyeDecrement", this.fAheEyeDecrement);
										for (int i = 0; i < 3; i++)
										{
											this.iAheExcite[i] = this.parseExIni<int>("AutoAHE", "ExciteThreshold_" + i, this.iAheExcite[i]);
											this.fAheNormalEyeMax[i] = this.parseExIni<float>("AutoAHE", "NormalEyeMax_" + i, this.fAheNormalEyeMax[i]);
											this.fAheOrgasmEyeMax[i] = this.parseExIni<float>("AutoAHE", "OrgasmEyeMax_" + i, this.fAheOrgasmEyeMax[i]);
											this.fAheOrgasmEyeMin[i] = this.parseExIni<float>("AutoAHE", "OrgasmEyeMin_" + i, this.fAheOrgasmEyeMin[i]);
											this.fAheOrgasmSpeed[i] = this.parseExIni<float>("AutoAHE", "OrgasmMotionSpeed_" + i, this.fAheOrgasmSpeed[i]);
											this.fAheOrgasmConvulsion[i] = this.parseExIni<float>("AutoAHE", "OrgasmConvulsion_" + i, this.fAheOrgasmConvulsion[i]);
											this.sAheOrgasmFace[i] = this.parseExIni<string>("AutoAHE", "OrgasmFace_" + i, this.sAheOrgasmFace[i]);
											this.sAheOrgasmFaceBlend[i] = this.parseExIni<string>("AutoAHE", "OrgasmFaceBlend_" + i, this.sAheOrgasmFaceBlend[i]);
										}
										this.panel["AutoTUN"].Enabled = this.parseExIni<bool>("AutoTUN", "Enabled", this.panel["AutoTUN"].Enabled);
										this.slider["ChikubiScale"].Value = this.parseExIni<float>("AutoTUN", "ChikubiScale", this.fChikubiScale);
										this.slider["ChikubiNae"].Value = this.parseExIni<float>("AutoTUN", "ChikubiNae", this.fChikubiNae);
										this.slider["ChikubiBokki"].Value = this.slider["ChikubiNae"].Value;
										this.slider["ChikubiTare"].Value = this.parseExIni<float>("AutoTUN", "ChikubiTare", this.fChikubiTare);
										this.iDefChikubiNae = this.slider["ChikubiNae"].Value;
										this.iDefChikubiTare = this.slider["ChikubiTare"].Value;
										this.panel["AutoBOTE"].Enabled = this.parseExIni<bool>("AutoBOTE", "Enabled", this.panel["AutoBOTE"].Enabled);
										this.toggle["SlowCreampie"].Value = this.parseExIni<bool>("AutoBOTE", "SlowCreampie", this.toggle["SlowCreampie"].Value);
										this.iHaraIncrement = this.parseExIni<int>("AutoBOTE", "Increment", this.iHaraIncrement);
										this.iBoteHaraMax = this.parseExIni<int>("AutoBOTE", "Max", this.iBoteHaraMax);
										this.panel["AutoKUPA"].Enabled = this.parseExIni<bool>("AutoKUPA", "Enabled", this.panel["AutoKUPA"].Enabled);
										this.slider["KupaLevel"].Value = this.parseExIni<float>("AutoKUPA", "KupaLevel", this.fKupaLevel);
										this.slider["LabiaKupa"].Value = this.parseExIni<float>("AutoKUPA", "LabiaKupa", this.fLabiaKupa);
										this.slider["VaginaKupa"].Value = this.parseExIni<float>("AutoKUPA", "VaginaKupa", this.fVaginaKupa);
										this.slider["NyodoKupa"].Value = this.parseExIni<float>("AutoKUPA", "NyodoKupa", this.fNyodoKupa);
										this.slider["Suji"].Value = this.parseExIni<float>("AutoKUPA", "Suji", this.fSuji);
										this.iKupaStart = this.parseExIni<int>("AutoKUPA", "Start", this.iKupaStart);
										this.iKupaIncrementPerOrgasm = this.parseExIni<int>("AutoKUPA", "IncrementPerOrgasm", this.iKupaIncrementPerOrgasm);
										this.iKupaNormalMax = this.parseExIni<int>("AutoKUPA", "NormalMax", this.iKupaNormalMax);
										this.iKupaWaitingValue = this.parseExIni<int>("AutoKUPA", "WaitingValue", this.iKupaWaitingValue);
										for (int i = 0; i < 2; i++)
										{
											this.iKupaValue[i] = this.parseExIni<int>("AutoKUPA", "Value_" + i, this.iKupaValue[i]);
										}
										this.iAnalKupaStart = this.parseExIni<int>("AutoKUPA_Anal", "Start", this.iAnalKupaStart);
										this.iAnalKupaIncrementPerOrgasm = this.parseExIni<int>("AutoKUPA_Anal", "IncrementPerOrgasm", this.iAnalKupaIncrementPerOrgasm);
										this.iAnalKupaNormalMax = this.parseExIni<int>("AutoKUPA_Anal", "NormalMax", this.iAnalKupaNormalMax);
										this.iAnalKupaWaitingValue = this.parseExIni<int>("AutoKUPA_Anal", "WaitingValue", this.iAnalKupaWaitingValue);
										for (int i = 0; i < 2; i++)
										{
											this.iAnalKupaValue[i] = this.parseExIni<int>("AutoKUPA_Anal", "Value_" + i, this.iAnalKupaValue[i]);
										}
										result = true;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06000040 RID: 64 RVA: 0x0000640C File Offset: 0x0000540C
		private Yotogi.SkillData getCurrentSkillData()
		{
			Yotogi.SkillData result;
			try
			{
				result = AddChikuwaSupporter.getFieldValue<YotogiPlayManagerWithChubLip, Yotogi.SkillDataPair>(this.yotogiPlayManagerWithChubLip, "skill_pair_").base_data;
			}
			catch (Exception)
			{
				result = null;
			}
			return result;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00006450 File Offset: 0x00005450
		private void initOnStartSkill()
		{
			this.bLoadBoneAnimetion = false;
			this.bSyncMotionSpeed = true;
			this.bKupaFuck = false;
			this.bAnalKupaFuck = false;
			this.iBoteCount = 0;
			this.iKupaDef = 0;
			this.iAnalKupaDef = 0;
			if (this.panel["AutoTUN"].Enabled)
			{
				if (this.slider["ChikubiBokki"].Value > 0f)
				{
					this.updateShapeKeyChikubiBokkiValue(this.slider["ChikubiBokki"].Value / 2f);
				}
			}
			if (this.panel["AutoBOTE"].Enabled)
			{
				this.updateMaidHaraValue((float)Mathf.Max(this.iCurrentHara, this.iDefHara));
			}
			else
			{
				this.updateMaidHaraValue((float)this.iDefHara);
			}
			Yotogi.SkillData currentSkillData = this.getCurrentSkillData();
			if (currentSkillData != null)
			{
				AddChikuwaSupporter.KupaLevel kupaLevel = this.checkSkillKupaLevel(currentSkillData);
				if (kupaLevel != AddChikuwaSupporter.KupaLevel.None)
				{
					this.iKupaDef = (int)((float)this.iKupaValue[(int)kupaLevel] * this.slider["KupaLevel"].Value / 100f);
				}
				kupaLevel = this.checkSkillAnalKupaLevel(currentSkillData);
				if (kupaLevel != AddChikuwaSupporter.KupaLevel.None)
				{
					this.iAnalKupaDef = (int)((float)this.iAnalKupaValue[(int)kupaLevel] * this.slider["KupaLevel"].Value / 100f);
				}
			}
			if (this.panel["AutoKUPA"].Enabled)
			{
				if (this.bKupaAvailable)
				{
					this.updateShapeKeyKupaValue((float)this.iKupaMin);
				}
				if (this.bAnalKupaAvailable)
				{
					this.updateShapeKeyAnalKupaValue((float)this.iAnalKupaMin);
				}
				if (this.bClitorisAvailable)
				{
					this.updateShapeKeyClitorisValue((float)this.iClitorisMin);
				}
				if (this.isDeviceIn)
				{
					this.animateAutoKupa();
				}
			}
			else
			{
				if (this.bKupaAvailable)
				{
					this.updateShapeKeyKupaValue(0f);
				}
				if (this.bAnalKupaAvailable)
				{
					this.updateShapeKeyAnalKupaValue(0f);
				}
				if (this.bClitorisAvailable)
				{
					this.updateShapeKeyClitorisValue(0f);
				}
			}
			if (this.bOrgasmAvailable)
			{
				this.updateShapeKeyOrgasmValue(0f);
			}
			foreach (KeyValuePair<string, AddChikuwaSupporter.PlayAnime> keyValuePair in this.pa)
			{
				if (keyValuePair.Value.NowPlaying)
				{
					keyValuePair.Value.Stop();
				}
			}
			base.StartCoroutine(this.getBoneAnimetionCoroutine(this.WaitBoneLoad));
			this.bSyncMotionSpeed = true;
			base.StartCoroutine(this.syncMotionSpeedSliderCoroutine(this.TimePerUpdateSpeed));
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00006744 File Offset: 0x00005744
		private void finalize()
		{
			try
			{
				this.visible = false;
				this.window = null;
				this.panel.Clear();
				this.slider.Clear();
				this.grid.Clear();
				this.toggle.Clear();
				this.lSelect.Clear();
				this.bInitCompleted = false;
				this.bSyncMotionSpeed = false;
				this.fPassedTimeOnCommand = -1f;
				this.bFadeInWait = false;
				this.iLastExcite = 0;
				this.iOrgasmCount = 0;
				this.iLastSliderFrustration = 0;
				this.fLastSliderSensitivity = 0f;
				this.iDefHara = 0;
				this.iCurrentHara = 0;
				this.iBoteCount = 0;
				this.bKupaFuck = false;
				this.bAnalKupaFuck = false;
				this.goCommandUnit = null;
				this.maid = null;
				this.maidStatusInfo = null;
				this.maidFoceKuchipakuSelfUpdateTime = null;
				this.yotogiParamBasicBar = null;
				this.yotogiParamBasicBarWithChubLip = null;
				this.yotogiPlayManagerWithChubLip = null;
				this.orgOnClickCommand = null;
				if (this.kagScriptCallbacksOverride)
				{
					this.kagScript.RemoveTagCallBack("face");
					this.kagScript.AddTagCallBack("face", new KagScript.KagTagCallBack(this.orgTagFace.Invoke));
					this.kagScript.RemoveTagCallBack("faceblend");
					this.kagScript.AddTagCallBack("faceblend", new KagScript.KagTagCallBack(this.orgTagFaceBlend.Invoke));
					this.kagScriptCallbacksOverride = false;
					this.kagScript = null;
					this.orgTagFace = null;
					this.orgTagFaceBlend = null;
				}
			}
			catch (Exception ex)
			{
				AddChikuwaSupporter.LogError("finalize() : {0}", new object[]
				{
					ex
				});
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00006908 File Offset: 0x00005908
		private void detectSkillCbl()
		{
			YotogiCBL.SkillData selectSkillData = YotogiPlayManagerWithChubLip.GetSelectSkillData();
			if (selectSkillData != null)
			{
				string skill_name = selectSkillData.skill_name;
				if (this.currentYotogiName == null || !skill_name.Equals(this.currentYotogiName))
				{
					Debug.Log(" AddChikuwaSupporter_feat_YotogiSlider : Yotogi changed: " + this.currentYotogiName + " >> " + skill_name);
					this.currentYotogiName = skill_name;
					this.initOnStartSkill();
					this.initAnimeOnCommand();
					this.animateAutoKupa();
				}
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00006984 File Offset: 0x00005984
		private bool VertexMorph_FromProcItem(TBody body, string sTag, float f)
		{
			bool result = false;
			for (int i = 0; i < body.goSlot.Count; i++)
			{
				TMorph morph = body.goSlot[i].morph;
				if (morph != null)
				{
					if (morph.Contains(sTag))
					{
						if (i == 1)
						{
							result = true;
						}
						int num = (int)body.goSlot[i].morph.hash[sTag];
						body.goSlot[i].morph.BlendValues[num] = f;
						body.goSlot[i].morph.FixBlendValues();
					}
				}
			}
			return result;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00006A54 File Offset: 0x00005A54
		private bool isExistVertexMorph(TBody body, string sTag)
		{
			for (int i = 0; i < body.goSlot.Count; i++)
			{
				TMorph morph = body.goSlot[i].morph;
				if (morph != null)
				{
					if (morph.Contains(sTag))
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00006AB4 File Offset: 0x00005AB4
		private void checkAutoIKUOnUpdate()
		{
			if (this.panel["AutoIKU"].Enabled)
			{
				if (this.toggle["ContinueInsert"].Value)
				{
					DateTime now = DateTime.Now;
					if ((now - this.lastStartCreamPieContinueTime).TotalSeconds > 0.5 && (this.chuBlipManager.GetPistonSpeedStage() == 1 || this.chuBlipManager.GetPistonSpeedStage() == null) && this.slider["InsertDepth"].Value <= 20f)
					{
						this.fHoleAutoSpeedBeforeEcstasy = this.slider["HoleAutoSpeed"].Value;
						this.fHoleAutoInsertStartPosBeforeEcstasy = this.slider["HoleAutoInsertStartPos"].Value;
						this.fHoleAutoInsertEndPosBeforeEcstasy = this.slider["HoleAutoInsertEndPos"].Value;
						this.updateHoleAutoSpeed(1);
						this.updateHoleAutoInsertStartPos(50);
						this.updateHoleAutoInsertEndPos(50);
						this.updateAutoPiston(true);
						this.slider["HoleAutoSpeed"].Value = 1f;
						this.slider["HoleAutoInsertStartPos"].Value = 50f;
						this.slider["HoleAutoInsertEndPos"].Value = 50f;
						this.panel["AutoPiston"].Enabled = true;
						this.boEnableSupportCreamPieContinue = true;
						this.lastStartCreamPieContinueTime = DateTime.Now;
					}
					else if (this.boEnableSupportCreamPieContinue && (this.slider["InsertDepth"].Value > 20f || !this.panel["AutoPiston"].Enabled))
					{
						if ((now - this.lastStartCreamPieContinueTime).TotalSeconds > 0.5)
						{
							this.updateAutoPiston(false);
							this.updateHoleAutoSpeed((int)this.fHoleAutoSpeedBeforeEcstasy);
							this.updateHoleAutoInsertStartPos((int)this.fHoleAutoInsertStartPosBeforeEcstasy);
							this.updateHoleAutoInsertEndPos((int)this.fHoleAutoInsertEndPosBeforeEcstasy);
							this.panel["AutoPiston"].Enabled = false;
							this.slider["HoleAutoSpeed"].Value = this.fHoleAutoSpeedBeforeEcstasy;
							this.slider["HoleAutoInsertStartPos"].Value = this.fHoleAutoInsertStartPosBeforeEcstasy;
							this.slider["HoleAutoInsertEndPos"].Value = this.fHoleAutoInsertEndPosBeforeEcstasy;
							this.fHoleAutoSpeedBeforeEcstasy = 0f;
							this.fHoleAutoInsertStartPosBeforeEcstasy = 0f;
							this.fHoleAutoInsertEndPosBeforeEcstasy = 0f;
							this.boEnableSupportCreamPieContinue = false;
							this.lastStartCreamPieContinueTime = DateTime.Now;
						}
					}
				}
				this.isReadyToEcstasy = (this.slider["Excite"].Value > this.slider["OrgasmThreshold"].Value);
				if (!this.isKeepFast && this.isReadyToEcstasy)
				{
					this.startEcstasy();
				}
				else if (this.isKeepFast)
				{
					DateTime now = DateTime.Now;
					TimeSpan timeSpan = now - this.lastReadyToEcstasyTime;
					if ((this.toggle["SupportCreamPie"].Value && this.slider["VaginalFornixReachCount"].Value == 0f && timeSpan.TotalMilliseconds > (double)((this.slider["OrgasmStartSecond"].Value - 0.5f) * 1000f)) || (this.slider["VaginalFornixReachCount"].Value > 0f && this.slider["VaginalFornixReachCount"].Value - 1f <= (float)this.iVaginalFornixReaches))
					{
						this.overflowHoleSensitivity();
					}
					if ((this.slider["VaginalFornixReachCount"].Value == 0f && timeSpan.TotalMilliseconds > (double)(this.slider["OrgasmStartSecond"].Value * 1000f)) || (this.slider["VaginalFornixReachCount"].Value > 0f && this.slider["VaginalFornixReachCount"].Value <= (float)this.iVaginalFornixReaches))
					{
						this.onaholeMotion.SendMessage("Finish");
						this.isOnFinish = true;
						base.StartCoroutine(this.OnClickFinishButtonCoroutine(0.1f));
						this.updateMaidExcite((int)(this.slider["OrgasmThreshold"].Value * (100f - this.slider["AfterOrgasmDecrement"].Value) / 100f));
						this.isReadyToEcstasy = false;
						this.endEcstasy();
					}
					else if (this.isOnFinish)
					{
						this.updateMaidExcite((int)(this.slider["OrgasmThreshold"].Value * (100f - this.slider["AfterOrgasmDecrement"].Value) / 100f));
						this.isReadyToEcstasy = false;
						this.endEcstasy();
					}
					else if (this.slider["VaginalFornixReachCount"].Value > 0f && this.iVaginalFornixReaches > 0 && timeSpan.TotalMilliseconds > (double)((this.slider["OrgasmStartSecond"].Value - 0.6f) * 1000f))
					{
						this.resetVaginalFornixReaches();
						this.restoreHoleSensitivity();
						this.resetTimer();
					}
					else if (this.slider["VaginalFornixReachCount"].Value > 0f && this.slider["InsertDepth"].Value < 950f && this.slider["VaginalFornixReachCount"].Value > (float)this.iVaginalFornixReaches)
					{
						this.awayFromVaginalFornix();
					}
					else if (this.slider["VaginalFornixReachCount"].Value > 0f && this.slider["InsertDepth"].Value >= 950f && this.boAwayFromVaginalFornix && this.slider["VaginalFornixReachCount"].Value > (float)this.iVaginalFornixReaches)
					{
						this.kickVaginalFornix();
					}
					else if (this.chuBlipManager.GetPistonSpeedStage() != 3 && this.slider["InsertDepth"].Value < 950f)
					{
						this.endEcstasy();
					}
				}
			}
		}

		// Token: 0x06000047 RID: 71 RVA: 0x000071E0 File Offset: 0x000061E0
		private void startEcstasy()
		{
			this.resetTimer();
			this.isKeepFast = true;
			this.saveHoleSensitivity();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x000071F8 File Offset: 0x000061F8
		private void resetTimer()
		{
			this.lastReadyToEcstasyTime = DateTime.Now;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00007206 File Offset: 0x00006206
		private void resetVaginalFornixReaches()
		{
			this.iVaginalFornixReaches = 0;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00007210 File Offset: 0x00006210
		private void saveHoleSensitivity()
		{
			if (this.toggle["SupportCreamPie"].Value && this.fCurrentHoleSensitivity != 1000f)
			{
				this.fCurrentHoleSensitivityBeforeEcstasy = this.slider["HoleSensitivity"].Value;
			}
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00007268 File Offset: 0x00006268
		private void overflowHoleSensitivity()
		{
			if (this.toggle["SupportCreamPie"].Value)
			{
				this.fCurrentHoleSensitivity = 1000f;
				this.updateHoleSensitivity((int)this.fCurrentHoleSensitivity);
				this.slider["HoleSensitivity"].Value = this.fCurrentHoleSensitivity;
			}
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000072CC File Offset: 0x000062CC
		private void restoreHoleSensitivity()
		{
			if (this.toggle["SupportCreamPie"].Value)
			{
				this.fCurrentHoleSensitivity = this.fCurrentHoleSensitivityBeforeEcstasy;
				this.updateHoleSensitivity((int)this.fCurrentHoleSensitivity);
				this.slider["HoleSensitivity"].Value = this.fCurrentHoleSensitivity;
			}
		}

		// Token: 0x0600004D RID: 77 RVA: 0x0000732E File Offset: 0x0000632E
		private void awayFromVaginalFornix()
		{
			this.boAwayFromVaginalFornix = true;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00007338 File Offset: 0x00006338
		private void kickVaginalFornix()
		{
			this.resetTimer();
			this.boAwayFromVaginalFornix = false;
			this.iVaginalFornixReaches++;
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00007357 File Offset: 0x00006357
		private void endEcstasy()
		{
			this.isKeepFast = false;
			this.restoreHoleSensitivity();
			this.resetVaginalFornixReaches();
			this.boAwayFromVaginalFornix = false;
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00007378 File Offset: 0x00006378
		private void incrementValuesOnUpdate()
		{
			string text = this.chuBlipManager.GetSexMode().ToString();
			YotogiCBL.SkillData selectSkillData = YotogiPlayManagerWithChubLip.GetSelectSkillData();
			int num = (int)this.checkCommandTunLevelCBL(selectSkillData);
			OnaholeFeelings.PISTON_SPEED pistonSpeedStage = this.chuBlipManager.GetPistonSpeedStage();
			if (!this.panel["AutoIKU"].Enabled || !this.toggle["CheckPistonSpeed"].Value || !text.Contains(0.ToString()) || pistonSpeedStage != null || num >= 0)
			{
				float num2 = 1f;
				if (!text.Contains(0.ToString()) && !text.Contains(2.ToString()))
				{
					num2 = 2.5f;
				}
				else if (this.panel["AutoIKU"].Enabled && num >= 0)
				{
					num2 = (float)this.iTunValue[num] / 2f;
				}
				else if (this.panel["AutoIKU"].Enabled && this.toggle["CheckPistonSpeed"].Value)
				{
					switch (pistonSpeedStage)
					{
					case 1:
						num2 = 1f;
						break;
					case 2:
						num2 = 1.7f;
						break;
					case 3:
						num2 = 2.5f;
						break;
					}
				}
				if (this.panel["Status"].Enabled && this.slider["Excite"].Pin)
				{
					this.updateMaidExcite((int)this.slider["Excite"].Value);
				}
				else
				{
					this.slider["Excite"].Value += this.slider["Sensitivity"].Value * num2 * 0.0005f;
				}
				if (this.panel["AutoAHE"].Enabled)
				{
					if (this.toggle["AutoIncrement"].Value && this.isDeviceIn && this.chuBlipManager.GetPistonSpeedStage() != 0)
					{
						if (this.slider["EyeY"].Value >= this.slider["AHEEyeMax"].Value)
						{
							this.slider["EyeY"].Value = this.slider["AHEEyeMax"].Value;
							this.updateMaidEyePosY(this.slider["AHEEyeMax"].Value);
						}
						else
						{
							this.slider["EyeY"].Value += this.slider["Sensitivity"].Value * num2 * 0.0002f;
							this.updateMaidEyePosY(this.slider["EyeY"].Value);
						}
					}
				}
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000076C8 File Offset: 0x000066C8
		private void overrideFaceBlendOnUpdate()
		{
			if (this.panel["AutoIKU"].Enabled && !this.isOnFinish)
			{
				if (this.panel["FaceAnime"].Enabled && this.panel["FaceAnime"].HeaderUILabelText != null)
				{
					this.maid.FaceAnime(this.panel["FaceAnime"].HeaderUILabelText, 1f, 0);
				}
				if (this.slider["Excite"].Value < this.slider["OrgasmThreshold"].Value * 0.4f)
				{
					this.maid.FaceBlend("頬０涙０");
				}
				else if (this.slider["Excite"].Value < this.slider["OrgasmThreshold"].Value * 0.7f)
				{
					this.maid.FaceBlend("頬１涙０");
				}
				else if (this.slider["Excite"].Value < this.slider["OrgasmThreshold"].Value * 1f)
				{
					this.maid.FaceBlend("頬２涙０");
				}
				else if (this.slider["VaginalFornixReachCount"].Value > 0f && this.slider["VaginalFornixReachCount"].Value - 1f <= (float)this.iVaginalFornixReaches)
				{
					this.maid.FaceBlend("頬３涙１よだれ");
				}
				else if (this.slider["VaginalFornixReachCount"].Value > 0f && this.slider["VaginalFornixReachCount"].Value - 2f <= (float)this.iVaginalFornixReaches)
				{
					this.maid.FaceBlend("頬３涙１");
				}
				else
				{
					this.maid.FaceBlend("頬３涙０");
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000791C File Offset: 0x0000691C
		private void animateAutoTun()
		{
			if (this.panel["AutoTUN"].Enabled)
			{
				YotogiCBL.SkillData selectSkillData = YotogiPlayManagerWithChubLip.GetSelectSkillData();
				float num = Mathf.Max(this.iDefChikubiNae, this.slider["ChikubiBokki"].Value);
				int num2 = (int)this.checkCommandTunLevelCBL(selectSkillData);
				if (num2 >= 0)
				{
					OnaholeFeelings.PISTON_SPEED pistonSpeedStage = this.chuBlipManager.GetPistonSpeedStage();
					if (num2 != 0 || pistonSpeedStage != 0)
					{
						float num3 = num + (float)this.iTunValue[num2] / 100f * this.slider["ChikubiScale"].Value / 100f;
						if (num3 >= 100f * this.slider["ChikubiScale"].Value / 100f)
						{
							this.slider["ChikubiBokki"].Value = 100f * this.slider["ChikubiScale"].Value / 100f;
						}
						else
						{
							this.slider["ChikubiBokki"].Value = num3;
						}
					}
				}
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00007A60 File Offset: 0x00006A60
		private void animateAutoKupa()
		{
			if (this.panel["AutoKUPA"].Enabled)
			{
				YotogiCBL.SkillData selectSkillData = YotogiPlayManagerWithChubLip.GetSelectSkillData();
				float value = this.slider["Kupa"].Value;
				int num = (int)this.checkCommandKupaLevelCBL(selectSkillData);
				if (num >= 0)
				{
					float num2 = (float)this.iKupaValue[num] * this.slider["KupaLevel"].Value / 100f;
					if (value < (float)((int)num2) && num2 - value > 1f)
					{
						this.pa["KUPA.挿入." + num].Play(value, (float)((int)num2));
					}
					this.bKupaFuck = true;
				}
				else if (this.bKupaFuck && this.checkCommandKupaStopCBL(selectSkillData))
				{
					this.pa["KUPA.止める"].Play(value, (float)this.iKupaMin);
					this.bKupaFuck = false;
				}
				value = this.slider["AnalKupa"].Value;
				num = (int)this.checkCommandAnalKupaLevelCBL(selectSkillData);
				if (num >= 0)
				{
					float num2 = (float)this.iAnalKupaValue[num] * this.slider["KupaLevel"].Value / 100f;
					if (value < (float)((int)num2) && num2 - value > 1f)
					{
						this.pa["AKPA.挿入." + num].Play(value, (float)((int)num2));
					}
					this.bAnalKupaFuck = true;
				}
				else if (this.bAnalKupaFuck && this.checkCommandAnalKupaStopCBL(selectSkillData))
				{
					this.pa["AKPA.止める"].Play(value, (float)this.iAnalKupaMin);
					this.bAnalKupaFuck = false;
				}
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00007C50 File Offset: 0x00006C50
		private void syncSlidersOnClickCommand(Yotogi.SkillData.Command.Data.Status cmStatus)
		{
			if (this.panel["Status"].Enabled && this.slider["Excite"].Pin)
			{
				this.updateMaidExcite((int)this.slider["Excite"].Value);
			}
			else
			{
				this.slider["Excite"].Value = (float)this.maid.Param.status.cur_excite;
			}
			if (this.panel["Status"].Enabled && this.slider["Mind"].Pin)
			{
				this.updateMaidMind((int)this.slider["Mind"].Value);
			}
			else
			{
				this.slider["Mind"].Value = (float)this.maid.Param.status.cur_mind;
			}
			if (this.panel["Status"].Enabled && this.slider["PistonDepthShallow"].Pin)
			{
				this.updatePistonDepthShallow((int)this.slider["PistonDepthShallow"].Value);
			}
			else
			{
				this.slider["PistonDepthShallow"].Value = this.chuBlipManager.GetInsertDepth() * 1000f;
			}
			if (this.panel["Status"].Enabled && this.slider["PistonDepthDeep"].Pin)
			{
				this.updatePistonDepthDeep((int)this.slider["PistonDepthDeep"].Value);
			}
			else
			{
				this.slider["PistonDepthDeep"].Value = this.chuBlipManager.GetInsertDepth() * 1000f;
			}
			if (this.panel["Status"].Enabled && this.slider["InsertDepth"].Pin)
			{
				this.updateInsertDepth((int)this.slider["InsertDepth"].Value);
			}
			else
			{
				this.slider["InsertDepth"].Value = this.chuBlipManager.GetInsertDepth() * 1000f;
			}
			if (this.panel["Status"].Enabled && this.slider["Reason"].Pin)
			{
				this.updateMaidReason((int)this.slider["Reason"].Value);
			}
			else
			{
				this.slider["Reason"].Value = (float)this.maid.Param.status.cur_reason;
			}
			if (this.panel["Status"].Enabled && this.slider["HoleSensitivity"].Pin)
			{
				this.updateHoleSensitivity((int)this.slider["HoleSensitivity"].Value);
			}
			else
			{
				this.slider["HoleSensitivity"].Value = GameMain.Instance.CMSystem.HoleSensitivity * 1000f;
			}
			if (this.panel["Status"].Enabled && this.slider["HoleSync"].Pin)
			{
				this.updateHoleAutoSpeed((int)this.slider["HoleSync"].Value);
			}
			else
			{
				this.slider["HoleSync"].Value = GameMain.Instance.CMSystem.HoleSync * 1000f;
			}
			if (this.panel["Status"].Enabled && this.slider["HoleAutoSpeed"].Pin)
			{
				this.updateHoleAutoSpeed((int)this.slider["HoleAutoSpeed"].Value);
			}
			else
			{
				this.slider["HoleAutoSpeed"].Value = (float)GameMain.Instance.CMSystem.HoleAutoSpeed;
			}
			if (this.panel["Status"].Enabled && this.slider["HoleAutoInsertStartPos"].Pin)
			{
				this.updateHoleAutoInsertStartPos((int)this.slider["HoleAutoInsertStartPos"].Value);
			}
			else
			{
				this.slider["HoleAutoInsertStartPos"].Value = (float)GameMain.Instance.CMSystem.HoleAutoInsertStartPos;
			}
			if (this.panel["Status"].Enabled && this.slider["HoleAutoInsertEndPos"].Pin)
			{
				this.updateHoleAutoInsertEndPos((int)this.slider["HoleAutoInsertEndPos"].Value);
			}
			else
			{
				this.slider["HoleAutoInsertEndPos"].Value = (float)GameMain.Instance.CMSystem.HoleAutoInsertEndPos;
			}
			this.slider["Sensitivity"].Value = (float)(this.maid.Param.status.correction_data.excite + (this.panel["Status"].Enabled ? (this.iLastSliderFrustration + cmStatus.frustration) : this.maid.Param.status.frustration) + ((this.maid.Param.status.cur_reason < 20) ? 20 : 0));
			if (this.panel["Status"].Enabled && this.slider["MotionSpeed"].Pin)
			{
				this.updateMotionSpeed(this.slider["MotionSpeed"].Value);
			}
			else
			{
				foreach (object obj in this.anm_BO_body001)
				{
					AnimationState animationState = (AnimationState)obj;
					if (animationState.enabled)
					{
						this.slider["MotionSpeed"].Value = animationState.speed * 100f;
					}
				}
			}
			this.slider["EyeY"].Value = this.maid.body0.trsEyeL.localPosition.y * this.fEyePosToSliderMul;
			this.slider["Hara"].Value = (float)this.maid.GetProp("Hara").value;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000845C File Offset: 0x0000745C
		private IEnumerator syncMotionSpeedSliderCoroutine(float waitTime)
		{
			while (this.bSyncMotionSpeed)
			{
				if (this.bLoadBoneAnimetion)
				{
				}
				yield return new WaitForSeconds(waitTime);
			}
			yield break;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00008484 File Offset: 0x00007484
		private void initAnimeOnCommand()
		{
			if (this.panel["AutoAHE"].Enabled)
			{
				this.fAheLastEye = this.maid.body0.trsEyeL.localPosition.y * this.fEyePosToSliderMul;
				for (int i = 0; i < 1; i++)
				{
					if (this.pa["AHE.絶頂." + i].NowPlaying)
					{
						this.pa["AHE.絶頂." + i].Stop();
					}
					if (this.pa["AHE.継続." + i].NowPlaying)
					{
						this.pa["AHE.継続." + i].Stop();
					}
				}
				for (int i = 0; i < 2; i++)
				{
					if (this.pa["KUPA.挿入." + i].NowPlaying)
					{
						this.updateShapeKeyKupaValue((float)((int)((float)this.iKupaValue[i] * this.slider["KupaLevel"].Value / 100f)));
					}
					this.pa["KUPA.挿入." + i].Stop();
				}
				for (int i = 0; i < 2; i++)
				{
					if (this.pa["AKPA.挿入." + i].NowPlaying)
					{
						this.updateShapeKeyAnalKupaValue((float)((int)((float)this.iAnalKupaValue[i] * this.slider["KupaLevel"].Value / 100f)));
					}
					this.pa["AKPA.挿入." + i].Stop();
				}
			}
			if (this.panel["AutoBOTE"].Enabled)
			{
				if (this.pa["BOTE.絶頂"].NowPlaying)
				{
					this.updateMaidHaraValue((float)Mathf.Min(this.iCurrentHara + this.iHaraIncrement, this.iBoteHaraMax));
				}
				if (this.pa["BOTE.止める"].NowPlaying)
				{
					this.updateMaidHaraValue((float)this.iDefHara);
				}
				this.pa["BOTE.絶頂"].Stop();
				this.pa["BOTE.止める"].Stop();
			}
			if (this.panel["AutoKUPA"].Enabled)
			{
				if (this.pa["KUPA.止める"].NowPlaying)
				{
					this.updateShapeKeyKupaValue((float)this.iKupaMin);
				}
				this.pa["KUPA.止める"].Stop();
				if (this.pa["AKPA.止める"].NowPlaying)
				{
					this.updateShapeKeyAnalKupaValue((float)this.iAnalKupaMin);
				}
				this.pa["AKPA.止める"].Stop();
			}
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000087F0 File Offset: 0x000077F0
		private void playAnimeOnCommand(Yotogi.SkillData.Command.Data.Basic data)
		{
			if (this.panel["AutoAHE"].Enabled)
			{
				float num = (float)this.maid.Param.status.cur_excite;
				int num2 = this.idxAheOrgasm;
				if (data.command_type == 4)
				{
					if (this.iLastExcite >= this.iAheExcite[num2])
					{
						this.pa["AHE.継続.0"].Play(this.fAheLastEye, this.fAheOrgasmEyeMax[num2]);
						float[] vform = new float[]
						{
							this.fAheOrgasmEyeMax[num2],
							this.fAheOrgasmSpeed[num2]
						};
						float[] vto = new float[]
						{
							this.fAheOrgasmEyeMin[num2],
							100f
						};
						this.updateMotionSpeed(this.fAheOrgasmSpeed[num2]);
						this.pa["AHE.絶頂.0"].Play(vform, vto);
						if (this.toggle["Convulsion"].Value)
						{
							if (this.pa["AHE.痙攣." + num2].NowPlaying)
							{
								this.iAheOrgasmChain++;
							}
							this.pa["AHE.痙攣." + num2].Play(0f, this.fAheOrgasmConvulsion[num2]);
						}
						this.iOrgasmCount++;
					}
				}
				else if (num >= (float)this.iAheExcite[num2])
				{
					float vto2 = this.fAheNormalEyeMax[num2] * (num - (float)this.iAheExcite[num2]) / (300f - (float)this.iAheExcite[num2]);
					this.pa["AHE.継続.0"].Play(this.fAheLastEye, vto2);
				}
			}
			if (this.panel["AutoBOTE"].Enabled)
			{
				float num3 = (float)this.maid.GetProp("Hara").value;
				if (data.command_type == 4)
				{
					if (data.name.Contains("中出し") || data.name.Contains("注ぎ込む"))
					{
						this.iBoteCount++;
						float vto2 = (float)Mathf.Min(this.iCurrentHara + this.iHaraIncrement, this.iBoteHaraMax);
						this.pa["BOTE.絶頂"].Play(num3, vto2);
					}
					else if (data.name.Contains("外出し"))
					{
						this.pa["BOTE.止める"].Play(num3, (float)this.iDefHara);
						this.iBoteCount = 0;
					}
				}
				else if (data.command_type == 5)
				{
					this.pa["BOTE.止める"].Play(num3, (float)this.iDefHara);
					this.iBoteCount = 0;
				}
			}
			if (this.panel["AutoKUPA"].Enabled)
			{
				float num3 = this.slider["Kupa"].Value;
				int num2 = (int)this.checkCommandKupaLevel(data);
				if (num2 >= 0)
				{
					if (num3 < (float)((int)((float)this.iKupaValue[num2] * this.slider["KupaLevel"].Value / 100f)))
					{
						this.pa["KUPA.挿入." + num2].Play(num3, (float)((int)((float)this.iKupaValue[num2] * this.slider["KupaLevel"].Value / 100f)));
					}
					this.bKupaFuck = true;
				}
				else if (this.bKupaFuck && this.checkCommandKupaStop(data))
				{
					this.pa["KUPA.止める"].Play(num3, (float)this.iKupaMin);
					this.bKupaFuck = false;
				}
				num3 = this.slider["AnalKupa"].Value;
				num2 = (int)this.checkCommandAnalKupaLevel(data);
				if (num2 >= 0)
				{
					if (num3 < (float)((int)((float)this.iAnalKupaValue[num2] * this.slider["KupaLevel"].Value / 100f)))
					{
						this.pa["AKPA.挿入." + num2].Play(num3, (float)((int)((float)this.iAnalKupaValue[num2] * this.slider["KupaLevel"].Value / 100f)));
					}
					this.bAnalKupaFuck = true;
				}
				else if (this.bAnalKupaFuck && this.checkCommandAnalKupaStop(data))
				{
					this.pa["AKPA.止める"].Play(num3, (float)this.iAnalKupaMin);
					this.bAnalKupaFuck = false;
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00008D48 File Offset: 0x00007D48
		private void playAnimeOnInputKeyDown(KeyCode keycode)
		{
			if (keycode == this.ToggleWindowKey)
			{
				if (this.visible)
				{
					float[] array = new float[2];
					array[0] = (float)Screen.width;
					this.fWinAnimeFrom = array;
					this.fWinAnimeTo = new float[]
					{
						this.winAnimeRect.x,
						1f
					};
				}
				else
				{
					this.fWinAnimeFrom = new float[]
					{
						this.winAnimeRect.x,
						1f
					};
					float[] array = new float[2];
					array[0] = ((this.winAnimeRect.x + this.winAnimeRect.width / 2f > (float)Screen.width / 2f) ? ((float)Screen.width) : (-this.winAnimeRect.width));
					this.fWinAnimeTo = array;
				}
				this.pa["WIN.Load"].Play(this.fWinAnimeFrom, this.fWinAnimeTo);
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00008E50 File Offset: 0x00007E50
		private void updateAnimeOnUpdate()
		{
			if (this.panel["AutoAHE"].Enabled)
			{
				if (this.pa["AHE.継続.0"].NowPlaying)
				{
					this.pa["AHE.継続.0"].Update();
				}
				if (this.pa["AHE.絶頂.0"].NowPlaying)
				{
					this.pa["AHE.絶頂.0"].Update();
					this.maid.FaceBlend(this.sAheOrgasmFaceBlend[this.idxAheOrgasm]);
					this.panel["FaceBlend"].HeaderUILabelText = this.sAheOrgasmFaceBlend[this.idxAheOrgasm];
				}
				for (int i = 0; i < 3; i++)
				{
					if (this.pa["AHE.痙攣." + i].NowPlaying)
					{
						this.pa["AHE.痙攣." + i].Update();
					}
				}
				if (!this.pa["AHE.継続.0"].NowPlaying && !this.pa["AHE.絶頂.0"].NowPlaying && (!this.isDeviceIn || this.chuBlipManager.GetPistonSpeedStage() == 0))
				{
					float num = this.maid.body0.trsEyeL.localPosition.y * this.fEyePosToSliderMul;
					if (num > this.fAheDefEye)
					{
						this.updateMaidEyePosY(num - this.fAheEyeDecrement * (float)((int)(this.fPassedTimeOnCommand / 10f)));
					}
				}
			}
			if (this.panel["AutoTUN"].Enabled)
			{
				this.updateShapeKeyChikubiBokkiValue(this.slider["ChikubiBokki"].Value);
				this.updateShapeKeyChikubiTareValue(this.slider["ChikubiTare"].Value);
				this.animateAutoTun();
			}
			if (this.panel["AutoBOTE"].Enabled)
			{
				if (this.pa["BOTE.絶頂"].NowPlaying)
				{
					this.pa["BOTE.絶頂"].Update();
				}
				if (this.pa["BOTE.止める"].NowPlaying)
				{
					this.pa["BOTE.止める"].Update();
				}
				if (this.pa["BOTE.流れ出る"].NowPlaying)
				{
					this.pa["BOTE.流れ出る"].Update();
				}
				if (this.pa["BOTE.止める"].NowPlaying || this.pa["BOTE.流れ出る"].NowPlaying)
				{
					this.iCurrentHara = (int)this.slider["Hara"].Value;
				}
				if (!this.pa["BOTE.絶頂"].NowPlaying)
				{
					if (this.isDeviceIn)
					{
						if (this.pa["BOTE.止める"].NowPlaying || this.pa["BOTE.流れ出る"].NowPlaying)
						{
							this.pa["BOTE.流れ出る"].Stop();
							this.pa["BOTE.止める"].Stop();
						}
						this.iBoteCount = 0;
					}
					else
					{
						float num2 = (float)Mathf.Max(this.iCurrentHara, this.iDefHara);
						float num3 = (float)this.iDefHara;
						if (!this.pa["BOTE.止める"].NowPlaying && !this.pa["BOTE.流れ出る"].NowPlaying && num2 > num3)
						{
							if (this.toggle["SlowCreampie"].Value)
							{
								this.pa["BOTE.流れ出る"].Play(num2, num3);
							}
							else
							{
								this.pa["BOTE.止める"].Play(num2, num3);
							}
							this.iBoteCount = 0;
						}
					}
				}
			}
			if (this.panel["AutoKUPA"].Enabled)
			{
				bool flag = false;
				string[] array = new string[]
				{
					"KUPA.挿入.0",
					"KUPA.挿入.1",
					"KUPA.止める",
					"AKPA.挿入.0",
					"AKPA.挿入.1",
					"AKPA.止める",
					"KUPACL.剥く.0",
					"KUPACL.剥く.1",
					"KUPACL.被る"
				};
				foreach (string key in array)
				{
					if (this.pa[key].NowPlaying)
					{
						if ((!this.isDeviceIn && this.pa[key].Contains("挿入")) || (this.isDeviceIn && this.pa[key].Contains("止める")))
						{
							this.pa[key].Stop();
							this.animateAutoKupa();
						}
						else
						{
							this.pa[key].Update();
							flag = true;
						}
					}
				}
				if (this.bKupaAvailable && this.iKupaWaitingValue > 0)
				{
					float value = this.slider["Kupa"].Value;
					if (!flag && value > 0f)
					{
						this.fPassedTimeOnAutoKupaWaiting += Time.deltaTime;
						float num4 = 180f * this.fPassedTimeOnAutoKupaWaiting * 0.017453292f;
						float num5 = 1f;
						float num6 = value + (float)this.iKupaWaitingValue * (1f + Mathf.Sin(num5 * num4)) / 2f;
						this.maid.body0.VertexMorph_FromProcItem("kupa", num6 / 100f);
					}
					else
					{
						this.fPassedTimeOnAutoKupaWaiting = 0f;
					}
				}
				if (this.bAnalKupaAvailable && this.iAnalKupaWaitingValue > 0)
				{
					float value = this.slider["AnalKupa"].Value;
					if (!flag && value > 0f)
					{
						this.fPassedTimeOnAutoAnalKupaWaiting += Time.deltaTime;
						float num4 = 180f * this.fPassedTimeOnAutoAnalKupaWaiting * 0.017453292f;
						float num5 = 1f;
						float num6 = value + (float)this.iAnalKupaWaitingValue * (1f + Mathf.Sin(num5 * num4)) / 2f;
						this.maid.body0.VertexMorph_FromProcItem("analkupa", num6 / 100f);
					}
					else
					{
						this.fPassedTimeOnAutoAnalKupaWaiting = 0f;
					}
				}
				if (this.bClitorisAvailable)
				{
					float num7;
					float num8;
					if (this.slider["Excite"].Value < this.slider["OrgasmThreshold"].Value * 0.4f)
					{
						num7 = 0f;
						num8 = 30f;
					}
					else if (this.slider["Excite"].Value < this.slider["OrgasmThreshold"].Value * 0.7f)
					{
						num7 = 40f;
						num8 = 30f;
					}
					else if (this.slider["Excite"].Value < this.slider["OrgasmThreshold"].Value * 1f)
					{
						num7 = 70f;
						num8 = 40f;
					}
					else
					{
						num7 = 100f;
						num8 = 50f;
					}
					string text = this.chuBlipManager.GetSexMode().ToString();
					if (text.Contains(3.ToString()))
					{
						if (!this.pa["KUPACL.剥く.1"].NowPlaying)
						{
							this.pa["KUPACL.剥く.1"].Play(0f + num7, num8 + num7);
						}
					}
					else if (!this.pa["KUPACL.剥く.0"].NowPlaying && !this.pa["KUPACL.剥く.1"].NowPlaying && this.slider["InsertDepth"].Value >= 700f && this.slider["Clitoris"].Value < num8 - 10f + num7)
					{
						this.pa["KUPACL.剥く.0"].Play(0f + num7, num8 + num7);
					}
					else if (!this.pa["KUPACL.被る"].NowPlaying && this.slider["InsertDepth"].Value < 700f && this.slider["Clitoris"].Value > num8 - 10f + num7)
					{
						this.pa["KUPACL.被る"].Play(num8 + num7, 0f + num7);
					}
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x000098B0 File Offset: 0x000088B0
		private void updateAnimeOnGUI()
		{
			if (this.pa["WIN.Load"].NowPlaying)
			{
				this.pa["WIN.Load"].Update();
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000098F2 File Offset: 0x000088F2
		private void dummyWin(int winID)
		{
		}

		// Token: 0x0600005C RID: 92 RVA: 0x000098F5 File Offset: 0x000088F5
		private void updateSlider(string name, float value)
		{
			UnityObsoleteGui.Container.Find<AddChikuwaSupporter.YotogiSlider>(this.window, name).Value = value;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000990C File Offset: 0x0000890C
		private void updateWindowAnime(float[] x)
		{
			this.winAnimeRect.x = x[0];
			GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, x[1]);
			GUIStyle guistyle = "box";
			guistyle.fontSize = PixelValuesCM3D2.Font("C1");
			guistyle.alignment = 2;
			this.winAnimeRect = GUI.Window(0, this.winAnimeRect, new GUI.WindowFunction(this.dummyWin), "0.1.0.15", guistyle);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x0000999D File Offset: 0x0000899D
		private void updateMaidExcite(int value)
		{
			this.updateSlider("Slider:Excite", (float)value);
			this.iLastExcite = value;
		}

		// Token: 0x0600005F RID: 95 RVA: 0x000099B5 File Offset: 0x000089B5
		private void updateMaidMind(int value)
		{
			this.maid.Param.SetCurMind(value);
			this.yotogiParamBasicBar.SetCurrentMind(value, true);
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000099D8 File Offset: 0x000089D8
		private void updatePistonDepthShallow(int value)
		{
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000099DB File Offset: 0x000089DB
		private void updatePistonDepthDeep(int value)
		{
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000099DE File Offset: 0x000089DE
		private void updateInsertDepth(int value)
		{
		}

		// Token: 0x06000063 RID: 99 RVA: 0x000099E1 File Offset: 0x000089E1
		private void updateCurrentPistonSpeed(int value)
		{
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000099E4 File Offset: 0x000089E4
		private void updateMaidReason(int value)
		{
			this.maid.Param.SetCurReason(value);
			this.yotogiParamBasicBarWithChubLip.SetCurrentReason(value, true);
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00009A07 File Offset: 0x00008A07
		private void updateHoleSensitivity(int value)
		{
			GameMain.Instance.CMSystem.HoleSensitivity = (float)value / 1000f;
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00009A22 File Offset: 0x00008A22
		private void updateHoleSync(int value)
		{
			GameMain.Instance.CMSystem.HoleSync = (float)value / 1000f;
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00009A3D File Offset: 0x00008A3D
		private void updateHoleAutoSpeed(int value)
		{
			GameMain.Instance.CMSystem.HoleAutoSpeed = value;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00009A51 File Offset: 0x00008A51
		private void updateHoleAutoInsertStartPos(int value)
		{
			GameMain.Instance.CMSystem.HoleAutoInsertStartPos = value;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00009A65 File Offset: 0x00008A65
		private void updateHoleAutoInsertEndPos(int value)
		{
			GameMain.Instance.CMSystem.HoleAutoInsertEndPos = value;
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00009A7C File Offset: 0x00008A7C
		private void updateMaidFrustration(int value)
		{
			Status status = (Status)this.maidStatusInfo.GetValue(this.maid.Param);
			status.frustration = value;
			this.maidStatusInfo.SetValue(this.maid.Param, status);
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00009ACB File Offset: 0x00008ACB
		private void updateOrgasmThreshold(float value)
		{
			this.updateSlider("Slider:OrgasmThreshold", value);
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00009ADB File Offset: 0x00008ADB
		private void updateOrgasmStartSecond(float value)
		{
			this.updateSlider("Slider:OrgasmStartSecond", value);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00009AEB File Offset: 0x00008AEB
		private void updateVaginalFornixReachCount(float value)
		{
			this.updateSlider("Slider:VaginalFornixReachCount", value);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00009AFB File Offset: 0x00008AFB
		private void updateAfterOrgasmDecrement(float value)
		{
			this.updateSlider("Slider:AfterOrgasmDecrement", value);
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00009B0C File Offset: 0x00008B0C
		private void updateMaidEyePosY(float value)
		{
			if (value < 0f)
			{
				value = 0f;
			}
			Vector3 localPosition = this.maid.body0.trsEyeL.localPosition;
			Vector3 localPosition2 = this.maid.body0.trsEyeR.localPosition;
			this.maid.body0.trsEyeL.localPosition = new Vector3(localPosition.x, Math.Max((this.fAheDefEye + value) / this.fEyePosToSliderMul, 0f), localPosition.z);
			this.maid.body0.trsEyeR.localPosition = new Vector3(localPosition.x, Math.Min((this.fAheDefEye - value) / this.fEyePosToSliderMul, 0f), localPosition.z);
			this.updateSlider("Slider:EyeY", value);
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00009BED File Offset: 0x00008BED
		private void updateAHEEyeMax(float value)
		{
			this.updateSlider("Slider:AHEEyeMax", value);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00009C00 File Offset: 0x00008C00
		private void updateChikubiNaeValue(float value)
		{
			float num = 0f;
			if (value < 0f)
			{
				num = 0f - value;
			}
			try
			{
				if (num > 0f)
				{
					this.updateShapeKeyChikubiTareValue(num);
				}
				if (this.slider["ChikubiBokki"].Value < value)
				{
					this.updateShapeKeyChikubiBokkiValue(value);
				}
			}
			catch
			{
			}
			this.updateSlider("Slider:ChikubiNae", value);
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00009C98 File Offset: 0x00008C98
		private void updateShapeKeyChikubiBokkiValue(float value)
		{
			float num = 0f;
			if (value < 0f)
			{
				num = 0f - value;
			}
			try
			{
				if (num > 0f)
				{
					this.updateShapeKeyChikubiTareValue(num);
				}
				this.VertexMorph_FromProcItem(this.maid.body0, "chikubi_bokki", (value > 0f) ? (value / 100f) : 0f);
			}
			catch
			{
			}
			this.updateSlider("Slider:ChikubiBokki", value);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00009D34 File Offset: 0x00008D34
		private void updateShapeKeyChikubiTareValue(float value)
		{
			try
			{
				this.VertexMorph_FromProcItem(this.maid.body0, "chikubi_tare", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:ChikubiTare", value);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00009D88 File Offset: 0x00008D88
		private void updateMaidHaraValue(float value)
		{
			try
			{
				this.maid.SetProp("Hara", (int)value, true);
				this.maid.body0.VertexMorph_FromProcItem("hara", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:Hara", value);
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00009DF0 File Offset: 0x00008DF0
		private void updateMaidFoceKuchipakuSelfUpdateTime(bool b)
		{
			this.maidFoceKuchipakuSelfUpdateTime.SetValue(this.maid, b);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00009E0C File Offset: 0x00008E0C
		private void updateShapeKeyKupaValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("kupa", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:Kupa", value);
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00009E60 File Offset: 0x00008E60
		private void updateShapeKeyAnalKupaValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("analkupa", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:AnalKupa", value);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00009EB4 File Offset: 0x00008EB4
		private void updateShapeKeyKupaLevelValue(float value)
		{
			this.updateSlider("Slider:KupaLevel", value);
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00009EC4 File Offset: 0x00008EC4
		private void updateShapeKeyLabiaKupaValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("labiakupa", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:LabiaKupa", value);
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00009F18 File Offset: 0x00008F18
		private void updateShapeKeyVaginaKupaValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("vaginakupa", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:VaginaKupa", value);
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00009F6C File Offset: 0x00008F6C
		private void updateShapeKeyNyodoKupaValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("nyodokupa", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:NyodoKupa", value);
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00009FC0 File Offset: 0x00008FC0
		private void updateShapeKeySujiValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("suji", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:Suji", value);
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000A014 File Offset: 0x00009014
		private void updateShapeKeyClitorisValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("clitoris", value / 100f);
			}
			catch
			{
			}
			this.updateSlider("Slider:Clitoris", value);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x0000A068 File Offset: 0x00009068
		private void updateShapeKeyOrgasmValue(float value)
		{
			try
			{
				this.maid.body0.VertexMorph_FromProcItem("orgasm", value / 100f);
			}
			catch
			{
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x0000A0B0 File Offset: 0x000090B0
		private void updateOrgasmConvulsion(float value)
		{
			this.updateShapeKeyOrgasmValue(value);
		}

		// Token: 0x06000080 RID: 128 RVA: 0x0000A0BB File Offset: 0x000090BB
		private void updateAutoPiston(bool b)
		{
			GameMain.Instance.CMSystem.HoleAutoPiston = b;
		}

		// Token: 0x06000081 RID: 129 RVA: 0x0000A0D0 File Offset: 0x000090D0
		private void updateMotionSpeed(float value)
		{
			foreach (object obj in this.anm_BO_body001)
			{
				AnimationState animationState = (AnimationState)obj;
				if (animationState.enabled)
				{
					animationState.speed = value / 100f;
				}
			}
			foreach (Animation animation in this.anm_BO_mbody)
			{
				foreach (object obj2 in animation)
				{
					AnimationState animationState = (AnimationState)obj2;
					if (animationState.enabled)
					{
						animationState.speed = value / 100f;
					}
				}
			}
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000A1D8 File Offset: 0x000091D8
		private void updateCameraControl()
		{
			Vector2 vector;
			vector..ctor(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
			bool flag = this.window.Rectangle.Contains(vector);
			if (flag != this.bCursorOnWindow)
			{
				GameMain.Instance.MainCamera.SetControl(!flag);
				UICamera.InputEnable = !flag;
				this.bCursorOnWindow = flag;
			}
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000A251 File Offset: 0x00009251
		private void updateAheOrgasm(float[] x)
		{
			this.updateMaidEyePosY(x[0]);
			this.updateMotionSpeed(x[1]);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x0000A268 File Offset: 0x00009268
		private int getMaidFrustration()
		{
			return ((Status)this.maidStatusInfo.GetValue(this.maid.Param)).frustration;
		}

		// Token: 0x06000085 RID: 133 RVA: 0x0000A2A0 File Offset: 0x000092A0
		private int getSliderFrustration()
		{
			return (int)(this.slider["Sensitivity"].Value - (float)this.maid.Param.status.correction_data.excite + (float)((this.maid.Param.status.cur_reason < 20) ? 20 : 0));
		}

		// Token: 0x06000086 RID: 134 RVA: 0x0000A498 File Offset: 0x00009498
		private IEnumerator getBoneAnimetionCoroutine(float waitTime)
		{
			yield return new WaitForSeconds(waitTime);
			this.anm_BO_body001 = this.maid.body0.GetAnimation();
			List<GameObject> go_BO_mbody = new List<GameObject>();
			int i = 0;
			for (;;)
			{
				GameObject gameObject = GameObject.Find("Man[" + i + "]/Offset/_BO_mbody");
				if (!gameObject)
				{
					break;
				}
				go_BO_mbody.Add(gameObject);
				i++;
			}
			this.anm_BO_mbody = new Animation[i];
			for (int j = 0; j < i; j++)
			{
				this.anm_BO_mbody[j] = go_BO_mbody[j].GetComponent<Animation>();
			}
			this.bLoadBoneAnimetion = true;
			yield break;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x0000A4C0 File Offset: 0x000094C0
		private AddChikuwaSupporter.TunLevel checkCommandTunLevelCBL(YotogiCBL.SkillData data)
		{
			AddChikuwaSupporter.TunLevel result;
			if (this.isOnFinish)
			{
				result = AddChikuwaSupporter.TunLevel.Nip;
			}
			else
			{
				string text = this.chuBlipManager.GetSexMode().ToString();
				if (text.Contains(2.ToString()))
				{
					result = AddChikuwaSupporter.TunLevel.Petting;
				}
				else if (data.skill_name.Contains("パイズリ"))
				{
					result = AddChikuwaSupporter.TunLevel.Friction;
				}
				else if (data.skill_name.Contains("MP全身洗い"))
				{
					result = AddChikuwaSupporter.TunLevel.Friction;
				}
				else if (data.skill_name.Contains("首絞め"))
				{
					result = AddChikuwaSupporter.TunLevel.Friction;
				}
				else
				{
					result = AddChikuwaSupporter.TunLevel.None;
				}
			}
			return result;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x0000A56C File Offset: 0x0000956C
		private AddChikuwaSupporter.KupaLevel checkSkillKupaLevel(Yotogi.SkillData sd)
		{
			AddChikuwaSupporter.KupaLevel result;
			if (sd.name.StartsWith("バイブ責めアナルセックス"))
			{
				result = AddChikuwaSupporter.KupaLevel.Vibe;
			}
			else if (sd.name.StartsWith("露出プレイ"))
			{
				result = AddChikuwaSupporter.KupaLevel.Vibe;
			}
			else if (sd.name.StartsWith("犬プレイ"))
			{
				result = AddChikuwaSupporter.KupaLevel.Vibe;
			}
			else
			{
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			return result;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000A5D0 File Offset: 0x000095D0
		private AddChikuwaSupporter.KupaLevel checkSkillAnalKupaLevel(Yotogi.SkillData sd)
		{
			AddChikuwaSupporter.KupaLevel result;
			if (sd.name.StartsWith("アナルバイブ責めセックス"))
			{
				result = AddChikuwaSupporter.KupaLevel.Vibe;
			}
			else
			{
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			return result;
		}

		// Token: 0x0600008A RID: 138 RVA: 0x0000A650 File Offset: 0x00009650
		private AddChikuwaSupporter.KupaLevel checkCommandKupaLevel(Yotogi.SkillData.Command.Data.Basic cmd)
		{
			if (cmd.command_type == 0)
			{
				if (!cmd.group_name.Contains("アナル"))
				{
					string[] source = new string[]
					{
						"セックス",
						"太バイブ",
						"正常位",
						"後背位",
						"騎乗位"
					};
					if (source.Any((string t) => cmd.group_name.Contains(t)))
					{
						return AddChikuwaSupporter.KupaLevel.Sex;
					}
					string[] source2 = new string[]
					{
						"愛撫",
						"オナニー",
						"バイブ",
						"シックスナイン",
						"ポーズ維持プレイ",
						"磔プレイ"
					};
					if (source2.Any((string t) => cmd.group_name.Contains(t)))
					{
						return AddChikuwaSupporter.KupaLevel.Vibe;
					}
				}
				else if (cmd.group_name.Contains("アナルバイブ責めセックス"))
				{
					return AddChikuwaSupporter.KupaLevel.Sex;
				}
			}
			else if (cmd.group_name.Contains("三角木馬"))
			{
				if (cmd.name.Contains("肩を押す"))
				{
					return AddChikuwaSupporter.KupaLevel.Vibe;
				}
			}
			else if (cmd.group_name.Contains("まんぐり"))
			{
				if (cmd.name.Contains("愛撫") || cmd.name.Contains("クンニ"))
				{
					return AddChikuwaSupporter.KupaLevel.Vibe;
				}
			}
			if (!cmd.group_name.Contains("アナル"))
			{
				if (cmd.name.Contains("指を増やして"))
				{
					return AddChikuwaSupporter.KupaLevel.Sex;
				}
				if (cmd.group_name.Contains("バイブ") || cmd.group_name.Contains("オナニー"))
				{
					if (cmd.name == "イカせる")
					{
						return AddChikuwaSupporter.KupaLevel.Vibe;
					}
				}
			}
			return AddChikuwaSupporter.KupaLevel.None;
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000A958 File Offset: 0x00009958
		private AddChikuwaSupporter.KupaLevel checkCommandKupaLevelCBL(YotogiCBL.SkillData data)
		{
			AddChikuwaSupporter.KupaLevel result;
			if (data.skill_name.Contains("バイブ責めアナルセックス"))
			{
				result = AddChikuwaSupporter.KupaLevel.Vibe;
			}
			else
			{
				if (this.isDeviceIn)
				{
					if (!data.skill_name.Contains("アナル"))
					{
						string[] source = new string[]
						{
							"セックス",
							"太バイブ",
							"正常位",
							"後背位",
							"騎乗位",
							"2穴"
						};
						if (source.Any((string t) => data.skill_name.Contains(t)))
						{
							return AddChikuwaSupporter.KupaLevel.Sex;
						}
						string[] source2 = new string[]
						{
							"愛撫",
							"オナニー",
							"バイブ",
							"シックスナイン",
							"ポーズ維持プレイ",
							"磔プレイ"
						};
						if (source2.Any((string t) => data.skill_name.Contains(t)))
						{
							return AddChikuwaSupporter.KupaLevel.Vibe;
						}
					}
					else if (data.skill_name.Contains("アナルバイブ責めセックス"))
					{
						return AddChikuwaSupporter.KupaLevel.Sex;
					}
				}
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			return result;
		}

		// Token: 0x0600008C RID: 140 RVA: 0x0000AB2C File Offset: 0x00009B2C
		private AddChikuwaSupporter.KupaLevel checkCommandAnalKupaLevel(Yotogi.SkillData.Command.Data.Basic cmd)
		{
			AddChikuwaSupporter.KupaLevel result;
			if (cmd.group_name.StartsWith("アナルバイブ責めセックス"))
			{
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			else if (cmd.group_name.StartsWith("乱交4Pセックス正常位"))
			{
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			else
			{
				if (cmd.command_type == 0)
				{
					string[] source = new string[]
					{
						"アナルセックス",
						"アナル正常位",
						"アナル後背位",
						"アナル騎乗位",
						"2穴",
						"4P",
						"アナル処女喪失"
					};
					if (source.Any((string t) => cmd.group_name.Contains(t)))
					{
						return AddChikuwaSupporter.KupaLevel.Sex;
					}
					string[] source2 = new string[]
					{
						"アナルバイブ",
						"アナルオナニー"
					};
					if (source2.Any((string t) => cmd.group_name.Contains(t)))
					{
						return AddChikuwaSupporter.KupaLevel.Vibe;
					}
				}
				if (cmd.group_name.Contains("アナルバイブ"))
				{
					if (cmd.name == "イカせる")
					{
						return AddChikuwaSupporter.KupaLevel.Vibe;
					}
				}
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			return result;
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000AD10 File Offset: 0x00009D10
		private AddChikuwaSupporter.KupaLevel checkCommandAnalKupaLevelCBL(YotogiCBL.SkillData data)
		{
			AddChikuwaSupporter.KupaLevel result;
			if (data.skill_name.StartsWith("アナルバイブ責めセックス"))
			{
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			else
			{
				if (this.isDeviceIn)
				{
					string[] source = new string[]
					{
						"アナルセックス",
						"アナル正常位",
						"アナル後背位",
						"アナル騎乗位",
						"2穴",
						"4P",
						"アナル処女喪失"
					};
					if (source.Any((string t) => data.skill_name.Contains(t)))
					{
						return AddChikuwaSupporter.KupaLevel.Sex;
					}
					string[] source2 = new string[]
					{
						"アナルバイブ",
						"アナルオナニー"
					};
					if (source2.Any((string t) => data.skill_name.Contains(t)))
					{
						return AddChikuwaSupporter.KupaLevel.Vibe;
					}
				}
				result = AddChikuwaSupporter.KupaLevel.None;
			}
			return result;
		}

		// Token: 0x0600008E RID: 142 RVA: 0x0000AE2C File Offset: 0x00009E2C
		private bool checkCommandKupaStop(Yotogi.SkillData.Command.Data.Basic cmd)
		{
			if (cmd.group_name == "まんぐり返しアナルセックス")
			{
				if (cmd.name.Contains("責める"))
				{
					return true;
				}
			}
			return this.checkCommandAnyKupaStop(cmd);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x0000AE7C File Offset: 0x00009E7C
		private bool checkCommandAnalKupaStop(Yotogi.SkillData.Command.Data.Basic cmd)
		{
			if (cmd.command_type == 4)
			{
				if (cmd.group_name.Contains("オナニー"))
				{
					return true;
				}
			}
			return this.checkCommandAnyKupaStop(cmd);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000AEC4 File Offset: 0x00009EC4
		private bool checkCommandAnyKupaStop(Yotogi.SkillData.Command.Data.Basic cmd)
		{
			bool result;
			if (cmd.command_type == 5)
			{
				result = true;
			}
			else
			{
				if (cmd.command_type == 4)
				{
					if (cmd.group_name.Contains("愛撫"))
					{
						return true;
					}
					if (cmd.group_name.Contains("まんぐり"))
					{
						return true;
					}
					if (cmd.group_name.Contains("シックスナイン"))
					{
						return true;
					}
					if (cmd.name.Contains("外出し"))
					{
						return true;
					}
				}
				else
				{
					if (cmd.name.Contains("頭を撫でる"))
					{
						return true;
					}
					if (cmd.name.Contains("口を責める"))
					{
						return true;
					}
					if (cmd.name.Contains("クリトリスを責めさせる"))
					{
						return true;
					}
					if (cmd.name.Contains("バイブを舐めさせる"))
					{
						return true;
					}
					if (cmd.name.Contains("擦りつける"))
					{
						return true;
					}
					if (cmd.name.Contains("放尿させる"))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000B024 File Offset: 0x0000A024
		private bool checkCommandKupaStopCBL(YotogiCBL.SkillData data)
		{
			return this.checkCommandAnyKupaStopCBL(data);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x0000B040 File Offset: 0x0000A040
		private bool checkCommandAnalKupaStopCBL(YotogiCBL.SkillData data)
		{
			if (this.isOnFinish)
			{
				if (data.skill_name.Contains("オナニー"))
				{
					return true;
				}
			}
			return this.checkCommandAnyKupaStopCBL(data);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x0000B084 File Offset: 0x0000A084
		private bool checkCommandAnyKupaStopCBL(YotogiCBL.SkillData data)
		{
			bool result;
			if (!this.toggle["ContinueInsert"].Value && !this.isDeviceIn)
			{
				result = true;
			}
			else
			{
				if (this.isOnFinish)
				{
					if (data.skill_name.Contains("愛撫"))
					{
						return true;
					}
					if (data.skill_name.Contains("まんぐり"))
					{
						return true;
					}
					if (data.skill_name.Contains("シックスナイン"))
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		// Token: 0x06000094 RID: 148 RVA: 0x0000B120 File Offset: 0x0000A120
		private T parseExIni<T>(string section, string key, T def)
		{
			T result = def;
			string text = this.parseExIniRaw(section, key, null);
			if (text != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter == null)
				{
					AddChikuwaSupporter.LogError("Ini: Invalid type: [{0}] {1} ({2})", new object[]
					{
						section,
						key,
						typeof(T)
					});
				}
				else
				{
					try
					{
						result = (T)((object)converter.ConvertFromString(text));
					}
					catch (Exception ex)
					{
						AddChikuwaSupporter.LogWarning("Ini: Convert failed: [{0}] {1}='{2}' ({3})", new object[]
						{
							section,
							key,
							text,
							ex
						});
					}
				}
			}
			return result;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000B1F0 File Offset: 0x0000A1F0
		private string parseExIniRaw(string section, string key, string def)
		{
			if (base.Preferences.HasSection(section))
			{
				if (base.Preferences[section].HasKey(key))
				{
					return base.Preferences[section][key].Value;
				}
			}
			return def;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000B24C File Offset: 0x0000A24C
		private void setExIni<T>(string section, string key, T value)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			if (converter != null)
			{
				try
				{
					base.Preferences[section][key].Value = converter.ConvertToString(value);
				}
				catch (NotSupportedException)
				{
				}
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000B2B4 File Offset: 0x0000A2B4
		[Conditional("DEBUG")]
		private static void LogDebug(string msg, params object[] args)
		{
			Debug.Log(" AddChikuwaSupporter_feat_YotogiSlider : " + string.Format(msg, args));
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000B2CE File Offset: 0x0000A2CE
		private static void LogWarning(string msg, params object[] args)
		{
			Debug.LogWarning(" AddChikuwaSupporter_feat_YotogiSlider : " + string.Format(msg, args));
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000B2E8 File Offset: 0x0000A2E8
		private static void LogError(string msg, params object[] args)
		{
			Debug.LogError(" AddChikuwaSupporter_feat_YotogiSlider : " + string.Format(msg, args));
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000B304 File Offset: 0x0000A304
		private static void LogError(object ex)
		{
			AddChikuwaSupporter.LogError("{0}", new object[]
			{
				ex
			});
		}

		// Token: 0x0600009B RID: 155 RVA: 0x0000B3DC File Offset: 0x0000A3DC
		internal static IEnumerator CheckFadeStatus(WfScreenChildren wsc, float waitTime)
		{
			for (;;)
			{
				yield return new WaitForSeconds(waitTime);
			}
			yield break;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000B404 File Offset: 0x0000A404
		internal static string GetFullPath(GameObject go)
		{
			string text = go.name;
			if (go.transform.parent != null)
			{
				text = AddChikuwaSupporter.GetFullPath(go.transform.parent.gameObject) + "/" + text;
			}
			return text;
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000B458 File Offset: 0x0000A458
		internal static void WriteComponent(GameObject go)
		{
			Component[] components = go.GetComponents<Component>();
			foreach (Component component in components)
			{
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x0000B48C File Offset: 0x0000A48C
		internal static void WriteTrans(string s)
		{
			GameObject gameObject = GameObject.Find(s);
			if (!AddChikuwaSupporter.IsNull<GameObject>(gameObject, s + " not found."))
			{
				AddChikuwaSupporter.WriteTrans(gameObject.transform, 0, null);
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000B4C9 File Offset: 0x0000A4C9
		internal static void WriteTrans(Transform t)
		{
			AddChikuwaSupporter.WriteTrans(t, 0, null);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000B4D8 File Offset: 0x0000A4D8
		internal static void WriteTrans(Transform t, int level, StreamWriter writer)
		{
			if (level == 0)
			{
				writer = new StreamWriter(".\\" + t.name + ".txt", false);
			}
			if (writer != null)
			{
				string text = "";
				for (int i = 0; i < level; i++)
				{
					text += "    ";
				}
				writer.WriteLine(string.Concat(new object[]
				{
					text,
					level,
					",",
					t.name
				}));
				foreach (object obj in t)
				{
					Transform t2 = (Transform)obj;
					AddChikuwaSupporter.WriteTrans(t2, level + 1, writer);
				}
				if (level == 0)
				{
					writer.Close();
				}
			}
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000B5E8 File Offset: 0x0000A5E8
		internal static bool IsNull<T>(T t) where T : class
		{
			return t == null;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000B608 File Offset: 0x0000A608
		internal static bool IsNull<T>(T t, string s) where T : class
		{
			bool result;
			if (t == null)
			{
				AddChikuwaSupporter.LogError(s, new object[0]);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000B640 File Offset: 0x0000A640
		internal static bool IsActive(GameObject go)
		{
			return go && go.activeInHierarchy;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000B664 File Offset: 0x0000A664
		internal static T getInstance<T>() where T : Object
		{
			return Object.FindObjectOfType(typeof(T)) as T;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x0000B690 File Offset: 0x0000A690
		internal static TResult getMethodDelegate<T, TResult>(T inst, string name) where T : class where TResult : class
		{
			return Delegate.CreateDelegate(typeof(TResult), inst, name) as TResult;
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x0000B6C4 File Offset: 0x0000A6C4
		internal static FieldInfo getFieldInfo<T>(string name)
		{
			BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
			return typeof(T).GetField(name, bindingAttr);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000B6EC File Offset: 0x0000A6EC
		internal static TResult getFieldValue<T, TResult>(T inst, string name)
		{
			TResult result;
			if (inst == null)
			{
				result = default(TResult);
			}
			else
			{
				FieldInfo fieldInfo = AddChikuwaSupporter.getFieldInfo<T>(name);
				if (fieldInfo == null)
				{
					result = default(TResult);
				}
				else
				{
					result = (TResult)((object)fieldInfo.GetValue(inst));
				}
			}
			return result;
		}

		// Token: 0x04000001 RID: 1
		public const string PluginName = " AddChikuwaSupporter_feat_YotogiSlider";

		// Token: 0x04000002 RID: 2
		public const string Version = "0.1.0.15";

		// Token: 0x04000003 RID: 3
		private const string LogLabel = " AddChikuwaSupporter_feat_YotogiSlider : ";

		// Token: 0x04000004 RID: 4
		private readonly float TimePerInit = 0f;

		// Token: 0x04000005 RID: 5
		private readonly float TimePerUpdateSpeed = 0.33f;

		// Token: 0x04000006 RID: 6
		private readonly float WaitFirstInit = 0f;

		// Token: 0x04000007 RID: 7
		private readonly float WaitBoneLoad = 1f;

		// Token: 0x04000008 RID: 8
		private readonly string commandUnitName = "/UI Root/YotogiPlayPanel/CommandViewer";

		// Token: 0x04000009 RID: 9
		private readonly string parameterUnitName = "/UI Root/YotogiPlayPanel/StatusViewer/ParameterViewer/MaskGroup/ParameterParent";

		// Token: 0x0400000A RID: 10
		private KeyCode ToggleWindowKey = 286;

		// Token: 0x0400000B RID: 11
		private int sceneLevel;

		// Token: 0x0400000C RID: 12
		private bool visible = false;

		// Token: 0x0400000D RID: 13
		private bool bInitCompleted = false;

		// Token: 0x0400000E RID: 14
		private bool bParameterViewerInitCompleted = false;

		// Token: 0x0400000F RID: 15
		private bool bFadeInWait = false;

		// Token: 0x04000010 RID: 16
		private bool bLoadBoneAnimetion = false;

		// Token: 0x04000011 RID: 17
		private bool bSyncMotionSpeed = false;

		// Token: 0x04000012 RID: 18
		private bool bCursorOnWindow = false;

		// Token: 0x04000013 RID: 19
		private bool isOnFinish = false;

		// Token: 0x04000014 RID: 20
		private bool isDeviceIn = false;

		// Token: 0x04000015 RID: 21
		private bool isBeforeDeviceIn = false;

		// Token: 0x04000016 RID: 22
		private float fPassedTimeOnLevel = 0f;

		// Token: 0x04000017 RID: 23
		private bool kagScriptCallbacksOverride = false;

		// Token: 0x04000018 RID: 24
		private string currentYotogiName;

		// Token: 0x04000019 RID: 25
		private string[] sKey = new string[]
		{
			"WIN",
			"STATUS",
			"AHE",
			"BOTE",
			"FACEBLEND",
			"FACEANIME"
		};

		// Token: 0x0400001A RID: 26
		private string[] sliderNameStatus = new string[]
		{
			"〈浅い〉",
			"挿入位置",
			"〈深い〉",
			"速度",
			"理性",
			"最大挿入値",
			"ピストン同期",
			"興奮",
			"感度"
		};

		// Token: 0x0400001B RID: 27
		private string[] sliderNameAutoPiston = new string[]
		{
			"ピストン速度",
			"挿入初期位置",
			"挿入深度"
		};

		// Token: 0x0400001C RID: 28
		private string[] sliderNameAutoIKU = new string[]
		{
			"絶頂限界",
			"持続:秒",
			"子宮口突回数",
			"絶頂後減少率"
		};

		// Token: 0x0400001D RID: 29
		private string[] sliderNameAutoAHE = new string[]
		{
			"瞳Y",
			"自然上昇上限"
		};

		// Token: 0x0400001E RID: 30
		private string[] sliderNameAutoTUN = new string[]
		{
			"乳首肥大度",
			"乳首萎え",
			"乳首勃起",
			"乳首たれ"
		};

		// Token: 0x0400001F RID: 31
		private string[] sliderNameAutoBOTE = new string[]
		{
			"腹"
		};

		// Token: 0x04000020 RID: 32
		private string[] sliderNameAutoKUPA = new string[]
		{
			"前",
			"後",
			"拡張度",
			"陰唇",
			"膣",
			"尿道",
			"すじ",
			"クリ"
		};

		// Token: 0x04000021 RID: 33
		private List<string> sStageNames = new List<string>();

		// Token: 0x04000022 RID: 34
		private Dictionary<string, AddChikuwaSupporter.PlayAnime> pa = new Dictionary<string, AddChikuwaSupporter.PlayAnime>();

		// Token: 0x04000023 RID: 35
		private Window window;

		// Token: 0x04000024 RID: 36
		private Rect winRatioRect = new Rect(0.75f, 0.25f, 0.2f, 0.65f);

		// Token: 0x04000025 RID: 37
		private Rect winAnimeRect;

		// Token: 0x04000026 RID: 38
		private float[] fWinAnimeFrom;

		// Token: 0x04000027 RID: 39
		private float[] fWinAnimeTo;

		// Token: 0x04000028 RID: 40
		private Dictionary<string, AddChikuwaSupporter.YotogiPanel> panel = new Dictionary<string, AddChikuwaSupporter.YotogiPanel>();

		// Token: 0x04000029 RID: 41
		private Dictionary<string, AddChikuwaSupporter.YotogiSlider> slider = new Dictionary<string, AddChikuwaSupporter.YotogiSlider>();

		// Token: 0x0400002A RID: 42
		private Dictionary<string, AddChikuwaSupporter.YotogiButtonGrid> grid = new Dictionary<string, AddChikuwaSupporter.YotogiButtonGrid>();

		// Token: 0x0400002B RID: 43
		private Dictionary<string, AddChikuwaSupporter.YotogiToggle> toggle = new Dictionary<string, AddChikuwaSupporter.YotogiToggle>();

		// Token: 0x0400002C RID: 44
		private Dictionary<string, AddChikuwaSupporter.YotogiLineSelect> lSelect = new Dictionary<string, AddChikuwaSupporter.YotogiLineSelect>();

		// Token: 0x0400002D RID: 45
		private int iLastExcite = 0;

		// Token: 0x0400002E RID: 46
		private int iOrgasmCount = 0;

		// Token: 0x0400002F RID: 47
		private int iLastSliderFrustration = 0;

		// Token: 0x04000030 RID: 48
		private float fLastSliderSensitivity = 0f;

		// Token: 0x04000031 RID: 49
		private float fPassedTimeOnCommand = -1f;

		// Token: 0x04000032 RID: 50
		private float fSensitivity = 30f;

		// Token: 0x04000033 RID: 51
		private float fOrgasmThreshold = 300f;

		// Token: 0x04000034 RID: 52
		private float fOrgasmStartSecond = 3f;

		// Token: 0x04000035 RID: 53
		private float fVaginalFornixReachCount = 0f;

		// Token: 0x04000036 RID: 54
		private float fAfterOrgasmDecrement = 50f;

		// Token: 0x04000037 RID: 55
		private float fCurrentHoleSensitivity = 0f;

		// Token: 0x04000038 RID: 56
		private float fCurrentHoleSensitivityBeforeEcstasy = 0f;

		// Token: 0x04000039 RID: 57
		private float fHoleAutoSpeedBeforeEcstasy = 0f;

		// Token: 0x0400003A RID: 58
		private float fHoleAutoInsertStartPosBeforeEcstasy = 0f;

		// Token: 0x0400003B RID: 59
		private float fHoleAutoInsertEndPosBeforeEcstasy = 0f;

		// Token: 0x0400003C RID: 60
		private bool boEnableSupportCreamPieContinue = false;

		// Token: 0x0400003D RID: 61
		private bool boAwayFromVaginalFornix = false;

		// Token: 0x0400003E RID: 62
		private bool isReadyToEcstasy = false;

		// Token: 0x0400003F RID: 63
		private bool isKeepFast = false;

		// Token: 0x04000040 RID: 64
		private int iVaginalFornixReaches = 0;

		// Token: 0x04000041 RID: 65
		private DateTime lastReadyToEcstasyTime = DateTime.Now;

		// Token: 0x04000042 RID: 66
		private DateTime lastStartCreamPieContinueTime = DateTime.Now;

		// Token: 0x04000043 RID: 67
		private bool bOrgasmAvailable = false;

		// Token: 0x04000044 RID: 68
		private float fAheEyeMax = 70f;

		// Token: 0x04000045 RID: 69
		private float fEyePosToSliderMul = 5000f;

		// Token: 0x04000046 RID: 70
		private float fOrgasmsPerAheLevel = 3f;

		// Token: 0x04000047 RID: 71
		private int[] iAheExcite = new int[]
		{
			267,
			233,
			200
		};

		// Token: 0x04000048 RID: 72
		private float fAheDefEye = 0f;

		// Token: 0x04000049 RID: 73
		private float fAheLastEye = 0f;

		// Token: 0x0400004A RID: 74
		private float fAheEyeDecrement = 0.0033333334f;

		// Token: 0x0400004B RID: 75
		private float[] fAheNormalEyeMax = new float[]
		{
			40f,
			45f,
			50f
		};

		// Token: 0x0400004C RID: 76
		private float[] fAheOrgasmEyeMax = new float[]
		{
			50f,
			60f,
			70f
		};

		// Token: 0x0400004D RID: 77
		private float[] fAheOrgasmEyeMin = new float[]
		{
			30f,
			35f,
			40f
		};

		// Token: 0x0400004E RID: 78
		private float[] fAheOrgasmSpeed = new float[]
		{
			90f,
			80f,
			70f
		};

		// Token: 0x0400004F RID: 79
		private float[] fAheOrgasmConvulsion = new float[]
		{
			60f,
			80f,
			100f
		};

		// Token: 0x04000050 RID: 80
		private string[] sAheOrgasmFace = new string[]
		{
			"エロ放心",
			"エロ好感３",
			"通常射精後１"
		};

		// Token: 0x04000051 RID: 81
		private string[] sAheOrgasmFaceBlend = new string[]
		{
			"頬１涙１",
			"頬２涙２",
			"頬３涙３よだれ"
		};

		// Token: 0x04000052 RID: 82
		private int iAheOrgasmChain = 0;

		// Token: 0x04000053 RID: 83
		private bool bBokkiChikubiAvailable = false;

		// Token: 0x04000054 RID: 84
		private int[] iTunValue = new int[]
		{
			3,
			5,
			100
		};

		// Token: 0x04000055 RID: 85
		private float fChikubiScale = 25f;

		// Token: 0x04000056 RID: 86
		private float fChikubiNae = 0f;

		// Token: 0x04000057 RID: 87
		private float fChikubiBokki = 0f;

		// Token: 0x04000058 RID: 88
		private float fChikubiTare = 0f;

		// Token: 0x04000059 RID: 89
		private float iDefChikubiNae;

		// Token: 0x0400005A RID: 90
		private float iDefChikubiTare;

		// Token: 0x0400005B RID: 91
		private int iDefHara;

		// Token: 0x0400005C RID: 92
		private int iCurrentHara;

		// Token: 0x0400005D RID: 93
		private int iHaraIncrement = 10;

		// Token: 0x0400005E RID: 94
		private int iBoteHaraMax = 100;

		// Token: 0x0400005F RID: 95
		private int iBoteCount = 0;

		// Token: 0x04000060 RID: 96
		private string[] sNoBoteSkillNames = new string[]
		{
			"愛撫",
			"バイブ責め",
			"アナルバイブ責め",
			"太バイブ責め",
			"足コキ",
			"オナニー",
			"アナルオナニー",
			"手コキオナニー",
			"フェラオナニー",
			"バイブオナニー",
			"アナルバイブオナニー",
			"太バイブオナニー",
			"実況オナニー",
			"手コキ",
			"フェラチオ",
			"パイズリ",
			"シックスナイン",
			"パイズリフェラ",
			"スケベ椅子フェラ",
			"mp全身洗い",
			"ディープスロート",
			"セルフイラマ",
			"オナホコキ",
			"拘束愛撫",
			"イラマチオ",
			"拘束イラマチオ",
			"足舐めオナニー",
			"拘束台愛撫"
		};

		// Token: 0x04000061 RID: 97
		private bool bKupaAvailable = false;

		// Token: 0x04000062 RID: 98
		private bool bKupaFuck = false;

		// Token: 0x04000063 RID: 99
		private float fKupaLevel = 70f;

		// Token: 0x04000064 RID: 100
		private float fLabiaKupa = 0f;

		// Token: 0x04000065 RID: 101
		private float fVaginaKupa = 0f;

		// Token: 0x04000066 RID: 102
		private float fNyodoKupa = 0f;

		// Token: 0x04000067 RID: 103
		private float fSuji = 0f;

		// Token: 0x04000068 RID: 104
		private int iKupaDef = 0;

		// Token: 0x04000069 RID: 105
		private int iKupaStart = 0;

		// Token: 0x0400006A RID: 106
		private int iKupaIncrementPerOrgasm = 0;

		// Token: 0x0400006B RID: 107
		private int iKupaNormalMax = 0;

		// Token: 0x0400006C RID: 108
		private int[] iKupaValue = new int[]
		{
			100,
			50
		};

		// Token: 0x0400006D RID: 109
		private int iKupaWaitingValue = 5;

		// Token: 0x0400006E RID: 110
		private float fPassedTimeOnAutoKupaWaiting = 0f;

		// Token: 0x0400006F RID: 111
		private bool bAnalKupaAvailable = false;

		// Token: 0x04000070 RID: 112
		private bool bAnalKupaFuck = false;

		// Token: 0x04000071 RID: 113
		private int iAnalKupaDef = 0;

		// Token: 0x04000072 RID: 114
		private int iAnalKupaStart = 0;

		// Token: 0x04000073 RID: 115
		private int iAnalKupaIncrementPerOrgasm = 0;

		// Token: 0x04000074 RID: 116
		private int iAnalKupaNormalMax = 0;

		// Token: 0x04000075 RID: 117
		private int[] iAnalKupaValue = new int[]
		{
			100,
			50
		};

		// Token: 0x04000076 RID: 118
		private int iAnalKupaWaitingValue = 5;

		// Token: 0x04000077 RID: 119
		private float fPassedTimeOnAutoAnalKupaWaiting = 0f;

		// Token: 0x04000078 RID: 120
		private bool bLabiaKupaAvailable = false;

		// Token: 0x04000079 RID: 121
		private bool bVaginaKupaAvailable = false;

		// Token: 0x0400007A RID: 122
		private bool bNyodoKupaAvailable = false;

		// Token: 0x0400007B RID: 123
		private bool bSujiAvailable = false;

		// Token: 0x0400007C RID: 124
		private bool bClitorisAvailable = false;

		// Token: 0x0400007D RID: 125
		private int iLabiaKupaMin = 0;

		// Token: 0x0400007E RID: 126
		private int iVaginaKupaMin = 0;

		// Token: 0x0400007F RID: 127
		private int iNyodoKupaMin = 0;

		// Token: 0x04000080 RID: 128
		private int iSujiMin = 0;

		// Token: 0x04000081 RID: 129
		private int iClitorisMin = 0;

		// Token: 0x04000082 RID: 130
		private string[] sFaceNames = new string[]
		{
			"エロ通常１",
			"エロ通常２",
			"エロ通常３",
			"エロ羞恥１",
			"エロ羞恥２",
			"エロ羞恥３",
			"エロ興奮０",
			"エロ興奮１",
			"エロ興奮２",
			"エロ興奮３",
			"エロ緊張",
			"エロ期待",
			"エロ好感１",
			"エロ好感２",
			"エロ好感３",
			"エロ我慢１",
			"エロ我慢２",
			"エロ我慢３",
			"エロ嫌悪１",
			"エロ怯え",
			"エロ痛み１",
			"エロ痛み２",
			"エロ痛み３",
			"エロメソ泣き",
			"エロ絶頂",
			"エロ痛み我慢",
			"エロ痛み我慢２",
			"エロ痛み我慢３",
			"エロ放心",
			"発情",
			"通常射精後１",
			"通常射精後２",
			"興奮射精後１",
			"興奮射精後２",
			"絶頂射精後１",
			"絶頂射精後２",
			"エロ舐め愛情",
			"エロ舐め愛情２",
			"エロ舐め快楽",
			"エロ舐め快楽２",
			"エロ舐め嫌悪",
			"エロ舐め通常",
			"閉じ舐め愛情",
			"閉じ舐め快楽",
			"閉じ舐め快楽２",
			"閉じ舐め嫌悪",
			"閉じ舐め通常",
			"接吻",
			"エロフェラ愛情",
			"エロフェラ快楽",
			"エロフェラ嫌悪",
			"エロフェラ通常",
			"エロ舌責",
			"エロ舌責快楽",
			"閉じフェラ愛情",
			"閉じフェラ快楽",
			"閉じフェラ嫌悪",
			"閉じフェラ通常",
			"閉じ目",
			"目口閉じ",
			"通常",
			"怒り",
			"笑顔",
			"微笑み",
			"悲しみ２",
			"泣き",
			"きょとん",
			"ジト目",
			"あーん",
			"ためいき",
			"ドヤ顔",
			"にっこり",
			"びっくり",
			"ぷんすか",
			"まぶたギュ",
			"むー",
			"引きつり笑顔",
			"疑問",
			"苦笑い",
			"困った",
			"思案伏せ目",
			"少し怒り",
			"誘惑",
			"拗ね",
			"優しさ",
			"居眠り安眠",
			"目を見開いて",
			"痛みで目を見開いて",
			"余韻弱",
			"目口閉じ",
			"口開け",
			"恥ずかしい",
			"照れ",
			"照れ叫び",
			"ウインク照れ",
			"にっこり照れ",
			"ダンス目つむり",
			"ダンスあくび",
			"ダンスびっくり",
			"ダンス微笑み",
			"ダンス目あけ",
			"ダンス目とじ",
			"ダンスウインク",
			"ダンスキス",
			"ダンスジト目",
			"ダンス困り顔",
			"ダンス真剣",
			"ダンス憂い",
			"ダンス誘惑",
			"頬０涙０",
			"頬０涙１",
			"頬０涙２",
			"頬０涙３",
			"頬１涙０",
			"頬１涙１",
			"頬１涙２",
			"頬１涙３",
			"頬２涙０",
			"頬２涙１",
			"頬２涙２",
			"頬２涙３",
			"頬３涙１",
			"頬３涙０",
			"頬３涙２",
			"頬３涙３",
			"追加よだれ",
			"頬０涙０よだれ",
			"頬０涙１よだれ",
			"頬０涙２よだれ",
			"頬０涙３よだれ",
			"頬１涙０よだれ",
			"頬１涙１よだれ",
			"頬１涙２よだれ",
			"頬１涙３よだれ",
			"頬２涙０よだれ",
			"頬２涙１よだれ",
			"頬２涙２よだれ",
			"頬２涙３よだれ",
			"頬３涙０よだれ",
			"頬３涙１よだれ",
			"頬３涙２よだれ",
			"頬３涙３よだれ",
			"エラー",
			"デフォ"
		};

		// Token: 0x04000083 RID: 131
		private string[] sFaceBlendCheek = new string[]
		{
			"頬０",
			"頬１",
			"頬２",
			"頬３"
		};

		// Token: 0x04000084 RID: 132
		private string[] sFaceBlendTear = new string[]
		{
			"涙０",
			"涙１",
			"涙２",
			"涙３"
		};

		// Token: 0x04000085 RID: 133
		private Maid maid;

		// Token: 0x04000086 RID: 134
		private FieldInfo maidStatusInfo;

		// Token: 0x04000087 RID: 135
		private FieldInfo maidFoceKuchipakuSelfUpdateTime;

		// Token: 0x04000088 RID: 136
		private YotogiPlayManagerWithChubLip yotogiPlayManagerWithChubLip;

		// Token: 0x04000089 RID: 137
		private YotogiParamBasicBar yotogiParamBasicBar;

		// Token: 0x0400008A RID: 138
		private YotogiParamBasicBarWithChubLip yotogiParamBasicBarWithChubLip;

		// Token: 0x0400008B RID: 139
		private GameObject goCommandUnit;

		// Token: 0x0400008C RID: 140
		private GameObject goParameterUnit;

		// Token: 0x0400008D RID: 141
		private Action<Yotogi.SkillData.Command.Data> orgOnClickCommand;

		// Token: 0x0400008E RID: 142
		private ChuBlipManager chuBlipManager;

		// Token: 0x0400008F RID: 143
		private OnaholeChuBlipDevice onaholeChuBlipDevice;

		// Token: 0x04000090 RID: 144
		private OnaholeMotion onaholeMotion;

		// Token: 0x04000091 RID: 145
		private KagScript kagScript;

		// Token: 0x04000092 RID: 146
		private Func<KagTagSupport, bool> orgTagFace;

		// Token: 0x04000093 RID: 147
		private Func<KagTagSupport, bool> orgTagFaceBlend;

		// Token: 0x04000094 RID: 148
		private Animation anm_BO_body001;

		// Token: 0x04000095 RID: 149
		private Animation[] anm_BO_mbody;

		// Token: 0x02000003 RID: 3
		public enum TunLevel
		{
			// Token: 0x04000097 RID: 151
			None = -1,
			// Token: 0x04000098 RID: 152
			Friction,
			// Token: 0x04000099 RID: 153
			Petting,
			// Token: 0x0400009A RID: 154
			Nip
		}

		// Token: 0x02000004 RID: 4
		public enum KupaLevel
		{
			// Token: 0x0400009C RID: 156
			None = -1,
			// Token: 0x0400009D RID: 157
			Sex,
			// Token: 0x0400009E RID: 158
			Vibe
		}

		// Token: 0x02000007 RID: 7
		private class YotogiPanel : UnityObsoleteGui.Container
		{
			// Token: 0x1700000E RID: 14
			// (get) Token: 0x060000CD RID: 205 RVA: 0x0000CB1C File Offset: 0x0000BB1C
			private Rect padding
			{
				get
				{
					return PixelValuesCM3D2.PropRect(this.paddingPx);
				}
			}

			// Token: 0x14000002 RID: 2
			// (add) Token: 0x060000CE RID: 206 RVA: 0x0000CB39 File Offset: 0x0000BB39
			// (remove) Token: 0x060000CF RID: 207 RVA: 0x0000CB52 File Offset: 0x0000BB52
			private event EventHandler<ToggleEventArgs> OnEnableChanged;

			// Token: 0x060000D0 RID: 208 RVA: 0x0000CB6B File Offset: 0x0000BB6B
			public YotogiPanel(string name, string title) : this(name, title, AddChikuwaSupporter.YotogiPanel.HeaderUI.None)
			{
			}

			// Token: 0x060000D1 RID: 209 RVA: 0x0000CB79 File Offset: 0x0000BB79
			public YotogiPanel(string name, string title, AddChikuwaSupporter.YotogiPanel.HeaderUI type) : this(name, title, type, null)
			{
			}

			// Token: 0x060000D2 RID: 210 RVA: 0x0000CB88 File Offset: 0x0000BB88
			public YotogiPanel(string name, string title, EventHandler<ToggleEventArgs> onEnableChanged) : this(name, title, AddChikuwaSupporter.YotogiPanel.HeaderUI.None, onEnableChanged)
			{
			}

			// Token: 0x060000D3 RID: 211 RVA: 0x0000CB98 File Offset: 0x0000BB98
			public YotogiPanel(string name, string title, AddChikuwaSupporter.YotogiPanel.HeaderUI type, EventHandler<ToggleEventArgs> onEnableChanged) : base(name, new Rect(-1f, -1f, -1f, 0f))
			{
				this.Title = title;
				this.headerUI = type;
				this.OnEnableChanged = (EventHandler<ToggleEventArgs>)Delegate.Combine(this.OnEnableChanged, onEnableChanged);
				this.Resize();
			}

			// Token: 0x060000D4 RID: 212 RVA: 0x0000CC58 File Offset: 0x0000BC58
			public override void Draw(Rect outRect)
			{
				Rect rect = PixelValuesCM3D2.InsideRect(outRect, this.padding);
				this.labelStyle = "box";
				GUI.Label(outRect, "", this.labelStyle);
				GUI.BeginGroup(rect);
				int num = PixelValuesCM3D2.Line(this.headerHeightPV);
				int fontSize = PixelValuesCM3D2.Font(this.headerFontSizePV);
				Rect rect2;
				rect2..ctor(0f, 0f, this.padding.width, (float)num);
				rect2.width = rect.width * 0.325f;
				this.buttonStyle.fontSize = fontSize;
				this.resizeOnChangeChildrenVisible(GUI.Toggle(rect2, this.childrenVisible, this.Title, this.buttonStyle));
				rect2.x += rect2.width;
				rect2.width = rect.width * 0.3f;
				rect2.y -= (float)PixelValuesCM3D2.PropPx(2);
				this.toggleStyle.fontSize = fontSize;
				this.toggleStyle.alignment = 3;
				this.toggleStyle.normal.textColor = this.toggleColor(this.Enabled);
				this.toggleStyle.hover.textColor = this.toggleColor(this.Enabled);
				this.onEnableChange(GUI.Toggle(rect2, this.Enabled, this.toggleText(this.Enabled), this.toggleStyle));
				rect2.y += (float)PixelValuesCM3D2.PropPx(2);
				rect2.x += rect2.width;
				this.labelStyle = "label";
				this.labelStyle.fontSize = fontSize;
				switch (this.headerUI)
				{
				case AddChikuwaSupporter.YotogiPanel.HeaderUI.Slider:
					rect2.width = rect.width * 0.375f;
					this.labelStyle.alignment = 5;
					GUI.Label(rect2, "Pin", this.labelStyle);
					break;
				case AddChikuwaSupporter.YotogiPanel.HeaderUI.Face:
					rect2.width = rect.width * 0.375f;
					this.labelStyle = "box";
					this.labelStyle.fontSize = fontSize;
					this.labelStyle.alignment = 5;
					GUI.Label(rect2, this.HeaderUILabelText, this.labelStyle);
					break;
				}
				rect2.x = 0f;
				rect2.y += rect2.height + (float)PixelValuesCM3D2.PropPx(3);
				rect2.width = rect.width;
				foreach (Element element in this.children)
				{
					if (element.Visible)
					{
						rect2.height = element.Height;
						element.Draw(rect2);
						rect2.y += rect2.height + (float)PixelValuesCM3D2.PropPx(3);
					}
				}
				GUI.EndGroup();
			}

			// Token: 0x060000D5 RID: 213 RVA: 0x0000CF9C File Offset: 0x0000BF9C
			public override void Resize()
			{
				this.Resize(false);
			}

			// Token: 0x060000D6 RID: 214 RVA: 0x0000CFA8 File Offset: 0x0000BFA8
			public override void Resize(bool broadCast)
			{
				float num = (float)(PixelValuesCM3D2.Line(this.headerHeightPV) + PixelValuesCM3D2.PropPx(3));
				foreach (Element element in this.children)
				{
					if (element.Visible)
					{
						num += element.Height + (float)PixelValuesCM3D2.PropPx(3);
					}
				}
				this.rect.height = num + (float)((int)this.padding.height * 2);
				if (!broadCast)
				{
					this.notifyParent(true, false);
				}
			}

			// Token: 0x060000D7 RID: 215 RVA: 0x0000D05C File Offset: 0x0000C05C
			private void resizeOnChangeChildrenVisible(bool b)
			{
				if (b != this.childrenVisible)
				{
					foreach (Element element in this.children)
					{
						element.Visible = b;
					}
					this.childrenVisible = b;
				}
			}

			// Token: 0x060000D8 RID: 216 RVA: 0x0000D0CC File Offset: 0x0000C0CC
			private void onEnableChange(bool newValue)
			{
				if (this.Enabled != newValue)
				{
					this.Enabled = newValue;
					if (this.OnEnableChanged != null)
					{
						this.OnEnableChanged(this, new ToggleEventArgs(this.Title, newValue));
					}
				}
			}

			// Token: 0x060000D9 RID: 217 RVA: 0x0000D118 File Offset: 0x0000C118
			private Color toggleColor(bool b)
			{
				return b ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0.2f, 0.2f, 1f);
			}

			// Token: 0x060000DA RID: 218 RVA: 0x0000D164 File Offset: 0x0000C164
			private string toggleText(bool b)
			{
				return b ? "Enabled" : "Disabled";
			}

			// Token: 0x040000A8 RID: 168
			private int paddingPx = 4;

			// Token: 0x040000A9 RID: 169
			private GUIStyle labelStyle = "label";

			// Token: 0x040000AA RID: 170
			private GUIStyle toggleStyle = "toggle";

			// Token: 0x040000AB RID: 171
			private GUIStyle buttonStyle = "button";

			// Token: 0x040000AC RID: 172
			private string headerHeightPV = "C1";

			// Token: 0x040000AD RID: 173
			private string headerFontSizePV = "C1";

			// Token: 0x040000AE RID: 174
			private AddChikuwaSupporter.YotogiPanel.HeaderUI headerUI;

			// Token: 0x040000AF RID: 175
			private bool childrenVisible = false;

			// Token: 0x040000B1 RID: 177
			public string Title;

			// Token: 0x040000B2 RID: 178
			public string HeaderUILabelText;

			// Token: 0x040000B3 RID: 179
			public bool Enabled = false;

			// Token: 0x040000B4 RID: 180
			public bool HeaderUIToggle = false;

			// Token: 0x02000008 RID: 8
			public enum HeaderUI
			{
				// Token: 0x040000B6 RID: 182
				None,
				// Token: 0x040000B7 RID: 183
				Slider,
				// Token: 0x040000B8 RID: 184
				Face
			}
		}

		// Token: 0x02000009 RID: 9
		private class YotogiSlider : Element
		{
			// Token: 0x1700000F RID: 15
			// (get) Token: 0x060000DB RID: 219 RVA: 0x0000D188 File Offset: 0x0000C188
			// (set) Token: 0x060000DC RID: 220 RVA: 0x0000D1A8 File Offset: 0x0000C1A8
			public float Value
			{
				get
				{
					return this.slider.Value;
				}
				set
				{
					if (!this.Pin)
					{
						this.slider.Value = value;
					}
				}
			}

			// Token: 0x060000DD RID: 221 RVA: 0x0000D1CC File Offset: 0x0000C1CC
			public YotogiSlider(string name, float min, float max, float def, EventHandler<SliderEventArgs> onChange, string label, bool pinEnabled) : base(name, new Rect(-1f, -1f, -1f, 0f))
			{
				this.slider = new HSlider(name + ":slider", this.rect, min, max, def, onChange);
				this.Default = def;
				this.labelText = label;
				this.pinEnabled = pinEnabled;
				this.Resize();
			}

			// Token: 0x060000DE RID: 222 RVA: 0x0000D278 File Offset: 0x0000C278
			public override void Draw(Rect outRect)
			{
				Rect rect = outRect;
				this.labelStyle = "label";
				rect.width = outRect.width * 0.3625f;
				this.labelStyle.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
				this.labelStyle.alignment = 4;
				GUI.Label(rect, this.labelText, this.labelStyle);
				rect.x += rect.width;
				rect.width = outRect.width * 0.1575f;
				this.labelStyle = "box";
				this.labelStyle.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
				this.labelStyle.alignment = 5;
				GUI.Label(rect, this.slider.Value.ToString("F0"), this.labelStyle);
				rect.x += rect.width + outRect.width * 0.005f;
				rect.width = outRect.width * 0.4f;
				rect.y += (float)PixelValuesCM3D2.PropPx(4);
				this.slider.Draw(rect);
				rect.y -= (float)PixelValuesCM3D2.PropPx(4);
				rect.x += rect.width;
				if (this.pinEnabled)
				{
					rect.width = outRect.width * 0.075f;
					rect.y -= (float)PixelValuesCM3D2.PropPx(2);
					this.Pin = GUI.Toggle(rect, this.Pin, "");
				}
			}

			// Token: 0x060000DF RID: 223 RVA: 0x0000D43E File Offset: 0x0000C43E
			public override void Resize()
			{
				this.Resize(false);
			}

			// Token: 0x060000E0 RID: 224 RVA: 0x0000D449 File Offset: 0x0000C449
			public override void Resize(bool broadCast)
			{
				this.rect.height = (float)PixelValuesCM3D2.Line(this.lineHeightPV);
			}

			// Token: 0x040000B9 RID: 185
			private HSlider slider;

			// Token: 0x040000BA RID: 186
			private string lineHeightPV = "C1";

			// Token: 0x040000BB RID: 187
			private string fontSizePV = "C1";

			// Token: 0x040000BC RID: 188
			private GUIStyle labelStyle = "label";

			// Token: 0x040000BD RID: 189
			private string labelText = "";

			// Token: 0x040000BE RID: 190
			private bool pinEnabled = false;

			// Token: 0x040000BF RID: 191
			public float Default;

			// Token: 0x040000C0 RID: 192
			public bool Pin;
		}

		// Token: 0x0200000A RID: 10
		private class YotogiToggle : Element
		{
			// Token: 0x17000010 RID: 16
			// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000D464 File Offset: 0x0000C464
			// (set) Token: 0x060000E2 RID: 226 RVA: 0x0000D481 File Offset: 0x0000C481
			public bool Value
			{
				get
				{
					return this.toggle.Value;
				}
				set
				{
					this.toggle.Value = value;
				}
			}

			// Token: 0x060000E3 RID: 227 RVA: 0x0000D494 File Offset: 0x0000C494
			public YotogiToggle(string name, bool def, string text, EventHandler<ToggleEventArgs> onChange) : base(name, new Rect(-1f, -1f, -1f, 0f))
			{
				this.toggle = new Toggle(name + ":toggle", this.rect, def, text, onChange);
				this.val = def;
				this.LabelText = text;
				this.Resize();
			}

			// Token: 0x060000E4 RID: 228 RVA: 0x0000D530 File Offset: 0x0000C530
			public override void Draw(Rect outRect)
			{
				Rect rect = outRect;
				rect.width = outRect.width * 0.5f;
				this.labelStyle.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
				this.labelStyle.alignment = 3;
				GUI.Label(rect, this.LabelText, this.labelStyle);
				rect.x += rect.width;
				rect.width = outRect.width * 0.5f;
				this.toggle.Style.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
				this.toggle.Style.alignment = 3;
				this.toggle.Style.normal.textColor = this.toggleColor(this.toggle.Value);
				this.toggle.Style.hover.textColor = this.toggleColor(this.toggle.Value);
				this.toggle.Content.text = this.toggleText(this.toggle.Value);
				rect.y -= (float)PixelValuesCM3D2.PropPx(2);
				this.toggle.Draw(rect);
			}

			// Token: 0x060000E5 RID: 229 RVA: 0x0000D678 File Offset: 0x0000C678
			public override void Resize()
			{
				this.Resize(false);
			}

			// Token: 0x060000E6 RID: 230 RVA: 0x0000D683 File Offset: 0x0000C683
			public override void Resize(bool broadCast)
			{
				this.rect.height = (float)PixelValuesCM3D2.Line(this.lineHeightPV);
			}

			// Token: 0x060000E7 RID: 231 RVA: 0x0000D6A0 File Offset: 0x0000C6A0
			private Color toggleColor(bool b)
			{
				return b ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0.2f, 0.2f, 1f);
			}

			// Token: 0x060000E8 RID: 232 RVA: 0x0000D6EC File Offset: 0x0000C6EC
			private string toggleText(bool b)
			{
				return b ? "Enabled" : "Disabled";
			}

			// Token: 0x040000C1 RID: 193
			private bool val;

			// Token: 0x040000C2 RID: 194
			private Toggle toggle;

			// Token: 0x040000C3 RID: 195
			private GUIStyle labelStyle = "label";

			// Token: 0x040000C4 RID: 196
			private GUIStyle toggleStyle = "toggle";

			// Token: 0x040000C5 RID: 197
			private string lineHeightPV = "C1";

			// Token: 0x040000C6 RID: 198
			private string fontSizePV = "C1";

			// Token: 0x040000C7 RID: 199
			public string LabelText;
		}

		// Token: 0x0200000B RID: 11
		private class YotogiButtonGrid : Element
		{
			// Token: 0x14000003 RID: 3
			// (add) Token: 0x060000E9 RID: 233 RVA: 0x0000D70D File Offset: 0x0000C70D
			// (remove) Token: 0x060000EA RID: 234 RVA: 0x0000D726 File Offset: 0x0000C726
			public event EventHandler<ButtonEventArgs> OnClick;

			// Token: 0x060000EB RID: 235 RVA: 0x0000D740 File Offset: 0x0000C740
			public YotogiButtonGrid(string name, string[] buttonNames, EventHandler<ButtonEventArgs> _onClick, int row, bool tabEnabled) : base(name, new Rect(-1f, -1f, -1f, 0f))
			{
				this.buttonNames = buttonNames;
				this.OnClick = (EventHandler<ButtonEventArgs>)Delegate.Combine(this.OnClick, _onClick);
				this.viewRow = row;
				this.tabEnabled = tabEnabled;
				if (tabEnabled)
				{
					this.selectButton = new SelectButton[]
					{
						new SelectButton("SelectButton:Cheek", this.rect, new string[]
						{
							"頬０",
							"頬１",
							"頬２",
							"頬３"
						}, new EventHandler<SelectEventArgs>(this.OnSelectButtonFaceBlend)),
						new SelectButton("SelectButton:Tear", this.rect, new string[]
						{
							"涙０",
							"涙１",
							"涙２",
							"涙３"
						}, new EventHandler<SelectEventArgs>(this.OnSelectButtonFaceBlend))
					};
					this.onChangeTab(0);
				}
				this.Resize();
			}

			// Token: 0x060000EC RID: 236 RVA: 0x0000D8EC File Offset: 0x0000C8EC
			public override void Draw(Rect outRect)
			{
				int num = PixelValuesCM3D2.PropPx(this.spacerPx);
				int num2 = this.buttonNames.Length;
				int num3 = PixelValuesCM3D2.Line(this.lineHeightPV) + PixelValuesCM3D2.PropPx(3);
				int num4 = (int)Math.Ceiling((double)num2 / (double)this.columns);
				GUI.BeginGroup(outRect);
				Rect rect;
				rect..ctor(0f, 0f, outRect.width, (float)PixelValuesCM3D2.Line(this.lineHeightPV));
				if (this.tabEnabled)
				{
					rect.width = outRect.width * 0.3f;
					this.toggleStyle.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
					this.toggleStyle.alignment = 3;
					this.toggleStyle.normal.textColor = this.toggleColor(this.GirdToggle);
					this.toggleStyle.hover.textColor = this.toggleColor(this.GirdToggle);
					this.onClickDroolToggle(GUI.Toggle(rect, this.GirdToggle, "よだれ", this.toggleStyle));
					rect.x += rect.width;
					rect.width = outRect.width * 0.7f;
					this.onChangeTab(GUI.Toolbar(rect, this.tabSelected, new string[]
					{
						"頬・涙・涎",
						"全種Face"
					}, this.buttonStyle));
					rect.x = 0f;
					rect.y += rect.height + (float)PixelValuesCM3D2.PropPx(3);
					rect.width = outRect.width;
				}
				if (!this.tabEnabled || this.tabSelected == 1)
				{
					Rect rect2;
					rect2..ctor(rect.x, rect.y, rect.width, outRect.height - (float)(this.tabEnabled ? num3 : 0));
					Rect rect3;
					rect3..ctor(0f, 0f, outRect.width - (float)PixelValuesCM3D2.Sys_("HScrollBar.Width") - (float)num, (float)(PixelValuesCM3D2.Line(this.lineHeightPV) * num4 + num * (num4 / this.rowPerSpacer)));
					this.scrollViewVector = GUI.BeginScrollView(rect2, this.scrollViewVector, rect3, false, true);
					Rect rect4;
					rect4..ctor(0f, 0f, rect3.width / (float)this.columns, (float)PixelValuesCM3D2.Line(this.lineHeightPV));
					int num5 = 1;
					int num6 = 1;
					foreach (string text in this.buttonNames)
					{
						this.onClick(GUI.Button(rect4, text), text);
						if (this.columns > 0 && num6 == this.columns)
						{
							rect4.x = 0f;
							rect4.y += rect4.height;
							if (this.rowPerSpacer > 0 && num5 % this.rowPerSpacer == 0)
							{
								rect4.y += (float)num;
							}
							num5++;
							num6 = 1;
						}
						else
						{
							rect4.x += rect4.width;
							if (this.colPerSpacer > 0 && num6 % this.colPerSpacer == 0)
							{
								rect4.x += (float)num;
							}
							num6++;
						}
					}
					GUI.EndScrollView();
				}
				else if (this.tabSelected == 0)
				{
					this.selectButton[0].Draw(rect);
					rect.y += rect.height;
					this.selectButton[1].Draw(rect);
				}
				GUI.EndGroup();
			}

			// Token: 0x060000ED RID: 237 RVA: 0x0000DCEF File Offset: 0x0000CCEF
			public override void Resize()
			{
				this.Resize(false);
			}

			// Token: 0x060000EE RID: 238 RVA: 0x0000DCFC File Offset: 0x0000CCFC
			public override void Resize(bool broadCast)
			{
				int num = PixelValuesCM3D2.PropPx(this.spacerPx);
				int num2 = PixelValuesCM3D2.Line(this.lineHeightPV) + PixelValuesCM3D2.PropPx(3);
				if (!this.tabEnabled)
				{
					this.rect.height = (float)(PixelValuesCM3D2.Line(this.lineHeightPV) * this.viewRow + num * (this.viewRow / this.rowPerSpacer));
				}
				else if (this.tabSelected == 0)
				{
					this.rect.height = (float)(num2 + PixelValuesCM3D2.Line(this.lineHeightPV) * 2);
				}
				else if (this.tabSelected == 1)
				{
					this.rect.height = (float)(num2 + PixelValuesCM3D2.Line(this.lineHeightPV) * this.viewRow + num * (this.viewRow / this.rowPerSpacer));
				}
				if (!broadCast)
				{
					this.notifyParent(true, false);
				}
			}

			// Token: 0x060000EF RID: 239 RVA: 0x0000DDE4 File Offset: 0x0000CDE4
			public void OnSelectButtonFaceBlend(object sb, SelectEventArgs args)
			{
				if (((AddChikuwaSupporter.YotogiPanel)this.Parent).Enabled)
				{
					string name = args.Name;
					string text = args.ButtonName;
					if (name == "SelectButton:Cheek")
					{
						text += this.selectButton[1].Value;
					}
					else if (name == "SelectButton:Tear")
					{
						text = this.selectButton[0].Value + text;
					}
					if (this.GirdToggle)
					{
						text += "よだれ";
					}
					this.OnClick(this, new ButtonEventArgs(this.name, text));
				}
			}

			// Token: 0x060000F0 RID: 240 RVA: 0x0000DE9C File Offset: 0x0000CE9C
			private void onClickDroolToggle(bool b)
			{
				if (b != this.GirdToggle)
				{
					string buttonName = this.selectButton[0].Value + this.selectButton[1].Value + (b ? "よだれ" : "");
					this.OnClick(this, new ButtonEventArgs(this.name, buttonName));
					this.GirdToggle = b;
				}
			}

			// Token: 0x060000F1 RID: 241 RVA: 0x0000DF08 File Offset: 0x0000CF08
			private void onChangeTab(int i)
			{
				if (i != this.tabSelected)
				{
					this.tabSelected = i;
					this.Resize();
				}
			}

			// Token: 0x060000F2 RID: 242 RVA: 0x0000DF34 File Offset: 0x0000CF34
			private void onClick(bool click, string s)
			{
				if (click)
				{
					this.OnClick(this, new ButtonEventArgs(this.name, s));
				}
			}

			// Token: 0x060000F3 RID: 243 RVA: 0x0000DF64 File Offset: 0x0000CF64
			private Color toggleColor(bool b)
			{
				return b ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 0.2f, 0.2f, 1f);
			}

			// Token: 0x040000C8 RID: 200
			private string[] buttonNames;

			// Token: 0x040000C9 RID: 201
			private GUIStyle labelStyle = "box";

			// Token: 0x040000CA RID: 202
			private GUIStyle toggleStyle = "toggle";

			// Token: 0x040000CB RID: 203
			private GUIStyle buttonStyle = "button";

			// Token: 0x040000CC RID: 204
			private string lineHeightPV = "C1";

			// Token: 0x040000CD RID: 205
			private string fontSizePV = "C1";

			// Token: 0x040000CE RID: 206
			private int viewRow = 6;

			// Token: 0x040000CF RID: 207
			private int columns = 2;

			// Token: 0x040000D0 RID: 208
			private int spacerPx = 5;

			// Token: 0x040000D1 RID: 209
			private int rowPerSpacer = 3;

			// Token: 0x040000D2 RID: 210
			private int colPerSpacer = -1;

			// Token: 0x040000D3 RID: 211
			private bool tabEnabled = false;

			// Token: 0x040000D4 RID: 212
			private int tabSelected = -1;

			// Token: 0x040000D5 RID: 213
			private Vector2 scrollViewVector = Vector2.zero;

			// Token: 0x040000D6 RID: 214
			private SelectButton[] selectButton;

			// Token: 0x040000D7 RID: 215
			public bool GirdToggle = false;

			// Token: 0x040000D8 RID: 216
			public string GirdLabelText = "";
		}

		// Token: 0x0200000C RID: 12
		private class YotogiLineSelect : Element
		{
			// Token: 0x17000011 RID: 17
			// (get) Token: 0x060000F4 RID: 244 RVA: 0x0000DFB0 File Offset: 0x0000CFB0
			public int CurrentIndex
			{
				get
				{
					return this.currentIndex;
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x060000F5 RID: 245 RVA: 0x0000DFC8 File Offset: 0x0000CFC8
			public string CurrentName
			{
				get
				{
					return this.names[this.currentIndex];
				}
			}

			// Token: 0x14000004 RID: 4
			// (add) Token: 0x060000F6 RID: 246 RVA: 0x0000DFE7 File Offset: 0x0000CFE7
			// (remove) Token: 0x060000F7 RID: 247 RVA: 0x0000E000 File Offset: 0x0000D000
			public event EventHandler<ButtonEventArgs> OnClick;

			// Token: 0x060000F8 RID: 248 RVA: 0x0000E01C File Offset: 0x0000D01C
			public YotogiLineSelect(string name, string _label, string[] _names, int def, EventHandler<ButtonEventArgs> _onClick) : base(name, new Rect(-1f, -1f, -1f, 0f))
			{
				this.label = _label;
				this.names = new string[_names.Length];
				Array.Copy(_names, this.names, _names.Length);
				this.currentIndex = def;
				this.OnClick = (EventHandler<ButtonEventArgs>)Delegate.Combine(this.OnClick, _onClick);
				this.Resize();
			}

			// Token: 0x060000F9 RID: 249 RVA: 0x0000E0D8 File Offset: 0x0000D0D8
			public override void Draw(Rect outRect)
			{
				Rect rect = outRect;
				int num = PixelValuesCM3D2.Font(this.fontSizePV);
				rect.width = outRect.width * 0.125f;
				this.buttonStyle.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
				this.labelStyle.alignment = 4;
				this.onClick(GUI.Button(rect, "<"), -1);
				rect.x += rect.width + outRect.width * 0.025f;
				rect.width = outRect.width * 0.7f;
				this.labelStyle = "box";
				this.labelStyle.alignment = 4;
				GUI.Label(rect, this.names[this.currentIndex], this.labelStyle);
				rect.x += rect.width + outRect.width * 0.025f;
				rect.width = outRect.width * 0.125f;
				this.buttonStyle.fontSize = PixelValuesCM3D2.Font(this.fontSizePV);
				this.buttonStyle.alignment = 4;
				this.onClick(GUI.Button(rect, ">"), 1);
			}

			// Token: 0x060000FA RID: 250 RVA: 0x0000E220 File Offset: 0x0000D220
			public override void Resize(bool bc)
			{
				this.rect.height = (float)PixelValuesCM3D2.Line(this.heightPV);
				if (!bc)
				{
					this.notifyParent(true, false);
				}
			}

			// Token: 0x060000FB RID: 251 RVA: 0x0000E254 File Offset: 0x0000D254
			private void onClick(bool click, int di)
			{
				if (click)
				{
					if ((di < 0 && this.currentIndex > 0) || (di > 0 && this.currentIndex < this.names.Length - 1))
					{
						this.currentIndex += di;
						this.OnClick(this, new ButtonEventArgs(this.name, this.names[this.currentIndex]));
					}
				}
			}

			// Token: 0x040000DA RID: 218
			private string label;

			// Token: 0x040000DB RID: 219
			private string[] names;

			// Token: 0x040000DC RID: 220
			private int currentIndex = 0;

			// Token: 0x040000DD RID: 221
			private GUIStyle labelStyle = "label";

			// Token: 0x040000DE RID: 222
			private GUIStyle buttonStyle = "button";

			// Token: 0x040000DF RID: 223
			private string heightPV = "C1";

			// Token: 0x040000E0 RID: 224
			private string fontSizePV = "C1";
		}

		// Token: 0x0200000D RID: 13
		private class PlayAnime
		{
			// Token: 0x17000013 RID: 19
			// (get) Token: 0x060000FC RID: 252 RVA: 0x0000E2D4 File Offset: 0x0000D2D4
			public float progress
			{
				get
				{
					return (this.passedTime - this.startTime) / (this.finishTime - this.startTime);
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x060000FD RID: 253 RVA: 0x0000E304 File Offset: 0x0000D304
			public string Key
			{
				get
				{
					return this.Name.Split(new char[]
					{
						'.'
					})[0];
				}
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x060000FE RID: 254 RVA: 0x0000E330 File Offset: 0x0000D330
			public bool NowPlaying
			{
				get
				{
					return this.play && this.passedTime < this.finishTime;
				}
			}

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x060000FF RID: 255 RVA: 0x0000E35C File Offset: 0x0000D35C
			public bool SetterExist
			{
				get
				{
					return (this.num == 1) ? (!AddChikuwaSupporter.IsNull<Action<float>>(this.setValue0)) : (!AddChikuwaSupporter.IsNull<Action<float[]>>(this.setValue));
				}
			}

			// Token: 0x06000100 RID: 256 RVA: 0x0000E395 File Offset: 0x0000D395
			public PlayAnime(string name, int n, float st, float ft) : this(name, n, st, ft, AddChikuwaSupporter.PlayAnime.Formula.Linear)
			{
			}

			// Token: 0x06000101 RID: 257 RVA: 0x0000E3A8 File Offset: 0x0000D3A8
			public PlayAnime(string name, int n, float st, float ft, AddChikuwaSupporter.PlayAnime.Formula t)
			{
				this.Name = name;
				this.num = n;
				this.value = new float[n];
				this.vFrom = new float[n];
				this.vTo = new float[n];
				this.startTime = st;
				this.finishTime = ft;
				this.type = t;
			}

			// Token: 0x06000102 RID: 258 RVA: 0x0000E440 File Offset: 0x0000D440
			public bool IsKye(string s)
			{
				return s == this.Key;
			}

			// Token: 0x06000103 RID: 259 RVA: 0x0000E460 File Offset: 0x0000D460
			public bool Contains(string s)
			{
				return this.Name.Contains(s);
			}

			// Token: 0x06000104 RID: 260 RVA: 0x0000E47E File Offset: 0x0000D47E
			public void SetFrom(float vform)
			{
				this.vFrom[0] = vform;
			}

			// Token: 0x06000105 RID: 261 RVA: 0x0000E48A File Offset: 0x0000D48A
			public void SetTo(float vto)
			{
				this.vTo[0] = vto;
			}

			// Token: 0x06000106 RID: 262 RVA: 0x0000E496 File Offset: 0x0000D496
			public void SetSetter(Action<float> func)
			{
				this.setValue0 = func;
			}

			// Token: 0x06000107 RID: 263 RVA: 0x0000E4A0 File Offset: 0x0000D4A0
			public void Set(float vform, float vto)
			{
				this.SetFrom(vform);
				this.SetTo(vto);
			}

			// Token: 0x06000108 RID: 264 RVA: 0x0000E4B4 File Offset: 0x0000D4B4
			public void SetFrom(float[] vform)
			{
				if (vform.Length == this.num)
				{
					Array.Copy(vform, this.vFrom, this.num);
				}
			}

			// Token: 0x06000109 RID: 265 RVA: 0x0000E4E8 File Offset: 0x0000D4E8
			public void SetTo(float[] vto)
			{
				if (vto.Length == this.num)
				{
					Array.Copy(vto, this.vTo, this.num);
				}
			}

			// Token: 0x0600010A RID: 266 RVA: 0x0000E51B File Offset: 0x0000D51B
			public void SetSetter(Action<float[]> func)
			{
				this.setValue = func;
			}

			// Token: 0x0600010B RID: 267 RVA: 0x0000E525 File Offset: 0x0000D525
			public void Set(float[] vform, float[] vto)
			{
				this.SetFrom(vform);
				this.SetTo(vto);
			}

			// Token: 0x0600010C RID: 268 RVA: 0x0000E538 File Offset: 0x0000D538
			public void Play()
			{
				if (this.SetterExist)
				{
					this.passedTime = 0f;
					this.play = true;
				}
			}

			// Token: 0x0600010D RID: 269 RVA: 0x0000E567 File Offset: 0x0000D567
			public void Play(float vform, float vto)
			{
				this.Set(vform, vto);
				this.Play();
			}

			// Token: 0x0600010E RID: 270 RVA: 0x0000E57A File Offset: 0x0000D57A
			public void Play(float[] vform, float[] vto)
			{
				this.Set(vform, vto);
				this.Play();
			}

			// Token: 0x0600010F RID: 271 RVA: 0x0000E58D File Offset: 0x0000D58D
			public void Stop()
			{
				this.play = false;
			}

			// Token: 0x06000110 RID: 272 RVA: 0x0000E598 File Offset: 0x0000D598
			public void Update()
			{
				if (this.play)
				{
					bool flag = false;
					for (int i = 0; i < this.num; i++)
					{
						if (this.vFrom[i] != this.vTo[i])
						{
							if (this.passedTime >= this.finishTime)
							{
								this.Stop();
							}
							else if (this.passedTime >= this.startTime)
							{
								switch (this.type)
								{
								case AddChikuwaSupporter.PlayAnime.Formula.Linear:
									this.value[i] = this.vFrom[i] + (this.vTo[i] - this.vFrom[i]) * this.progress;
									flag = true;
									break;
								case AddChikuwaSupporter.PlayAnime.Formula.Quadratic:
									this.value[i] = this.vFrom[i] + (this.vTo[i] - this.vFrom[i]) * Mathf.Pow(this.progress, 2f);
									flag = true;
									break;
								case AddChikuwaSupporter.PlayAnime.Formula.Convulsion:
								{
									float num = Mathf.Pow(this.progress + 0.05f * Random.value, 2f) * 2f * 3.1415927f * 6f;
									this.value[i] = (this.vTo[i] - this.vFrom[i]) * Mathf.Clamp(Mathf.Clamp(Mathf.Pow((Mathf.Cos(num - 1.5707964f) + 1f) / 2f, 3f) * Mathf.Pow(1f - this.progress, 2f) * 4f, 0f, 1f) + Mathf.Sin(num * 3f) * 0.1f * Mathf.Pow(1f - this.progress, 3f), 0f, 1f);
									if (this.progress < 0.03f)
									{
										this.value[i] *= Mathf.Pow(1f - (0.03f - this.progress) * 33f, 2f);
									}
									flag = true;
									break;
								}
								}
							}
						}
					}
					if (flag)
					{
						if (this.num == 1)
						{
							this.setValue0(this.value[0]);
						}
						else
						{
							this.setValue(this.value);
						}
					}
				}
				this.passedTime += Time.deltaTime;
			}

			// Token: 0x040000E2 RID: 226
			private float[] value;

			// Token: 0x040000E3 RID: 227
			private float[] vFrom;

			// Token: 0x040000E4 RID: 228
			private float[] vTo;

			// Token: 0x040000E5 RID: 229
			private AddChikuwaSupporter.PlayAnime.Formula type;

			// Token: 0x040000E6 RID: 230
			private int num;

			// Token: 0x040000E7 RID: 231
			private bool play = false;

			// Token: 0x040000E8 RID: 232
			private float passedTime = 0f;

			// Token: 0x040000E9 RID: 233
			private float startTime = 0f;

			// Token: 0x040000EA RID: 234
			private float finishTime = 0f;

			// Token: 0x040000EB RID: 235
			private Action<float> setValue0 = null;

			// Token: 0x040000EC RID: 236
			private Action<float[]> setValue = null;

			// Token: 0x040000ED RID: 237
			public string Name;

			// Token: 0x0200000E RID: 14
			public enum Formula
			{
				// Token: 0x040000EF RID: 239
				Linear,
				// Token: 0x040000F0 RID: 240
				Quadratic,
				// Token: 0x040000F1 RID: 241
				Convulsion
			}
		}
	}
}

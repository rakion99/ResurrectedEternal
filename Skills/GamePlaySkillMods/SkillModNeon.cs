﻿using RRFull.BaseObjects;
using RRFull.ClientObjects;
using RRFull.Memory;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RRFull.Skills
{
    class SkillModNeon : SkillMod
    {
        private DateTime _lastGlowUpdate = DateTime.Now;
        private TimeSpan _Interval = TimeSpan.FromMilliseconds(33);

        private byte m_dwOrignialValue = 0x74;
        private byte m_dwForcedValue = 0xEB;

        public SkillModNeon(Engine engine, Client client) : base(engine, client)
        {
            //read the original byte on glow enforcement.
            //m_dwOrignialValue = MemoryLoader.instance.Reader.Read<byte>(Client.ModuleAddress + g_Globals.Offset.dwForceGlow);
        }

        public override void AfterUpdate()
        {
            base.AfterUpdate();
        }

        public override void Before()
        {
            base.Before();

        }
        private bool CanProcess()
        {
            if (ClientModus == Events.Modus.leaguemode
                || ClientModus == Events.Modus.streammode
                || ClientModus == Events.Modus.streammodefull)
                return false;
            return true;
        }
        //private void Write(BasePlayer P)
        //{
        //    WriteGlowObject(P);
        //}

        //private void WriteGlowObject(BasePlayer p)
        //{
        //    var _glowObj = Client.GetGlowObject(p.m_iGlowIndex);
        //    SetGlowStruct(_glowObj, Generators.GetColorBySetting(Config, p.Team, p.IsVisible, p.m_bGunGameImmunity));
        //    Client.WriteGlowObject(_glowObj, p.m_iGlowIndex);
        //}

        private void WriteByIndex(m_dwGlowObject glowObject, int idx, SharpDX.Color color, bool visible)
        {


            SetGlowStruct(ref glowObject, color, visible);
            Write(glowObject, idx);
        }

        private void Write(m_dwGlowObject glowObject, int idx)
        {
            //IntPtr _addr = Client.m_dwGlowManager.m_pGlowArray + (0x38 * idx);

            //ILog.AddToLog("[GLOW]Before Write", idx + " " + glowObject.dwEntity.ToString());
            if (idx > Client.m_dwGlowManager.m_iNumObjects)
                return;
            IntPtr _addr = Client.m_dwGlowManager.m_pGlowArray + (idx * 0x38);

            var verify = MemoryLoader.instance.Reader.Read<IntPtr>(_addr);
            //ILog.AddToLog("[GLOW]Before Write - Verify ", idx + " " + verify.ToString());
            if (verify == IntPtr.Zero || glowObject.dwEntity != verify)
                return;
            //MemoryLoader.instance.Reader.Write<m_dwGlowObject>(_addr, glowObject);
            MemoryLoader.instance.Reader.Write<Vector3>(_addr + 0x04,
                new Vector3(glowObject.R,
                glowObject.G,
                glowObject.B
               ));
            MemoryLoader.instance.Reader.Write<float>(_addr + 0x10, glowObject.A);
            MemoryLoader.instance.Reader.Write<float>(_addr + 0x18, glowObject.m_flGlowAlphaFunctionOfMaxVelocity);
            MemoryLoader.instance.Reader.Write<float>(_addr + 0x1C, glowObject.m_flGlowAlphaMax);
            MemoryLoader.instance.Reader.Write<bool>(_addr + 0x24, glowObject.bRenderWhenOccluded);
            MemoryLoader.instance.Reader.Write<bool>(_addr + 0x25, glowObject.bRenderWhenUnoccluded);
            MemoryLoader.instance.Reader.Write<bool>(_addr + 0x26, glowObject.bFullBloom);
            MemoryLoader.instance.Reader.Write<byte>(_addr + 0x2C, glowObject.m_nRenderStyle);
            //ILog.AddToLog("[GLOW]After Write", "SUCCESS");
            //MemoryLoader.instance.Reader.Write<m_dwGlowObject_NOENT>(_addr, glowObject); //+ 0x4 ?
        }

        private void UnGlow(m_dwGlowObject glowObject, int idx)
        {
            if (glowObject.R == 0f && glowObject.G == 0f && glowObject.B == 0f && glowObject.A == 0f)
                return;

            if (idx > Client.m_dwGlowManager.m_iNumObjects)
                return;
            IntPtr _addr = Client.m_dwGlowManager.m_pGlowArray + (idx * 0x38);

            var verify = MemoryLoader.instance.Reader.Read<IntPtr>(_addr);
            //ILog.AddToLog("[GLOW]Before Write - Verify ", idx + " " + verify.ToString());
            if (verify == IntPtr.Zero || glowObject.dwEntity != verify)
                return;
            MemoryLoader.instance.Reader.Write<Vector3>(_addr + 0x04,
                new Vector3(0f,
                0f,
                0f
               ));
            MemoryLoader.instance.Reader.Write<float>(_addr + 0x10, 0f);
        }

        bool ShouldUpdate(m_dwGlowObject s, SharpDX.Color c)
        {
            if (!EngineMath.AlmostEquals(s.R, (c.R / 255f), 0.0001f))
                return true;
            if (!EngineMath.AlmostEquals(s.G, (c.G / 255f), 0.0001f))
                return true;
            if (!EngineMath.AlmostEquals(s.B, (c.B / 255f), 0.0001f))
                return true;
            if (!EngineMath.AlmostEquals(s.A, (c.A / 255f), 0.0001f))
                return true;
            if (s.m_nRenderStyle != Convert.ToByte(Config.NeonConfig.GlowStyle.Value))
                return true;
            if (s.bFullBloom != (bool)Config.NeonConfig.FullBloom.Value)
                return true;
            if (s.m_flGlowAlphaFunctionOfMaxVelocity != Convert.ToSingle(Config.NeonConfig.MaxVelocity.Value))
                return true;
            if (s.m_flGlowAlphaMax != (float)Config.NeonConfig.AlphaMax.Value)
                return true;

            return false;
        }

        void SetGlowStruct(ref m_dwGlowObject s, SharpDX.Color c, bool visible = false)
        {
            //m_dwGlowObject_NOENT _noEnt = new m_dwGlowObject_NOENT();
            //_noEnt.Color = new float[]
            //{
            //    c.R / 255f,
            //    c.G / 255f,
            //    c.B / 255f,
            //    c.A / 255f

            //};
            s.R = c.R / 255f;
            s.G = c.G / 255f;
            s.B = c.B / 255f;
            s.A = c.A / 255f;
            s.m_nRenderStyle = GetGlowStyle(visible);
            s.bRenderWhenOccluded = true;
            s.bRenderWhenUnoccluded = false;
            s.bFullBloom = (bool)Config.NeonConfig.FullBloom.Value;
            s.m_flGlowAlphaFunctionOfMaxVelocity = Convert.ToSingle(Config.NeonConfig.MaxVelocity.Value);
            s.m_flGlowAlphaMax = (float)Config.NeonConfig.AlphaMax.Value;
            //return _noEnt;
            //s.m_flGlowPulseOverdrive = (float)Config.NeonConfig.PulseOverDrive.Value;
        }

        private byte GetGlowStyle(bool visible)
        {
            if ((GlowRenderStyle_t)Config.NeonConfig.GlowStyleWhenVisible.Value == GlowRenderStyle_t.GLOWRENDERSTYLE_DEFAULT)
                return Convert.ToByte(Config.NeonConfig.GlowStyle.Value);
            if (visible)
                return Convert.ToByte(Config.NeonConfig.GlowStyleWhenVisible.Value);
            else
                return Convert.ToByte(Config.NeonConfig.GlowStyle.Value);
        }

        //private void SetGlowStruct(ref GlowObjectDefinition_t s, SharpDX.Color c, bool MatrixGlowOnSpot = false)
        //{

        //    s.fR = c.R / 255f;
        //    s.fG = c.G / 255f;
        //    s.fB = c.B / 255f;
        //    s.fAlpha = c.A / 255f;
        //    s.m_bInnerGlow = (bool)Config.NeonConfig.NeonInnerGlow.Value;
        //    s.bRenderWhenOccluded = true;
        //    s.bRenderWhenUnoccluded = false;
        //    s.bFullBloomRender = (bool)Config.NeonConfig.FullBloom.Value;
        //}


        private void UpdateGlows()
        {

            var _glowManager = Client.m_dwGlowManager;
            //m_dwGlowObject[] glowObjects = new m_dwGlowObject[_glowManager.m_iNumObjects];
            var _num = _glowManager.m_iNumObjects;
            var _ptr = _glowManager.m_pGlowArray;

            //var _r = MemoryLoader.instance.Reader.Read<m_dwGlowObject>(_ptr, glowObjects.Length);

            for (int i = 0; i < _num; i++)
            {
                var _next = MemoryLoader.instance.Reader.Read<m_dwGlowObject>(_ptr + (i * 0x38));
                if (_next.dwEntity == IntPtr.Zero) continue;
                var _ent = Client.GetEntityByAddress(_next.dwEntity);
                if (_ent == null || !_ent.IsValid || _ent.m_bDormant || _ent.m_vecOrigin == Vector3.Zero) continue;

                var _color = (Color)Config.OtherConfig.cFontColor.Value;
                bool _vis = false;
                if (_ent.ClientClass == ClientClass.CCSPlayer)
                {
                    var bp = _ent as BasePlayer;
                    _vis = bp.IsVisible;
                    if (!bp.m_bIsActive) continue;

                    switch ((TargetType)Config.NeonConfig.GlowAt.Value)
                    {

                        case TargetType.Enemy:
                            if (bp.IsFriendly(Client.LocalPlayer.Team))
                            {
                                UnGlow(_next, i);
                                continue;
                            }
                                
                            _color = Generators.GetNeonColorBySetting(Config, bp.Team, _vis, bp.m_bGunGameImmunity);
                            break;
                        case TargetType.Friendly:
                            if (!bp.IsFriendly(Client.LocalPlayer.Team))
                            {
                                UnGlow(_next, i);
                                continue;
                            }
                            _color = Generators.GetNeonColorBySetting(Config, bp.Team, _vis, bp.m_bGunGameImmunity);
                            break;
                        case TargetType.All:
                        default:
                            _color = Generators.GetNeonColorBySetting(Config, bp.Team, _vis, bp.m_bGunGameImmunity);
                            break;
                    }


                }
                else if (Generators.IsGrenade(_ent.ClientClass) && (bool)Config.NeonConfig.NeonGlowGrenades.Value)
                {
                   // var _t = _ent as BaseGrenade;
                    _color = (Color)Config.OtherConfig.cGrenadeColor.Value;
                }
                else if (Generators.IsProjectile(_ent.ClientClass) && (bool)Config.NeonConfig.NeonGlowProjectiles.Value)
                {
                   // var _t = _ent as ProjectileEntity;
                    _color = (Color)Config.OtherConfig.cProjectileColor.Value;
                }
                else if (Generators.IsWeapon(_ent.ClientClass) && (bool)Config.NeonConfig.NeonGlowWeapons.Value)
                {
                    //var _t = _ent as BaseCombatWeapon;
                    _color = (Color)Config.OtherConfig.cWeaponColor.Value;
                }
                else if (_ent.ClientClass == ClientClass.CChicken && (bool)Config.NeonConfig.NeonGlowChickens.Value)
                {
                    _color = (Color)Config.OtherConfig.cChickenColor.Value;
                }
                else if (_ent.ClientClass == ClientClass.CEconEntity && (bool)Config.NeonConfig.NeonGlowDefuse.Value)
                {
                    _color = (Color)Config.OtherConfig.cDefuseKitColor.Value;
                }
                else if ((_ent.ClientClass == ClientClass.CC4
                    || _ent.ClientClass == ClientClass.CPlantedC4) && (bool)Config.NeonConfig.NeonGlowBomb.Value)
                {
                    _color = (Color)Config.OtherConfig.cBombColor.Value;
                }
                else
                    continue;

                if (ShouldUpdate(_next, _color))
                {
                    //ILog.AddToLog("[GLOW]Prepare Write", _ent.BaseAddress + " " + _ent.ClientClass.ToString());
                    WriteByIndex(_next, i, _color, _vis);
                }
                    
            }
        }

        public override bool Update()
        {
            //OHNE ANGST BABY
            if (!CanProcess())
                return false;
            if (!Client.UpdateModules || Client.DontGlow || !Engine.IsInGame || Client.LocalPlayer == null || !Client.LocalPlayer.IsValid || Client.LocalPlayer.m_bIsSpectator )
                return false;

            if (DateTime.Now - _lastGlowUpdate < _Interval)
                return false;


            if ((bool)Config.NeonConfig.Enable.Value)
            {
                EnableForce();
                UpdateGlows();
                _lastGlowUpdate = DateTime.Now;

                //var _players = Filter.GetActivePlayers((TargetType)Config.VisualConfig.Type.Value);

                //if (_players == null || _players.Count == 0)
                //    return false;

                //foreach (var item in _players)
                //{
                //    if (!item.m_bIsActive) continue;
                //    Write(item);
                //}
            }
            else
            {
                DisableForce();
            }
            return true;
        }
        //private void Enforce()
        //{
        //    var _r = MemoryLoader.instance.Reader.Read<byte>(ModuleAddress + g_Globals.Offset.dwForceGlow);
        //    if (_r == 0xEB)
        //    {
        //        PushCip(string.Format("[{0}]Glow steady...", "0x" + (ModuleAddress + g_Globals.Offset.dwForceGlow).ToString("X")), 5f, SharpDX.Color.Green);
        //        return;
        //    }

        //    MemoryLoader.instance.Reader.Write<byte>(ModuleAddress + g_Globals.Offset.dwForceGlow, 0xEB);
        //    PushCip(string.Format("[{0}]Glow no steady...", "0x" + (ModuleAddress + g_Globals.Offset.dwForceGlow).ToString("X")), 5f, SharpDX.Color.Green);
        //}
        private void DisableForce()
        {
            if (MemoryLoader.instance.Reader.Read<byte>(Client.ModuleAddress + g_Globals.Offset.dwForceGlow) == m_dwOrignialValue)
                return;
            MemoryLoader.instance.Reader.Write<byte>(Client.ModuleAddress + g_Globals.Offset.dwForceGlow, m_dwOrignialValue);
        }

        private void EnableForce()
        {
            if (MemoryLoader.instance.Reader.Read<byte>(Client.ModuleAddress + g_Globals.Offset.dwForceGlow) == m_dwForcedValue)
                return;
            MemoryLoader.instance.Reader.Write<byte>(Client.ModuleAddress + g_Globals.Offset.dwForceGlow, m_dwForcedValue);
        }
    }
}
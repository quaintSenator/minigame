#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.1.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class AkInitSettings : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkInitSettings(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkInitSettings obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkInitSettings() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkInitSettings(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public uint uMaxNumPaths { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumPaths_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxNumPaths_get(swigCPtr); } 
  }

  public uint uCommandQueueSize { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCommandQueueSize_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCommandQueueSize_get(swigCPtr); } 
  }

  public bool bEnableGameSyncPreparation { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_bEnableGameSyncPreparation_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_bEnableGameSyncPreparation_get(swigCPtr); } 
  }

  public uint uContinuousPlaybackLookAhead { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uContinuousPlaybackLookAhead_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uContinuousPlaybackLookAhead_get(swigCPtr); } 
  }

  public uint uNumSamplesPerFrame { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uNumSamplesPerFrame_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uNumSamplesPerFrame_get(swigCPtr); } 
  }

  public uint uMonitorQueuePoolSize { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorQueuePoolSize_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMonitorQueuePoolSize_get(swigCPtr); } 
  }

  public uint uCpuMonitorQueueMaxSize { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCpuMonitorQueueMaxSize_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uCpuMonitorQueueMaxSize_get(swigCPtr); } 
  }

  public AkOutputSettings settingsMainOutput { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_settingsMainOutput_set(swigCPtr, AkOutputSettings.getCPtr(value)); } 
    get {
      global::System.IntPtr cPtr = AkSoundEnginePINVOKE.CSharp_AkInitSettings_settingsMainOutput_get(swigCPtr);
      AkOutputSettings ret = (cPtr == global::System.IntPtr.Zero) ? null : new AkOutputSettings(cPtr, false);
      return ret;
    } 
  }

  public uint uMaxHardwareTimeoutMs { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxHardwareTimeoutMs_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uMaxHardwareTimeoutMs_get(swigCPtr); } 
  }

  public bool bUseSoundBankMgrThread { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseSoundBankMgrThread_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseSoundBankMgrThread_get(swigCPtr); } 
  }

  public bool bUseLEngineThread { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseLEngineThread_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_bUseLEngineThread_get(swigCPtr); } 
  }

  public string szPluginDLLPath { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_szPluginDLLPath_set(swigCPtr, value); }  get { return AkSoundEngine.StringFromIntPtrOSString(AkSoundEnginePINVOKE.CSharp_AkInitSettings_szPluginDLLPath_get(swigCPtr)); } 
  }

  public AkFloorPlane eFloorPlane { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_eFloorPlane_set(swigCPtr, (int)value); }  get { return (AkFloorPlane)AkSoundEnginePINVOKE.CSharp_AkInitSettings_eFloorPlane_get(swigCPtr); } 
  }

  public float fGameUnitsToMeters { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_fGameUnitsToMeters_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_fGameUnitsToMeters_get(swigCPtr); } 
  }

  public uint uBankReadBufferSize { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_uBankReadBufferSize_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_uBankReadBufferSize_get(swigCPtr); } 
  }

  public float fDebugOutOfRangeLimit { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_fDebugOutOfRangeLimit_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_fDebugOutOfRangeLimit_get(swigCPtr); } 
  }

  public bool bDebugOutOfRangeCheckEnabled { set { AkSoundEnginePINVOKE.CSharp_AkInitSettings_bDebugOutOfRangeCheckEnabled_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkInitSettings_bDebugOutOfRangeCheckEnabled_get(swigCPtr); } 
  }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.2.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class AkAudioFormat : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkAudioFormat(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkAudioFormat obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkAudioFormat() {
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
          AkUnitySoundEnginePINVOKE.CSharp_delete_AkAudioFormat(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public uint uSampleRate { set { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uSampleRate_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uSampleRate_get(swigCPtr); } 
  }

  public AkChannelConfig channelConfig { set { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_channelConfig_set(swigCPtr, AkChannelConfig.getCPtr(value)); } 
    get {
      global::System.IntPtr cPtr = AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_channelConfig_get(swigCPtr);
      AkChannelConfig ret = (cPtr == global::System.IntPtr.Zero) ? null : new AkChannelConfig(cPtr, false);
      return ret;
    } 
  }

  public uint uBitsPerSample { set { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uBitsPerSample_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uBitsPerSample_get(swigCPtr); } 
  }

  public uint uBlockAlign { set { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uBlockAlign_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uBlockAlign_get(swigCPtr); } 
  }

  public uint uTypeID { set { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uTypeID_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uTypeID_get(swigCPtr); } 
  }

  public uint uInterleaveID { set { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uInterleaveID_set(swigCPtr, value); }  get { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_uInterleaveID_get(swigCPtr); } 
  }

  public uint GetNumChannels() { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_GetNumChannels(swigCPtr); }

  public uint GetBitsPerSample() { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_GetBitsPerSample(swigCPtr); }

  public uint GetBlockAlign() { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_GetBlockAlign(swigCPtr); }

  public uint GetTypeID() { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_GetTypeID(swigCPtr); }

  public uint GetInterleaveID() { return AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_GetInterleaveID(swigCPtr); }

  public void SetAll(uint in_uSampleRate, AkChannelConfig in_channelConfig, uint in_uBitsPerSample, uint in_uBlockAlign, uint in_uTypeID, uint in_uInterleaveID) { AkUnitySoundEnginePINVOKE.CSharp_AkAudioFormat_SetAll(swigCPtr, in_uSampleRate, AkChannelConfig.getCPtr(in_channelConfig), in_uBitsPerSample, in_uBlockAlign, in_uTypeID, in_uInterleaveID); }

  public AkAudioFormat() : this(AkUnitySoundEnginePINVOKE.CSharp_new_AkAudioFormat(), true) {
  }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 0.0.1
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace C4d {

using System;
using System.Runtime.InteropServices;

public class GetCustomIconData : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal GetCustomIconData(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(GetCustomIconData obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~GetCustomIconData() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_GetCustomIconData(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public GetCustomIconData() : this(C4dApiPINVOKE.new_GetCustomIconData(), true) {
  }

  public SWIGTYPE_p_IconData dat {
    set {
      C4dApiPINVOKE.GetCustomIconData_dat_set(swigCPtr, SWIGTYPE_p_IconData.getCPtr(value));
    } 
    get {
      IntPtr cPtr = C4dApiPINVOKE.GetCustomIconData_dat_get(swigCPtr);
      SWIGTYPE_p_IconData ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_IconData(cPtr, false);
      return ret;
    } 
  }

  public bool filled {
    set {
      C4dApiPINVOKE.GetCustomIconData_filled_set(swigCPtr, value);
    } 
    get {
      bool ret = C4dApiPINVOKE.GetCustomIconData_filled_get(swigCPtr);
      return ret;
    } 
  }

}

}
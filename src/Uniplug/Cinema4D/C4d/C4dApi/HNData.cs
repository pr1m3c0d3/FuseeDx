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

public class HNData : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal HNData(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(HNData obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~HNData() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          C4dApiPINVOKE.delete_HNData(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public HNData() : this(C4dApiPINVOKE.new_HNData(), true) {
  }

  public SWIGTYPE_p_LONG points {
    set {
      C4dApiPINVOKE.HNData_points_set(swigCPtr, SWIGTYPE_p_LONG.getCPtr(value));
    } 
    get {
      IntPtr cPtr = C4dApiPINVOKE.HNData_points_get(swigCPtr);
      SWIGTYPE_p_LONG ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_LONG(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_LONG polys {
    set {
      C4dApiPINVOKE.HNData_polys_set(swigCPtr, SWIGTYPE_p_LONG.getCPtr(value));
    } 
    get {
      IntPtr cPtr = C4dApiPINVOKE.HNData_polys_get(swigCPtr);
      SWIGTYPE_p_LONG ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_LONG(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_p_SReal pointweight {
    set {
      C4dApiPINVOKE.HNData_pointweight_set(swigCPtr, SWIGTYPE_p_p_SReal.getCPtr(value));
    } 
    get {
      IntPtr cPtr = C4dApiPINVOKE.HNData_pointweight_get(swigCPtr);
      SWIGTYPE_p_p_SReal ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_p_SReal(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_p_PolyWeight polyweight {
    set {
      C4dApiPINVOKE.HNData_polyweight_set(swigCPtr, SWIGTYPE_p_p_PolyWeight.getCPtr(value));
    } 
    get {
      IntPtr cPtr = C4dApiPINVOKE.HNData_polyweight_get(swigCPtr);
      SWIGTYPE_p_p_PolyWeight ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_p_PolyWeight(cPtr, false);
      return ret;
    } 
  }

  public SWIGTYPE_p_Bool changed {
    set {
      C4dApiPINVOKE.HNData_changed_set(swigCPtr, SWIGTYPE_p_Bool.getCPtr(value));
    } 
    get {
      IntPtr cPtr = C4dApiPINVOKE.HNData_changed_get(swigCPtr);
      SWIGTYPE_p_Bool ret = (cPtr == IntPtr.Zero) ? null : new SWIGTYPE_p_Bool(cPtr, false);
      return ret;
    } 
  }

}

}
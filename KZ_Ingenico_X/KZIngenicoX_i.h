

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.01.0622 */
/* at Tue Jan 19 05:14:07 2038
 */
/* Compiler settings for KZIngenicoX.idl:
    Oicf, W1, Zp8, env=Win32 (32b run), target_arch=X86 8.01.0622 
    protocol : dce , ms_ext, c_ext, robust
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
/* @@MIDL_FILE_HEADING(  ) */



/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 500
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif /* __RPCNDR_H_VERSION__ */

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __KZIngenicoX_i_h__
#define __KZIngenicoX_i_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IPos_FWD_DEFINED__
#define __IPos_FWD_DEFINED__
typedef interface IPos IPos;

#endif 	/* __IPos_FWD_DEFINED__ */


#ifndef __Pos_FWD_DEFINED__
#define __Pos_FWD_DEFINED__

#ifdef __cplusplus
typedef class Pos Pos;
#else
typedef struct Pos Pos;
#endif /* __cplusplus */

#endif 	/* __Pos_FWD_DEFINED__ */


/* header files for imported files */
#include "oaidl.h"
#include "ocidl.h"
#include "shobjidl.h"

#ifdef __cplusplus
extern "C"{
#endif 


#ifndef __IPos_INTERFACE_DEFINED__
#define __IPos_INTERFACE_DEFINED__

/* interface IPos */
/* [unique][nonextensible][dual][uuid][object] */ 


EXTERN_C const IID IID_IPos;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("24b456e7-3c05-4792-831e-7ddd4b139d8f")
    IPos : public IDispatch
    {
    public:
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE Initialize( 
            /* [in] */ BSTR dllPath,
            /* [in] */ BSTR setupPath,
            /* [retval][out] */ BYTE *result) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE StartPurchase( 
            /* [in] */ DOUBLE amount,
            /* [in] */ BSTR ECRNr,
            /* [in] */ BSTR TRNr,
            /* [retval][out] */ BYTE *result) = 0;
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE StartSettlement(
            /* [in] */ BSTR ECRNr,
            /* [in] */ BSTR TRNr,
            /* [retval][out] */ BYTE* result) = 0;
        
        virtual /* [id] */ HRESULT STDMETHODCALLTYPE GetResponce( void) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_lastError( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_LastStatMsgCode( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_LastStatMsgDescription( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_ResponseCode( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_LastErrorCode( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_PAN( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_ExpDate( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_IssuerName( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_AuthCode( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_RRN( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_LastErrorDescription( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Receipt( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_Amount( 
            /* [retval][out] */ LONG *pVal) = 0;
        
        virtual /* [id][propget] */ HRESULT STDMETHODCALLTYPE get_outString( 
            /* [retval][out] */ BSTR *pVal) = 0;
        
    };
    
    
#else 	/* C style interface */

    typedef struct IPosVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            IPos * This,
            /* [in] */ REFIID riid,
            /* [annotation][iid_is][out] */ 
            _COM_Outptr_  void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            IPos * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            IPos * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            IPos * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            IPos * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            IPos * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [range][in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            IPos * This,
            /* [annotation][in] */ 
            _In_  DISPID dispIdMember,
            /* [annotation][in] */ 
            _In_  REFIID riid,
            /* [annotation][in] */ 
            _In_  LCID lcid,
            /* [annotation][in] */ 
            _In_  WORD wFlags,
            /* [annotation][out][in] */ 
            _In_  DISPPARAMS *pDispParams,
            /* [annotation][out] */ 
            _Out_opt_  VARIANT *pVarResult,
            /* [annotation][out] */ 
            _Out_opt_  EXCEPINFO *pExcepInfo,
            /* [annotation][out] */ 
            _Out_opt_  UINT *puArgErr);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *Initialize )( 
            IPos * This,
            /* [in] */ BSTR dllPath,
            /* [in] */ BSTR setupPath,
            /* [retval][out] */ BYTE *result);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *StartPurchase )( 
            IPos * This,
            /* [in] */ DOUBLE amount,
            /* [in] */ BSTR ECRNr,
            /* [in] */ BSTR TRNr,
            /* [retval][out] */ BYTE *result);
        
        /* [id] */ HRESULT ( STDMETHODCALLTYPE *GetResponce )( 
            IPos * This);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_lastError )( 
            IPos * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LastStatMsgCode )( 
            IPos * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LastStatMsgDescription )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ResponseCode )( 
            IPos * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LastErrorCode )( 
            IPos * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_PAN )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_ExpDate )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_IssuerName )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_AuthCode )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_RRN )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_LastErrorDescription )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Receipt )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_Amount )( 
            IPos * This,
            /* [retval][out] */ LONG *pVal);
        
        /* [id][propget] */ HRESULT ( STDMETHODCALLTYPE *get_outString )( 
            IPos * This,
            /* [retval][out] */ BSTR *pVal);
        
        END_INTERFACE
    } IPosVtbl;

    interface IPos
    {
        CONST_VTBL struct IPosVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define IPos_QueryInterface(This,riid,ppvObject)	\
    ( (This)->lpVtbl -> QueryInterface(This,riid,ppvObject) ) 

#define IPos_AddRef(This)	\
    ( (This)->lpVtbl -> AddRef(This) ) 

#define IPos_Release(This)	\
    ( (This)->lpVtbl -> Release(This) ) 


#define IPos_GetTypeInfoCount(This,pctinfo)	\
    ( (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo) ) 

#define IPos_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    ( (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo) ) 

#define IPos_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    ( (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId) ) 

#define IPos_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    ( (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr) ) 


#define IPos_Initialize(This,dllPath,setupPath,result)	\
    ( (This)->lpVtbl -> Initialize(This,dllPath,setupPath,result) ) 

#define IPos_StartPurchase(This,amount,ECRNr,TRNr,result)	\
    ( (This)->lpVtbl -> StartPurchase(This,amount,ECRNr,TRNr,result) ) 

#define IPos_GetResponce(This)	\
    ( (This)->lpVtbl -> GetResponce(This) ) 

#define IPos_get_lastError(This,pVal)	\
    ( (This)->lpVtbl -> get_lastError(This,pVal) ) 

#define IPos_get_LastStatMsgCode(This,pVal)	\
    ( (This)->lpVtbl -> get_LastStatMsgCode(This,pVal) ) 

#define IPos_get_LastStatMsgDescription(This,pVal)	\
    ( (This)->lpVtbl -> get_LastStatMsgDescription(This,pVal) ) 

#define IPos_get_ResponseCode(This,pVal)	\
    ( (This)->lpVtbl -> get_ResponseCode(This,pVal) ) 

#define IPos_get_LastErrorCode(This,pVal)	\
    ( (This)->lpVtbl -> get_LastErrorCode(This,pVal) ) 

#define IPos_get_PAN(This,pVal)	\
    ( (This)->lpVtbl -> get_PAN(This,pVal) ) 

#define IPos_get_ExpDate(This,pVal)	\
    ( (This)->lpVtbl -> get_ExpDate(This,pVal) ) 

#define IPos_get_IssuerName(This,pVal)	\
    ( (This)->lpVtbl -> get_IssuerName(This,pVal) ) 

#define IPos_get_AuthCode(This,pVal)	\
    ( (This)->lpVtbl -> get_AuthCode(This,pVal) ) 

#define IPos_get_RRN(This,pVal)	\
    ( (This)->lpVtbl -> get_RRN(This,pVal) ) 

#define IPos_get_LastErrorDescription(This,pVal)	\
    ( (This)->lpVtbl -> get_LastErrorDescription(This,pVal) ) 

#define IPos_get_Receipt(This,pVal)	\
    ( (This)->lpVtbl -> get_Receipt(This,pVal) ) 

#define IPos_get_Amount(This,pVal)	\
    ( (This)->lpVtbl -> get_Amount(This,pVal) ) 

#define IPos_get_outString(This,pVal)	\
    ( (This)->lpVtbl -> get_outString(This,pVal) ) 

#endif /* COBJMACROS */


#endif 	/* C style interface */




#endif 	/* __IPos_INTERFACE_DEFINED__ */



#ifndef __KZIngenicoXLib_LIBRARY_DEFINED__
#define __KZIngenicoXLib_LIBRARY_DEFINED__

/* library KZIngenicoXLib */
/* [version][uuid] */ 


EXTERN_C const IID LIBID_KZIngenicoXLib;

EXTERN_C const CLSID CLSID_Pos;

#ifdef __cplusplus

class DECLSPEC_UUID("0028f797-1947-4d02-a6e1-b333cf020b92")
Pos;
#endif
#endif /* __KZIngenicoXLib_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

unsigned long             __RPC_USER  BSTR_UserSize(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree(     unsigned long *, BSTR * ); 

unsigned long             __RPC_USER  BSTR_UserSize64(     unsigned long *, unsigned long            , BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserMarshal64(  unsigned long *, unsigned char *, BSTR * ); 
unsigned char * __RPC_USER  BSTR_UserUnmarshal64(unsigned long *, unsigned char *, BSTR * ); 
void                      __RPC_USER  BSTR_UserFree64(     unsigned long *, BSTR * ); 

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif



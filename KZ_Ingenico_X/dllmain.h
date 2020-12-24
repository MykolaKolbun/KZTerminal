// dllmain.h : Declaration of module class.

class CKZIngenicoXModule : public ATL::CAtlDllModuleT< CKZIngenicoXModule >
{
public :
	DECLARE_LIBID(LIBID_KZIngenicoXLib)
	DECLARE_REGISTRY_APPID_RESOURCEID(IDR_KZINGENICOX, "{d89963cf-7444-4cd6-9eca-a73ba6c0e613}")
};

extern class CKZIngenicoXModule _AtlModule;

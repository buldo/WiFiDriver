namespace Rtl8812auNet.Rtl8812au;

public static class drv_types
{
    public static DvObj adapter_to_dvobj(_adapter adapter) => ((adapter).dvobj);
    public static u8 GetRegAmplifierType2G(_adapter _Adapter) => (_Adapter.registrypriv.AmplifierType_2G);
    public static u8 GetRegAmplifierType5G(_adapter _Adapter) => (_Adapter.registrypriv.AmplifierType_5G);
    public static u8 GetRegRFEType(_adapter _Adapter) => (_Adapter.registrypriv.RFE_Type);
}
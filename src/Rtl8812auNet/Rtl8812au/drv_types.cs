namespace Rtl8812auNet.Rtl8812au;

public static class drv_types
{
    public static DvObj adapter_to_dvobj(AdapterState adapterState) => ((adapterState).dvobj);
    public static u8 GetRegAmplifierType2G(AdapterState adapterState) => (adapterState.registrypriv.AmplifierType_2G);
    public static u8 GetRegAmplifierType5G(AdapterState adapterState) => (adapterState.registrypriv.AmplifierType_5G);
    public static u8 GetRegRFEType(AdapterState adapterState) => (adapterState.registrypriv.RFE_Type);
}
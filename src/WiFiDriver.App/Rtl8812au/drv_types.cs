namespace WiFiDriver.App.Rtl8812au;

public static class drv_types
{
    public static dvobj_priv adapter_to_dvobj(_adapter adapter) => ((adapter).dvobj);
    public static u8 GetRegAmplifierType2G(_adapter _Adapter) => (_Adapter.registrypriv.AmplifierType_2G);
    public static u8 GetRegAmplifierType5G(_adapter _Adapter) => (_Adapter.registrypriv.AmplifierType_5G);
    public static pwrctrl_priv adapter_to_pwrctl(_adapter adapter) => dvobj_to_pwrctl(adapter_to_dvobj((adapter)));
    public static pwrctrl_priv dvobj_to_pwrctl(dvobj_priv dvobj) => ((dvobj.pwrctl_priv));
    public static u8 GetRegRFEType(_adapter _Adapter) => (_Adapter.registrypriv.RFE_Type);
    public static registry_priv adapter_to_regsty(_adapter adapter) => dvobj_to_regsty(adapter_to_dvobj((adapter)));
    public static registry_priv dvobj_to_regsty(dvobj_priv dvobj) => ((dvobj.padapters[0].registrypriv));
}
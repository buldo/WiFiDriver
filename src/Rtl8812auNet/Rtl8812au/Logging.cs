using System.Text;

namespace Rtl8812auNet.Rtl8812au;

internal static class Logging
{
    public static void RTW_INFO(string str, params object[] list)
    {
        var strBuilder = new StringBuilder();
        strBuilder.Append(str+" ");
        foreach (var obj in list)
        {
            strBuilder.Append(obj);
            strBuilder.Append(" ");
        }

        Console.WriteLine(strBuilder);
    }

    public static void RTW_ERR(string str)
    {
        Console.WriteLine(str);
    }

    public static void RTW_WARN(string str)
    {
        Console.WriteLine(str);
    }

    public static void RTW_PRINT(string str)
    {
        Console.WriteLine(str);
    }
}
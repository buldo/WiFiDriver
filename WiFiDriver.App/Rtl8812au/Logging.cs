﻿using System.Text;

namespace WiFiDriver.App.Rtl8812au;

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
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace OpenHdDriver.Windows.ViewModels;

internal partial class MainWindowViewModel: ObservableObject
{
    [RelayCommand]
    private void CloseApplication()
    {
        App.Current.Shutdown();
    }
}
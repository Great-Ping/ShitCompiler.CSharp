using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace ShitCompiler.Pages;

public partial class MainViewModel() : ViewModelBase
{
    [ObservableProperty] 
    private string _codeInput = String.Empty;
    
    [ObservableProperty]
    private string _infoOutput = String.Empty;
    
    [ObservableProperty]
    private string _compileOutput = String.Empty;

    partial void OnCodeInputChanged(string value)
    {   
        InfoOutput = value;
        CompileOutput = value;
    }
}
/*
    Helper class focused on making sintax easyer to understand and shorten code 
    //with out this class 
    private string _example;
    public string Example
    {
        get { return _example; }
        set
        {
            if (_example != value)
            {
                _example = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Example)));
            }
        }
    }
    //with this class 
    private string _example;
    public string Example { get => _example; set => SetProperty(ref _example, value); }
 
 */
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Trojan.ViewModels;

public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TechLabManagement.ViewModels;

// Base view-model implementing INotifyPropertyChanged for MVVM bindings
public abstract class BaseViewModel : INotifyPropertyChanged
{
	// Raised when a bound property value changes
	public event PropertyChangedEventHandler? PropertyChanged;

    /* Raises PropertyChanged for the specified property. */
	protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

    /* Sets the storage field, raises PropertyChanged if changed, and returns whether it changed. */
	protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
		storage = value;
		RaisePropertyChanged(propertyName);
		return true;
	}
}



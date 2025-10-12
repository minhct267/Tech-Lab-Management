using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TechLabManagement.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
	public event PropertyChangedEventHandler? PropertyChanged;

	protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
		storage = value;
		RaisePropertyChanged(propertyName);
		return true;
	}
}



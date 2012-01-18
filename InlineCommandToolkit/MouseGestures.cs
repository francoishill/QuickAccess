
namespace MouseGesturePlugins
{
	public interface IMouseGesture
	{
		string GestureString { get; }
		string ThisCommandName { get; }
		bool PerformCommandAfterGesture(out string ErrorMessage);
	}
}

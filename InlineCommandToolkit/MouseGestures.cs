
namespace InlineCommandToolkit
{
	public interface IMouseGesture: InlineCommandToolkit.IQuickAccessPluginInterface
	{
		string GestureString { get; }
		string ThisCommandName { get; }
		bool PerformCommandAfterGesture(out string ErrorMessage);
	}
}

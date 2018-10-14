namespace Amber.Interaction
{
	public interface ICommand
	{
		void Execute();
		void Undo();
	}
}

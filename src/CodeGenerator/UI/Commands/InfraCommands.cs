using Library.Wpf.Windows.Input.Commands;
using System.Windows.Input;

namespace HanyCo.Infra.UI.Commands
{
    public static class InfraCommands
    {
        public static readonly LibRoutedUICommand GenerateCodeCommand = new(
                "Generate Code",
                "GenerateCodeCommand",
                typeof(InfraCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.N, ModifierKeys.Control)
                }
            );
    }
}

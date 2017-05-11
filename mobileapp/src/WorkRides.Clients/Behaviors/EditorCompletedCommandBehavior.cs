using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Behaviors
{
    public sealed class EditorCompletedCommandBehavior
    {
        public static readonly BindableProperty CompletedCommandProperty =
            BindableProperty.CreateAttached(
                "CompletedCommand",
                typeof(ICommand),
                typeof(EditorCompletedCommandBehavior),
                default(ICommand),
                BindingMode.OneWay,
                null,
                PropertyChanged);

        private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var editor = bindable as Editor;
            if (editor != null)
            {
                editor.Completed -= EditorOnCompleted;
                editor.Completed += EditorOnCompleted;
            }
        }

        private static void EditorOnCompleted(object sender, EventArgs e)
        {
            var editor = sender as Editor;
            if (editor != null && editor.IsEnabled)
            {
                var command = GetCompletedCommand(editor);
                if (command != null && command.CanExecute(e))
                {
                    command.Execute(e);
                }
            }
        }

        public static ICommand GetCompletedCommand(BindableObject bindableObject)
        {
            return (ICommand)bindableObject.GetValue(CompletedCommandProperty);
        }

        public static void SetCompletedCommand(BindableObject bindableObject, object value)
        {
            bindableObject.SetValue(CompletedCommandProperty, value);
        }
    }
}
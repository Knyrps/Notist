using System.Windows;
using System.Windows.Controls;

namespace Notist.Components.Tray
{
    internal class ConditionalMenuItem : MenuItem
    {
		public static readonly DependencyProperty ConditionProperty =
			DependencyProperty.Register(
				nameof(Condition),
				typeof(bool),
				typeof(ConditionalMenuItem),
				new PropertyMetadata(true, OnConditionChanged));

		public bool Condition
		{
			get => (bool)GetValue(ConditionProperty);
			set => SetValue(ConditionProperty, value);
		}

		private static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is ConditionalMenuItem menuItem)
			{
				menuItem.Visibility = (bool)e.NewValue ? Visibility.Visible : Visibility.Collapsed;
			}
		}

		protected override void OnInitialized(System.EventArgs e)
		{
			base.OnInitialized(e);

			Visibility = Condition ? Visibility.Visible : Visibility.Collapsed;
		}
    }
}

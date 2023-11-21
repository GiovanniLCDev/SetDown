using System;
using System.Windows;


namespace SetDown
{
    public partial class ModalWindow : Window
    {

        public ModalWindow()
        {
            InitializeComponent();
        }

        public event EventHandler? ButtonClicked;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}

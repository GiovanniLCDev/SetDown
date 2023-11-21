using System.Diagnostics;
using System.Media;
using System.Windows;

namespace SetDown
{
    public class Counter
    {
        protected int count;
        protected int hours;
        protected int minutes;
        protected int seconds;

        public Counter(int initialcount)
        {
            count = initialcount;

            hours = count / 3600;

            minutes = count / 60 - hours * 60;

            seconds = count % 3600 - minutes * 60;
        }

        public void SetCounter(int count)
        {
            this.count = count;
        }

       override
       public string ToString()
       {
            hours = count / 3600;

            minutes = count / 60 - hours * 60;

            seconds = count % 3600 - minutes * 60;

            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
       }
    }


    public class ComplexCounter : Counter
    {
        private readonly ModalWindow AlertWindow;
        private readonly MainWindow parent;

        public ComplexCounter(int initialcount, ModalWindow alertwindow, MainWindow main) : base(initialcount)
        {
            parent = main;

            AlertWindow = alertwindow;
        }

        public string Next()
        {
            if (this.count == 0) return ToString();

            count--;

            if (count == 600)
            {
                Application.Current.Dispatcher.Invoke(() => AlertWindow.Show());

                SystemSounds.Exclamation.Play();

                Application.Current.Dispatcher.Invoke(() => parent.owl.Visibility = Visibility.Visible);

                Application.Current.Dispatcher.Invoke(() => parent.clock.Visibility = Visibility.Hidden);
            }

            if (count == 0) Process.Start("ShutDown", "-s -t 0");

            return ToString();
        }

        public void Cancel()
        {
            count = 0;

            hours = 0;

            minutes = 0;

            seconds = 0;

            Application.Current.Dispatcher.Invoke(() => AlertWindow.Hide());
        }

    }
}
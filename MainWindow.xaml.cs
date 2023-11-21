using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;



namespace SetDown
{
    public partial class MainWindow : Window
    {
        private readonly ModalWindow AlertWindow;
        private readonly ComplexCounter Counter;
        private Thread executing;
        private Boolean checkExecuting = false;
        private int countSelected;
        private int DisplaySelected = 0;
        private int DisplayCount = 1;
        private List<int> CountList;

        public MainWindow()
        {
            AlertWindow = new ModalWindow { Topmost = true };

            AlertWindow.ButtonClicked += Check_Modal;

            Counter = new ComplexCounter(countSelected, AlertWindow, this);

            executing = new Thread(ThreadCounter);

            CountList = new List<int> {1800,3600,5400,7200,9000,10800};

            InitializeComponent();

            UpdateConfig(null,null);
        }

        private void UpdateConfig(object sender, EventArgs e)
        {
            StreamReader file = new("assets/conf.txt");

            for (int i = 1; i <= 6; i++)
            {
                string line = file.ReadLine();
                CountList[i-1] = int.Parse(line);
                Counter count = new(int.Parse(line));
                RadioButton rb = System.Windows.Application.Current.Dispatcher.Invoke(() => (RadioButton)this.FindName("RadioButton" + i));
                TextBlock tb = System.Windows.Application.Current.Dispatcher.Invoke(() => (TextBlock)rb.Content);
                System.Windows.Application.Current.Dispatcher.Invoke(() => tb.Text = count.ToString());
            }

        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Application.Current.Dispatcher.Invoke(() => e.ChangedButton == MouseButton.Left)) this.DragMove();

        }




        private void CloseApplication(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void MinimizeWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SetConfig(object sender, RoutedEventArgs e)
        {
            Process updating = Process.Start("notepad.exe", "assets/conf.txt");
            updating.EnableRaisingEvents = true;
            updating.Exited += new EventHandler(UpdateConfig);
        }





        public void ThreadCounter()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(999);
                    UpdateCounter(Counter.Next());
                }
            }
            catch (ThreadInterruptedException) { }
        }



        private void UpdateCounter(String count)
        {
            if (graphCounter.Dispatcher.CheckAccess()) graphCounter.Content = count;

            else graphCounter.Dispatcher.BeginInvoke(new Action<String>(UpdateCounter), count);
        }



        private void InterruptCounter()
        {
            try { executing.Interrupt(); }
            catch (Exception) { }

            Counter.Cancel();
            Privilege(() => this.owl.Visibility = Visibility.Hidden);
            Privilege(() => this.clock.Visibility = Visibility.Visible);
        }



        private void CancelButton(Object sender, RoutedEventArgs e)
        {
            checkExecuting = false;

            InterruptCounter();

            UpdateCounter(Counter.ToString());

            countSelected = 0;

            UnselectRadioButton();

            BorderCounter.Visibility = Visibility.Hidden;
        }



        private void AcceptButton(Object sender, RoutedEventArgs e)
        {
            if (countSelected == 0) return;

            checkExecuting = true;

            InterruptCounter();

            Counter.SetCounter(countSelected);

            executing = new Thread(ThreadCounter);

            executing.Start();

            BorderCounter.Visibility = Visibility.Hidden;

            if (countSelected <= 600) Privilege(() => AlertWindow.Show());
        }



        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not System.Windows.Controls.RadioButton radioButton) return;

            switch (radioButton.Name)
            {
                case "RadioButton1":
                    countSelected = CountList[0];
                    break;
                case "RadioButton2":
                    countSelected = CountList[1];
                    break;
                case "RadioButton3":
                    countSelected = CountList[2];
                    break;
                case "RadioButton4":
                    countSelected = CountList[3];
                    break;
                case "RadioButton5":
                    countSelected = CountList[4];
                    break;
                case "RadioButton6":
                    countSelected = CountList[5];
                    break;
            }

            if (checkExecuting) return;

            Counter.SetCounter(countSelected);

            UpdateCounter(Counter.ToString());
        }



        private void Check_Modal(object? sender, EventArgs e)
        {
            InterruptCounter();

            UpdateCounter(Counter.ToString());

            this.Focus();
        }



        private void UnselectRadioButton()
        {
            List<string> RadioButtonList = new() { "RadioButton1", "RadioButton2", "RadioButton3", "RadioButton4", "RadioButton5", "RadioButton6" };

            foreach (String button in RadioButtonList)
            {
                System.Windows.Controls.RadioButton? radioButton = (System.Windows.Controls.RadioButton)FindName(button);
                radioButton.IsChecked = false;
            }
        }



        private void ControlButton_Checked(object sender, RoutedEventArgs e)
        {
            if (checkExecuting) return;

            if (sender is not Button Button) return;

            BorderCounter.Visibility = Visibility.Visible;

            switch (Button.Name)
            {

                case "Up":
                    countSelected += DisplayCount;
                    Counter.SetCounter(countSelected);
                    UpdateCounter(Counter.ToString());
                    break;
                case "Down":
                    if (countSelected <= 0) return;
                    countSelected -= DisplayCount;
                    Counter.SetCounter(countSelected);
                    UpdateCounter(Counter.ToString());
                    break;
                case "Left":
                    DisplaySelected++;
                    DisplaySelected = DisplaySelected % 3;
                    UpdateDisplay();
                    break;
                case "Right":
                    DisplaySelected--;
                    if (DisplaySelected == -1) DisplaySelected = 2;
                    UpdateDisplay();
                    break;
            }
        }

        private void UpdateDisplay()
        {
            switch (DisplaySelected)
            {
                case 0:
                    DisplayCount = 1;
                    BorderCounter.Margin = new Thickness(170, 68, 0, 0);
                    break;
                case 1:
                    DisplayCount = 60;
                    BorderCounter.Margin = new Thickness(1, 68, 0, 0);
                    break;
                case 2:
                    DisplayCount = 3600;
                    BorderCounter.Margin = new Thickness(-168, 68, 0, 0);
                    break;
            }
        }


        public static void Privilege(Action accion)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(accion);
        }
    }
}

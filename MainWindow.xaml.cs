using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.Threading;
using System.IO;

namespace SimpleTwitchTimer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        byte hasStopped,isRunning;
        int duration, currentTime;
        System.Timers.Timer timer = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            RadioMinutes.IsChecked = true; //Preselect this Radio Button
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

            if (!File.Exists("Timer.txt"))
            {
                FileStream file = new FileStream("Timer.txt", FileMode.Create); //file is created
                file.Close();
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            hasStopped = 1;
        }

        private void TextBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            //Check for Spaces and automaticly trim them.
            if (TextBoxInput.Text.Contains(" "))
            {
                TextBoxInput.Text = TextBoxInput.Text.Trim(' ');
                TextBoxInput.SelectionLength = 0;
                TextBoxInput.SelectionStart = TextBoxInput.Text.Length + 1;
            }
        }

        //PreCheck Input and allow only numbers (and sadly spaces) which is prevented in TextBoxInput_TextChanged
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ButtonStartPause_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning == 0)
            {
                if (Convert.ToBoolean(RadioMinutes.IsChecked))
                {
                    duration = int.Parse(TextBoxInput.Text) * 60;
                }
                else
                {
                    duration = int.Parse(TextBoxInput.Text);
                }
                currentTime = duration;
                //Need To do Threading here. 
                SetTimer(currentTime);
                timer.Start();
                while (hasStopped == 0)
                {
                    SetToFile(MakeToString(currentTime));
                    System.Threading.Thread.Sleep(1000);
                }

            }

            //Timer(currentTime);


        }

        private void SetTimer(int currentTimeInS)
        {
            timer.Interval = currentTimeInS;

        }
        private void Timer(int currentTime)
        {
            while (currentTime != 0)
            {
                SetToFile(MakeToString(currentTime));

                System.Threading.Thread.Sleep(1000);
                currentTime--;
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RadioMinutes_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void RadioSeconds_Checked(object sender, RoutedEventArgs e)
        {

        }

        public String MakeToString(int currentTime)
        {
            TimeSpan time = TimeSpan.FromSeconds(currentTime);
            String str = time.ToString(@"hh\:mm\:ss");
            return str;
        }

        public void SetToFile(String content)
        {
            File.WriteAllText("Timer.txt", content);


        }
    }
}

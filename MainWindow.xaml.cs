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
        bool isNewStart = true,hasEnded;
        byte isRunning;
        int duration, currentTime;
        System.Timers.Timer timer = new System.Timers.Timer();
        public MainWindow()
        {
            InitializeComponent();
            RadioMinutes.IsChecked = true; //Preselect this Radio Button

            if (!File.Exists("Timer.txt")) //Check if file already exists
            {
                FileStream file = new FileStream("Timer.txt", FileMode.Create); //file is created
                file.Close();
            }
        }

        private void TextBoxInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            isNewStart = true;
            isRunning = 0;
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

        private async void ButtonStartPause_Click(object sender, RoutedEventArgs e)
        {
            hasEnded = true;
            if (TextBoxInput.Text.Length == 0)
            {
                MessageBox.Show("An empty Input is not allowed!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Console.WriteLine(isNewStart);
                if (isRunning == 0 && isNewStart)
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
                    isRunning = 1;
                    ButtonStartPause.Content = "Pause";
                    await Task.Run(() => TimerStart(currentTime));
                    if (!hasEnded)
                    { } else
                    {
                        hasEnded = true;
                        Reset();
                    }
                }
                else if (isRunning == 0 && !isNewStart)
                {
                    isRunning = 1;
                    ButtonStartPause.Content = "Pause";
                    await Task.Run(() => TimerStart(currentTime));
                    if (!hasEnded) { } else
                    {
                        hasEnded = true;
                        Reset();
                    }
                }        
                else
                {
                    isRunning = 0;
                    isNewStart = false;
                    ButtonStartPause.Content = "Start";
                }

            }
        }

        private void Reset()
        {
            isRunning = 0;
            isNewStart = true;
            currentTime = duration;
            ButtonStartPause.Content = "Start";
            if(hasEnded)
            {
                 SetToFile(MakeToString(0));
            } else
            {
                int seconds;
                if (Convert.ToBoolean(RadioMinutes.IsChecked))
                {
                    seconds = duration * 60;
                } else
                {
                    seconds = duration / 60;
                }
                SetToFile(MakeToString(seconds));
            }

        }
        private void SetTimer(int currentTimeInS)
        {
            timer.Interval = currentTimeInS;

        }
        private void TimerStart(int seconds)
        {
            while (seconds != 0 && isRunning == 1)
            {
                Console.WriteLine(seconds);
                SetToFile(MakeToString(seconds));

                System.Threading.Thread.Sleep(1000);
                seconds--;
                currentTime--;
            }
        }

        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            hasEnded = false;
            Reset();
        }

        private void RadioMinutes_Checked(object sender, RoutedEventArgs e)
        {
            hasEnded = false;
            Reset();
        }

        private void RadioSeconds_Checked(object sender, RoutedEventArgs e)
        {
            hasEnded = false;
            Reset();
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

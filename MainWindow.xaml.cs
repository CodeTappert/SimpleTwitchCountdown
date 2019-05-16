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
        bool isNewStart = true, hasEnded;
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
                //Trim Spaces and then set the cursor again at the end of the input.
                TextBoxInput.Text = TextBoxInput.Text.Trim(' ');
                TextBoxInput.SelectionLength = 0;
                TextBoxInput.SelectionStart = TextBoxInput.Text.Length + 1;
            }
        }

        //PreCheck Input and allow only numbers (and sadly spaces) which is prevented in TextBoxInput_TextChanged
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+"); //Only Numbers
            e.Handled = regex.IsMatch(e.Text);
        }

        //When the Start Button is Pressed there is different behaviour depending on status
        private async void ButtonStartPause_Click(object sender, RoutedEventArgs e)
        {
            hasEnded = true;
            //Empty Inputs are not allowed. Messagebox will be shown
            if (TextBoxInput.Text.Length == 0)
            {
                MessageBox.Show("An empty Input is not allowed!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                //If it is not running and it was resettet/previous countdown has ended or first countdown
                if (isRunning == 0 && isNewStart)
                {
                    duration = GetInput();
                    currentTime = duration;
                    isRunning = 1; //Set it running
                    ButtonStartPause.Content = "Pause"; //Change label on button
                    await Task.Run(() => TimerStart(currentTime)); //wait until Timer is finished.
                    if (!hasEnded)
                    { }
                    else
                    {
                        //When it ended without interruptions then it will display 0:00 until a new one is started
                        hasEnded = true;
                        Reset();
                    }
                }
                //If it isnt running but was already running (after a pause) then it will be resumed.
                else if (isRunning == 0 && !isNewStart)
                {
                    //Set it to running
                    isRunning = 1;
                    ButtonStartPause.Content = "Pause"; //Change the Button Label to Pause
                    await Task.Run(() => TimerStart(currentTime)); //Wait for Finished Countdown
                    if (!hasEnded) { }
                    else
                    {
                        //When it ended without interruptions then it will display 0:00 until a new one is started
                        hasEnded = true;
                        Reset();
                    }
                }
                else
                {
                    //Set to not Running
                    isRunning = 0;
                    hasEnded = false;
                    isNewStart = false; //Change new Start to false so it will resume and not start over again
                    ButtonStartPause.Content = "Resume"; //Change Button Label to resume
                }

            }
        }

        //Get the UserInput and calculate it accordingly to the radio boxes
        private int GetInput()
        {
            int duration;
            //If there is no input than return 0.
            if (TextBoxInput.Text.Length > 0)
            {
                //If Minutes Radio Box is checked we have to calculate it times 60
                if (Convert.ToBoolean(RadioMinutes.IsChecked))
                {
                    duration = int.Parse(TextBoxInput.Text) * 60;
                }
                else
                {
                    duration = int.Parse(TextBoxInput.Text);
                }
                return duration;
            }
            else
            {
                return 0;
            }

        }

        //Reset the Countdown. Depending on if it finished or was interrupted mid countdown
        private void Reset()
        {
            isRunning = 0; //Set to not running
            isNewStart = true; //Set that the next countdown starts all over again
            currentTime = duration; //set current time to max time
            ButtonStartPause.Content = "Start";
            //If Countdown was finished without interrupts than show 00:00:00
            if (hasEnded)
            {
                SetToFile(MakeToString(0));
            }
            //If it was interrupted mid countdown then show the max time again
            else
            {
                SetToFile(MakeToString(duration));
            }
        }

        //The Timer, its a simple while loop that counts seconds down. Can be interruppted when isRunning is changing to 0.
        private void TimerStart(int seconds)
        {
            while (seconds != 0 && isRunning == 1)
            {
                SetToFile(MakeToString(seconds));

                System.Threading.Thread.Sleep(1000); //Wait 1 sec to generate the seconds counting.
                seconds--;
                currentTime--;
            }
        }

        //Resets the Timer. When Button Reset is clicked
        private void ButtonReset_Click(object sender, RoutedEventArgs e)
        {
            hasEnded = false;
            Reset();
        }

        //If Radio Boxes are changed then there should be the corret time shown. SetToFile makes this.
        private void RadioMinutes_Checked(object sender, RoutedEventArgs e)
        {
            hasEnded = false;
            Reset();
            SetToFile(MakeToString(GetInput()));
        }

        //If Radio Boxes are changed then there should be the corret time shown. SetToFile makes this.
        private void RadioSeconds_Checked(object sender, RoutedEventArgs e)
        {
            hasEnded = false;
            Reset();
            SetToFile(MakeToString(GetInput()));
        }

        //Makes a String in the corret TimeFormat from an int which are seconds
        public String MakeToString(int seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            String str = time.ToString(@"hh\:mm\:ss");
            return str;
        }

        //Writes a String to txtfile. With deleting what was in there
        public void SetToFile(String content)
        {
            File.WriteAllText("Timer.txt", content);
        }
    }
}

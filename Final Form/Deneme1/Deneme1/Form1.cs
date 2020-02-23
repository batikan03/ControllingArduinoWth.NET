using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Threading;
using System.IO.Ports;

namespace Deneme1
{
    public partial class Form1 : Form
    {
        SerialPort myPort = new SerialPort();
        SpeechRecognitionEngine re = new SpeechRecognitionEngine();
        SpeechSynthesizer ss = new SpeechSynthesizer(); // When you want program to talk back to you 
        Choices commands = new Choices(); // This is an important class as name suggest we will store our commands in this object
        public Form1()
        {

            InitializeComponent();
            myPort.PortName = "COM3"; // My Port name in Arduino IDE selected COM5 you need to change Port name if it is different  just check in arduinoIDE
            myPort.BaudRate = 9600;  // This Rate is Same as arduino Serial.begin(9600) bits per second
            processing();
        }

        void processing()
        {
            //First of all storing commands
            commands.Add(new string[] { "Blue On", "Red On", "Green On", "Blue Off", "Red Off", "Green Off", "Exit", "All On", "All Off", "Arduino Say Good Bye to makers" });
            //Now we will create object of Grammer in which we will pass commands as parameter
            Grammar gr = new Grammar(new GrammarBuilder(commands));
            re.RequestRecognizerUpdate(); // Pause Speech Recognition Engine before loading commands
            re.LoadGrammarAsync(gr);
            re.SetInputToDefaultAudioDevice();// As Name suggest input device builtin microphone or you can also connect earphone etc...
            re.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(re_SpeechRecognized);


        }

        void re_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                ////For Led State ON 
                // For blue led
                case "Blue On":
                    sendDataToArduino('B');
                    break;

                // For red led
                case "Red On":
                    sendDataToArduino('R');
                    break;

                // For green led
                case "Green On":
                    sendDataToArduino('G');
                    break;

                //For Led State OFF 
                // For blue led
                case "Blue Off":
                    sendDataToArduino('Z');
                    break;

                // For red led
                case "Red Off":
                    sendDataToArduino('X');
                    break;

                // For green led
                case "Green Off":
                    sendDataToArduino('C');
                    break;

                //For turning ON all leds at once :)
                case "All On":
                    sendDataToArduino('V');
                    break;

                //For turning OFF all leds at once :)
                case "All Off":
                    sendDataToArduino('M');
                    break;
                //Program will talk back 
                case "Arduino Say Good Bye to makers":
                    ss.SpeakAsync("Good Bye Makers"); // speech synthesis object is used for this purpose
                    break;

                // To Exit Program using Voice :)
                case "Exit":
                    Application.Exit();
                    break;
            }
            richTextBox1.Text += e.Result.Text.ToString() + Environment.NewLine;// Whenever we command arduino text will append and shown in textbox

        }

        void sendDataToArduino(char character)
        {
            myPort.Open();
            myPort.Write(character.ToString());
            myPort.Close();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            re.RecognizeAsyncStop();
            //btnStart.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            re.RecognizeAsync(RecognizeMode.Multiple);
            button1.Enabled = true;
            button2.Enabled = false;
            MessageBox.Show("Voice Recognition Started !", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }
}

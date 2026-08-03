using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ClientApp
{
    public partial class ClientAppForm : Form
    {        
        public bool exitStatus = false;
        public const int BYTE_SIZE = 1024;
        public const string HOST_NAME = "localhost";
        public const int PORT_NUMBER = 8888;

        // set up a client connection for TCP network service
        private TcpClient clientSocket;

        // set up data stream object
        private NetworkStream netStream;

        // set up thread to run ReceiveStream() method
        private Thread clientThread = null;

        // set up delegate
        delegate void SetTextCallback(string text);

        private int correctAnswer;
        private int submittedAnswer;
        private string answerStatus = string.Empty;
   //     private string answerResult = "n";


        public ClientAppForm()
        {
            InitializeComponent();


            ClientSubmitButton.Enabled = false;

            // clear all text boxes
            ClientQuestionTextBox.Text = "";
            ClientAnswerTextBox.Text = "";

            // start client
            StartClient();
        }

        private void StartClient()
        {
            try
            {
                // create TCPClient object (as the socket)
                clientSocket = new TcpClient(HOST_NAME, PORT_NUMBER);
                // create stream
                netStream = clientSocket.GetStream();
                // set up thread to run ReceiveStream() method
                clientThread = new Thread(ReceiveStream);
                // start thread
                clientThread.Start();
                SystemMsgLabel.Text = "Client started ..." + Environment.NewLine;
            }
            catch (Exception e)
            {
                // display exception message
                ClientQuestionTextBox.Text = e.StackTrace;
            }
        }

        public void ReceiveStream()
        {
            byte[] bytesReceived = new byte[BYTE_SIZE];
            // loop to read any incoming messages
            while (!exitStatus)
            {
                try
                {
                    int bytesRead = netStream.Read(bytesReceived, 0,
                    bytesReceived.Length);
                    this.SetText(Encoding.ASCII.GetString(bytesReceived,
                    0, bytesRead));
                }
                catch (System.IO.IOException)
                {
                    Console.WriteLine("Server has exited!");
                    exitStatus = true;
                }
            }
        }

        private void SetText(string text)
        {
            // InvokeRequired compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // if these threads are different, it returns true.
            if (this.ClientQuestionTextBox.InvokeRequired)
            {
                // d is a Delegate reference to the SetText() method
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {

                string[] questionElements = text.Split(' ');
                string firstNum = questionElements[0];
                string mathOperator = questionElements[1];
                string secondNum = questionElements[2];
                string answer = questionElements[4];

                string formattedQuestion = $"{firstNum} {mathOperator} {secondNum} = ?";
                ClientQuestionTextBox.Text = formattedQuestion + Environment.NewLine;

                correctAnswer = int.Parse(answer);
                ClientSubmitButton.Enabled = true;

                //   this.Receive_TextBox.Text += "Server: " + text + Environment.NewLine;

            }
        }

        private void ClientSubmitButton_Click(object sender, EventArgs e)
        {


            string errorMessage = string.Empty;
            
            if(string.IsNullOrWhiteSpace(this.ClientAnswerTextBox.Text))
            {
                errorMessage = "Please check your answer - Cannot be left blank";
                MessageBox.Show(errorMessage, "Incorrect Answer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(ClientAnswerTextBox.Text, out _))
            {
                errorMessage = "Please check your answer - Not a valid number";
                MessageBox.Show(errorMessage, "Incorrect Answer", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            submittedAnswer = int.Parse(ClientAnswerTextBox.Text.Trim());
            answerStatus = CompareAnswer(correctAnswer, submittedAnswer);

            if (answerStatus.Equals("y"))
            {
                MessageBox.Show("Congratulations! Correct answer", "Correct Answer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Incorrect! Please check your answer", "Incorrect Answer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            // construct byte array to stream in write mode
            String strToSend = answerStatus;
            byte[] bytesToSend = Encoding.ASCII.GetBytes(strToSend);
            netStream.Write(bytesToSend, 0, bytesToSend.Length);
            //   Receive_TextBox.Text += "Client: " + strToSend + Environment.NewLine;
            //   Send_TextBox.Text = "";
        }

        private string CompareAnswer(int correctAnswer, int submittedAnswer)
        {
            if (correctAnswer == submittedAnswer)
            {
                return "y";
            }

            return "n";
        }

        private void ClientExitButton_Click(object sender, EventArgs e)
        {
            // terminate client thread if still running
            if (clientThread.IsAlive)
            {
                Console.WriteLine("Client thread is alive");
                clientThread.Interrupt();
                if (clientThread.IsAlive)
                {
                    Console.WriteLine("Client thread is now terminated");
                }
            }
            else
            {
                Console.WriteLine("Client thread is terminated");
            }
            // close the application for good
            Environment.Exit(0);
        }
    }
}

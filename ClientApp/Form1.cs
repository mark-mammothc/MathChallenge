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

            // start client
            StartClient();
        }

        /// <summary>
        /// Method:     StartClient.
        /// Desc:       Starts the client and establishes a connection to the server.
        /// </summary>
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

        /// <summary>
        /// Method:     ReceiveStream.
        /// Desc:       Receives the stream of data from the server and updates 
        ///             the ClientQuestionTextBox with the received question.
        /// </summary>
        public void ReceiveStream()
        {
            byte[] bytesReceived = new byte[BYTE_SIZE];

            while (!exitStatus)
            {
                try
                {
                    int bytesRead = netStream.Read(bytesReceived, 0, bytesReceived.Length);

                    // 0 bytes read indicates the remote peer gracefully closed the connection
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Server disconnected gracefully.");
                        exitStatus = true;
                        break;
                    }

                    string receivedText = Encoding.ASCII.GetString(bytesReceived, 0, bytesRead);
                    this.SetText(receivedText);
                }
                catch (Exception ex)
                {
                    // Catches IOException, SocketException, ObjectDisposedException on disconnect
                    Console.WriteLine($"Server connection lost: {ex.Message}");
                    exitStatus = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Method:     SetText.
        /// Desc:       Sets the text of the ClientQuestionTextBox.
        /// </summary>
        /// <param name="text"></param>
        private void SetText(string text)
        {
            // Prevent UI updates if the form is closing or disposed
            if (this.IsDisposed || this.Disposing) return;

            if (this.ClientQuestionTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(text)) return;

                string[] questionElements = text.Trim().Split(' ');

                // Ensure array has enough elements before indexing
                if (questionElements.Length >= 5)
                {
                    string firstNum = questionElements[0];
                    string mathOperator = questionElements[1];
                    string secondNum = questionElements[2];
                    string answer = questionElements[4];

                    string formattedQuestion = $"{firstNum} {mathOperator} {secondNum} = ?";
                    ClientQuestionTextBox.Text = formattedQuestion + Environment.NewLine;

                    if (int.TryParse(answer, out int parsedAnswer))
                    {
                        correctAnswer = parsedAnswer;
                        ClientSubmitButton.Enabled = true;
                    }
                }
            }
        }

        /// <summary>
        /// Method:     ClientSubmitButton_Click.
        /// Desc:       Handles the click event for the ClientSubmitButton. 
        ///             Validates the user's answer.
        ///             Compares it with the correct answer.
        ///             Sends the result back to the server.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
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
                MessageBox.Show("Incorrect Answer", "Incorrect Answer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ClientSubmitButton.Enabled = false;

            // clear all text boxes
            ClientQuestionTextBox.Text = "";
            ClientAnswerTextBox.Text = "";

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

        /// <summary>
        /// Method:     ClientExitButton_Click.
        /// Desc:       Handles the click event for the ClientExitButton. 
        ///             Sets the exit status and closes the client socket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ClientExitButton_Click(object sender, EventArgs e)
        {
            exitStatus = true;

            try
            {
                // Closing the stream causes any blocking netStream.Read() 
                // in clientThread to unblock and throw/return 0 immediately
                netStream?.Close();
                clientSocket?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing client socket: {ex.Message}");
            }

            // Wait briefly for the receive thread to finish its loop
            if (clientThread != null && clientThread.IsAlive)
            {
                clientThread.Join(500);
            }

            // Safely close the Windows Form
            this.Close();
        }

        /// <summary>
        /// Method:     OnFormClosing.
        /// Desc:       Handles the form closing event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            exitStatus = true;

            try
            {
                netStream?.Close();
                clientSocket?.Close();

                if (clientThread != null && clientThread.IsAlive)
                {
                    clientThread.Join(200); // Wait briefly for the loop to terminate
                }
            }
            catch
            {
                // Ignore cleanup exceptions on application shutdown
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using BinTree;

namespace MathQuestionChallenge
{
    public partial class InstructorAppForm : Form
    {

        // private fields, data structures etc go here.
        public bool exitStatus = false;
        public const int BYTE_SIZE = 1024;
        public const int PORT_NUMBER = 8888;

        // listens for and accept incoming connection requests
        private TcpListener serverListener;

        // TcpClient is used to connect with the TcpListener object
        private TcpClient serverSocket;

        // set up data stream object
        private NetworkStream netStream;

        // set up thread to run ReceiveStream() method
        private Thread serverThread = null;

        // set up delegate
        // a delegate is a reference variable to a method
        // and used for a call back by the delegate object
        // delegate ref variable is declared in SetText() method below
        delegate void SetTextCallback(string text);

        // data structures
        List<MathQuestion> mathQuesList;
        LinkedList<MathQuestion> mathQuesLinkedList;
        BinaryTree<MathQuestion> mathQuestionBinTree;
        Hashtable mathQuesHashTable;

        // current math question sent / answered
        MathQuestion currentQuestion;

        public InstructorAppForm()
        {
            InitializeComponent();

            // set math operators
            PopulateDropdown();

            // set datagrid
            CreateDataGridViewCol();

            // run server
            StartServer();

            // clear fields at some point


            // server thread
            serverThread = null;

            // instantiate data structures
            mathQuesList = new List<MathQuestion>();
            mathQuesLinkedList = new LinkedList<MathQuestion>();
            mathQuestionBinTree = new BinaryTree<MathQuestion>();
            mathQuesHashTable = new Hashtable();

            // current Question
            currentQuestion = null;

        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            if(ValidateInputs())
            {
                AnswerTextBox.Text = calculateAnswer(FirstNumberTextBox.Text, SecondNumberTextBox.Text, OperatorComboBox.SelectedItem.ToString());
            

                // int leftOperand, string mathOperator, int rightOperand, int answer
                currentQuestion = new MathQuestion(int.Parse(FirstNumberTextBox.Text), OperatorComboBox.SelectedItem.ToString(), int.Parse(SecondNumberTextBox.Text), int.Parse(AnswerTextBox.Text));

                // populate data structures
                mathQuesList.Add(currentQuestion);

                // add the question into the QuestionArrayDataGridView
                DisplayTable();


                if (!string.IsNullOrWhiteSpace(AnswerTextBox.Text))
                {
                    // construct byte array to stream in write mode
                    String strToSend = currentQuestion.ToQuestionStr();
                    byte[] bytesToSend = Encoding.ASCII.GetBytes(strToSend);
                    netStream.Write(bytesToSend, 0, bytesToSend.Length);
                 //   SystemMsgLabel.Text += "Server: " + strToSend + Environment.NewLine;
                //    Send_TextBox.Text = "";

                    SendButton.Enabled = false;
                }
            }

        }

        private void PopulateDropdown()
        {
            // Populate the dropdown with question types
            OperatorComboBox.Items.AddRange(new object[] { "+", "-", "x", "/" });
            OperatorComboBox.SelectedIndex = 0;
        }

        private void CreateDataGridViewCol()
        {
            QuestionArrayDataGridView.ReadOnly = true;
            
            var myColumns = new DataGridViewColumn[]
            {
                new DataGridViewTextBoxColumn { Name = "QuestionLeftOperand", DataPropertyName = "", HeaderText = "Number 1" },
                new DataGridViewTextBoxColumn { Name = "QuestionOperator", DataPropertyName = "", HeaderText = "Math" },
                new DataGridViewTextBoxColumn { Name = "QuestionRightOperand", DataPropertyName = "", HeaderText = "Number 2" },
                new DataGridViewTextBoxColumn { Name = "QuestionEqualSymbol", DataPropertyName = "", HeaderText = "=" },
                new DataGridViewTextBoxColumn { Name = "QuestionAnswer", DataPropertyName = "", HeaderText = "Answer" },
            };

            // add all created column elements
            QuestionArrayDataGridView.Columns.AddRange(myColumns);
            QuestionArrayDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }



        private bool ValidateInputs()
        {
            // 1: Check if any fields are empty first
            if (!ValidateNotEmpty()) return false;

            // 2: Check if inputs are numbers (only runs if all fields have text)
            if (!ValidateAreNumbers()) return false;

            return true; // All good!
        }

        // 1: Collects all empty errors
        private bool ValidateNotEmpty()
        {
            string errorMessage = "";

            if (string.IsNullOrWhiteSpace(FirstNumberTextBox.Text))
            {
                errorMessage += "- First number missing\n";
            }

            if (string.IsNullOrWhiteSpace(SecondNumberTextBox.Text))
            {
                errorMessage += "- Second number missing\n";
            }

            // If any missing text errors, display them and exit
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError("Please complete all required fields:\n\n" + errorMessage);
                return false;
            }

            return true;
        }

        // 2: Collects all numeric format errors
        private bool ValidateAreNumbers()
        {
            string errorMessage = "";

            // out_ is because TryParse expects to return something. This negates it.
            if (!int.TryParse(FirstNumberTextBox.Text, out _))
            {
                errorMessage += "- First number not a valid number\n";
            }

            if (!int.TryParse(SecondNumberTextBox.Text, out _))
            {
                errorMessage += "- Second number not a valid number\n";
            }

            // If any invalid numeric errors, display them and exit
            if (!string.IsNullOrEmpty(errorMessage))
            {
                ShowError("Please correct invalid entries:\n\n" + errorMessage);
                return false;
            }

            return true;
        }

        // Show error dialog
        private void ShowError(string fullMessage)
        {
            MessageBox.Show(fullMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        private string calculateAnswer(string firstNumber, string secondNumber, string operation)
        {

            int firstNum = int.Parse(firstNumber);
            int secondNum = int.Parse(secondNumber);

            switch (operation)
            {
                case "+":
                    return (firstNum + secondNum).ToString();
                case "-":
                    return (firstNum - secondNum).ToString();
                case "x":
                    return (firstNum * secondNum).ToString();
                case "/":
                    if (secondNum == 0)
                    {
                        throw new DivideByZeroException("Cannot divide by zero.");
                    }
                    return (firstNum / secondNum).ToString();
                default:
                    throw new InvalidOperationException("Invalid operation.");
            }
        }

        private void clearTextBoxes()
        {
            FirstNumberTextBox.Clear();
            SecondNumberTextBox.Clear();
            AnswerTextBox.Clear();
        }


        // QuestionArrayDataGridView
        private void DisplayTable()
        {
            if (mathQuesList.Count == 0)
            {
                return;
            }
            else
            {
                // first remove rows that are already displayed
                QuestionArrayDataGridView.Rows.Clear();

                // questionArrayList has content of questions
                for (int i = 0; i < mathQuesList.Count; i++)
                {
                    QuestionArrayDataGridView.Rows.Add(mathQuesList[i].GetStrArray());
                }

                QuestionArrayDataGridView.Refresh();
            }

        }


        private void StartServer()
        {
            try
            {
                // create listener and start
                serverListener = new TcpListener(IPAddress.Loopback,
                PORT_NUMBER);
                serverListener.Start();

                // create acceptance socket
                // this creates a socket connection for the server
                serverSocket = serverListener.AcceptTcpClient();

                // create stream
                netStream = serverSocket.GetStream();

                // set up thread to run ReceiveStream() method
                serverThread = new Thread(ReceiveStream);

                // start thread
                serverThread.Start();
                SystemMsgLabel.Text = "Server started ..." +
                Environment.NewLine;
            }
            catch (Exception e)
            {
                // display exception message
                BinaryTreeTextBox.Text = e.StackTrace;
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
                    Console.WriteLine("Client has exited!");
                    exitStatus = true;
                }
            }
        }

        private void SetText(string text)
        {
            if (this.BinaryTreeTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                string resultOutcome = string.Empty;
  
                if (text.Trim().Equals("y"))
                {
                    resultOutcome = "Student answered the question correctly";
                }
                else
                {
                    resultOutcome = "Student answered the question incorrectly";
                }

                this.BinaryTreeTextBox.Text = resultOutcome;
                clearTextBoxes();
                SendButton.Enabled = true;

            }
        }


    }
}

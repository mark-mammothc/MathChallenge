using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using BinTree;

namespace MathQuestionChallenge
{
    public partial class InstructorAppForm : Form
    {

        // private fields, data structures etc go here.
        public bool exitStatus = false;
        public const int BYTE_SIZE = 1024;
        public const int PORT_NUMBER = 8888;

        // private int to keep track of whether there are any incorrect answers or not
        private int incorrectAnswerCount = 0;
        private bool isVisible = false;

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

    //    string questionResult = "n"; // default to incorrect answer

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

            QuestionArrayDataGridView.AutoGenerateColumns = false;
        }

        /// <summary>
        /// Handles the click event for the Send button.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void SendButton_Click(object sender, EventArgs e)
        {
            if(ValidateInputs())
            {
                AnswerTextBox.Text = calculateAnswer(FirstNumberTextBox.Text, SecondNumberTextBox.Text, OperatorComboBox.SelectedItem.ToString());

                // int leftOperand, string mathOperator, int rightOperand, int answer
                currentQuestion = new MathQuestion(int.Parse(FirstNumberTextBox.Text), OperatorComboBox.SelectedItem.ToString(), int.Parse(SecondNumberTextBox.Text), int.Parse(AnswerTextBox.Text));

                // populate data structures
                mathQuesList.Add(currentQuestion);
                mathQuestionBinTree.Add(currentQuestion);
                mathQuesHashTable.Add(currentQuestion.ToQuestionStr(), currentQuestion);

                // Rebuild the complete "ORDER ASKED" string from mathQuesList
                string allQuestionsAsked = string.Join(", ", mathQuesList);

                // Update the text box display with full history
                BinaryTreeTextBox.Text = "ORDER ASKED: " + allQuestionsAsked + ".";

                // add the question into the QuestionArrayDataGridView
                DisplayTable();

                if (!string.IsNullOrWhiteSpace(AnswerTextBox.Text))
                {
                    // construct byte array to stream in write mode
                    String strToSend = currentQuestion.ToQuestionStr();
                    byte[] bytesToSend = Encoding.ASCII.GetBytes(strToSend);
                    netStream.Write(bytesToSend, 0, bytesToSend.Length);
                    // SystemMsgLabel.Text += "Server: " + strToSend + Environment.NewLine;
                    //  Send_TextBox.Text = "";

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

        /// <summary>
        /// Method:     CreateDataGridViewCol
        /// Desc:       Creates the columns for the DataGridView.
        /// </summary>
        private void CreateDataGridViewCol()
        {
            QuestionArrayDataGridView.ReadOnly = true;

            // STOP WinForms from auto-generating new columns alongside your custom ones
            QuestionArrayDataGridView.AutoGenerateColumns = false;

            var myColumns = new DataGridViewColumn[]
            {
        // DataPropertyName MUST match the property names in your MathQuestion class!
        new DataGridViewTextBoxColumn { Name = "QuestionLeftOperand", DataPropertyName = "LeftOperand", HeaderText = "Number 1" },
        new DataGridViewTextBoxColumn { Name = "QuestionOperator", DataPropertyName = "Operator", HeaderText = "Math" },
        new DataGridViewTextBoxColumn { Name = "QuestionRightOperand", DataPropertyName = "RightOperand", HeaderText = "Number 2" },
        new DataGridViewTextBoxColumn { Name = "QuestionEqualSymbol", DataPropertyName = "EqualSymbol", HeaderText = "=" },
        new DataGridViewTextBoxColumn { Name = "QuestionAnswer", DataPropertyName = "Answer", HeaderText = "Answer" },
            };

            // Add all created column elements
            QuestionArrayDataGridView.Columns.AddRange(myColumns);
            QuestionArrayDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Validates the user inputs for the math question.
        /// </summary>
        /// <returns>true if all inputs are valid, false otherwise.</returns>
        private bool ValidateInputs()
        {
            // 1: Check if any fields are empty first
            if (!ValidateNotEmpty()) return false;

            // 2: Check if inputs are numbers (only runs if all fields have text)
            if (!ValidateAreNumbers()) return false;

            return true;
        }

        /// <summary>
        /// Validates that the input fields are not empty.
        /// </summary>
        /// <returns>true if all fields have text, false otherwise.</returns>
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

        /// <summary>
        /// Validates that the input values in the text boxes are valid integers.
        /// </summary>
        /// <returns>true if all inputs are valid numbers, false otherwise.</returns>
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

        /// <summary>
        /// Method:     calculateAnswer
        /// Desc:       Calculates the answer to a math question based on the provided numbers and operation.
        /// </summary>
        /// <param name="firstNumber">The first number.</param>
        /// <param name="secondNumber">The second number.</param>
        /// <param name="operation">The operation to perform.</param>
        /// <returns>The calculated answer as a string.</returns>
        private string calculateAnswer(string firstNumber, string secondNumber, string operation)
        {
            // Convert the string inputs to integers
            try
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
            catch (FormatException)
            {
                throw new FormatException("One or both of the input numbers are not valid integers.");
            }



        }

        // Clear the text boxes after sending a question --
        // consider removing this as it is only used in the SetText()
        // method after receiving a response from the client
        private void clearTextBoxes()
        {
            FirstNumberTextBox.Clear();
            SecondNumberTextBox.Clear();
            AnswerTextBox.Clear();
        }

        /// <summary>
        /// Method:     DisplayTable
        /// Desc:       Displays the list of math questions in the DataGridView.
        /// </summary>
        private void DisplayTable()
        {
            // Ensure data-binding is detached so Rows.Clear() won't throw an exception
            QuestionArrayDataGridView.DataSource = null;

            // Clear existing rows
            QuestionArrayDataGridView.Rows.Clear();

            if (mathQuesList == null || mathQuesList.Count == 0)
            {
                return;
            }

            // Add each question row using your GetStrArray() method
            for (int i = 0; i < mathQuesList.Count; i++)
            {
                QuestionArrayDataGridView.Rows.Add(mathQuesList[i].GetStrArray());
            }

            QuestionArrayDataGridView.Refresh();
        }

        /// <summary>
        /// Method:     UpdateBinaryTreeDisplay
        /// Desc:       Updates the display of the binary tree based on the specified traversal type.
        /// </summary>
        /// <param name="traversalType">The type of traversal to perform.</param>
        private void UpdateBinaryTreeDisplay(string traversalType)
        {
            if (mathQuestionBinTree.Count == 0)
            {
                BinaryTreeTextBox.Text = "The binary tree is empty. No questions have been asked yet.";
                return;
            }
            else
            {
                // Clear the tree's traversal string
                mathQuestionBinTree.TraversalString = "";

                // Perform the requested traversal starting from the root node
                switch (traversalType.ToUpper())
                {
                    case "PRE":
                        mathQuestionBinTree.Preorder(mathQuestionBinTree.GetRoot());
                        break;
                    case "IN":
                        mathQuestionBinTree.Inorder(mathQuestionBinTree.GetRoot());
                        break;
                    case "POST":
                        mathQuestionBinTree.Postorder(mathQuestionBinTree.GetRoot());
                        break;
                }

                // Trim trailing commas/spaces
                string formattedResult = mathQuestionBinTree.TraversalString.TrimEnd(',');

                // Update the text box
                BinaryTreeTextBox.Text = $"{traversalType.ToUpper()}-ORDER: {formattedResult}.";
            }
        }


        /// <summary>
        /// Method:     StartServer.
        /// Desc:       Starts the TCP server to listen for incoming connections.
        /// </summary>
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

        /// <summary>
        /// Method:     ReceiveStream.
        /// Desc:       Receives the stream of data from the client and updates state.
        /// </summary>
        public void ReceiveStream()
        {
            byte[] bytesReceived = new byte[BYTE_SIZE];

            while (!exitStatus)
            {
                try
                {
                    int bytesRead = netStream.Read(bytesReceived, 0, bytesReceived.Length);

                    // 0 bytes read indicates the client gracefully disconnected
                    if (bytesRead == 0)
                    {
                        Console.WriteLine("Client disconnected gracefully.");
                        exitStatus = true;
                        break;
                    }

                    string receivedText = Encoding.ASCII.GetString(bytesReceived, 0, bytesRead);
                    this.SetText(receivedText);
                }
                catch (Exception ex)
                {
                    // Catches IOException, SocketException, ObjectDisposedException on disconnect
                    Console.WriteLine($"Client connection lost: {ex.Message}");
                    exitStatus = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Method:     SetText.
        /// Desc:       Updates UI and data structures based on client response.
        /// </summary>
        /// <param name="text"></param>
        private void SetText(string text)
        {
            // Prevent UI updates if the form is closing or disposed
            if (this.IsDisposed || this.Disposing) return;

            if (this.BinaryTreeTextBox.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(text)) return;

                string trimmedText = text.Trim();

                if (trimmedText.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    // Student answered correctly
                    clearTextBoxes();
                    SendButton.Enabled = true;
                }
                else if (trimmedText.Equals("n", StringComparison.OrdinalIgnoreCase))
                {
                    // Student answered incorrectly
                    if (currentQuestion != null)
                    {
                        mathQuesLinkedList.AddFirst(currentQuestion);
                        incorrectAnswerCount = 1;
                        DisplayLinkedList();
                    }

                    clearTextBoxes();
                    SendButton.Enabled = true;
                }
            }
        }


        private void PreOrderDisplayButton_Click(object sender, EventArgs e)
        {
            UpdateBinaryTreeDisplay("PRE");
        }

        private void InOrderDisplayButton_Click(object sender, EventArgs e)
        {
            UpdateBinaryTreeDisplay("IN");
        }

        private void PostOrderDisplayButton_Click(object sender, EventArgs e)
        {
            UpdateBinaryTreeDisplay("POST");
        }

        /// <summary>
        /// Method:     DisplayLinkedListButton_Click
        /// Desc:       Handles the click event for the DisplayLinkedListButton. Displays the linked list of math questions.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void DisplayLinkedListButton_Click(object sender, EventArgs e)
        {

            isVisible = true;

            if (mathQuesList.Count == 0)
            {
                LinkedListTextBox.Text = "No math questions answered";
                return;
            }
            
            if (incorrectAnswerCount == 1)
            {
                LinkedListTextBox.Text = "HEAD <->";
                foreach (var item in mathQuesLinkedList)
                {
                    LinkedListTextBox.Text += $" {item.ToQuestionStr()} <->";
                }
                LinkedListTextBox.Text += " TAIL";
            }
            else
            {
                LinkedListTextBox.Text = "There have been no incorrect answers provided.";
            }
        }

        /// <summary>
        /// Method:     DisplayLinkedList
        /// Desc:       Displays the linked list of math questions in the LinkedListTextBox.
        /// </summary>
        private void DisplayLinkedList()
        {
            if (isVisible)
            {
                LinkedListTextBox.Text = "HEAD <->";
                foreach (var item in mathQuesLinkedList)
                {
                    LinkedListTextBox.Text += $" {item.ToQuestionStr()} <->";
                }
                LinkedListTextBox.Text += " TAIL";
            }
        }



        /// <summary>
        /// Method:     ExitButton_Click
        /// Desc:       Handles the click event for the ExitButton. Sets the exit status and closes the server socket.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void ExitButton_Click(object sender, EventArgs e)
        {
            exitStatus = true;

            try
            {
                // Close stream and client socket
                netStream?.Close();
                serverSocket?.Close();

                // Stop accepting new connections
                serverListener?.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing server socket: {ex.Message}");
            }

            // Wait briefly for the server receive thread to finish
            if (serverThread != null && serverThread.IsAlive)
            {
                serverThread.Join(500);
            }

            // Safely close the Windows Form
            this.Close();
        }

        /// <summary>
        /// Method:     OnFormClosing.
        /// Desc:       Overrides the default form closing behavior to ensure proper cleanup.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            exitStatus = true;

            try
            {
                netStream?.Close();
                serverSocket?.Close();
                serverListener?.Stop();

                if (serverThread != null && serverThread.IsAlive)
                {
                    serverThread.Join(200); // Wait briefly for the loop to terminate
                }
            }
            catch
            {
                // Ignore cleanup exceptions on application shutdown
            }
        }

        /// <summary>
        /// Method:     BubbleSortAscButton_Click()
        /// Desc:       Handles the click event for the BubbleSortAscButton. Sorts the math questions in ascending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void BubbleSortAscButton_Click(object sender, EventArgs e)
        {
            
            if (mathQuesList.Count == 0)
            {
                MessageBox.Show("No math questions to sort.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            // 1. Sort the list (modifies mathQuesList)
            mathQuesList = Sorting.BubbleSortAscending(mathQuesList).ToList();

            // 2. Refresh the display manually (NO DataSource assignment!)
            DisplayTable();
        }

        /// <summary>
        /// Method:     BubbleSortDescButton_Click()
        /// Desc:       Handles the click event for the BubbleSortDescButton. Sorts the math questions in descending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void BubbleSortDescButton_Click(object sender, EventArgs e)
        {

            if (mathQuesList.Count == 0)
            {
                MessageBox.Show("No math questions to sort.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            mathQuesList = Sorting.BubbleSortDescending(mathQuesList).ToList();
            DisplayTable();
        }

        /// <summary>
        /// Method:     InsertionSortButton_Click()
        /// Desc:       Handles the click event for the InsertionSortButton. Sorts the math questions in ascending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void InsertionSortButton_Click(object sender, EventArgs e)
        {

            if (mathQuesList.Count == 0)
            {
                MessageBox.Show("No math questions to sort.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            mathQuesList = Sorting.InsertionSortAscending(mathQuesList).ToList();
            DisplayTable();
        }



        // The following three methods handle the click events for the PreOrder, InOrder, and PostOrder save buttons.
        // They update the binary tree display and then write the traversal data to a file.
        //
        // I am updating the TreeDisplay first to make sure the TraversalString is in the correct format before saving
        // it to the file. Otherwise, you could end up saving an empty or incorrectly formatted string if the user
        // hasn't clicked the display button first.

        private void PreOrderSaveButton_Click(object sender, EventArgs e)
        {
            UpdateBinaryTreeDisplay("PRE");
            WriteFile();
        }

        private void InOrderSaveButton_Click(object sender, EventArgs e)
        {
            UpdateBinaryTreeDisplay("IN");
            WriteFile();
        }

        private void PostOrderSaveButton_Click(object sender, EventArgs e)
        {
            UpdateBinaryTreeDisplay("POST");
            WriteFile();
        }

        /// <summary>
        /// Method:     WriteFile();
        /// Desc:       Saves the binary tree traversal data to a file.
        /// </summary>
        private void WriteFile()
        {
            // Check if there is actual traversal data ready to save
            if (string.IsNullOrWhiteSpace(mathQuestionBinTree.TraversalString))
            {
                MessageBox.Show("No tree data available to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Grab the existing string, clean trailing commas, and save directly
            string formattedData = $"Binary Tree Data: \n{mathQuestionBinTree.TraversalString.TrimEnd(' ', ',')}";

            string filePath = "SavedMathTree.txt";
            bool success = FileIO.SaveToFile(filePath, formattedData);

            if (success)
            {
                MessageBox.Show($"File saved successfully to {Path.GetFullPath(filePath)}");
            }
        }

        private void BinarySearchButton_Click(object sender, EventArgs e)
        {
            if (mathQuesList.Count == 0)
            {
                MessageBox.Show("No math questions available for search.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else if (string.IsNullOrEmpty(BinaryTreeInputTextBox.Text))
            {
                MessageBox.Show("Please enter a value to search for in the binary tree.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {

                string[] searchKey = BinaryTreeInputTextBox.Text.Split(' ');

                if (searchKey.Length != 5)
                {
                    MessageBox.Show("Incorrect search format. \nPlease use the following example format: '3 + 4 = 7'", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    // check the formatting of the question - i.e. "3 + 4 = 7" is valid, but "3 + 4 = seven" is not valid
                    bool validSearchKey = SearchKeyFormat(searchKey);

                    if (validSearchKey)
                    {
                        mathQuesList = Sorting.InsertionSortAscending(mathQuesList).ToList();

                        int foundIndex = BinarySearch();

                        if (foundIndex != -1)
                        {
                            MessageBox.Show($"Value found at index {foundIndex}", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Value not found in the binary tree.", "Search Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                }
            }
        }


        // Binary Search
        private int BinarySearch()
        {
            int posFound = -1;
            bool foundStatus = false;
            int first = 0;
            int last = mathQuesList.Count - 1;
            int mid;

            string[] splitArray = BinaryTreeInputTextBox.Text.Split(' ');
            int ansToSearch = int.Parse(splitArray[4]);

            while(!foundStatus && first <= last) {
                mid = (first + last) / 2;

                if (ansToSearch < mathQuesList[mid].Answer)
                {
                    last = mid - 1;
                }
                else if (ansToSearch > mathQuesList[mid].Answer)
                {
                    first = mid + 1;
                }
                else
                {
                    foundStatus = true;
                    posFound = mid;
                }
            }
            return posFound;
        }




        private bool SearchKeyFormat(string[] key)
        {
            // Ensure the array has exactly 5 parts to prevent IndexOutOfRangeException
            if (key == null || key.Length != 5)
            {
                MessageBox.Show("Invalid search format. Please ensure you are using the correct format: '3 + 4 = 7'", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string[] operators = { "+", "-", "x", "/" };

            if (int.TryParse(key[0], out int leftOperand) &&
                operators.Contains(key[1]) &&
                int.TryParse(key[2], out int rightOperand) &&
                key[3] == "=" &&
                int.TryParse(key[4], out int answer))
            {
                Console.WriteLine("Valid search format detected.");
                return true;
            }
            else
            {
                MessageBox.Show("Invalid search format. Please ensure you are using the correct format: '3 + 4 = 7'", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
    }
}

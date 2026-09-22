# MathChallenge Application

A networked desktop application developed as a TAFE Diploma assessment. The system consists of two separate GUI windows—an **Instructor** interface and a **Student** interface—that transmit math questions and answers back and forth over a network connection.

---

## Key Features

* **Dual-GUI Client/Server Setup:** 
  * **Instructor Window:** Sends questions over the network and monitors incoming responses.
  * **Student Window:** Receives questions, allows input, and submits answers back to the instructor.

* **DataGrid Display:** 
  * Displays questions in a clear grid format, separated into individual operands and operators.

* **Data Structures:** 
  * **List & Binary Search:** All active questions are stored in a primary `List` that supports `binarySearch` for quick lookup.
  * **LinkedList:** Incorrect answers are captured and stored in a `LinkedList` for tracking and review.

* **Custom Sorting Algorithms:** 
  * The DataGrid contents can be sorted manually using three distinct algorithms:
    * **Bubble Sort**
    * **Selection Sort**
    * **Insertion Sort**

---

## How It Works

1. Launch the **Instructor** window to host the session.
2. Launch the **Student** window and connect over the local network.
3. Questions are sent from the instructor to the student.
4. As the student submits answers, correct responses update score states while incorrect answers are routed into the `LinkedList`.
5. Use the GUI sorting options to reorder the questions displayed in the DataGrid using Bubble, Selection, or Insertion sort.

---

## Built With

* **Language/Framework:** C# / .NET (WPF or Windows Forms)
* **Networking:** Sockets / TCP Communication
* **Data Structures & Algorithms:** `List`, `LinkedList`, Binary Search, Custom Sorting Algorithms

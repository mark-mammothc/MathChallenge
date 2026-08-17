using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathQuestionChallenge
{
    class MathQuestion : IComparable<MathQuestion>
    {
        // public properties (for get() and set() access)
        public int LeftOperand { get; set; }
        public string MathOperator { get; set; }
        public int RightOperand { get; set; }
        public int Answer { get; set; }

        // constructor method
        public MathQuestion(int leftOperand, string mathOperator, int rightOperand, int answer)
        {
            this.LeftOperand = leftOperand;
            this.MathOperator = mathOperator;
            this.RightOperand = rightOperand;
            this.Answer = answer;
        }

        // returns 0 if answers are the same for the 2 questions
        // returns -1 if this.Answer is < otherMathQues.Answer
        // returns 1 if this.Answer is > otherMathQues.Answer
        // Note: BinaryTree<MathQues> rejects questions with the same answer
        // so, 3 + 4 = 7 and 8 - 1 = 7 are effectively the same as far as the BinaryTree
        // is concerned.
        public int CompareTo(MathQuestion otherMathQues)
        {
            return this.Answer.CompareTo(otherMathQues.Answer);
        }

        // returns string format: "3 + 4 = 7"
        // // use this to send the math q from the instructor to the student
        // // also - use this method for hash search (hashtable Key)
        public string ToQuestionStr()
        {
            return this.LeftOperand.ToString() + " " + this.MathOperator + " " + this.RightOperand.ToString() + " = " + this.Answer.ToString();
        }

        // returns string format: "7(3+4)"
        // use this method for the binary tree
        public override string ToString()
        {
            return this.Answer.ToString() + "(" + this.LeftOperand.ToString() + this.MathOperator + this.RightOperand.ToString() + ")";
        }

        public string[] GetStrArray()
        {
            string[] strArray = new string[5];
            strArray[0] = LeftOperand.ToString();
            strArray[1] = MathOperator;
            strArray[2] = RightOperand.ToString();
            strArray[3] = "=";
            strArray[4] = Answer.ToString();

            return strArray;
        }


    }

}

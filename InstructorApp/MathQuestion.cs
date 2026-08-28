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

        /// <summary>
        /// Method:     Constructor for MathQuestion class
        /// Desc:       Initializes a new instance of the MathQuestion class with the specified left operand, math operator, right operand, and answer.
        /// </summary>
        /// <param name="leftOperand"></param>
        /// <param name="mathOperator"></param>
        /// <param name="rightOperand"></param>
        /// <param name="answer"></param>
        public MathQuestion(int leftOperand, string mathOperator, int rightOperand, int answer)
        {
            this.LeftOperand = leftOperand;
            this.MathOperator = mathOperator;
            this.RightOperand = rightOperand;
            this.Answer = answer;
        }

        /// <summary>
        /// Method:         CompareTo(MathQuestion otherMathQues)
        /// Description:    Compares this math question with another math question based on their answers.
        /// </summary>
        /// <param name="otherMathQues"></param>
        /// <returns></returns>
        public int CompareTo(MathQuestion otherMathQues)
        {
            return this.Answer.CompareTo(otherMathQues.Answer);
        }

        
        /// <summary>
        /// Method:         ToQuestionStr()
        /// Description:    Returns a string representation of the math question in the format "leftOperand operator rightOperand = answer".
        /// </summary>
        /// <returns></returns>
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

namespace BSearchUnitTest
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

        public int CompareTo(MathQuestion otherMathQues)
        {
            return this.Answer.CompareTo(otherMathQues.Answer);
        }

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

    public class Tests
    {
        List<MathQuestion> mathQuesList = new List<MathQuestion>();

        private int BinarySearch(string mathQuestionStr)
        {
            int posFound = -1;
            bool foundStatus = false;
            int first = 0;
            int last = mathQuesList.Count - 1;
            int mid;

            string[] splitArray = mathQuestionStr.Split(' ');
            int ansToSearch = int.Parse(splitArray[4]);

            while (!foundStatus && first <= last)
            {
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

        [SetUp]
        public void Setup()
        {
            mathQuesList.Add(new MathQuestion(3, "+", 4, 7));
            mathQuesList.Add(new MathQuestion(3, "+", 4, 12));
            mathQuesList.Add(new MathQuestion(3, "+", 4, 2));
            mathQuesList.Add(new MathQuestion(3, "+", 4, 14));
            mathQuesList.Sort();
        }

        [Test]
        // Test math question "3 + 4 = 12" is at index 3 in the sorted list
        public void Test1()
        {
           int actualResult = BinarySearch("3 + 4 = 12");
           int expectedResult = 2;
           Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        // Test math question "3 + 4 = 12" is at index 3 in the sorted list
        public void Test2()
        {
            int actualResult = BinarySearch("7 + 10 = 21");
            int expectedResult = -1;
            Assert.AreEqual(expectedResult, actualResult);
        }



    }
}
using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;


using PNNLProteomics.Data.Factors;
using MultiAlignWin.Forms.Factors;

namespace Test
{
    public class TestFactorsProgram
    {
        static void Print(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Console.WriteLine(message);
        }

        /// <summary>
        /// Given the test name and the expected output with the received from
        /// the test, prints the passing or failed message.
        /// </summary>
        /// <param name="testname"></param>
        /// <param name="expected"></param>
        /// <param name="received"></param>
        static void Validate(string testname, object expected, object received)
        {
            if (expected.Equals(received) == true)
            {
                Print(string.Format("Test: {0} [PASSED]", testname));
            }
            else
            {
                Print(string.Format("Test: {0} [FAILED].  {1}->{2}", testname,
                                                                        expected,
                                                                        received));
            }
        }

        /// <summary>
        /// Regression test harness for the factor classes.
        /// </summary>
        public static void TestFactorClass()
        {
            /// 
            /// Test 1: Add a value without the factor
            /// 
            string badFactor = "fooboo";
            string newFactor = "boofoo";


            string value1 = "value1";
            classFactorDefinition factors = new classFactorDefinition();
            Validate("Add Value Without Factor",
                        classFactorDefinition.CONST_FACTOR_DOES_NOT_EXIST,
                        factors.AddFactorValue(badFactor, value1));


            /// 
            /// Test 0: Is Defined
            ///                         
            Validate("Is Defined - No Factors",
                        true,
                        factors.FullyDefined);

            /// 
            /// Test 2: Add a factor
            ///                         
            Validate("Add Factor",
                        classFactorDefinition.CONST_FACTOR_ADDED,
                        factors.AddFactor(badFactor));

            /// 
            /// Test 3: Add a factor
            ///                         
            Validate("Add Existing Factor",
                        classFactorDefinition.CONST_FACTOR_EXISTS,
                        factors.AddFactor(badFactor));

            /// 
            /// Test 4: Add a factor value
            ///                         
            Validate("Add Factor Value",
                        classFactorDefinition.CONST_FACTOR_VALUE_ADDED,
                        factors.AddFactorValue(badFactor, value1));

            /// 
            /// Test 5: Add a factor value
            ///                         
            Validate("Add Existing Factor Value",
                        classFactorDefinition.CONST_FACTOR_VALUE_EXISTS,
                        factors.AddFactorValue(badFactor, value1));

            /// 
            /// Test 6: Rename a factor value
            ///                         
            Validate("Rename Non-existent Factor",
                        classFactorDefinition.CONST_FACTOR_DOES_NOT_EXIST,
                        factors.RenameFactor(newFactor, newFactor));

            /// 
            /// Test 7: Rename existing to existing factor 
            ///                         
            Validate("Rename existing to existing Factor",
                        classFactorDefinition.CONST_FACTOR_NEW_EXISTS,
                        factors.RenameFactor(badFactor, badFactor));

            /// 
            /// Test 8: Rename existing to new factor
            ///                         
            Validate("Rename to new Factor",
                        classFactorDefinition.CONST_FACTOR_RENAMED,
                        factors.RenameFactor(badFactor, newFactor));

            /// 
            /// Test 9: Is Defined
            ///                         
            Validate("Is Defined",
                        false,
                        factors.FullyDefined);

            /// 
            /// Test 10: Add a factor value
            ///                         
            Validate("Add New Factor Value With Old Name",
                        classFactorDefinition.CONST_FACTOR_DOES_NOT_EXIST,
                        factors.AddFactorValue(badFactor, value1 + "new"));
            /// 
            /// Test 11: Add a factor value
            ///                         
            Validate("Add New Factor Value With New Name",
                        classFactorDefinition.CONST_FACTOR_VALUE_ADDED,
                        factors.AddFactorValue(newFactor, value1 + "new"));

            /// 
            /// Test 12: Is Defined
            ///                         
            Validate("Is Defined - true",
                        true,
                        factors.FullyDefined);


        }

        /// <summary>
        /// Tests the form controls loading of factor classes via constructor and properties.
        /// </summary>
        public static void TestNewFactorsForm()
        {
            /// 
            /// New form
            /// 
            frmFactorDefinition formFactors = new frmFactorDefinition();
            DialogResult result = formFactors.ShowDialog();
            classFactorDefinition factors = formFactors.Factors;
            Print(string.Format("\t\tTest New Factor Dialog Result: {0}", result));

            /// 
            /// Constructor
            /// 
            frmFactorDefinition formFactors2 = new frmFactorDefinition(factors);
            DialogResult result2 = formFactors2.ShowDialog();
            classFactorDefinition factors2 = formFactors2.Factors;
            Print(string.Format("\t\tTest Constructor Factors Dialog Result: {0}", result2));

            /// 
            /// Property
            /// 
            frmFactorDefinition formFactors3 = new frmFactorDefinition();
            formFactors3.Factors = factors2;
            DialogResult result3 = formFactors3.ShowDialog();
            classFactorDefinition factors3 = formFactors3.Factors;
            Print(string.Format("\t\tTest PropertyFactors Dialog Result: {0}", result3));
        }
    }
}

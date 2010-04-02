using System;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;

using Test.PeakMatching;
using Test.MTDBLoading;

namespace Test
{
    public class Program
    {
        static bool testMTDB = true;

        static void Main(string[] args)
        {
            if (false)
            {

                /*Test.Factors.Form1 f = new TestFactors.Factors.Form1();
                f.ShowDialog();

                bool boolTestFactors = false;
                bool boolTestHeatmap = false;

                /// 
                /// Factor tests
                /// 
                if (boolTestFactors == true)
                {
                    Test.TestFactorsProgram.TestFactorClass();
                    Test.TestFactorsProgram.TestNewFactorsForm();
                }*/

                /// 
                /// Heatmap tests
                ///
                /*
                if (boolTestHeatmap == true)
                {
                    Test.classTestHeatMap.TestHeatMap();
                }*/
            }

            if (true)
            {
                PeakMatchingTest test = new PeakMatchingTest();
                test.RunTests();
            }
            if (testMTDB)
            {
                MTDBLoaderAccessTest test = new MTDBLoaderAccessTest();
                test.RunTests();
            }

            Console.WriteLine("Exit-Press Key");
            Console.ReadKey();
        }
    }
}

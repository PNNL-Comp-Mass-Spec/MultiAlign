using System;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;

using PNNLControls.Drawing.Plotting;

namespace Test
{
    public class classTestHeatMap
    {
        /// <summary>
        /// Prints a message to the Console.
        /// </summary>
        /// <param name="message"></param>
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
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateSimpleData(int cols, int rows, float value)
        {
            float[,] data = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    data[i, j] = value;
                }
            }

            return data;
        }


        /// <summary>
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateRandomData(int cols, int rows)
        {
            float[,] data = new float[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Random r = new Random();
                    data[i, j] = Convert.ToSingle(r.NextDouble());
                }
            }

            return data;
        }

        /// <summary>
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateSteps()
        {
            float[,] data   = new float[4, 4];
            
            data[0,0] = 1; 
            data[0,1] = 1; 
            data[0,2] = 1; 
            data[0,3] = 1; 
            
            data[1,0] = 2; 
            data[1,1] = 2; 
            data[1,2] = 2; 
            data[1,3] = 2; 
            
            data[2,0] = 3; 
            data[2,1] = 3; 
            data[2,2] = 3; 
            data[2,3] = 3; 

            data[3,0] = 4; 
            data[3,1] = 4; 
            data[3,2] = 4; 
            data[3,3] = 4; 

            return data;
        }

        /// <summary>
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateBlocks()
        {
            float[,] data = new float[4, 4];

            data[0, 0] = 1;
            data[0, 1] = 1;
            data[0, 2] = 2;
            data[0, 3] = 2;

            data[1, 0] = 1;
            data[1, 1] = 1;
            data[1, 2] = 2;
            data[1, 3] = 2;

            data[2, 0] = 3;
            data[2, 1] = 3;
            data[2, 2] = 4;
            data[2, 3] = 4;

            data[3, 0] = 3;
            data[3, 1] = 3;
            data[3, 2] = 4;
            data[3, 3] = 4;

            return data;
        }

        /// <summary>
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateSquare()
        {
            float[,] data = new float[4, 4];

            data[0, 0] = 1;
            data[0, 1] = 2;
            data[0, 2] = 1;
            data[0, 3] = 2;

            data[1, 0] = 3;
            data[1, 1] = 4;
            data[1, 2] = 3;
            data[1, 3] = 4;

            data[2, 0] = 1;
            data[2, 1] = 2;
            data[2, 2] = 1;
            data[2, 3] = 2;

            data[3, 0] = 3;
            data[3, 1] = 4;
            data[3, 2] = 3;
            data[3, 3] = 4;

            return data;
        }


        /// <summary>
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateSequence()
        {
            float[,] data = new float[4, 4];

            data[0, 0] = 1;
            data[0, 1] = 2;
            data[0, 2] = 3;
            data[0, 3] = 4;

            data[1, 0] = 5;
            data[1, 1] = 6;
            data[1, 2] = 7;
            data[1, 3] = 8;

            data[2, 0] = 9;
            data[2, 1] = 10;
            data[2, 2] = 11;
            data[2, 3] = 12;

            data[3, 0] = 13;
            data[3, 1] = 14;
            data[3, 2] = 15;
            data[3, 3] = 16;

            return data;
        }



        /// <summary>
        /// Generates a set of simple data.
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        static float[,] GenerateUneven()
        {
            float[,] data = new float[3, 3];

            data[0, 0] = 1;
            data[0, 1] = 0;
            data[0, 2] = 1;
            data[1, 0] = 0;
            data[1, 1] = 0;
            data[1, 2] = 0;
            data[2, 0] = 1;
            data[2, 1] = 0;
            data[2, 2] = 1;
            return data;
        }

        private static void TestInterpolation()
        {

            classHeatMap map = new classHeatMap();
            map.InterpolationMode = HeatMapInterpolationMode.Average;

            float[,] idata  = null;
            float[,] odata  = null;

            idata = GenerateSimpleData(4, 4, 1);
            odata = map.InterpolateData(idata, 4, 4);

            idata = GenerateSimpleData(4, 4, 1);
            odata = map.InterpolateData(idata, 2, 2);

            idata = GenerateSquare();
            odata = map.InterpolateData(idata, 2, 2);

            idata = GenerateSquare();
            odata = map.InterpolateData(idata, 1, 1);

            idata = GenerateSteps();
            odata = map.InterpolateData(idata, 2, 2);

            idata = GenerateBlocks();
            odata = map.InterpolateData(idata, 2, 2);

            idata = GenerateSequence();
            odata = map.InterpolateData(idata, 2, 2);

            idata = GenerateSequence();
            odata = map.InterpolateData(idata, 1, 1);
            
            idata = GenerateSequence();
            odata = map.InterpolateData(idata, 1, 2);
            
            idata = GenerateUneven();
            odata = map.InterpolateData(idata, 2, 2);

            idata = GenerateSimpleData(2, 4, 1);
            odata = map.InterpolateData(idata, 3, 2);

            idata = GenerateRandomData(1000, 1000);
            controlHeatMap heatMap = new controlHeatMap();
            heatMap.Data = idata;
            heatMap.Dock = DockStyle.Fill;

            Form heatForm = new Form();
            heatForm.Size = new System.Drawing.Size(120, 120);
            heatForm.Controls.Add(heatMap);
            heatForm.ShowDialog();

        }

        /// <summary>
        /// Displays a simple heat map
        /// </summary>
        public static void TestHeatMap()
        {
            TestInterpolation();            
        }
    }
}

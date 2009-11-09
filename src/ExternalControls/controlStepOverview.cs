using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ExternalControls
{
    public partial class controlStepOverview : UserControl
    {
        /// <summary>
        /// Label for handling the current step.
        /// </summary>
        private Label mlabel_currentStep;
        /// <summary>
        /// List of labels for handling the current step message.
        /// </summary>
        private List<Label> mlist_stepLabels;


        /// <summary>
        /// Default construtor.
        /// </summary>
        public controlStepOverview()
        {
            InitializeComponent();

            mlist_stepLabels = new List<Label>();
        }

        /// <summary>
        /// Displays the list of steps that will be performed.
        /// </summary>
        /// <param name="steps"></param>
        public void DisplayListOfSteps(List<string> steps)
        {
            /// 
            /// Clear the current steps if any.
            /// 
            mlist_stepLabels.Clear();
            mlabel_currentStep = null;
            mpanel_steps.Controls.Clear();

            /// 
            /// Then create a new label for each step.
            /// 
            int i = 1;
            foreach (string step in steps)
            {
                Label stepLabel = new Label();
                stepLabel.Font = new Font("Agency FB", 13.0f, FontStyle.Regular);
                stepLabel.ForeColor = Color.Gray;
                stepLabel.AutoSize = true;
                stepLabel.Text = string.Format("Step {0}. {1}", i, step);
                stepLabel.Dock = DockStyle.Left;

                /// 
                /// Make sure it gets added to the control for display
                /// and to our list for later dynamic editing 
                /// when a step is selected.              
                /// 
                mpanel_steps.Controls.Add(stepLabel);
                stepLabel.BringToFront();

                mlist_stepLabels.Add(stepLabel);
                i++;
            }
        }

        /// <summary>
        /// Displays the current step.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="stepName"></param>
        public void SetStep(int index, string stepName)
        {
            /// 
            /// Reset the old label.
            /// 
            if (mlabel_currentStep != null)
            {
                mlabel_currentStep.ForeColor = Color.LightGray;
                mlabel_currentStep.Font = new Font(mlabel_currentStep.Font,
                                                         FontStyle.Regular);
            }

            /// 
            /// Highlight the new label;
            /// 
            Label label     = mlist_stepLabels[index];
            label.ForeColor = Color.Black;
            label.Font      = new Font(label.Font, FontStyle.Bold);
            
            mlabel_currentStep = label;
        }
    }
}

using System;
using System.IO;
using System.Data;
using System.Drawing;
using System.Reflection; 
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices; 

using AgentObjects;
using AgentServerObjects;

namespace PNNLControls
{
    /// <summary>
    /// Merlin Help Wizard class.
    /// </summary>
    public class clsHelpWizard: IDisposable
    {
        #region Members
        private const string CONST_ANIMATE_REST = "Greet";
        private const string CONST_ANIMATE_GREET = "RestPose";
        private const string CONST_ANIMATE_THINK = "Think";
        private const string CONST_ANIMATE_SEARCH = "Searching";
        private const string CONST_ANIMATE_CONGRATS = "Congratulate";
        private const string CONST_ANIMATE_PROCESSING = "Processing";
        private const string CONST_ANIMATE_DO_MAGIC = "DoMagic1";
        private const string CONST_ANIMATE_SAD = "Sad";
        private const string CONST_NAME = "Merlin";
        private const string CONST_NAME_FILE_EXTENSION = ".acs";

        #region Agent Members
        IAgentCharacterEx   mobj_characterEx;
        AgentServer         mobj_server;
        IAgentEx            mobj_serverEx;
        #endregion            

        List<string>        mlist_commands;
        /// <summary>
        /// Agent character filename.
        /// </summary>
        private string mstring_agentFileName;
        /// <summary>
        /// Path to agent character file.
        /// </summary>
        private string mstring_agentPath;
        /// <summary>
        ///  The last reqest identifier for an animation.
        /// </summary>
        private int mint_lastRequestID;
        /// <summary>
        /// Flag indicating if the wizard is enabled and visible.
        /// </summary>
        private bool mbool_enabled;
        #endregion

        /// <summary>
        /// Tells the wizard whether to chill or not.
        /// </summary>
        private bool mbool_chill;

        /// <summary>
        /// Default constructor for a new character agent.
        /// </summary>
        /// <param name="helpFilePath">Path to where the character resource file is located.</param>
		public clsHelpWizard(string helpFilePath)
		{
            mstring_agentFileName   = CONST_NAME + CONST_NAME_FILE_EXTENSION;
            mstring_agentPath       = helpFilePath;

            mbool_enabled = true;
            LoadAgent();

        
            mbool_chill = false;
        }

        /// <summary>
        /// Gets or sets whether the wizard should perform actions 
        /// or just hang out where the user asks.
        /// </summary>
        public bool Chill
        {
            get
            {
                return mbool_chill;
            }
            set
            {
                mbool_chill = value;
            }
        }


        #region Loading and Properties
        /// <summary>
        /// Load the agent content file.
        /// </summary>
        private void LoadAgent()
        {
            mobj_server = new AgentServer();
            if (mobj_server == null)
            {
                return;
            }

            ///  
            ///  The following cast does the QueryInterface to fetch
            ///  IAgentEx interface from the IAgent interface, directly supported by the object
            ///  
            mobj_serverEx = (IAgentEx)mobj_server;

            /// 
            /// First try to load the default character
            /// 
            int dwCharID = 0;
            int dwReqID  = 0;

            try
            {
                ///  
                ///  Null is used where VT_EMPTY variant is expected by the COM object
                ///  
                string strAgentCharacterFile = null;
                if (!mstring_agentFileName.Equals(string.Empty))
                {
                    strAgentCharacterFile = Path.Combine(mstring_agentPath, mstring_agentFileName);
                }
                mobj_serverEx.Load(strAgentCharacterFile, out dwCharID, out dwReqID);
            }
            catch (Exception)
            {
                return;
            }
            mobj_serverEx.GetCharacterEx(dwCharID, out mobj_characterEx); 
            

            if (mlist_commands == null)
                mlist_commands = new List<string>();
            else
                mlist_commands.Clear();

            lock (this)
            {
                IEnumerator enumerator;
                object o;
                mobj_characterEx.GetAnimationNames(out o);
                enumerator = o as IEnumerator;                                        
                while (enumerator.MoveNext())
                {
                    string command = (string) enumerator.Current;
                    mlist_commands.Add(command);
                } 
            }
        }
        /// <summary>
        /// Gets or sets whether the help wizard is enabled (visible) or disabled (invisible).
        /// </summary>
        public bool Enabled
        {
            get
            {
                return mbool_enabled;
            }
            set
            {
                mbool_enabled = value;
                if (value == false)
                    HideAgent();
                else
                    ShowAgent();
            }
        }
        /// <summary>
        /// Gets the list of available animations.
        /// </summary>
        public List<string> Animations
        {
            get
            {
                return mlist_commands;
            }
        }
        #endregion

        #region Animations
        /// <summary>
        /// Animates the character to do a dance of some sort.
        /// </summary>
        public void Dance()
        {
            Animate(CONST_ANIMATE_DO_MAGIC);
        }
        public void Rest()
        {
            Animate(CONST_ANIMATE_REST);
        }
        public void Think()
        {
            Animate(CONST_ANIMATE_THINK);
        }
        public void Search()
        {
            Animate(CONST_ANIMATE_SEARCH);
        }
        public void Greet()
        {
            Animate(CONST_ANIMATE_GREET);
        }
        public void Congratulate()
        {
            Animate(CONST_ANIMATE_CONGRATS);
        }
        public void Process()
        {
            Animate(CONST_ANIMATE_PROCESSING);
        }
        public void Sad()
        {
            Animate(CONST_ANIMATE_SAD);
        }
        public void Animate(string animateName)
        {
            if (Chill == false)
            {
                if (mlist_commands.Contains(animateName) == true)
                {
                    if (mbool_enabled == true)
                    {
                        Stop();
                        mobj_characterEx.Play(animateName, out mint_lastRequestID);
                    }
                }
            }
        }
        #endregion

        #region Control
        public void Stop()
        {            
            if (mbool_enabled == true)
            {
                try
                {                    
                    mobj_characterEx.Stop(mint_lastRequestID);                    
                }
                catch
                {
                }
            }
        }        
        public void SetPosition(int left, int top)
        {
            mobj_characterEx.SetPosition(left, top);
        }
		/// <summary>
		/// Tells the agent to speak.
		/// </summary>
		/// <param name="message">Text to speak.</param>
        public void Speak(string message)
        {            
            Stop();

            if (Chill == false)
            {

                if (mbool_enabled == false)
                    return;

                if (message.Trim() == "")
                    return;

                if (mobj_characterEx == null)
                    return;

                Stop();

                mobj_characterEx.Speak(message, null, out mint_lastRequestID);
            }
		}
        /// <summary>
        /// Moves the Wizard to the specified location at the given speed.
        /// </summary>
        /// <param name="x">Screen X-position</param>
        /// <param name="y">Screen Y-position</param>
        /// <param name="speed">Speed to move the character.</param>
        public void MoveTo(short x, short y, int speed)
        {
            if (Chill == false)
            {
                int dwReqID = 0;
                mobj_characterEx.MoveTo(x, y, speed, out dwReqID);                
            }
        }
		/// <summary>
		/// Hide Agent.
		/// </summary>
		public void HideAgent()
		{
			int dwReqID = 0;			
            mobj_characterEx.Hide(0, out dwReqID);					
		}
		/// <summary>
		/// Show Agent.
		/// </summary>
		public void ShowAgent()
        {
            if (mbool_enabled == false)
                return;

			int dwReqID = 0;
			mobj_characterEx.Show(0, out dwReqID);
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            HideAgent();
        }
        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace WindowsFormsApplication2
{
    public partial class frmPalette : Form
    {
        #region Class Variables
        public const string TITLENAME = "Palette Controller";
        public int[] palette = new int[16];//array of module IDs
        public int activeProfile;//remembers the active profile
        public int activeProgram;//remembers the active program
        public string RxString; //used for serial communication
        public int row, col = 0;

        List<Profile> profileList = new List<Profile>();//lists all of the existing profiles
        List<List<int>> genericsList = new List<List<int>>();//list of profiles; list of locations which contain modules programmed to be generics

        System.Windows.Forms.Button[] btnArray = new System.Windows.Forms.Button[16];//used to draw buttons
        System.Windows.Forms.Label[] lblArray = new System.Windows.Forms.Label[16];//used to draw labels

        //TODO: this should be a list
        System.Windows.Forms.TabPage[] tabArray = new System.Windows.Forms.TabPage[25];//used to draw tabs for profiles
        #endregion

        public frmPalette()
        {
            InitializeComponent();
            tabControl1.Selecting += new TabControlCancelEventHandler(tabControl1_Selecting);

            CreateDictionary("Programs.txt");
            InitializeConfiguration();
            //Calls for functions
        }

        #region Threads
        private void Serial_Connection()
        {
            serialPort1.PortName = "COM7";
            serialPort1.BaudRate = 57600;
            
            try
            {
                while(true)
                {
                    serialPort1.Open();
                }

            }
            catch(ThreadAbortException ex){}
            catch(Exception ex)
            {
                MessageBox.Show("Error: Serial communication cannot be established. Original error: " + ex.Message, TITLENAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            /*byte[] c = new byte[] {
               0x01,
               0x00,
               0x00,
               0x02,
               0x37,
               0x30,
               0x04
            };
            serialPort1.Write(c, 0, c.Length);*/
        }
        #endregion

        #region Functions
        private void InitializeConfiguration()
        {
            //file containing location and ID of modules
            #region Read File 1
            string file = ReadFile("Config1.txt");//read current location and IDs
            
            //break data down into useful chunks
            char[] delimiters = new char[] { ',', '\r', '\n' };
            string[] data = file.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            row = Convert.ToInt32(data[0]);
            col = Convert.ToInt32(data[1]);
            int size = row * col;

            //build the palette array
            for (int i = 0; i < size; i++)
            {
                palette[i] = Convert.ToInt32(data[i + 2]);
            }
            #endregion

            //file containing information about module mappings
            #region Read File 2
            string file2 = ReadFile("Config2.txt"); //read current profile(s) and mappings
            
            //break data down into useful chunks
            data = file2.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int numProfiles = Convert.ToInt32(data[0]);

            //initialize vars needed for reading
            int index = 1, profileID, moduleID, actionID, numModules;
            string profileName, moduleAction;
            List<Module> moduleList = new List<Module>();

            int m = 0;
            while (m < numProfiles)//for each profile
            {
                moduleList.Clear(); 

                profileID = Convert.ToInt32(data[index++]);//grabs profile ID
                profileName = data[index++];//grabs profile name

                numModules = Convert.ToInt32(data[index++]);
                for (int i = 0; i<numModules;i++)
                {
                    moduleID = Convert.ToInt32(data[index++]);//grabs module ID
                    actionID = Convert.ToInt32(data[index++]);//grabs action ID
                    moduleAction = data[index++];//grabs action string
                    moduleList.Add(new Module(moduleID, actionID, moduleAction));
                }

                List<Module> tempCopyList = new List<Module>(moduleList);//copy the list so that when I clear moduleList, the profileList.moduleList doesn't clear too
                profileList.Add(new Profile(profileID, profileName, tempCopyList));

                m++;
            }
            #endregion

            buildPalette();
        }

        private Dictionary <string, ActionInfo> CreateDictionary(string fileName)//TODO: implement generics
        {
            int tempActionID;
            string tempName, temp;
            int tempProgramID = 0;
            List<string> tempActions = new List<string>();
            Dictionary<int, List<string>> GenericList = new Dictionary<int, List<string>>();
            string[] tempArray;

            //Create dictionary
            var programInfo = new Dictionary<string, ActionInfo> { };

            //Read data from file
            string file = ReadFile(fileName);

            //Process data from file
            char[] delimiters = new char[] { '\r','\n' };
            char[] delimiters2 = new char[] { ',' };
            string[] data = file.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int i = 0;
            while (i < data.Count())
            {
                //clear all Lists
                tempActions.Clear();

                //read data into respective temporary variables
                tempActionID = Convert.ToInt32(data[i++]);
                tempArray = data[i++].Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
                tempName = tempArray[0];

                //actions
                if (tempActionID == 0)//custom
                {
                    tempProgramID = Convert.ToInt32(data[i++]);
                    tempArray = data[i++].Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in tempArray)
                    {
                        tempActions.Add(item);
                    }
                    //add to dictionary;
                    programInfo.Add(tempActionID.ToString(), new ActionInfo(tempActionID, tempName, tempProgramID, tempActions));
                }
                else if (tempActionID <= 31)//generic
                {
                    while (true)
                    {
                        tempActions.Clear();
                        temp = data[i++];
                        if (temp == "END")
                        {
                            break;
                        }
                        tempProgramID = Convert.ToInt32(temp);
                        tempArray = data[i++].Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in tempArray)
                        {
                            tempActions.Add(item);
                        }
                        GenericList.Add(tempProgramID, tempActions);
                    }
                    //add to dictionary;
                    programInfo.Add(tempActionID.ToString(), new ActionInfo(tempActionID, tempName, GenericList));
                    GenericList.Clear();
                }
                else//regular
                {
                    tempProgramID = Convert.ToInt32(data[i++]);
                    tempArray = data[i++].Split(delimiters2, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in tempArray)
                    {
                        tempActions.Add(item);
                    }
                    //add to dictionary;
                    programInfo.Add(tempActionID.ToString(), new ActionInfo(tempActionID, tempName, tempProgramID, tempActions));
                }
            }

            return programInfo;
            //txtData.AppendText(programInfo.FirstOrDefault(x => x.Key == "1").Value.actionID.ToString());
        }

        private void moduleChanged(int location, int ID)//location & new ID are provided
        {
            //removes all copies of previous module from list of generics
            /*if (palette[location] > 0 && palette[location] < 32)
            {
                for (int i = 0; i < profileList.Count; i++)
                {
                    genericsList[i].RemoveAll(x => x == location);
                }
            }*/

            //make change in palette array
            palette[location] = ID;

            for (int i = 0; i < profileList.Count; i++)//for each profile in profileList
            {
                //transfer old module object out of first 16
                Module tempModule = profileList[i].moduleList[location];
                profileList[i].moduleList.Add(tempModule);//transfer to new location

                //replace old module with new module
                int index = profileList[i].moduleList.FindIndex(item => item.ID == ID);
                if (index >= 0)//use existing info for module
                {
                    profileList[i].moduleList[location] = new Module(ID, profileList[i].moduleList[index].actionID, profileList[i].moduleList[index].actions);//transfer new info to 
                }
                else//no existing info was found, create new empty module
                {
                    profileList[i].moduleList[location] = new Module(ID, 0, "");
                }
            }

            buildPalette();
        }
        #endregion

        #region Helper Functions
        private void buildPalette()
        {
            int size = row * col;

            tabControl1.TabPages.Clear();//clear the tabs completely.
            List<int> tempGenericsList = new List<int>(); 

            for (int i = 0; i < profileList.Count; i++)//for each profile in profileList
            {
                tempGenericsList.Clear();

                //create tab for profile
                string tabTitle = profileList[i].name;
                tabArray[i] = new System.Windows.Forms.TabPage(tabTitle);
                tabControl1.TabPages.Add(tabArray[i]);
                tabArray[i].Tag = profileList[i].ID.ToString();//save profileID in the tab's tag

                //Create imaginary array of buttons & labels for each profile
                #region Create Array of Buttons & Labels
                int xPos = 20;
                int yPos = 20;

                for (int j = 0; j < size; j++)
                {
                    btnArray[j] = new System.Windows.Forms.Button();
                    btnArray[j].FlatStyle = FlatStyle.Flat;
                    btnArray[j].FlatAppearance.BorderSize = 0;
                    lblArray[j] = new System.Windows.Forms.Label();
                    lblArray[j].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                }
                #endregion

                //Add information to buttons & labels
                #region Draw Buttons & Labels
                for (int n = 0; n < palette.Count(); n++ )//for each module in the palette
                {
                    btnArray[n].Width = 100; // Width of button 
                    btnArray[n].Height = 100; // Height of button 
                    lblArray[n].Width = 100;
                    lblArray[n].Height = 20;

                    if (n == row) // Location of second line of buttons: 
                    {
                        xPos = 20;
                        yPos = 160;
                    }
                    else if (n == (2 * row)) // Location of second line of buttons: 
                    {
                        xPos = 20;
                        yPos = 300;
                    }
                    else if (n == (3 * row)) // Location of second line of buttons: 
                    {
                        xPos = 20;
                        yPos = 440;
                    }

                    // Location of button: 
                    btnArray[n].Left = xPos;
                    btnArray[n].Top = yPos;
                    // Add buttons to a Panel: 
                    tabArray[activeProfile].Controls.Add(btnArray[n]); // Let panel hold the Buttons 

                    // Location of label: 
                    lblArray[n].Left = xPos;
                    lblArray[n].Top = yPos + 100;
                    // Add label to a Panel: 
                    tabArray[activeProfile].Controls.Add(lblArray[n]); // Let panel hold the Labels

                    xPos = xPos + btnArray[n].Width + 20; // Left of next button 

                    //Add information to buttons & labels
                    #region Add Info to Buttons & Labels
                    assignModuleImage(n);
                    btnArray[n].Tag = n;//add location to each button's tag
                    lblArray[n].Text = profileList[activeProfile].moduleList[n].actions;//writes action as text of label
                    btnArray[n].Click += new System.EventHandler(ClickButton);// the Event of click Button 
                    #endregion

                    #region Create List of Generics
                    if (profileList[i].moduleList[n].actionID > 0 && profileList[i].moduleList[n].actionID < 32)//find generics in the first 16 modules
                    {
                        tempGenericsList.Add(n);
                    }
                    #endregion
                }
                #endregion

                List<int> tempCopyList2 = new List<int>(tempGenericsList);
                genericsList.Add(tempCopyList2);//adds the list of generics for this profile

                activeProfile++;
            }

            //reset the active profile/tab
            activeProfile = 0;
            tabControl1.SelectedTab = tabArray[0];
        }

        private void assignModuleImage(int n)
        {
            if (palette[n] < 10)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\button.png");
            }
            else if (palette[n] < 20)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\knob.png");
            }
            else if (palette[n] < 30)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\joystick.png");
            }
            else
            {
                btnArray[n].Text = "Not Present";
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = null;
            }
        }
        
        // Result of (Click Button) event, get the text of button 
        private void ClickButton(Object sender, System.EventArgs e)
        {
            Button btn = (Button)sender;
            int num = Convert.ToInt32(btn.Tag);//gets the module ID for each module on the screen
            lblInfoName.Text = profileList[activeProfile].moduleList[num].type;
            lblInfoDescription.Text = profileList[activeProfile].moduleList[num].actions;
        }

        private void SendSerial(string text)
        {
            byte[] bytes = GetBytes(text);
            serialPort1.Write(bytes, 0, bytes.Length);
        }

        private void SendSerial(byte[] bytes)
        {
            serialPort1.Write(bytes, 0, bytes.Length);
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private string ReadFile(string name)
        {
            string something = System.IO.File.ReadAllText(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\" + name);
            return something;
        }
        #endregion

        #region Control Event Code
        void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabPage current = (sender as TabControl).SelectedTab;
            try
            {
                activeProfile = current.TabIndex;
            }
            catch { }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            RxString = serialPort1.ReadExisting();
            //this.Invoke(new EventHandler(DisplayText));
            txtData.AppendText(RxString);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] data = new byte[] {
               0x50
            };
            SendSerial(data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moduleChanged(0, 18);
        }

    }//public partial class namespace
} //namespace
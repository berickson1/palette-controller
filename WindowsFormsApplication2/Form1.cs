using System;
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

<<<<<<< HEAD
        Dictionary<int, string> ProgramDict = new Dictionary<int, string>();
        Dictionary<string, ActionInfo> programInfo = new Dictionary<string, ActionInfo>();

        List<Profile> profileList = new List<Profile>();//lists all of the existing profiles
        List<List<int>> genericsList = new List<List<int>>();//list of profiles; list of locations which contain modules programmed to be generics

        System.Windows.Forms.Button[] btnArray = new System.Windows.Forms.Button[16];//used to draw buttons
        System.Windows.Forms.Label[] lblArray = new System.Windows.Forms.Label[16];//used to draw labels

=======
        List<Profile> profileList = new List<Profile>();//lists all of the existing profiles
        List<List<int>> genericsList = new List<List<int>>();//list of profiles; list of locations which contain modules programmed to be generics

        System.Windows.Forms.Button[] btnArray = new System.Windows.Forms.Button[16];//used to draw buttons
        System.Windows.Forms.Label[] lblArray = new System.Windows.Forms.Label[16];//used to draw labels

>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        //TODO: this should be a list so it's not a fixed size
        System.Windows.Forms.TabPage[] tabArray = new System.Windows.Forms.TabPage[25];//used to draw tabs for profiles
        #endregion

        public frmPalette()
        {
            InitializeComponent();//leave this here, C# basics

<<<<<<< HEAD
            SerialConnection();//eastablishes serial connection
            //GetInitialConfig();//send serial message to Brent asking for initial configuration

            InitializeConfiguration();

            CreateProgramDictionary("Programs.txt");
            CreateDictionary("Actions.txt");//reads dictionary files into program
=======
            //SerialConnection();//eastablishes serial connection
            //GetInitialConfig();//send serial message to Brent asking for initial configuration

            InitializeConfiguration();
            
            CreateDictionary("Programs.txt");//reads dictionary files into program
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
            tabControl1.Selecting += new TabControlCancelEventHandler(tabControl1_Selecting);//now when you switch tabs it will react
        }

        #region Functions
        // Connects to a serial port defined through the current settings
        public void SerialConnection()
        {
            // Closing serial port if it is open
            if (serialPort1 != null && serialPort1.IsOpen)
                serialPort1.Close();

            // Setting serial port settings
            serialPort1.PortName = "COM3";
            serialPort1.BaudRate = 57600;
            serialPort1.Open();
        }

        private void GetInitialConfig()
        {
            if (!serialPort1.IsOpen) return;
            byte[] data = new byte[] {
               0x50
            };
            SendSerial(data);
        }

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
<<<<<<< HEAD

                profileID = Convert.ToInt32(data[index++]);//grabs profile ID
                profileName = data[index++];//grabs profile name

=======

                profileID = Convert.ToInt32(data[index++]);//grabs profile ID
                profileName = data[index++];//grabs profile name

>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
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

<<<<<<< HEAD
        private void CreateProgramDictionary(string fileName)
        {
            string file = ReadFile(fileName);
            char[] delimiters = new char[] { '\r', '\n' };
            string[] data = file.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            string tempName;
            int tempID;

            int i = 0; 
            while (i < data.Count())
            {
                tempID = Convert.ToInt32(data[i++]);
                tempName = data[i++];

                ProgramDict.Add(tempID, tempName);
            }
        }

        private Dictionary <string, ActionInfo> CreateDictionary(string fileName)
        {
            int tempActionID;
            string tempName, temp;
            int tempProgramID = 0;
            List<string> tempActions = new List<string>();
            Dictionary<int, List<string>> GenericList = new Dictionary<int, List<string>>();
            string[] tempArray;
=======
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
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9

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

                    var tempCopyDict = new Dictionary<int, List<string>>(GenericList);//cppy by value, not reference

                    //add to dictionary;
                    programInfo.Add(tempActionID.ToString(), new ActionInfo(tempActionID, tempName, tempCopyDict));
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
<<<<<<< HEAD
            //make change in palette array
            palette[location] = ID;

            //make changes in profile array
            for (int i = 0; i < profileList.Count; i++)//for each profile in profileList
            {
                //removes all copies of previous module from list of generics
                if (profileList[i].moduleList[location].actionID > 0 && profileList[i].moduleList[location].actionID < 32)
                {
                    genericsList[i].RemoveAll(x => x == location);
                }

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

                //generics addition
                if (profileList[i].moduleList[location].actionID > 0 && profileList[i].moduleList[location].actionID < 32)
                {
                    genericsList[i].Add(location);
                }
            }

            buildButton(location);//change the UI
=======
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
            buildButton(location);
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        }
        #endregion

        #region Helper Functions
        private void buildButton(int location)
        {
            for (int i = 0; i < profileList.Count; i++)
            {
<<<<<<< HEAD
                string btnName = "prf-" + i + "-btn-" + location;
                string lblName = "prf-" + i + "-lbl-" + location;

                assignModuleImage(i, location);

                tabArray[i].Controls.Find(btnName, true)[0].Tag = location;//add location to each button's tag
                tabArray[i].Controls.Find(lblName, true)[0].Text = profileList[i].moduleList[location].actions;//writes action as text of label
                tabArray[i].Controls.Find(btnName, true)[0].Click += new System.EventHandler(ClickButton);// the Event of click Button
=======
                tabArray[i].Controls.Remove(lblArray[location]);

                int xPos = getPositionX(location);
                int yPos = getPositionY(location);

                // Location of button: 
                btnArray[location].Left = xPos;
                btnArray[location].Top = yPos;

                tabArray[i].Controls.Remove(btnArray[location]);
                tabArray[i].Controls.Add(btnArray[location]); // Let panel hold the Buttons

                // Location of label: 
                lblArray[location].Left = xPos;
                lblArray[location].Top = yPos + 100;

                tabArray[i].Controls.Remove(lblArray[location]);
                tabArray[activeProfile].Controls.Add(lblArray[location]); // Let panel hold the Labels

                assignModuleImage(location);

                btnArray[location].Tag = location;//add location to each button's tag
                lblArray[location].Text = profileList[activeProfile].moduleList[location].actions;//writes action as text of label
                btnArray[location].Click += new System.EventHandler(ClickButton);// the Event of click Button
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
            }         
        }

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
                int xPos, yPos;

                for (int n = 0; n < palette.Count(); n++)
                {
                    btnArray[n] = new System.Windows.Forms.Button();
                    btnArray[n].FlatStyle = FlatStyle.Flat;
                    btnArray[n].FlatAppearance.BorderSize = 0;
                    lblArray[n] = new System.Windows.Forms.Label();
                    lblArray[n].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

                    btnArray[n].Width = 100; // Width of button 
                    btnArray[n].Height = 100; // Height of button 
                    lblArray[n].Width = 100;
                    lblArray[n].Height = 20;

                    xPos = getPositionX(n);
                    yPos = getPositionY(n);

                    // Location of button: 
                    btnArray[n].Left = xPos;
                    btnArray[n].Top = yPos;

                    // Add buttons to a Panel: 
<<<<<<< HEAD
                    btnArray[n].Name = "prf-" + i + "-btn-" + n;
=======
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
                    tabArray[activeProfile].Controls.Add(btnArray[n]); // Let panel hold the Buttons 

                    // Location of label: 
                    lblArray[n].Left = xPos;
                    lblArray[n].Top = yPos + 100;

                    // Add label to a Panel: 
<<<<<<< HEAD
                    lblArray[n].Name = "prf-" + i + "-lbl-" + n;
=======
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
                    tabArray[activeProfile].Controls.Add(lblArray[n]); // Let panel hold the Labels

                    //Add information to buttons & labels
                    #region Add Info to Buttons & Labels
<<<<<<< HEAD
                    assignModuleImages(n);
=======
                    assignModuleImage(n);
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
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
<<<<<<< HEAD

                List<int> tempCopyList2 = new List<int>(tempGenericsList);
                genericsList.Add(tempCopyList2);//adds the list of generics for this profile

                activeProfile++;
            }

            //reset the active profile/tab
            activeProfile = 0;
            tabControl1.SelectedTab = tabArray[0];
        }

        private int getPositionY(int n)
        {
            if (n >= (3 * row))  return 440;
            else if (n >= (2 * row)) return 300;
            else if (n >= row) return 160;
            else return 20;
        }
        private int getPositionX(int n)
        {
            if (n == 1 || n == (3 * row + 1) || n == (2 * row + 1) || n == (row + 1)) return 140;
            else if (n == 2 || n == (3 * row + 2) || n == (2 * row + 2) || n == (row + 2)) return 260;
            else if (n == 3 || n == (3 * row + 3) || n == (2 * row + 3) || n == (row + 3)) return 380;
            else return 20;
        }

        private void assignModuleImages(int n)
        {
            for (int i = 0; i < profileList.Count; i++)
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
        }

        private void assignModuleImage(int i, int n)
        {
            string btnName = "prf-" + i + "-btn-" + n;
            string lblName = "prf-" + i + "-lbl-" + n;

            if (palette[n] < 10)
            {
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImageLayout = ImageLayout.Stretch;
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImage = Image.FromFile(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\button.png");
            }
            else if (palette[n] < 20)
            {
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImageLayout = ImageLayout.Stretch;
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImage = Image.FromFile(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\knob.png");
            }
            else if (palette[n] < 30)
            {
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImageLayout = ImageLayout.Stretch;
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImage = Image.FromFile(@"C:\Users\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\joystick.png");
            }
            else
            {
                tabArray[i].Controls.Find(btnName, true)[0].Text = "Not Present";
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImageLayout = ImageLayout.Stretch;
                tabArray[i].Controls.Find(btnName, true)[0].BackgroundImage = null;
=======

                List<int> tempCopyList2 = new List<int>(tempGenericsList);
                genericsList.Add(tempCopyList2);//adds the list of generics for this profile

                activeProfile++;
            }

            //reset the active profile/tab
            activeProfile = 0;
            tabControl1.SelectedTab = tabArray[0];
        }

        private int getPositionY(int n)
        {
            if (n >= (3 * row))  return 440;
            else if (n >= (2 * row)) return 300;
            else if (n >= row) return 160;
            else return 20;
        }
        private int getPositionX(int n)
        {
            if (n == 1 || n == (3 * row + 1) || n == (2 * row + 1) || n == (row + 1)) return 140;
            else if (n == 2 || n == (3 * row + 2) || n == (2 * row + 2) || n == (row + 2)) return 260;
            else if (n == 3 || n == (3 * row + 3) || n == (2 * row + 3) || n == (row + 3)) return 380;
            else return 20;
        }

        private void assignModuleImage(int n)
        {
            for (int i = 0; i < profileList.Count; i++)
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
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
            }
        }
        
        private void SendSerial(string text)
        {
<<<<<<< HEAD
=======
            Button btn = (Button)sender;
            int num = Convert.ToInt32(btn.Tag);//gets the module ID for each module on the screen
            lblInfoName.Text = profileList[activeProfile].moduleList[num].type;
            lblInfoDescription.Text = profileList[activeProfile].moduleList[num].actions;
        }

        private void SendSerial(string text)
        {
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
            byte[] bytes = GetBytes(text);
            serialPort1.Write(bytes, 0, bytes.Length);
        }

        private void SendSerial(byte[] bytes)
        {
            if (serialPort1.IsOpen)
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

<<<<<<< HEAD
        #region Control Event Code

        // Result of (Click Button) event, get the text of button 
        private void ClickButton(Object sender, System.EventArgs e)
        {
            //fill in static information (name, action, etc)
            Button btn = (Button)sender;
            int num = Convert.ToInt32(btn.Tag);//gets the module ID for each module on the screen
            lblInfoName.Text = profileList[activeProfile].moduleList[num].type;
            lblInfoDescription.Text = profileList[activeProfile].moduleList[num].actions;

            //clear combo boxes
            cmbPrograms.Items.Clear();
            cmbActions.Items.Clear();

            //fill in combo boxes
            foreach (KeyValuePair<int, string> pair in ProgramDict)
            {
                cmbPrograms.Items.Add(pair.Value);
                cmbActions.Enabled = false;
            }
        }

        private void cmbPrograms_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ComboBox senderComboBox = (ComboBox)sender;

            cmbActions.Items.Clear();
            cmbActions.Enabled = true;

            int tempKey = ProgramDict.FirstOrDefault(x => x.Value == cmbPrograms.SelectedText).Key;
            IEnumerable <ActionInfo> tempActions = programInfo.Where(pv => pv.Value.programID == tempKey).Select(pv=> pv.Value);
            foreach (ActionInfo ai in tempActions)
            {
                cmbActions.Items.Add(ai.name);
            } 
=======
        private void DisplayText(object sender, EventArgs e)
        {
            txtData.AppendText(RxString);
            txtData.AppendText("\n");
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        }
        #endregion

        #region Control Event Code
        void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabPage current = (sender as TabControl).SelectedTab;
            try
            {
                activeProfile = current.TabIndex;
<<<<<<< HEAD
            } 
            catch { }
        }

        private void DisplayText(object sender, EventArgs e)
        {
            txtData.AppendText(RxString);
            txtData.AppendText("\n");
=======
            }
            catch { }
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            int numBytes = serialPort1.BytesToRead;
            int[] byteArray = new int[numBytes];
            string[] stringArray = new string[numBytes];
            for (int i =0; i<numBytes;i++)
            {
                byteArray [i] = serialPort1.ReadByte();
                stringArray [i] = byteArray[i].ToString("X");
                RxString = stringArray[i];
                this.Invoke(new EventHandler(DisplayText));
            }
            AnalyseIncomingSerial(stringArray);
<<<<<<< HEAD
        }

        private void AnalyseIncomingSerial (string [] strArray)
        {
            if (Convert.ToInt32(strArray[0]) == 70)//current location & IDs
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 80)//profile & mappings
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 30)//hardware change
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 40)//profile switch
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 10)//wassup
            {

            }
            else
            {

            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) return;
            byte[] data = new byte[] {
               0x50
            };
            SendSerial(data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moduleChanged(0, 18);
        }

=======
        }

        private void AnalyseIncomingSerial (string [] strArray)
        {
            if (Convert.ToInt32(strArray[0]) == 70)//current location & IDs
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 80)//profile & mappings
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 30)//hardware change
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 40)//profile switch
            {

            }
            else if (Convert.ToInt32(strArray[0]) == 10)//wassup
            {

            }
            else
            {

            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (!serialPort1.IsOpen) return;
            byte[] data = new byte[] {
               0x50
            };
            SendSerial(data);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moduleChanged(0, 18);
        }

>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        private void frmPalette_FormClosing(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen) serialPort1.Close();
        }

        #endregion

    }//public partial class namespace
} //namespace
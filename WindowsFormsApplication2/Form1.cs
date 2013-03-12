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
            string file = ReadFile("Config1.txt");//read current location and IDs
            
            //break data down into useful chunks
            char[] delimiters = new char[] { ',', '\r', '\n' };
            string[] data = file.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int row = Convert.ToInt32(data[0]);
            int col = Convert.ToInt32(data[1]);
            int size = row * col;

            string file2 = ReadFile("Config2.txt"); //read current profile(s) and mappings
            
            //break data down into useful chunks
            string[] data2 = file2.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            int numProfiles = Convert.ToInt32(data2[0]);

            int index = 1, moduleID, numExtraModules;
            List<int> profileID = new List<int>(); 
            List<string> profileName = new List<string>();
            string moduleAc;
            List<Module> moduleList = new List<Module>();

            List<int> tempGenericsList = new List<int>(); 

            int m = 0;
            while (m < numProfiles)
            {
                tempGenericsList.Clear();
                moduleList.Clear(); 
                profileID.Add(Convert.ToInt32(data2[index++]));//grabs profile ID
                profileName.Add(data2[index++]);//grabs profile name

                numExtraModules = Convert.ToInt32(data2[index++]);
                for (int i = 0; i<(size+numExtraModules);i++)
                {
                    moduleID = Convert.ToInt32(data2[index++]);//grabs module ID
                    moduleAc = data2[index++];//grabs action ID
                    moduleList.Add(new Module(moduleID, moduleAc));

                    if (i < size && moduleID > 0 && moduleID < 32)//find generics in the first 16 modules
                    {
                        tempGenericsList.Add(i);
                    }
                }

                List<Module> tempList = new List<Module>(moduleList);//copy the list so that when I clear moduleList, the profileList.moduleList doesn't clear too
                profileList.Add(new Profile(profileID[m], profileName[m], tempList));

                List<int> tempList2 = new List<int>(tempGenericsList);
                genericsList.Add(tempList2);
                m++;
            }

            buildPalette(row, col, data, profileName, profileID);
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

        private void moduleChanged(int ID, int location)//location & new ID are provided
        {
            //removes all copies of previous module from list of generics
            if (palette[location] > 0 && palette[location] < 32)
            {
                for (int i = 0; i < profileList.Count; i++)
                {
                    genericsList[i].RemoveAll(x => x == location);
                }
            }

            //make change in storage
            palette[location] = ID;

            //make change on display
            changeType(location);
            btnArray[location].Tag = location;

            //transfer old module object out of first 16
            Module tempModule = profileList[activeProfile].moduleList[location];
            profileList[activeProfile].moduleList.Add(tempModule);//transfer to new location

            //create new module object in proper location
            int index = profileList[activeProfile].moduleList.FindIndex(item => item.ID == ID);
            if (index >= 0)//use existing info for module
            {
                profileList[activeProfile].moduleList[location] = new Module(ID, profileList[activeProfile].moduleList[index].actions);//transfer new info to 
            }
            else//no existing info was found, create new empty module
            {
                profileList[activeProfile].moduleList[location] = new Module(ID, "");
            }

            lblArray[location].Text = profileList[activeProfile].moduleList[location].actions;
            lblArray[location].Tag = profileList[activeProfile].moduleList[location].actionID;
        }
        #endregion

        #region Helper Functions
        private void buildPalette(int row, int col, string[] data, List<string> profileName, List<int> profileID)
        {
            int size = row * col;

            //PALETTE
            tabControl1.TabPages.Clear();
            for (int i = 0; i < profileList.Count; i++)
            {
                //create tab for profile
                string tabTitle = "Profile " + (i+1).ToString();
                tabArray[i] = new System.Windows.Forms.TabPage(tabTitle);
                tabControl1.TabPages.Add(tabArray[i]);
                //save necessary info
                tabArray[i].Text = profileName[i];
                tabArray[i].Tag = profileID[i].ToString();

                //create array of buttons & labels for each profile
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

                //Create buttons and label for Palette
                int n = 0;
                while (n < size)
                {
                    palette[n] = Convert.ToInt32(data[n + 2]);
                    btnArray[n].Tag = n;
                    lblArray[n].Text = data[n + 2];

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

                    //Writes module type name to each button in array
                    changeType(n);
                    lblArray[n].Text = profileList[activeProfile].moduleList[n].actions;

                    // the Event of click Button 
                    btnArray[n].Click += new System.EventHandler(ClickButton);
                    n++;
                }
                activeProfile++;
            }
            activeProfile = 0;
            tabControl1.SelectedTab = tabArray[0];
        }

        private void changeType(int n)
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
            else if (palette[n] < 40)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = null;
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
            activeProfile = current.TabIndex;
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
            moduleChanged(18, 0);
        }

    }//public partial class namespace
} //namespace
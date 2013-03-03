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
        public int[] palette = new int[16];
        public int activeTab;

        List<Profile> profileList = new List<Profile>();

        System.Windows.Forms.Button[] btnArray = new System.Windows.Forms.Button[16];
        System.Windows.Forms.Label[] lblArray = new System.Windows.Forms.Label[16];
        System.Windows.Forms.TabPage[] tabArray = new System.Windows.Forms.TabPage[25];//TODO: this should be a list

        //initialize global variables
        #endregion

        public frmPalette()
        {
            InitializeComponent();
            tabControl1.Selecting += new TabControlCancelEventHandler(tabControl1_Selecting);

            InitializeConfiguration();
            //Calls for functions
        }

        #region Threads
        private void Serial_Connection()
        {
            try
            {
                while(true)
                {
                    //stuff
                }

            }
            catch(ThreadAbortException ex){}
            catch(Exception ex)
            {
                MessageBox.Show("Error: Serial communication cannot be established. Original error: " + ex.Message, TITLENAME, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

            int m = 0;
            while (m < numProfiles)
            {
                profileID.Add(Convert.ToInt32(data2[index++]));//grabs profile ID
                profileName.Add(data2[index++]);//grabs profile name

                numExtraModules = Convert.ToInt32(data2[index++]);
                for (int i = 0; i<(size+numExtraModules);i++)
                {
                    moduleID = Convert.ToInt32(data2[index++]);//grabs module ID
                    moduleAc = data2[index++];//grabs action ID
                    moduleList.Add(new Module(moduleID, moduleAc));
                }

                List<Module> tempList = new List<Module>(moduleList);//copy the list so that when I clear moduleList, the profileList.moduleList doesn't clear too
                profileList.Add(new Profile(profileID[m], profileName[m], tempList));

                moduleList.Clear(); 
                m++;
            }

            buildPalette(row, col, data, profileName, profileID);
        }

        private void moduleChanged(int ID, int location)
        {
            //make change in storage
            palette[location] = ID;

            //make change on display
            changeType(location);
            btnArray[location].Tag = location;

            //transfer old module object out of first 16
            Module tempModule = profileList[activeTab].moduleList[location];
            profileList[activeTab].moduleList.Add(tempModule);//transfer to new location

            //create new module object in proper location
            int index = profileList[activeTab].moduleList.FindIndex(item => item.ID == ID);
            if (index >= 0)
            {
                profileList[activeTab].moduleList[location] = new Module(ID, profileList[activeTab].moduleList[index].actions);//transfer new info to 
            }
            else
            {
                profileList[activeTab].moduleList[location] = new Module(ID, "");
            }

            lblArray[location].Text = profileList[activeTab].moduleList[location].actions;
            lblArray[location].Tag = profileList[activeTab].moduleList[location].actionID;
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
                    lblArray[j] = new System.Windows.Forms.Label();
                    lblArray[j].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                }

                int n = 0;
                //Create buttons and label for Palette
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
                    tabArray[activeTab].Controls.Add(btnArray[n]); // Let panel hold the Buttons 

                    // Location of label: 
                    lblArray[n].Left = xPos;
                    lblArray[n].Top = yPos + 100;
                    // Add label to a Panel: 
                    tabArray[activeTab].Controls.Add(lblArray[n]); // Let panel hold the Labels

                    xPos = xPos + btnArray[n].Width + 20; // Left of next button 

                    //Writes module type name to each button in array
                    changeType(n);
                    lblArray[n].Text = profileList[activeTab].moduleList[n].actions;

                    // the Event of click Button 
                    btnArray[n].Click += new System.EventHandler(ClickButton);
                    n++;
                }
                //tabControl.SelectedIndexChanged += new EventHandler(tabControl1_SelectedIndexChanged);
                activeTab++;
            }
            activeTab = 0;
            tabControl1.SelectedTab = tabArray[0];
        }

        private void changeType(int n)
        {
            if (palette[n] < 10)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"D:\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\button.jpg");
            }
            else if (palette[n] < 20)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"D:\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\encoder.jpg");
            }
            else if (palette[n] < 30)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"D:\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\jog.jpg");
            }
            else if (palette[n] < 40)
            {
                btnArray[n].BackgroundImageLayout = ImageLayout.Stretch;
                btnArray[n].BackgroundImage = Image.FromFile(@"D:\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\Images\joy.jpg");
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
            lblInfoName.Text = profileList[activeTab].moduleList[num].type;
            lblInfoDescription.Text = profileList[activeTab].moduleList[num].actions;
        }

        private string ReadFile(string name)
        {
            string something = System.IO.File.ReadAllText(@"D:\Julia\Documents\Visual Studio 2012\Projects\WindowsFormsApplication2\" + name);
            return something;
        }
        #endregion

        #region Control Event Code
        private void btnTest_Click(Object sender, EventArgs e)
        {
            moduleChanged(38, 0);
        }

        void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            TabPage current = (sender as TabControl).SelectedTab;
            activeTab = current.TabIndex;
        }
        #endregion

    }//public partial class namespace
} //namespace
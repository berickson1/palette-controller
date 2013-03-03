using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class Module
    {
        public int ID, actionID;
        public int[] modifiers;
        public string actions, type;
        
        public Module(int id, int aID, int[] mod, string a)
        {
            ID = id;
            actionID = aID;
            modifiers = mod;
            actions = a;
            
            if (ID < 10)
            {
                type = "Button";
            }
            else if (ID < 20)
            {
                type = "Encoder";
            }
            else if (ID < 30)
            {
                type = "Pot";
            }
            else if (ID < 40)
            {
                type = "Joystick";
            }
        }
        public Module(int id, string a)
        {
            ID = id;
            actions = a;

            if (ID < 10)
            {
                type = "Button";
            }
            else if (ID < 20)
            {
                type = "Encoder";
            }
            else if (ID < 30)
            {
                type = "Pot";
            }
            else if (ID < 40)
            {
                type = "Joystick";
            }
        }
        //other functions
    }

}

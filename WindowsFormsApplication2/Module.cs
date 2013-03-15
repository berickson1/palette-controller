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
<<<<<<< HEAD

            if (ID < 10) type = "Button";
            else if (ID < 20) type = "Knob";
            else if (ID < 30) type = "Slider";
=======
            
            if (ID < 10)
            {
                type = "Button";
            }
            else if (ID < 20)
            {
                type = "Knob";
            }
            else if (ID < 30)
            {
                type = "Slider";
            }
>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        }
        public Module(int id, int aid, string a)
        {
            ID = id;
            actionID = aid;
            actions = a;

<<<<<<< HEAD
            if (ID < 10) type = "Button";
            else if (ID < 20) type = "Knob";
            else if (ID < 30) type = "Slider";
=======
            if (ID < 10)
            {
                type = "Button";
            }
            else if (ID < 20)
            {
                type = "Knob";
            }
            else if (ID < 30)
            {
                type = "Slider";
            }

>>>>>>> 9c13166c24e32831c65013a1331e1b96bbce59a9
        }
        //other functions
    }

}

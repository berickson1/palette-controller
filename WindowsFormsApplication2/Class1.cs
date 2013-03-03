using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class Module
    {
        int ID, actionID;
        int[] modifiers;
        string[] actions;
        public Module(int id, int aID, int[] mod, string[] a)
        {
            ID = id;
            actionID = aID;
            modifiers = mod;
            actions = a;
        }
        //other functions
    }

}

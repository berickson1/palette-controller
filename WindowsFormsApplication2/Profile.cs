using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class Profile
    {
        public int ID;
        public string name;
        public List<Module> moduleList = new List<Module>();

        public Profile(int id, string nm, List<Module> mL)
        {
            ID = id;
            name = nm;
            moduleList = mL;
        }
        //other functions
    }
}

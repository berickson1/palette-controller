using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication2
{
    class ActionInfo
    {
        public int actionID;
        public string name;
        public int programID; 
        public List<string> action = new List<string>();
        public Dictionary<int, List<string>> GenericList;

        public ActionInfo(int aID, string nm, int pID, List<string> ac)
        {
            actionID = aID;
            name = nm;

            programID = pID;
            action = ac;
        }

        public ActionInfo(int aID, string nm, Dictionary<int, List<string>> programActionDictionary)
        {
            actionID = aID;
            name = nm;

            GenericList = programActionDictionary;
        }
    }
    
    /*class AProgram
    {
        public int actionID;
        public string generic;
        public List<string> actions = new List<string>();

    }*/
}

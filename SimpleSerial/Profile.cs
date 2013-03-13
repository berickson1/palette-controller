using System;

public class Profile
{
    int ID;
    string name;
    List<Module> moduleList = new List<Module>();

    public Profile(int id, string nm, List<Module> mL)
    {
        ID = id;
        name = nm;
        moduleList = mL;
    }
    //other functions
}

using System;

public class Module
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

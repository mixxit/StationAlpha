using System.Collections;
using System.Runtime.Serialization;
using System;

[Serializable]
public class Task {
	public Task()
	{
		
	}
	string username = "";
	
	public string getUsername() {
		return username;
	}
	
	string taskname = "";
	
	public string getTaskname() {
		return taskname;
	}
	
	int timeleft = 0;
	
	public int getTimeleft() {
		return timeleft;
	}
	
	public void setTimeleft(int timeleft)
	{
		this.timeleft = timeleft;
	}
	
	int valint1 = 0;
	public int getValint1()
	{
		return valint1;
	}
	public void setValint1(int value)
	{
		this.valint1 = value;
	}
	
	
	public Task(string username,string taskname,
	            int timeleft)
	{
		this.username = username;
		this.taskname = taskname;
		this.timeleft = timeleft;
	}
}

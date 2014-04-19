using System.Collections;
using System.Runtime.Serialization;
using System;

[Serializable]
public class User {
	public User()
	{
		
	}
	string username = "";
	
	public string getUsername() {
		return username;
	}
	
	string password = "";
	
	public string getPassword() {
		return password;
	}
	
	string credits = "0";
	public string getCredits() {
		return credits;
	}
	
	public void setCredits(string newcredits) {
		credits = newcredits;
	}
	
	string x = "1";
	public string getX() {
		return x;
	}
	string y = "1";
	public string getY() {
		return y;
	}
	
	string z = "1";
	public string getZ() {
		return z;
	}
	
	string s = "1";
	public string getS() {
		return s;
	}
	
	string rank = "0";
	
	public string getRank() {
		return rank;
	}

	public User(string username, 
	            string password,
	            string x,
	            string y,
	            string z,
	            string s,
	            string credits,
	            string rank)
	{
		this.username = username;
		this.password = password;
		this.x = x;
		this.y = y;
		this.z = z;
		this.s = s;
		this.credits = credits;
		this.rank = rank;
	}
}

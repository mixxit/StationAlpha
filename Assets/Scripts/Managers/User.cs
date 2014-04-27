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
	
	int credits = 0;
	public int getCredits() {
		return credits;
	}
	
	public void setCredits(int newcredits) {
		credits = newcredits;
	}
	
	int x = 1;
	public int getX() {
		return x;
	}
	int y = 1;
	public int getY() {
		return y;
	}
	
	int z = 1;
	public int getZ() {
		return z;
	}
	
	int s = 1;
	public int getS() {
		return s;
	}
	
	int character = 0;
	
	public int getCharacter() {
		return character;
	}

	int oxygen = 1;
	public int getOxygen() {
		return oxygen;
	}

	int health = 1;
	public int getHealth() {
		return health;
	}

	int food = 1;
	public int getFood() {
		return food;
	}

	int drink = 1;
	public int getDrink() {
		return drink;
	}

	int role = 1;
	public int getRole() {
		return role;
	}

	bool playmode = false;
	public void setPlaymode(bool playmode)
	{
		this.playmode = playmode;
	}

	public bool getPlayMode()
	{
		return this.playmode;
	}

	public User(string username, 
	            string password,
	            int x,
	            int y,
	            int z,
	            int s,
	            int credits,
	            int character,
	            int oxygen,
	            int health,
	            int food,
	            int drink,
	            int role
	            )
	{
		this.username = username;
		this.password = password;
		this.x = x;
		this.y = y;
		this.z = z;
		this.s = s;
		this.credits = credits;
		this.character = character;
		this.oxygen = oxygen;
		this.health = health;
		this.food = food;
		this.drink = drink;
		this.role = role;
	}
}

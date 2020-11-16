using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Linq;

enum Gender {K, M};

abstract class Person
{
  const int PeselDigitCount = 11;
  readonly string[] DateFormats = {"yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yy", "dd-MMM-yy", "dd-MMM-yyyy"};

  public string name {get; set; }
  public string surname {get; set; }
  private DateTime birthDate;
  public string pesel {get; }
  public Gender gender {get; }

  public Person() {
    name = surname = "";
    birthDate = new DateTime();
    pesel = new String('0', PeselDigitCount);
    gender = Gender.M;
  }
  
  public Person(string name, string surname, string birthDate, string pesel, Gender gender)
  {
    this.name = name;
    this.surname = surname;
    this.birthDate = ParseDate(birthDate);
    this.pesel = pesel;
    this.gender = gender;
  }
  
  protected DateTime ParseDate(string date)
  {
    DateTime result;
    DateTime.TryParseExact(
        date,
        DateFormats,
        null, 
        DateTimeStyles.None, 
        out result);
    return result;
  }

  public int Age()
  {
    const int DaysInYear = 365;
    TimeSpan timeSpan = DateTime.Now - birthDate;
    return timeSpan.Days / DaysInYear;
  }

  public override string ToString() {
    const string DateFormat = "yyyy-MM-dd";
    return String.Format(
          "{0} {1} ({2}) {3} {4} {5}",
          name, surname, Age(),
          birthDate.ToString(DateFormat),
          pesel,
          gender);
  }
}

class TeamMember : Person
{
  public DateTime joinDate;
  public string function;
  public TeamMember(
        string name, 
        string surname, 
        string birthDate, 
        string pesel, 
        Gender gender,
        string function,
        string joinDate) : base(name, surname, birthDate, pesel, gender)
  {
    this.joinDate = ParseDate(joinDate);
    this.function = function;
  }

  public override string ToString()
  {
    const string DateFormat = "dd-MMM-yyyy";
    string memberString = String.Format(
          "{0} ({1})",
          function,
          joinDate.ToString(DateFormat));

    return base.ToString() + " " + memberString;
  }
}

class TeamLeader : Person
{
  int yearsOfExperience;
  public TeamLeader(
        string name, 
        string surname, 
        string birthDate, 
        string pesel, 
        Gender gender,
        int yearsOfExperience) : base(name, surname, birthDate, pesel, gender)
  {
    this.yearsOfExperience = yearsOfExperience;
  }

  public override string ToString()
  {
    string leaderString = yearsOfExperience.ToString();
    return base.ToString() + " " + leaderString;
  }
}

class Team
{
  public int MemberCount { get { return members.Count; } }
  public string name;
  public TeamLeader leader;
  List<TeamMember> members;

  public Team()
  {
    name = null;
    leader = null;
    members = new List<TeamMember>();
  }

  public Team(string name, TeamLeader leader) : this()
  {
    this.name = name;
    this.leader = leader;
  }

  public void AddMember(TeamMember member)
  {
    members.Add(member);
  }

  public override string ToString()
  {
    StringBuilder sb = new StringBuilder();
    sb.Append("Team: " + name + "\n");
    sb.Append("Leader: ");
    sb.Append(leader);
    sb.Append("\n");
    foreach (var member in members)
    { 
      sb.Append(member); 
      sb.Append("\n");
    }

    return sb.ToString();
  }

  public bool IsMember(string pesel)
  {
    return members.Count(member => member.pesel == pesel) > 0;
  }
  
  public bool IsMember(string name, string surname)
  {
    return members.Count(member => 
        member.name == name &&
        member.surname == surname) > 0;
  }

  public void RemoveMember(string pesel)
  {
    members.RemoveAll(member => member.pesel == pesel);
  }
  
  public void RemoveMember(string name, string surname)
  {
    members.RemoveAll(member => 
        member.name == name &&
        member.surname == surname);
  }

  public void RemoveEveryone()
  {
    members.Clear();    
  }
  
  public List<TeamMember> FindMembersByFunction(string function)
  {
    return members.FindAll(member => member.function == function);
  }
  
  public List<TeamMember> FindMembersByMonth(int month)
  {
    return members.FindAll(member => member.joinDate.Month == month);
  }
}


class Program
{
  public static void Main(string[] args) 
  {
    TeamMember p1 = new TeamMember(
          "Beata", "Nowak", 
          "1992-10-22", 
          "92102201347", 
          Gender.K,
          "projektant",
          "01-Jan-2020");
    TeamMember p2 = new TeamMember(
          "Jan", "Janowski", 
          "1993-03-15", 
          "92031507772", 
          Gender.M,
          "programista",
          "01-Jun-2019");
  
    TeamLeader p3 = new TeamLeader(
            "Adam", "Kowalski",
            "1990-07-01",
            "90070100211",
            Gender.M,
            5);

    Console.WriteLine(p1);
    Console.WriteLine(p2);
    Console.WriteLine(p3);

    Team t = new Team("Test", p3);
    t.AddMember(p1);
    t.AddMember(p2);
    Console.WriteLine(t);
    
    Console.WriteLine(t.MemberCount);
    t.RemoveMember("Jan", "Janowski");
    Console.WriteLine(t.MemberCount);
  }
}

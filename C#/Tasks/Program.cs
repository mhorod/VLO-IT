using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;

public enum Gender { K, M };

public interface IWritable
{
  void WriteBinary(string filename);
  void ReadBinary(string filename);
  void WriteXml(string filename);
  void ReadXml(string filename);
};

[Serializable]
public abstract class Person : IEquatable<Person>
{
  const int PeselDigitCount = 11;
  readonly string[] DateFormats = { "yyyy-MM-dd", "yyyy/MM/dd", "MM/dd/yy", "dd-MMM-yy", "dd-MMM-yyyy" };

  public string name { get; set; }
  public string surname { get; set; }
  private DateTime birthDate;
  protected string birthDateString;
  public string pesel { get; }
  public Gender gender { get; }

  public Person()
  {
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
    this.birthDateString = birthDate;
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

  public override string ToString()
  {
    const string DateFormat = "yyyy-MM-dd";
    return String.Format(
        "{0} {1} ({2}) {3} {4} {5}",
        name, surname, Age(),
        birthDate.ToString(DateFormat),
        pesel,
        gender);
  }

  public bool Equals(Person other)
  {
    return pesel == other.pesel;
  }
}


[Serializable]
public class TeamMember : Person, ICloneable, IComparable<TeamMember>
{
  public DateTime joinDate;
  private string joinDateString;
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
    joinDateString = joinDate;
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

  public object Clone()
  {
    return new TeamMember(
        name, surname, birthDateString,
        pesel, gender, function, joinDateString);
  }

  public int CompareTo(TeamMember other)
  {
    if (name == other.name)
      return surname.CompareTo(other.surname);
    return name.CompareTo(other.name);
  }

  public int CompareByPesel(TeamMember other)
  {
    return pesel.CompareTo(other.pesel);
  }
}

[Serializable]
public class TeamLeader : Person, ICloneable
{
  int yearsOfExperience;
  public TeamLeader() { }
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
  public object Clone()
  {
    return new TeamLeader(
      name, surname, birthDateString,
      pesel, gender, yearsOfExperience);
  }
}

[Serializable]
public class Team : ICloneable, IWritable
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

  public object Clone()
  {
    Team result = new Team(name, leader.Clone() as TeamLeader);
    foreach (TeamMember member in members)
    {
      result.AddMember(member.Clone() as TeamMember);
    }
    return result;
  }

  public void Sort()
  {
    members.Sort();
  }

  public void SortByPesel()
  {
    members.Sort((x, y) => x.CompareByPesel(y));
  }

  public void WriteBinary(string filename)
  {
    var stream = new FileStream(filename, FileMode.Create);
    var formatter = new BinaryFormatter();
    formatter.Serialize(stream, this);
  }

  public void ReadBinary(string filename) { }

  public void WriteXml(string filename)
  {
    var stream = new StreamWriter(filename);
    var serializer = new XmlSerializer(this.GetType());
    serializer.Serialize(stream, this);
  }
  public void ReadXml(string filename) { }
}


class Program
{
  public static void Main(string[] args)
  {
    TeamLeader leader = new TeamLeader(
        "Adam", "Kowalski",
         "1990-07-01",
         "90070142412",
         Gender.M,
         5);

    Team team = new Team("Grupa IT", leader);

    team.AddMember(
        new TeamMember(
          "Witold", "Adamski",
          "1992-10-22",
          "92102266738",
          Gender.M,
          "sekretarz",
          "2015-05-13"));

    Team cloned = team.Clone() as Team;
    cloned.name = "NewTeam";
    cloned.leader.name = "Rafal";

    Console.WriteLine(team);
    Console.WriteLine(cloned);

    cloned.WriteXml("test.xml");
  }

}


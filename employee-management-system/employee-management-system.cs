using System;
using System.Collections.Generic;

enum Department {       // Şirkette kullanacağımız sabit departmanları tutmak için enum

HR,
IT,
Finance,
Marketing
}

interface IReportable     // Rapor hazırlayabilen roller için bir arayüz
{   
  void GenerateReport();    // Rapor üretmek için kullanacağımız metot
}

interface ITrainable      // Eğitim alabilen roller için bir arayüz
{
  void AttendTraining();    // Eğitimlere katılma metodu
}


abstract class Employee     // Tüm çalışanlar için ortak özellikleri topladığımız sınıf
{
  private string _name = string.Empty;     // Property üzerinden erişim sağlayarak hata kontrolü yapabilmek için private belirledik
  private int _age;
  public string Name
  {
    get => _name;

    set
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException("İsim boş olamaz.", nameof(Name));     //ismin bos olma ihtimaline karşı exception atıyoruz
      }
      _name = value;              // Eğer değer geçerliyse private alana atama yapar
    }
  }
  
  public int Age                // Yaş bilgisine dışarıdan erişimi kontrol eden Age özelliği
  {
    get => _age;
    set
    {
      if(value < 18)
      {
        throw new ArgumentOutOfRangeException(nameof(Age), "Yaş 18'den küçük olamaz.");
      }
       _age = value;
    }
  }

  public Department Department{ get; set;}          // Burada Department tipi enum olduğu için ekstra doğrulama yazmamıza gerek yok

  protected Employee(string name, int age, Department department)
  {
    // Property'ler üzerinden atama yaparak, yukarıda yazdığımız doğrulama kurallarının çalışmasını sağlarız
    Name = name;
    Age = age;
    Department = department;
  }

  public abstract void Work();

// Çalışan bilgisini konsolda daha okunaklı gösterebilmek için ToString metodunu override ediyoruz
  public override string ToString()
  {
    return $"{Name} ({Age}) - Departman: {Department}";
  }
}

class Developer : Employee, IReportable
{
  public Developer(string name, int age, Department department)
    :base(name, age, department)           // üst sinifin contructorini çağirdik
  {}
 
  public override void Work()          // Developer'ın yaptığı işi konsola yazdırmak için Work metodunu override ediyoruz.
  {
    Console.WriteLine($"{Name} yazilim gelistiriyor. (Developer, {Department})");
  }

   
  public void GenerateReport()              // IReportable arayüzünden gelen GenerateReport metodunu burada dolduruyoruz.
  {
    Console.WriteLine($"{Name} yazilim raporu hazirliyor.");
  }
}


class Manager: Employee, IReportable, ITrainable
{
  public Manager(string name, int age, Department department)         // Manager için constructor tanımlıyoruz ve değerleri üst sınıfa iletiyoruz.
    : base(name, age, department){}

  public override void Work()
  {
    Console.WriteLine($"{Name} ekibi yonetiyor. (, {Department})");

  }
  public void GenerateReport()
        {
            Console.WriteLine($"{Name} yonetim raporu hazirliyor.");
        }

  public void AttendTraining()
  {
    Console.WriteLine($"{Name} yonetim egitimi aliyor.");
  }
}
 class Intern : Employee, ITrainable
    {
        public Intern(string name, int age, Department department)
            : base(name, age, department)
        {}

        public override void Work()
        {
            Console.WriteLine($"{Name} basit islerde calisiyor. (Intern, {Department})");
        }

        public void AttendTraining()          // Stajyerin eğitim alma davranışını tanımlıyoruz.
        {
            Console.WriteLine($"{Name} mesleki egitim aliyor.");
        }
    }
class Program
{
    static void Main(string[] args)
    {
        // Tüm çalışanları tek bir listede tutmak için List<Employee> oluşturuyoruz

        List<Employee> staff = new List<Employee>();
          // Örnek çalışan nesneleri oluşturup listeye ekliyoruz
          staff.Add(new Developer("Esma", 25, Department.IT));
          staff.Add(new Manager("Sultan", 35, Department.HR));
          staff.Add(new Intern("Mehmet", 20, Department.Marketing));
          staff.Add(new Developer("Zeynep", 28, Department.Finance));
          staff.Add(new Intern("Salih", 22, Department.IT));

  
      try
      {
      staff.Add(new Intern("", 18, Department.IT));            //Ad sınırlamasını denemek için örnek
      }
      
      catch (ArgumentException ex)
      {
          Console.WriteLine($"Arguman hatasi: {ex.Message}");   //Ad null ya da bos ise hata mesajı verir
      }

      try
      {
        staff.Add(new Intern("Safa", 17, Department.IT));       //Yas sınırlamasını denemek için örnek
      }
      catch (ArgumentOutOfRangeException ex)
      {
          Console.WriteLine($"Deger araligi hatasi: {ex.Message}");     //Yas 18 den küçük ise hata mesajı
      }
      

        // Önce personel listesini ekrana yazdırır
        Console.WriteLine("=== PERSONEL LISTESI ===");
        foreach (var emp in staff)
        {
            Console.WriteLine(emp.ToString());        // Her bir çalışan için ToString metodunu çağırdık
        }

        Console.WriteLine();
        Console.WriteLine("=== IS AKISI VE YETKINLIKLER ===");

        
        foreach (var emp in staff)            // Her çalışan için iş akışını gösterdik
        {
            Console.WriteLine();
            Console.WriteLine($"-> {emp.Name} icin is akisi:");

            
            emp.Work();                   // Polymorphism -> emp Developer/Manager/Intern olabilir ama Work çağrıldığında hangi sınıftan ise onun override ettiği Work metodu çalışıyor

            
            if (emp is IReportable r)r.GenerateReport();     // Pattern matching ile bu çalışanın rapor hazırlayıp hazırlayamayacağını kontrol ediyoruz
            if (emp is ITrainable t) t.AttendTraining();      // Çalışanın eğitim alabilen bir rol olup olmadığını kontrol ediyoruz
          
        }

        Console.WriteLine();
        // Program sonlanmadan önce kullanıcıya bir tuşa basmasını söyleyerek konsol penceresinin hemen kapanmasını engelliyoruz
        Console.WriteLine("Programi kapatmak icin bir tusa basin...");
        Console.ReadKey();
    }
}

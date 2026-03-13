using Domain.Shared.Entities;

namespace Domain.Entities
{
    public class Person : BaseEntity
    {
        public Person() { }
        public Person(string name, DateTime birthDate)
        {
            Name = name;
            BirthDate = birthDate;
        }

        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }
}

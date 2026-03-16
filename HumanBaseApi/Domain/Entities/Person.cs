using Domain.Shared.Entities;

namespace Domain.Entities
{
    public class Person : BaseEntity
    {
        public Person() { }
        public Person(string name, DateTimeOffset birthDate)
        {
            Name = name;
            BirthDate = birthDate;
        }

        public string Name { get; set; }
        public DateTimeOffset BirthDate { get; set; }
    }
}

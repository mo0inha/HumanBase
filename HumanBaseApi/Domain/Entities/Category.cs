using Domain.Entities.EType;
using Domain.Shared.Entities;

namespace Domain.Entities
{
    public class Category : BaseEntity
    {
        public Category() { }

        public Category(string description, ETypeFinancial typeFinancial)
        {
            Description = description;
            TypeFinancial = typeFinancial;
        }

        public string Description { get; set; }
        public ETypeFinancial TypeFinancial { get; set; }
    }
}

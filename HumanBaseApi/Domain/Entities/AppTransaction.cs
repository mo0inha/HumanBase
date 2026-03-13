using Domain.Entities.EType;
using Domain.Shared.Entities;

namespace Domain.Entities
{
    public class AppTransaction : BaseEntity
    {
        #region Relansionships
        public Category Categories { get; set; }
        public Person People { get; set; }

        #endregion

        public AppTransaction() { }

        public AppTransaction(string description, decimal value, ETypeFinancial typeFinancial, Guid personId, Guid categoryId)
        {
            Description = description;
            Value = value;
            TypeFinancial = typeFinancial;
            PersonId = personId;
            CategoryId = categoryId;
        }

        public string Description { get; set; }
        public decimal Value { get; set; }
        public ETypeFinancial TypeFinancial { get; set; }
        public Guid CategoryId { get; set; }
        public Guid PersonId { get; set; }
    }
}

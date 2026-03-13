using MediatR;

namespace Domain.Shared.Request
{
    public class BaseRequest<TResponse> : IRequest<TResponse>
    {
        private Guid Id;
        private int NumberRegistry { get; set; }
        private bool Resume { get; set; }
        private int Page { get; set; }

        public void SetId(Guid id)
        {
            Id = id;
        }

        public Guid GetId()
        {
            return Id;
        }

        public int GetNumber()
        {
            if (NumberRegistry == 0) return 9999;

            return NumberRegistry;
        }

        public int GetPage()
        {
            if (Page == 0) return 1;

            return Page;
        }

        public void SetNumberRegistryPage(int numberRegistry, int page)
        {
            NumberRegistry = numberRegistry;

            Page = page;
        }

        public void SetResume(bool resume)
        {
            Resume = resume;
        }

        public bool GetResume()
        {
            return Resume;
        }
    }
}

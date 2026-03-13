using System.Reflection;

namespace Infrastructure.Shared;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}

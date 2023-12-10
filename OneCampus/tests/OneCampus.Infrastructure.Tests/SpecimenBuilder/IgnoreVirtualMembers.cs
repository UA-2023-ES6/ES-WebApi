using AutoFixture.Kernel;
using System.Reflection;

namespace OneCampus.Infrastructure.Tests;

public class IgnoreVirtualMembers : ISpecimenBuilder
{
    public object Create(object request, ISpecimenContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var pi = request as PropertyInfo;
        if (pi == null)
        {
            return new NoSpecimen();
        }

        if (pi.GetGetMethod()!.IsVirtual)
        {
            return null!;
        }
        return new NoSpecimen();
    }
}

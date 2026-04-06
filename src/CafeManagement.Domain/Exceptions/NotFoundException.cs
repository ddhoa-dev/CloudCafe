namespace CafeManagement.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"Entity '{entityName}' với ID ({key}) không tìm thấy.")
    {
    }
}

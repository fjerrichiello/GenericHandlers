using System.ComponentModel.DataAnnotations;

namespace Common.DbContext;

public class ApplicationDate
{
    [Key]
    public required DateTimeOffset DateValue { get; set; }
}
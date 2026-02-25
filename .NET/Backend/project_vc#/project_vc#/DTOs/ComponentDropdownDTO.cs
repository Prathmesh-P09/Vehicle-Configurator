using System.Collections.Generic;

namespace project_vc_.DTOs;

public class ComponentDropdownDTO
{
    public int? BaseCompId { get; set; }
    public string? ComponentName { get; set; }
    public List<OptionDTO>? Options { get; set; }

    public ComponentDropdownDTO() { }
    public ComponentDropdownDTO(int? baseCompId, string? componentName, List<OptionDTO>? options)
    {
        BaseCompId = baseCompId;
        ComponentName = componentName;
        Options = options;
    }
}

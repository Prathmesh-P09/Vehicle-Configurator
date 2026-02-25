using System.Collections.Generic;

namespace project_vc_.DTOs;

public class AlternateComponentSaveDTO
{
    public int? ModelId { get; set; }
    public List<AlternateComponentDTO>? Components { get; set; }
}

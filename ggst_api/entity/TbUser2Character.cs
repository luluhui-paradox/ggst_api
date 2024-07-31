using System;
using System.Collections.Generic;

namespace ggst_api.entity;

public partial class TbUser2Character
{
    public int TbUser2CharacterId { get; set; }

    public int UserId { get; set; }

    public string PlayUuid { get; set; } = null!;

    public virtual TbLoginUser User { get; set; } = null!;

    
}

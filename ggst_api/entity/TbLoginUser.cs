using System;
using System.Collections.Generic;

namespace ggst_api.entity;

public partial class TbLoginUser
{
    public int LoginUserId { get; set; }

    public string LoginUserAccount { get; set; } = null!;

    public string LoginUserName { get; set; } = null!;

    public string LoginUserPassword { get; set; } = null!;

    public int LoginUserRole { get; set; }

    public virtual ICollection<TbUser2Character> TbUser2Characters { get; set; } = new List<TbUser2Character>();
}

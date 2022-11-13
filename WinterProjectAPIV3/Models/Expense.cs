using System;
using System.Collections.Generic;

namespace WinterProjectAPIV3.Models;

public partial class Expense
{
    //Attribute to add:     NAME
    //Attribute to add:     DESCRIPTION
    public int ExpenseId { get; set; }

    public int? UserGroupId { get; set; }

    public double? Amount { get; set; }

    public virtual UserGroup? UserGroup { get; set; }
}

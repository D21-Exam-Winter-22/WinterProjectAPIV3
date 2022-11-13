namespace WinterProjectAPIV3.DataTransferObjects;

public class MoneyOwedByUserGroupDto
{
    public int UserID { get; set; }
    public int GroupID { get; set; }
    public double AmountOwed { get; set; }
}
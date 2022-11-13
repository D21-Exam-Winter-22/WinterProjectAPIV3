namespace WinterProjectAPIV3.DataTransferObjects
{
    public class CreateExpenditureDto
    {
        public int UserID { get; set; }
        public int GroupID { get; set; }
        public double? Amount { get; set; }
    }
}

namespace WinterProjectAPIV3.DataTransferObjects
{
    public class CreateShareGroupDto
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool HasConcluded { get; set; }

        public int UserID { get; set; }
    }
}

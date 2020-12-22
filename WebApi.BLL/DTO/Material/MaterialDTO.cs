namespace WebApi.BLL.DTO.Material
{
    public class MaterialDTO
    {

        public int Id { get; set; }
        
        public string Name { get; set; }

        public int CategoryId { get; set; }

        public int ActualVersionNum { get; set; }

        public string OwnerUserId { get; set; }
    }
}

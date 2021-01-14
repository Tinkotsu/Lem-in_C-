namespace WebApi.BLL.BusinessModels.Material
{
    public class MaterialFileBm
    {
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public byte[] FileBytes { get; set; }
    }
}
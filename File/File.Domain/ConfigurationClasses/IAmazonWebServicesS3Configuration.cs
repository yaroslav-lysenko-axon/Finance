namespace File.Domain.ConfigurationClasses
{
    public interface IAmazonWebServicesS3Configuration
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string IpAddressAndPort { get; set; }
        public string AuthRegion { get; set; }
        public string BucketName { get; set; }
    }
}

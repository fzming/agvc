namespace AgvcService.Users.Models
{
    public class UserAddressContactorDto
    {
        public string AddressId { get; set; }

        /// <summary>
        ///     名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     详细地址
        /// </summary>
        public string Address { get; set; }

        public double Lng { get; set; }
        public double Lat { get; set; }
        public LiteContactorDto Contactor { get; set; }
    }
}
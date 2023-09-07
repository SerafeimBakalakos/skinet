using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class AddressDto
    {
    // Validations should also exist here in the Data Acces Layer. Validations should also exist in the Presentation Layer, i.e. the DTOs and Controlers. And there should be client-side validations
    //QUESTION: What about validations in the Entities? The teacher of the Udemy course says that the ones in DAL cover those in Entities, but I am not sold.

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string ZipCode { get; set; }
    }
}
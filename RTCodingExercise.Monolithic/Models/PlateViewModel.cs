using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using RTCodingExercise.Monolithic.Services;

namespace RTCodingExercise.Monolithic.Models
{
    public class PlateViewModel
    {
        [Required(ErrorMessage = "{0} is required")]
        [RegularExpression("[A-Z0-9]+", ErrorMessage = "Plate must only contain uppercase letters or numbers")]
        [MaxLength(PlateLetterHelper.MaxPlateLength, ErrorMessage = "Plate must not contain more than {0} characters")]
        public string? Plate { get; set; }

        [DisplayName("Purchase Price")]
        [Required(ErrorMessage = "{0} is required")]
        [Range(0.01, 9_999_999.99, ErrorMessage = "Value for {0} can be no less than £{1} and no greater than £{2}")]
        public decimal PurchasePrice { get; set; }

        [DisplayName("Sale Price")]
        [Required(ErrorMessage = "{0} is required")]
        [Range(0.01, 9_999_999.99, ErrorMessage = "Value for {0} can be no less than £{1} and no greater than £{2}")]
        public decimal SalePrice { get; set; }
    }
}

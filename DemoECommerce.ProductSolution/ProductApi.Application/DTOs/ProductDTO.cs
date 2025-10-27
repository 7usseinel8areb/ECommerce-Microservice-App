using System.ComponentModel.DataAnnotations;

namespace ProductApi.Application.DTOs;

public record ProductDTO(
    int Id,
    [Required]string Name,
    [Required, DataType(DataType.Currency)]decimal Price,
    [Required, Range(1, int.MaxValue)] int Quantity
);

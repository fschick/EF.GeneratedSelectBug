using System;
using System.ComponentModel.DataAnnotations;

namespace EF.GeneratedSelectBug.Models;

public class Order
{
    [Required]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string Number { get; set; }

    [Required]
    public Guid CustomerId { get; set; }

    public Customer Customer { get; set; }

    [Required]
    [Range(0, double.PositiveInfinity)]
    public int HourlyRate { get; set; }
}
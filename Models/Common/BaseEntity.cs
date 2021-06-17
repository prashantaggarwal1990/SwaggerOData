using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SwaggerOData.Models
{
	public class BaseEntity : IBaseEntity
	{
		[Key]
		public Guid Id { get; set; }

		[Display(Name = "Created", Description="DateTime of creation")]
		public DateTime Created { get; set; } = DateTime.UtcNow;
	    
		[Display(Name = "Modified", Description="DateTime of modification")]
		public DateTime Modified { get; set; } = DateTime.UtcNow;
	}
}

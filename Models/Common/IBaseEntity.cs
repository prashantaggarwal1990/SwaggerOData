using System;

namespace SwaggerOData.Models
{
    public interface IBaseEntity
    {
		Guid Id { get; set; }

	    DateTime Created { get; set; }
		
	    DateTime Modified { get; set; }
    }
}
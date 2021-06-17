using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwaggerOData.Models;
using SwaggerOData.Repositories;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SwaggerOData.Controllers
{
    [ApiVersion("1")]
	[ODataRoutePrefix("employees")]
	[Produces("application/json")]
	public class EmployeesController : ODataController
	{
		private readonly IEmployeeRepository _repository;
		private readonly IServiceProvider _services;

		public EmployeesController(IEmployeeRepository repository, IServiceProvider services)
		{
			_repository = repository;
			_services = services;
		}

		[HttpDelete]
		[ProducesResponseType((int)HttpStatusCode.NoContent)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> Delete([FromODataUri] Guid key)
		{
			var entity = await _repository.FindOneAsync(employee => employee.Id == key);
			if (entity == null)
			{
				return NotFound();
			}
			await _repository.DeleteAsync(entity);
			return NoContent();
		}

		[HttpGet, EnableQuery(PageSize = 10, MaxExpansionDepth = 5), ODataRoute]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Employee>))]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> Get()
		{
			var entity = await _repository.FindAsync(e => e.Books);
			if (entity == null)
			{
				return NotFound();
			}
			return Ok(entity);
		}

		[HttpGet]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Employee))]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> Get([FromODataUri] Guid key)
		{
			var entity = await _repository.FindOneAsync(emp => emp.Id== key);
			if (entity == null)
			{
				return NotFound();
			}
			return Ok(entity);
		}

		[AcceptVerbs("PATCH")]
		[EnableQuery]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Employee))]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.NotFound)]
		public async Task<IActionResult> Patch([FromODataUri] Guid key, [FromBody] Delta<Employee> patch)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var entity = await _repository.FindOneAsync(emp => emp.Id== key);
			if (entity == null)
			{
				return NotFound();
			}
			patch.Patch(entity);
			if (!ModelState.IsValid)
			{
				return new BadRequestObjectResult(ModelState);
			}
			try
			{
				await _repository.UpdateAsync(entity);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (await _repository.FindOneAsync(emp => emp.Id== key) == null)
				{
					return NotFound();
				}
				throw;
			}
			return Ok(await _repository.FindOneAsync(emp => emp.Id == key));
		}

		[HttpPost]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Employee))]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		public async Task<IActionResult> Post([FromBody] Employee employee)
		{
			if (employee == null)
			{
				return Unauthorized();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			await _repository.InsertAsync(employee);
			return Ok(await _repository.FindOneAsync(emp => emp.Id == employee.Id));
		}

		[HttpPut]
		[ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Employee))]
		[ProducesResponseType((int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Unauthorized)]
		public async Task<IActionResult> Put([FromODataUri] Guid key, [FromBody] Employee employee)
		{
			if (employee == null)
			{
				return Unauthorized();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (!key.Equals(employee.Id))
			{
				return BadRequest();
			}
			try
			{
				await _repository.UpdateAsync(employee);
			}
			catch (DbUpdateConcurrencyException)
			{
				if (await _repository.FindOneAsync(emp => emp.Id == key) == null)
				{
					return NotFound();
				}
				throw;
			}
			return Ok(await _repository.FindOneAsync(emp => emp.Id == key));
		}
	}
}

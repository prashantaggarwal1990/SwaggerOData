using Microsoft.Extensions.Logging;
using SwaggerOData.DbContexts;
using SwaggerOData.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwaggerOData.Repositories
{
    public sealed class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext appDbContext, ILoggerFactory loggerFactory) 
	        : base(appDbContext, loggerFactory)
        {
        }

		public void Initialize()
		{
			if (_context.Employees.Any())
			{
				return;
			}

			_context.Employees.AddRange(
				new Employee("John", "Whyne", new DateTime(1991, 8, 7)) { Id = new Guid("e8a44dbf-c63b-4993-801e-d880525a4f2c"), Created = DateTime.UtcNow, Books = new List<Book>(), Properties = new Dictionary<string, object> { { "street", "Main St"}, { "city", "New York" } } },
				new Employee("Mathias", "Gernold", new DateTime(1997, 10, 12)) { Id = new Guid("770c4caa-4c5b-4e2d-ace8-21577523a72b"), Created = DateTime.UtcNow, Books = new List<Book>(), Properties = new Dictionary<string, object> { { "street", "Broad St" }, { "city", "Boston" } } },
				new Employee("Julia", "Reynolds", new DateTime(1955, 12, 16)) { Id = new Guid("4533fdbc-376e-4a93-960a-56b83d70d707"), Created = DateTime.UtcNow, Books = new List<Book>() },
				new Employee("Alois", "Mock", new DateTime(1935, 2, 9)) { Id = new Guid("2a4d916d-cf97-4f6f-98f8-d66f01249b3d"), Created = DateTime.UtcNow, Books = new List<Book>() },
				new Employee("Gertraud", "Bochold", new DateTime(2001, 3, 4)) { Id = new Guid("285ed95a-c687-49ce-a9ec-cb0476bf31f0"), Created = DateTime.UtcNow, Books = new List<Book>() }
			);

			_context.SaveChanges();

			//_context.Books.AddRange(
			//	new Book { Id = new Guid("e8a44dbf-c63b-4993-801e-d880525a4f2c"), Name = "Book 1", AuthorId = new Guid("e8a44dbf-c63b-4993-801e-d880525a4f2c"), Author = _context.Employees.Find(1) },
			//	new Book { Id = new Guid("770c4caa-4c5b-4e2d-ace8-21577523a72b"), Name = "Book 2", AuthorId = new Guid("770c4caa-4c5b-4e2d-ace8-21577523a72b"), Author = _context.Employees.Find(2) },
			//	new Book { Id = new Guid("4533fdbc-376e-4a93-960a-56b83d70d707"), Name = "Book 3", AuthorId = new Guid("4533fdbc-376e-4a93-960a-56b83d70d707"), Author = _context.Employees.Find(3) },
			//	new Book { Id = new Guid("2a4d916d-cf97-4f6f-98f8-d66f01249b3d"), Name = "Book 4", AuthorId = new Guid("2a4d916d-cf97-4f6f-98f8-d66f01249b3d"), Author = _context.Employees.Find(4) }
			//);

			//_context.SaveChanges();
		}
	}
}

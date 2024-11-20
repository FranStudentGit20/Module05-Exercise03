using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Module05_Exercise01.Model;
using MySql.Data.MySqlClient;

namespace Module05_Exercise01.Services
{
    internal class EmployeeService
    {
        private readonly string _connectionString;

        public EmployeeService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            var employeeService = new List<Employee>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                //Retrieve Data
                var cmd = new MySqlCommand("SELECT * FROM tblemployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        employeeService.Add(new Employee
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"),
                            email = reader.GetString("email"),
                            ContactNo = reader.GetString("ContactNo"),
                        });
                    }
                }
            }
            return employeeService;
        }

        public async Task<bool> AddEmployeeAsync(Employee newEmployee)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("INSERT INTO tblemployee (Name, Address, email, ContactNo) VALUES (@Name, @Address, @Email, @ContactNo)", conn);
                    cmd.Parameters.AddWithValue("@Name", newEmployee.Name);
                    cmd.Parameters.AddWithValue("@Email", newEmployee.email);
                    cmd.Parameters.AddWithValue("@Address", newEmployee.Address);
                    cmd.Parameters.AddWithValue("@ContactNo", newEmployee.ContactNo);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding personal record: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> DeleteEmployeeAsync(int employeeID)
        {
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("DELETE FROM tblemployee WHERE employeeID = @ID", conn);
                    cmd.Parameters.AddWithValue("@ID", employeeID);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting personal record: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
          string connectionString = "Server=localhost;Database=companydb;User ID=testuser; Password=testuser";

            try
            {
                // Establish a connection to the database
                using (var connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // SQL query to update employee details
                    string updateQuery = @"UPDATE tblEmployee
                                   SET Name = @Name, Address = @Address, Email = @Email, ContactNo = @ContactNo
                                   WHERE EmployeeID = @EmployeeID";

                    using (var command = new MySqlCommand(updateQuery, connection))
                    {
                        // Add parameters to prevent SQL injection
                        command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeID);
                        command.Parameters.AddWithValue("@Name", employee.Name);
                        command.Parameters.AddWithValue("@Address", employee.Address);
                        command.Parameters.AddWithValue("@Email", employee.email);
                        command.Parameters.AddWithValue("@ContactNo", employee.ContactNo);

                        // Execute the query and check the result
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        return rowsAffected > 0; // Returns true if the update was successful
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Employee>> SearchEmployeeAsync(string searchQuery)
        {
            var employeeService= new List<Employee>();
            try
            {
                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    var cmd = new MySqlCommand("SELECT * FROM tblemployee WHERE Name LIKE @SearchQuery", conn);
                    cmd.Parameters.AddWithValue("@SearchQuery", $"{searchQuery}");

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            employeeService.Add(new Employee
                            {
                                EmployeeID = reader.GetInt32("EmployeeID"),
                                Name = reader.GetString("Name"),
                                Address = reader.GetString("Address"),
                                email = reader.GetString("email"),
                                ContactNo = reader.GetString("ContactNo")
                            });
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error searching employees: {ex.Message}");
            }
            return employeeService;
        }
    }
}

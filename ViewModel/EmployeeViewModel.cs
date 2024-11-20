using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module05_Exercise01.Model;
using Module05_Exercise01.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Module05_Exercise01.ViewModel
{
    internal class EmployeeViewModel:INotifyPropertyChanged
    {
        private readonly EmployeeService _employeeService;
        public ObservableCollection<Employee> EmployeeList { get; set; }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }

        }
        private Employee _selectedEmployee;
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                OnPropertyChanged(nameof(SelectedEmployee));
                OnPropertyChanged(nameof(IsEmployeeSelected)); // Enable Update button
                if (_selectedEmployee != null)
                {

                    UpdateEmployeeName = _selectedEmployee.Name;
                    UpdateEmployeeAddress = _selectedEmployee.Address;
                    UpdateEmployeeEmail = _selectedEmployee.email;
                    UpdateEmployeeContactNo = _selectedEmployee.ContactNo;
                }
            }
            
        }
        private bool _isEmployeeSelected;

        public bool IsEmployeeSelected => SelectedEmployee != null;
        private string _statusMessage;

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }
        public string _newEmployeeName;

        public string NewEmployeeName
        {
            get => _newEmployeeName;
            set
            {
                _newEmployeeName = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeAddress;

        public string NewEmployeeAddress
        {
            get => _newEmployeeAddress;
            set
            {
                _newEmployeeAddress = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeEmail;

        public string NewEmployeeEmail
        {
            get => _newEmployeeEmail;
            set
            {
                _newEmployeeEmail = value;
                OnPropertyChanged();
            }
        }

        private string _newEmployeeContactNo;

        public string NewEmployeeContactNo
        {
            get => _newEmployeeContactNo;
            set
            {
                _newEmployeeContactNo = value;
                OnPropertyChanged();
            }
        }

        private string _searchQuery;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
            }
        }

        private string _updateEmployeeName;
        public string UpdateEmployeeName
        {
            get => _updateEmployeeName;
            set
            {
                _updateEmployeeName = value;
                OnPropertyChanged();
            }
        }

        private string _updateEmployeeAddress;
        public string UpdateEmployeeAddress
        {
            get => _updateEmployeeAddress;
            set
            {
                _updateEmployeeAddress = value;
                OnPropertyChanged();
            }
        }

        private string _updateEmployeeEmail;
        public string UpdateEmployeeEmail
        {
            get => _updateEmployeeEmail;
            set
            {
                _updateEmployeeEmail = value;
                OnPropertyChanged();
            }
        }

        private string _updateEmployeeContactNo;
        public string UpdateEmployeeContactNo
        {
            get => _updateEmployeeContactNo;
            set
            {
                _updateEmployeeContactNo = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Name cannot be empty.");
                _name = value;
            }
        }
        private string _name;

        public ICommand LoadDataCommand { get; }
        public ICommand AddEmployeeCommand { get; }

        public ICommand SelectedEmployeeCommand { get; }

        public ICommand DeleteEmployeeCommand { get; }

        public ICommand SearchEmployeeCommand {  get; }

        public ICommand UpdateEmployeeCommand {  get; }


        public Command<Employee> SelectEmployeeCommand { get; }
        //PweaonalViewModel Constructor
        public EmployeeViewModel()
        {
            _employeeService = new EmployeeService();
            EmployeeList = new ObservableCollection<Employee>();
            LoadDataCommand = new Command(async () => await LoadData());
            AddEmployeeCommand = new Command(async () => await AddEmployee());
            SelectedEmployeeCommand = new Command<Employee>(employee => SelectedEmployee = employee);
            DeleteEmployeeCommand = new Command(async () => await DeleteEmployee(), () => IsEmployeeSelected);
            SearchEmployeeCommand = new Command(async () => await SearchEmployee());
            UpdateEmployeeCommand = new Command(UpdateEmployee, () => IsEmployeeSelected);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(IsEmployeeSelected))
                {
                    ((Command)UpdateEmployeeCommand).ChangeCanExecute();
                }
            };

            LoadData();
        }

        public async Task LoadData()
        {
            IsBusy = true;
            StatusMessage = "Loading employee data...";

            try
            {
                // Fetch updated data from the service
                var employees = await _employeeService.GetEmployeesAsync();

                // Update the ObservableCollection
                EmployeeList.Clear();
                foreach (var emp in employees)
                {
                    if (!string.IsNullOrEmpty(emp.Name) && !string.IsNullOrEmpty(emp.Address))
                    {
                        EmployeeList.Add(emp);
                    }
                }

                StatusMessage = "Data loaded successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }

        }

        private async Task AddEmployee()
        {
            if (IsBusy || string.IsNullOrWhiteSpace(NewEmployeeName) || string.IsNullOrWhiteSpace(NewEmployeeAddress) || string.IsNullOrWhiteSpace(NewEmployeeEmail) || string.IsNullOrWhiteSpace(NewEmployeeContactNo))
            {
                StatusMessage = "Please fill in all fields before adding";
                return;
            }
            IsBusy = true;
            StatusMessage = "Adding new person...";
            try
            {
                var newEmployee = new Employee
                {
                    Name = NewEmployeeName,
                    Address = NewEmployeeAddress,
                    email = NewEmployeeEmail,
                    ContactNo = NewEmployeeContactNo
                };
                var isSuccess = await _employeeService.AddEmployeeAsync(newEmployee);
                if (isSuccess)
                {
                    NewEmployeeName = string.Empty;
                    NewEmployeeAddress = string.Empty;
                    NewEmployeeEmail = string.Empty;
                    NewEmployeeContactNo = string.Empty;
                }
                else
                {
                    StatusMessage = "Failed to add new person";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Failed adding person: {ex.Message}";
            }
            finally { IsBusy = false; }
        }

        private async Task DeleteEmployee()
        {
            if (SelectedEmployee == null) return;
            var answer = await Application.Current.MainPage.DisplayAlert("Confirm Delete", $"Are you sure you want to delete{SelectedEmployee.Name}?",
                "Yes", "No");

            if (!answer) return;

            IsBusy = true;
            StatusMessage = "Deleting person...";

            try
            {
                var success = await _employeeService.DeleteEmployeeAsync(SelectedEmployee.EmployeeID);
                StatusMessage = success ? "Person deleted successfully!" : "Failed to delete Employee";

                if (success)
                {
                    EmployeeList.Remove(SelectedEmployee);
                    SelectedEmployee = null;
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error deleting person: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchEmployee()
        {
            if(string.IsNullOrWhiteSpace(SearchQuery)) return;
            IsBusy= true;
            StatusMessage = "Searching...";
            try
            {
                var employees = await _employeeService.GetEmployeesAsync();
                var filteredEmployees = employees.Where(emp => emp.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase));
                EmployeeList.Clear();
                foreach (var employee in filteredEmployees)
                {
                    EmployeeList.Add(employee);
                }
                StatusMessage = "Search completed";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Search failed: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void UpdateEmployee()
        {
            if (SelectedEmployee == null) return;

            // Sync UI fields with SelectedEmployee properties
            SelectedEmployee.Name = UpdateEmployeeName;
            SelectedEmployee.Address = UpdateEmployeeAddress;
            SelectedEmployee.email = UpdateEmployeeEmail;
            SelectedEmployee.ContactNo = UpdateEmployeeContactNo;

            IsBusy = true;
            StatusMessage = "Updating employee...";

            try
            {
                var isUpdated = await _employeeService.UpdateEmployeeAsync(SelectedEmployee);

                if (isUpdated)
                {
                    StatusMessage = "Employee updated successfully!";

                    // Refresh the UI collection
                    var existingEmployee = EmployeeList.FirstOrDefault(e => e.EmployeeID == SelectedEmployee.EmployeeID);
                    if (existingEmployee != null)
                    {
                        existingEmployee.Name = SelectedEmployee.Name;
                        existingEmployee.Address = SelectedEmployee.Address;
                        existingEmployee.email = SelectedEmployee.email;
                        existingEmployee.ContactNo = SelectedEmployee.ContactNo;
                    }
                }
                else
                {
                    StatusMessage = "Failed to update employee.";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ClearFields()
        {
            NewEmployeeName = string.Empty;
            NewEmployeeAddress = string.Empty;
            NewEmployeeEmail = string.Empty;
            NewEmployeeContactNo = string.Empty;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

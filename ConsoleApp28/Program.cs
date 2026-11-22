#region complete code

//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Net.Mail;
//using System.Reflection.Emit;

//// PART 1: MAIN PROGRAM AND INTERACTIVE MENU

//using (var db = new BankContext())
//{
//    try
//    {
//        db.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("Database Error: " + ex.GetBaseException().Message);
//        Console.WriteLine("Check your SQL Server connection.");
//        return;
//    }
//}

//while (true)
//{
//    Console.Clear();

//    Console.WriteLine("======================================");
//    Console.WriteLine("       National Bank - Management");
//    Console.WriteLine("======================================");
//    Console.WriteLine("1) Add a new Customer");
//    Console.WriteLine("2) Open a new Account for a Customer");
//    Console.WriteLine("3) Update Account Status (Active / Closed)");
//    Console.WriteLine("4) Remove an Account from a Customer");
//    Console.WriteLine("5) List all Customers (with accounts)");
//    Console.WriteLine("0) Exit");
//    Console.WriteLine("--------------------------------------");

//    int choice = ReadInt("Enter choice: ", 0, 5);

//    try
//    {
//        switch (choice)
//        {
//            case 1:
//                AddCustomer();
//                break;

//            case 2:
//                OpenAccount();
//                break;

//            case 3:
//                UpdateAccountStatus();
//                break;

//            case 4:
//                RemoveAccountFromCustomer();
//                break;

//            case 5:
//                ListCustomersWithAccounts();
//                break;

//            case 0:
//                Console.WriteLine("Goodbye!");
//                return;
//        }
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine("Error: " + ex.GetBaseException().Message);
//    }

//    Pause();
//}


//// PART 2: ADD A NEW CUSTOMER

//static void AddCustomer()
//{
//    using var db = new BankContext();

//    Console.WriteLine("\n--- Add New Customer ---");

//    string fullName = ReadRequired("Full Name: ");
//    string nationalId = ReadNationalId();
//    DateTime dateOfBirth = ReadDate("Date of Birth (yyyy-MM-dd): ");
//    string email = ReadEmail();
//    string phone = ReadRequired("Phone: ");
//    string address = ReadRequired("Address: ");

//    Console.WriteLine("Customer Type:");
//    Console.WriteLine("1) Individual");
//    Console.WriteLine("2) Business");

//    var customerType = (CustomerType)ReadInt("Choice: ", 1, 2);

//    bool exists = db.Customers.Any(c => c.NationalId == nationalId);

//    if (exists)
//    {
//        Console.WriteLine("This National ID already exists.");
//        return;
//    }

//    var customer = new Customer
//    {
//        FullName = fullName,
//        NationalId = nationalId,
//        DateOfBirth = dateOfBirth,
//        Email = email,
//        PhoneNumber = phone,
//        Address = address,
//        CustomerType = customerType
//    };

//    db.Customers.Add(customer);
//    db.SaveChanges();

//    Console.WriteLine(
//        $"Customer created successfully. CustomerId = {customer.Id}");
//}


//// PART 3: OPEN A NEW ACCOUNT FOR A CUSTOMER

//static void OpenAccount()
//{
//    using var db = new BankContext();

//    Console.WriteLine("\n--- Open New Account ---");

//    string accountNumber = ReadRequired("Account Number: ");

//    Console.WriteLine("Account Type:");
//    Console.WriteLine("1) Savings");
//    Console.WriteLine("2) Current");
//    Console.WriteLine("3) Business");

//    var accountType = (AccountType)ReadInt("Choice: ", 1, 3);

//    string branchCode =
//        ReadRequired("Branch Code: ").ToUpperInvariant();

//    int customerId = ReadInt("Customer Id: ", 1, int.MaxValue);

//    Console.WriteLine("Ownership Role:");
//    Console.WriteLine("1) Primary");
//    Console.WriteLine("2) CoHolder");

//    var ownershipType =
//        (OwnershipType)ReadInt("Choice: ", 1, 2);

//    var branch = db.Branches
//        .FirstOrDefault(b => b.Code == branchCode);

//    if (branch == null)
//    {
//        Console.WriteLine("Branch does not exist.");
//        return;
//    }

//    var customer = db.Customers.Find(customerId);

//    if (customer == null)
//    {
//        Console.WriteLine("Customer does not exist.");
//        return;
//    }

//    var account = db.Accounts
//        .FirstOrDefault(a => a.AccountNumber == accountNumber);

//    if (account == null)
//    {
//        account = new Account
//        {
//            AccountNumber = accountNumber,
//            AccountType = accountType,
//            BranchCode = branchCode,
//            CurrentBalance = 0,
//            OpeningDate = DateTime.Today
//        };

//        db.Accounts.Add(account);
//    }
//    else
//    {
//        if (account.BranchCode != branchCode ||
//            account.AccountType != accountType)
//        {
//            Console.WriteLine("Account branch or type does not match.");
//            return;
//        }
//    }

//    bool alreadyLinked = db.CustomerAccounts.Any(ca =>
//        ca.CustomerId == customerId &&
//        ca.AccountNumber == accountNumber);

//    if (alreadyLinked)
//    {
//        Console.WriteLine("This customer is already linked to this account.");
//        return;
//    }

//    var customerAccount = new CustomerAccount
//    {
//        CustomerId = customerId,
//        AccountNumber = accountNumber,
//        OwnershipType = ownershipType,
//        OwnershipStartDate = DateTime.Today,
//        AccountStatus = AccountStatus.Active
//    };

//    db.CustomerAccounts.Add(customerAccount);

//    db.SaveChanges();

//    Console.WriteLine(
//        $"Account '{accountNumber}' created/linked successfully.");
//}


//// PART 4: UPDATE ACCOUNT STATUS

//static void UpdateAccountStatus()
//{
//    using var db = new BankContext();

//    Console.WriteLine("\n--- Update Account Status ---");

//    string accountNumber =
//        ReadRequired("Account Number: ");

//    int customerId =
//        ReadInt("Customer Id: ", 1, int.MaxValue);

//    var customerAccount = db.CustomerAccounts
//        .FirstOrDefault(ca =>
//            ca.AccountNumber == accountNumber &&
//            ca.CustomerId == customerId);

//    if (customerAccount == null)
//    {
//        Console.WriteLine("Account or customer relationship not found.");
//        return;
//    }

//    Console.WriteLine("New Status:");
//    Console.WriteLine("1) Active");
//    Console.WriteLine("2) Closed");

//    var newStatus =
//        (AccountStatus)ReadInt("Choice: ", 1, 2);

//    customerAccount.AccountStatus = newStatus;

//    db.SaveChanges();

//    Console.WriteLine(
//        $"Status updated to {newStatus} successfully.");
//}


//// PART 5: REMOVE AN ACCOUNT FROM A CUSTOMER

//static void RemoveAccountFromCustomer()
//{
//    using var db = new BankContext();

//    Console.WriteLine("\n--- Remove Account From Customer ---");

//    string accountNumber =
//        ReadRequired("Account Number: ");

//    int customerId =
//        ReadInt("Customer Id: ", 1, int.MaxValue);

//    var customerAccount = db.CustomerAccounts
//        .FirstOrDefault(ca =>
//            ca.AccountNumber == accountNumber &&
//            ca.CustomerId == customerId);

//    if (customerAccount == null)
//    {
//        Console.WriteLine("Account relationship not found.");
//        return;
//    }

//    db.CustomerAccounts.Remove(customerAccount);
//    db.SaveChanges();

//    bool hasOtherOwners = db.CustomerAccounts.Any(ca =>
//        ca.AccountNumber == accountNumber);

//    if (!hasOtherOwners)
//    {
//        var account = db.Accounts
//            .Include(a => a.Transactions)
//            .FirstOrDefault(a =>
//                a.AccountNumber == accountNumber);

//        if (account != null)
//        {
//            db.Accounts.Remove(account);
//            db.SaveChanges();
//        }

//        Console.WriteLine("Ownership link deleted.");
//        Console.WriteLine(
//            "The account was also removed because it had no other owners.");
//    }
//    else
//    {
//        Console.WriteLine(
//            "Ownership link deleted. Account still has other owners.");
//    }
//}


//// PART 6: LIST ALL CUSTOMERS WITH THEIR ACCOUNTS

//static void ListCustomersWithAccounts()
//{
//    using var db = new BankContext();

//    Console.WriteLine("\n--- All Customers ---");

//    var customers = db.Customers
//        .Include(c => c.CustomerAccounts)
//            .ThenInclude(ca => ca.Account)
//                .ThenInclude(a => a.Branch)
//        .OrderBy(c => c.Id)
//        .ToList();

//    if (customers.Count == 0)
//    {
//        Console.WriteLine("No customers found.");
//        return;
//    }

//    foreach (var customer in customers)
//    {
//        Console.WriteLine(
//            $"\n#{customer.Id} {customer.FullName} ({customer.CustomerType})");

//        if (customer.CustomerAccounts.Count == 0)
//        {
//            Console.WriteLine("   (no accounts)");
//            continue;
//        }

//        foreach (var link in customer.CustomerAccounts)
//        {
//            var account = link.Account;

//            Console.WriteLine(
//                $"   {account.AccountNumber,-12} " +
//                $"{account.AccountType,-9} " +
//                $"Balance: {account.CurrentBalance,12:N2} " +
//                $"{link.OwnershipType,-8} " +
//                $"{link.AccountStatus,-7} " +
//                $"@ {account.Branch.Name}");
//        }
//    }
//}


//// PART 7: INPUT VALIDATION METHODS

//static string ReadRequired(string prompt)
//{
//    while (true)
//    {
//        Console.Write(prompt);

//        string? value = Console.ReadLine()?.Trim();

//        if (!string.IsNullOrWhiteSpace(value))
//            return value;

//        Console.WriteLine("Input cannot be empty.");
//    }
//}

//static int ReadInt(string prompt, int min, int max)
//{
//    while (true)
//    {
//        Console.Write(prompt);

//        string? input = Console.ReadLine();

//        if (int.TryParse(input, out int value) &&
//            value >= min && value <= max)
//        {
//            return value;
//        }

//        Console.WriteLine("Invalid number. Try again.");
//    }
//}

//static DateTime ReadDate(string prompt)
//{
//    while (true)
//    {
//        Console.Write(prompt);

//        string? input = Console.ReadLine();

//        if (DateTime.TryParseExact(
//            input,
//            "yyyy-MM-dd",
//            CultureInfo.InvariantCulture,
//            DateTimeStyles.None,
//            out DateTime date) &&
//            date <= DateTime.Today)
//        {
//            return date;
//        }

//        Console.WriteLine(
//            "Invalid date. Use yyyy-MM-dd and a date in the past.");
//    }
//}

//static string ReadNationalId()
//{
//    while (true)
//    {
//        string id = ReadRequired("National ID (14 digits): ");

//        if (id.Length == 14 && id.All(char.IsDigit))
//            return id;

//        Console.WriteLine(
//            "National ID must contain exactly 14 digits.");
//    }
//}

//static string ReadEmail()
//{
//    while (true)
//    {
//        string email = ReadRequired("Email: ");

//        try
//        {
//            var mail = new MailAddress(email);

//            if (mail.Address == email)
//                return email;
//        }
//        catch
//        {
//        }

//        Console.WriteLine("Invalid email. Try again.");
//    }
//}

//static void Pause()
//{
//    Console.WriteLine(
//        "\nPress any key to return to the menu...");

//    Console.ReadKey(true);
//}


//// PART 8: ENUMS

//public enum CustomerType
//{
//    Individual = 1,
//    Business = 2
//}

//public enum AccountType
//{
//    Savings = 1,
//    Current = 2,
//    Business = 3
//}

//public enum OwnershipType
//{
//    Primary = 1,
//    CoHolder = 2
//}

//public enum AccountStatus
//{
//    Active = 1,
//    Closed = 2
//}

//public enum BankTransactionType
//{
//    Deposit = 1,
//    Withdrawal = 2,
//    Transfer = 3
//}


//// PART 9: ENTITY CLASSES

//public class Branch
//{
//    public string Code { get; set; } = null!;
//    public string Name { get; set; } = null!;
//    public string Address { get; set; } = null!;
//    public string PhoneNumber { get; set; } = null!;

//    public int ManagerId { get; set; }
//    public Manager Manager { get; set; } = null!;

//    public ICollection<Account> Accounts { get; set; }
//        = new List<Account>();
//}

//public class Manager
//{
//    public int Id { get; set; }
//    public string FullName { get; set; } = null!;
//    public string Email { get; set; } = null!;
//    public string PhoneNumber { get; set; } = null!;
//    public DateTime HireDate { get; set; }

//    public Branch Branch { get; set; } = null!;
//}

//public class Customer
//{
//    public int Id { get; set; }
//    public string FullName { get; set; } = null!;
//    public string NationalId { get; set; } = null!;
//    public DateTime DateOfBirth { get; set; }
//    public string Email { get; set; } = null!;
//    public string PhoneNumber { get; set; } = null!;
//    public string Address { get; set; } = null!;
//    public CustomerType CustomerType { get; set; }

//    public ICollection<CustomerAccount> CustomerAccounts { get; set; }
//        = new List<CustomerAccount>();
//}

//public class Account
//{
//    public string AccountNumber { get; set; } = null!;
//    public decimal CurrentBalance { get; set; }
//    public AccountType AccountType { get; set; }
//    public DateTime OpeningDate { get; set; }

//    public string BranchCode { get; set; } = null!;
//    public Branch Branch { get; set; } = null!;

//    public ICollection<CustomerAccount> CustomerAccounts { get; set; }
//        = new List<CustomerAccount>();

//    public ICollection<BankTransaction> Transactions { get; set; }
//        = new List<BankTransaction>();
//}

//public class CustomerAccount
//{
//    public int CustomerId { get; set; }
//    public Customer Customer { get; set; } = null!;

//    public string AccountNumber { get; set; } = null!;
//    public Account Account { get; set; } = null!;

//    public OwnershipType OwnershipType { get; set; }
//    public DateTime OwnershipStartDate { get; set; }
//    public AccountStatus AccountStatus { get; set; }
//}

//public class BankTransaction
//{
//    public int TransactionNumber { get; set; }
//    public DateTime TransactionDate { get; set; }
//    public decimal Amount { get; set; }
//    public BankTransactionType TransactionType { get; set; }
//    public string? Note { get; set; }

//    public string AccountNumber { get; set; } = null!;
//    public Account Account { get; set; } = null!;
//}


//// PART 10: DB CONTEXT AND DATABASE CONFIGURATION

//public class BankContext : DbContext
//{
//    public DbSet<Branch> Branches => Set<Branch>();
//    public DbSet<Manager> Managers => Set<Manager>();
//    public DbSet<Customer> Customers => Set<Customer>();
//    public DbSet<Account> Accounts => Set<Account>();
//    public DbSet<CustomerAccount> CustomerAccounts
//        => Set<CustomerAccount>();
//    public DbSet<BankTransaction> Transactions
//        => Set<BankTransaction>();

//    protected override void OnConfiguring(
//        DbContextOptionsBuilder optionsBuilder)
//    {
//        if (!optionsBuilder.IsConfigured)
//        {
//            optionsBuilder.UseSqlServer(
//                @"Server=(localdb)\MSSQLLocalDB;Database=NationalBankManagementDb;Trusted_Connection=True;TrustServerCertificate=True;");
//        }
//    }


//    // PART 11: FLUENT API AND RELATIONSHIPS

//    protected override void OnModelCreating(ModelBuilder modelBuilder)
//    {
//        base.OnModelCreating(modelBuilder);

//        modelBuilder.Entity<Branch>(entity =>
//        {
//            entity.HasKey(b => b.Code);

//            entity.Property(b => b.Code)
//                .HasMaxLength(10);

//            entity.Property(b => b.Name)
//                .HasMaxLength(100)
//                .IsRequired();

//            entity.Property(b => b.Address)
//                .HasMaxLength(200)
//                .IsRequired();

//            entity.Property(b => b.PhoneNumber)
//                .HasMaxLength(20)
//                .IsRequired();

//            entity.HasOne(b => b.Manager)
//                .WithOne(m => m.Branch)
//                .HasForeignKey<Branch>(b => b.ManagerId)
//                .OnDelete(DeleteBehavior.Restrict);

//            entity.HasIndex(b => b.ManagerId)
//                .IsUnique();
//        });


//        modelBuilder.Entity<Manager>(entity =>
//        {
//            entity.HasKey(m => m.Id);

//            entity.Property(m => m.FullName)
//                .HasMaxLength(100)
//                .IsRequired();

//            entity.Property(m => m.Email)
//                .HasMaxLength(100)
//                .IsRequired();

//            entity.Property(m => m.PhoneNumber)
//                .HasMaxLength(20)
//                .IsRequired();
//        });


//        modelBuilder.Entity<Customer>(entity =>
//        {
//            entity.HasKey(c => c.Id);

//            entity.Property(c => c.FullName)
//                .HasMaxLength(120)
//                .IsRequired();

//            entity.Property(c => c.NationalId)
//                .HasMaxLength(14)
//                .IsRequired();

//            entity.HasIndex(c => c.NationalId)
//                .IsUnique();

//            entity.Property(c => c.Email)
//                .HasMaxLength(120)
//                .IsRequired();

//            entity.Property(c => c.PhoneNumber)
//                .HasMaxLength(20)
//                .IsRequired();

//            entity.Property(c => c.Address)
//                .HasMaxLength(200)
//                .IsRequired();

//            entity.Property(c => c.CustomerType)
//                .HasConversion<string>()
//                .HasMaxLength(20);
//        });


//        modelBuilder.Entity<Account>(entity =>
//        {
//            entity.HasKey(a => a.AccountNumber);

//            entity.Property(a => a.AccountNumber)
//                .HasMaxLength(30);

//            entity.Property(a => a.CurrentBalance)
//                .HasPrecision(18, 2);

//            entity.Property(a => a.AccountType)
//                .HasConversion<string>()
//                .HasMaxLength(20);

//            entity.HasOne(a => a.Branch)
//                .WithMany(b => b.Accounts)
//                .HasForeignKey(a => a.BranchCode)
//                .OnDelete(DeleteBehavior.Restrict);
//        });


//        modelBuilder.Entity<CustomerAccount>(entity =>
//        {
//            entity.HasKey(ca => new
//            {
//                ca.CustomerId,
//                ca.AccountNumber
//            });

//            entity.Property(ca => ca.OwnershipType)
//                .HasConversion<string>()
//                .HasMaxLength(20);

//            entity.Property(ca => ca.AccountStatus)
//                .HasConversion<string>()
//                .HasMaxLength(20);

//            entity.HasOne(ca => ca.Customer)
//                .WithMany(c => c.CustomerAccounts)
//                .HasForeignKey(ca => ca.CustomerId)
//                .OnDelete(DeleteBehavior.Cascade);

//            entity.HasOne(ca => ca.Account)
//                .WithMany(a => a.CustomerAccounts)
//                .HasForeignKey(ca => ca.AccountNumber)
//                .OnDelete(DeleteBehavior.Cascade);
//        });


//        modelBuilder.Entity<BankTransaction>(entity =>
//        {
//            entity.HasKey(t => t.TransactionNumber);

//            entity.Property(t => t.TransactionNumber)
//                .ValueGeneratedOnAdd();

//            entity.Property(t => t.Amount)
//                .HasPrecision(18, 2);

//            entity.Property(t => t.TransactionType)
//                .HasConversion<string>()
//                .HasMaxLength(20);

//            entity.Property(t => t.Note)
//                .HasMaxLength(300);

//            entity.HasOne(t => t.Account)
//                .WithMany(a => a.Transactions)
//                .HasForeignKey(t => t.AccountNumber)
//                .OnDelete(DeleteBehavior.Cascade);
//        });


//        // PART 12: SEEDING

//        modelBuilder.Entity<Manager>().HasData(
//            new Manager
//            {
//                Id = 1,
//                FullName = "Ahmed Hassan",
//                Email = "ahmed@nationalbank.com",
//                PhoneNumber = "01000000001",
//                HireDate = new DateTime(2018, 1, 10)
//            },

//            new Manager
//            {
//                Id = 2,
//                FullName = "Mona Ali",
//                Email = "mona@nationalbank.com",
//                PhoneNumber = "01000000002",
//                HireDate = new DateTime(2019, 5, 15)
//            }
//        );


//        modelBuilder.Entity<Branch>().HasData(
//            new Branch
//            {
//                Code = "CAI-01",
//                Name = "Cairo Main Branch",
//                Address = "Tahrir Square, Cairo",
//                PhoneNumber = "0220000001",
//                ManagerId = 1
//            },

//            new Branch
//            {
//                Code = "ALX-01",
//                Name = "Alexandria Branch",
//                Address = "Corniche Road, Alexandria",
//                PhoneNumber = "0330000002",
//                ManagerId = 2
//            }
//        );
//    }
//}

#endregion
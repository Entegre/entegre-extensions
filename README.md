<p align="center">
  <h1 align="center">Entegre.Extensions</h1>
  <p align="center">
    <strong>A comprehensive .NET 8+ developer toolkit</strong>
  </p>
  <p align="center">
    Extensions for strings, collections, encryption, transformations, dates with holidays, validation, and functional programming patterns.
  </p>
</p>

<p align="center">
  <a href="https://www.nuget.org/packages/Entegre.Extensions">
    <img src="https://img.shields.io/nuget/v/Entegre.Extensions?style=flat-square&logo=nuget&logoColor=white&label=NuGet&color=004880" alt="NuGet Version">
  </a>
  <a href="https://www.nuget.org/packages/Entegre.Extensions">
    <img src="https://img.shields.io/nuget/dt/Entegre.Extensions?style=flat-square&logo=nuget&logoColor=white&label=Downloads&color=004880" alt="NuGet Downloads">
  </a>
  <a href="https://dotnet.microsoft.com/">
    <img src="https://img.shields.io/badge/.NET-8.0%20|%209.0-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt=".NET 8 | 9">
  </a>
  <a href="LICENSE">
    <img src="https://img.shields.io/badge/License-MIT-green?style=flat-square" alt="MIT License">
  </a>
</p>

---

## Quick Start

### Installation

```bash
# Package Manager
Install-Package Entegre.Extensions

# .NET CLI
dotnet add package Entegre.Extensions

# PackageReference
<PackageReference Include="Entegre.Extensions" Version="1.0.0" />
```

### Basic Usage

```csharp
using Entegre.Extensions;

// String operations
"Türkçe Başlık".ToSlug();              // "turkce-baslik"
"Hello World".Truncate(8);              // "Hello..."
"test@example.com".IsEmail();           // true

// Collections
items.Batch(100);                        // Split into batches
items.DistinctBy(x => x.Category);       // Distinct by property
people.FuzzySearch("Jon", p => p.Name);  // Fuzzy matching

// Cryptography
"password".ToBCrypt();                   // Secure password hash
"secret".EncryptAES("key");              // AES encryption

// DateTime with holidays
date.AddWorkdays(5, HolidayProvider.Turkey);
date.IsHoliday(HolidayProvider.US);

// Validation
Guard.Against.NullOrEmpty(name);
user.Validate().ValidEmail(x => x.Email).Build();

// Result pattern
Result.Success(data).Map(x => x.Name).OnFailure(HandleError);
```

---

## Table of Contents

- [String Extensions](#-string-extensions)
- [Collection Extensions](#-collection-extensions)
- [Cryptography Extensions](#-cryptography-extensions)
- [DateTime Extensions](#-datetime-extensions)
- [Holiday Calendar System](#-holiday-calendar-system)
- [Transformation Extensions](#-transformation-extensions)
- [Validation](#-validation)
- [Result Pattern](#-result-pattern)
- [Maybe/Option Type](#-maybeoption-type)
- [Either Type](#-either-type)
- [License](#license)

---

## Features

### 🔤 String Extensions

Comprehensive string manipulation with Turkish language support.

<details>
<summary><b>View Examples</b></summary>

```csharp
using Entegre.Extensions;

// Fluent null checks
"hello".IsNullOrEmpty();           // false
"  ".IsNullOrWhiteSpace();         // true

// URL-friendly slugs (Turkish character support)
"Türkçe Başlık".ToSlug();          // "turkce-baslik"
"Hello World!".ToSlug();           // "hello-world"

// Safe string operations
"Hello World".Truncate(8);          // "Hello..."
"Hello World".Truncate(8, "…");     // "Hello W…"
"Hello".Left(3);                    // "Hel"
"Hello".Right(3);                   // "llo"
"abc".Repeat(3);                    // "abcabcabc"

// Case conversions
"hello world".ToPascalCase();       // "HelloWorld"
"hello world".ToCamelCase();        // "helloWorld"
"HelloWorld".ToSnakeCase();         // "hello_world"
"helloWorld".ToKebabCase();         // "hello-world"

// Encoding
"Hello".ToBase64();                 // "SGVsbG8="
"SGVsbG8=".FromBase64();            // "Hello"
"Hello".ToHex();                    // "48656c6c6f"

// Masking sensitive data
"4111111111111111".Mask(4, 8);      // "4111********1111"

// Validation
"test@example.com".IsEmail();       // true
"https://example.com".IsUrl();      // true
"10000000146".IsTCKN();             // true (Turkish ID)
"TR330006100519786457841326".IsIBAN();    // true
"4111111111111111".IsCreditCard();        // true (Luhn)
```

</details>

---

### 📦 Collection Extensions

Powerful LINQ extensions for filtering, searching, and transforming collections.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Null-safe operations
List<int>? items = null;
items.IsNullOrEmpty();              // true
items.EmptyIfNull();                // Empty enumerable (no exception)

// Batching for bulk operations
foreach (var batch in items.Batch(100))
{
    await ProcessBatchAsync(batch);
}

// Advanced filtering
items.WhereNotNull();
items.DistinctBy(x => x.Category);
items.WhereIf(includeInactive, x => x.IsActive);

// Safe access (no exceptions)
items.SafeFirst();                  // Returns default instead of throwing
items.SafeLast();
items.SafeFirst(x => x.Id == 5);

// Partitioning
var (evens, odds) = numbers.Partition(x => x % 2 == 0);

// Full-text search
var results = people.Search("john smith", p => p.Name, p => p.Email);

// Fuzzy search with Levenshtein distance
var fuzzy = people.FuzzySearch("Jon", threshold: 0.7, p => p.Name);

// Dynamic ordering (useful for APIs)
var sorted = items.OrderByDynamic("CreatedDate", descending: true);

// Find duplicates
var duplicates = items.FindDuplicatesBy(x => x.Email);

// Random operations
var shuffled = items.Shuffle();
var sample = items.TakeRandom(5);
```

</details>

---

### 🔐 Cryptography Extensions

Secure hashing, encryption, and random generation utilities.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Hashing
"data".ToSHA256();                  // SHA-256 hash
"data".ToSHA512();                  // SHA-512 hash
"data".ToHMACSHA256("secretKey");   // HMAC-SHA256

// Password hashing (BCrypt - recommended)
var hash = "password".ToBCrypt();
var isValid = "password".VerifyBCrypt(hash);  // true

// AES Encryption
var encrypted = "secret data".EncryptAES("password");
var decrypted = encrypted.DecryptAES("password");

// AES-256 with explicit IV
var (key, iv) = (EncryptionExtensions.GenerateAESKey(),
                 EncryptionExtensions.GenerateIV());
var encrypted = "data".EncryptAES256(key, iv);
var decrypted = encrypted.DecryptAES256(key, iv);

// Secure random generation
var token = SecureRandomExtensions.GenerateSecureToken(32);
var otp = SecureRandomExtensions.GenerateOTP(6);     // "847293"

// Password generation with options
var password = SecureRandomExtensions.GeneratePassword(16, new PasswordOptions
{
    IncludeUppercase = true,
    IncludeLowercase = true,
    IncludeDigits = true,
    IncludeSpecialChars = true
});
```

</details>

---

### 📅 DateTime Extensions

Rich date/time manipulation with boundary calculations and relative formatting.

<details>
<summary><b>View Examples</b></summary>

```csharp
var date = DateTime.Now;

// Boundaries
date.StartOfDay();                  // 2024-01-15 00:00:00
date.EndOfDay();                    // 2024-01-15 23:59:59.999
date.StartOfWeek();                 // Monday of current week
date.EndOfMonth();                  // Last moment of month
date.StartOfQuarter();              // First day of quarter
date.StartOfYear();                 // January 1st

// Boolean checks
date.IsWeekend();
date.IsWeekday();
date.IsToday();
date.IsPast();
date.IsFuture();
date.IsBetween(start, end);

// Age calculation
birthDate.Age();                    // Calculate exact age in years

// Unix timestamps
date.ToUnixTimestamp();             // 1705312800
1705312800L.FromUnixTimestamp();    // DateTime

// Relative time (supports English & Turkish)
pastDate.ToRelativeTime();          // "2 hours ago" / "2 saat önce"
futureDate.ToRelativeTime();        // "in 3 days" / "3 gün sonra"

// Friendly formatting
date.ToFriendlyString();            // "Monday, January 15, 2024"
```

</details>

---

### 🗓️ Holiday Calendar System

Business day calculations with built-in support for Turkey and US holidays.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Get calendar
var trCalendar = HolidayProvider.Turkey;
var usCalendar = HolidayProvider.US;

// Holiday checks
date.IsHoliday(trCalendar);
date.IsWorkday(trCalendar);         // Not weekend AND not holiday

// Business day navigation
date.NextWorkday(trCalendar);
date.PreviousWorkday(trCalendar);

// Add business days (skips weekends and holidays)
var deadline = date.AddWorkdays(5, trCalendar);

// Count workdays between dates
var workdays = startDate.GetWorkdaysBetween(endDate, trCalendar);

// Get all holidays for a year
var holidays = trCalendar.GetHolidays(2024);
foreach (var holiday in holidays)
{
    Console.WriteLine($"{holiday.Name}: {holiday.Date:d}");
}

// Turkey holidays include:
// - Fixed: Yılbaşı, 23 Nisan, 1 Mayıs, 19 Mayıs, 30 Ağustos, 29 Ekim
// - Islamic (calculated): Ramazan Bayramı, Kurban Bayramı

// US holidays include:
// - New Year, MLK Day, Presidents Day, Memorial Day
// - Independence Day, Labor Day, Columbus Day
// - Veterans Day, Thanksgiving, Christmas
```

</details>

---

### 🔄 Transformation Extensions

JSON operations, object mapping, type conversion, and string interpolation.

<details>
<summary><b>View Examples</b></summary>

```csharp
// JSON operations
var json = obj.ToJson();
var pretty = obj.ToJsonPretty();
var obj = json.FromJson<MyClass>();
var merged = json1.MergeJson(json2);

// JSON path access
var city = json.GetJsonValue<string>("person.address.city");

// Object mapping (reflection-based)
var dto = entity.MapTo<UserDto>();
var dtos = entities.MapToList<UserDto>();

// Deep cloning
var clone = obj.CloneDeep();

// Object to dictionary
var dict = obj.ToDictionary();
var obj = dict.FromDictionary<MyClass>();

// Object diff
var differences = obj1.Diff(obj2);

// Universal type conversion
"42".To<int>();                     // 42
"3.14".To<decimal>();               // 3.14m
"true".ToBool();                    // true (also: "1", "yes", "on")
"Second".ToEnum<Priority>();        // Priority.Second

// Safe conversion with defaults
"invalid".ToOrDefault(0);           // 0

// Named string interpolation
var message = "Hello {Name}, you have {Count} messages"
    .Interpolate(new { Name = "John", Count = 5 });
// "Hello John, you have 5 messages"

// Dictionary-based interpolation
var template = "Order #{OrderId} shipped to {City}"
    .InterpolateWith(new Dictionary<string, object>
    {
        ["OrderId"] = 12345,
        ["City"] = "Istanbul"
    });
```

</details>

---

### ✅ Validation

Guard clauses and fluent validation builder for clean, expressive validation.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Guard clauses (throw on invalid)
Guard.Against.Null(user, nameof(user));
Guard.Against.NullOrEmpty(name, nameof(name));
Guard.Against.NullOrWhiteSpace(description);
Guard.Against.OutOfRange(age, 0, 120, nameof(age));
Guard.Against.Zero(quantity);
Guard.Against.Negative(price);
Guard.Against.InvalidEmail(email);
Guard.Against.Default(guid);  // Throws if Guid.Empty

// Custom guard
Guard.Against.InvalidInput(
    password,
    p => p.Length >= 8,
    "Password must be at least 8 characters"
);

// Fluent validation builder
var result = user.Validate()
    .NotNull(x => x.Name, "Name is required")
    .NotEmpty(x => x.Name, "Name cannot be empty")
    .MinLength(x => x.Name, 2, "Name too short")
    .MaxLength(x => x.Name, 100, "Name too long")
    .ValidEmail(x => x.Email, "Invalid email format")
    .InRange(x => x.Age, 18, 120, "Age must be 18-120")
    .Matches(x => x.Phone, @"^\+?[\d\s-]+$", "Invalid phone")
    .Must(x => x.Password == x.ConfirmPassword, "Passwords must match")
    .Build();

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

</details>

---

### 🎯 Result Pattern

Railway-oriented programming for elegant error handling without exceptions.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Creating results
var success = Result.Success(user);
var failure = Result.Failure<User>(Error.NotFound("User not found"));

// Function returning Result
Result<User> GetUser(int id)
{
    if (id <= 0)
        return Result.Failure<User>(Error.Validation("ID must be positive"));

    var user = repository.Find(id);
    return user is null
        ? Result.Failure<User>(Error.NotFound("User not found"))
        : Result.Success(user);
}

// Chaining operations (Railway pattern)
var result = GetUser(1)
    .Map(u => u.Email)                    // Transform value
    .Bind(email => ValidateEmail(email))  // Chain another Result
    .OnSuccess(email => SendNotification(email))
    .OnFailure(error => logger.LogError(error.Message));

// Pattern matching
var message = result.Match(
    onSuccess: user => $"Welcome, {user.Name}!",
    onFailure: error => $"Error: {error.Message}"
);

// Get value safely
var user = result.GetValueOrDefault(defaultUser);
var user = result.GetValueOrThrow();  // Throws if failure

// Error types
Error.Failure("Something went wrong");
Error.Validation("Invalid input");
Error.NotFound("Resource not found");
Error.Conflict("Already exists");
Error.Unauthorized("Access denied");
```

</details>

---

### 🔮 Maybe/Option Type

Explicit handling of optional values - no more null reference exceptions.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Creating Maybe values
var some = Maybe<User>.Some(user);
var none = Maybe<User>.None;
var maybe = Maybe<User>.From(possiblyNullUser);

// Function returning Maybe
Maybe<User> FindUser(int id)
{
    var user = repository.Find(id);
    return Maybe<User>.From(user);
}

// Safe chaining
var displayName = FindUser(1)
    .Map(u => u.Profile)
    .Map(p => p.DisplayName)
    .GetValueOrDefault("Anonymous");

// Pattern matching
var greeting = maybeUser.Match(
    some: user => $"Hello, {user.Name}!",
    none: () => "Hello, Guest!"
);

// Conditional execution
maybeUser
    .WhenSome(user => SendWelcomeEmail(user))
    .WhenNone(() => LogAnonymousVisit());

// Collection operations
var users = maybeUsers.WhereSome();      // Filter out None values
var first = items.FirstOrNone();          // Returns Maybe<T>
var found = items.SingleOrNone(x => x.Id == 5);

// Dictionary operations
var value = dictionary.TryGetValue("key"); // Returns Maybe<TValue>
```

</details>

---

### ⚖️ Either Type

Represent values that can be one of two types - perfect for error handling.

<details>
<summary><b>View Examples</b></summary>

```csharp
// Creating Either values
Either<Error, User> success = user;           // Implicit right
Either<Error, User> failure = new Error(...); // Implicit left

// Function returning Either
Either<ValidationError, User> ParseUser(string json)
{
    try
    {
        var user = json.FromJson<User>();
        return user ?? new ValidationError("Null user");
    }
    catch (Exception ex)
    {
        return new ValidationError(ex.Message);
    }
}

// Chaining
var result = ParseUser(json)
    .MapRight(user => user.Name)
    .MapLeft(error => new DetailedError(error));

// Pattern matching
var message = result.Match(
    onLeft: error => $"Error: {error.Message}",
    onRight: name => $"User: {name}"
);

// Conditional execution
result
    .WhenRight(user => SaveToDatabase(user))
    .WhenLeft(error => LogError(error));

// Extracting values
if (result.IsRight)
{
    var user = result.Right;
}
```

</details>

---

## Requirements

- .NET 8.0 or .NET 9.0
- C# 12.0 or later

## Dependencies

- **BCrypt.Net-Next** - Secure password hashing

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.


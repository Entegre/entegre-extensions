# Entegre.Extensions

A comprehensive .NET 8+ developer toolkit providing extensions for strings, collections, encryption, transformations, dates with holidays, validation, and more.

[![.NET](https://img.shields.io/badge/.NET-8.0+-512BD4)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Installation

```bash
dotnet add package Entegre.Extensions
```

## Features

### String Extensions

```csharp
using Entegre.Extensions;

// Fluent null checks
"hello".IsNullOrEmpty();      // false
"  ".IsNullOrWhiteSpace();    // true

// URL-friendly slugs (Turkish support)
"Türkçe Başlık".ToSlug();     // "turkce-baslik"

// Safe string operations
"Hello World".Truncate(8);     // "Hello..."
"Hello".Left(3);               // "Hel"
"Hello".Right(3);              // "llo"
"abc".Repeat(3);               // "abcabcabc"

// Case conversions
"hello world".ToPascalCase();  // "HelloWorld"
"HelloWorld".ToSnakeCase();    // "hello_world"
"helloWorld".ToKebabCase();    // "hello-world"

// Encoding
"Hello".ToBase64();            // "SGVsbG8="
"SGVsbG8=".FromBase64();       // "Hello"

// Validation
"test@example.com".IsEmail();  // true
"10000000146".IsTCKN();        // true (Turkish ID)
"TR330006100519786457841326".IsIBAN(); // true
"4111111111111111".IsCreditCard();     // true (Luhn check)
```

### Collection Extensions

```csharp
// Null-safe operations
List<int>? items = null;
items.IsNullOrEmpty();         // true
items.EmptyIfNull();           // Empty enumerable

// Batching
var batches = items.Batch(100);

// Filtering
items.WhereNotNull();
items.DistinctBy(x => x.Category);

// Safe access
items.SafeFirst();             // Returns default instead of throwing
items.SafeLast();

// Partitioning
var (evens, odds) = numbers.Partition(x => x % 2 == 0);

// Search
var results = people.Search("john", p => p.Name, p => p.Email);
var fuzzy = people.FuzzySearch("Jon", threshold: 0.7, p => p.Name);

// Dynamic ordering
var sorted = items.OrderByDynamic("Name", descending: true);
```

### Cryptography Extensions

```csharp
// Hashing
"password".ToSHA256();
"data".ToHMACSHA256("secretKey");

// Password hashing (BCrypt)
var hash = "password".ToBCrypt();
var isValid = "password".VerifyBCrypt(hash);

// AES Encryption
var encrypted = "secret data".EncryptAES("password");
var decrypted = encrypted.DecryptAES("password");

// Secure random generation
var token = SecureRandomExtensions.GenerateSecureToken(32);
var password = SecureRandomExtensions.GeneratePassword(16);
var otp = SecureRandomExtensions.GenerateOTP(6);
```

### DateTime Extensions

```csharp
var date = DateTime.Now;

// Boundaries
date.StartOfDay();
date.EndOfMonth();
date.StartOfQuarter();

// Checks
date.IsWeekend();
date.IsWorkday();
date.IsBetween(start, end);

// Age calculation
birthDate.Age();               // Calculate age

// Unix timestamps
date.ToUnixTimestamp();
timestamp.FromUnixTimestamp();

// Relative time
date.ToRelativeTime();         // "2 hours ago" / "2 saat önce"
```

### Holiday Calendar System

```csharp
// Turkey holidays
var trCalendar = HolidayProvider.Turkey;
date.IsHoliday(trCalendar);
date.IsWorkday(trCalendar);
date.NextWorkday(trCalendar);
date.AddWorkdays(5, trCalendar);

// US holidays
var usCalendar = HolidayProvider.US;

// Get holidays for a year
var holidays = trCalendar.GetHolidays(2024);

// Includes:
// - Fixed holidays (Cumhuriyet Bayramı, Independence Day, etc.)
// - Dynamic Islamic holidays (Ramazan Bayramı, Kurban Bayramı)
// - Observed holidays
```

### Transformation Extensions

```csharp
// JSON
var json = obj.ToJson();
var obj = json.FromJson<MyClass>();
var merged = json1.MergeJson(json2);
var value = json.GetJsonValue<string>("person.address.city");

// Object mapping
var dto = entity.MapTo<MyDto>();
var clone = obj.CloneDeep();
var dict = obj.ToDictionary();
var diff = obj1.Diff(obj2);

// Type conversion
"42".To<int>();
"true".ToBool();               // Also: "1", "yes", "on"
"Second".ToEnum<MyEnum>();

// String interpolation
"Hello {Name}!".Interpolate(new { Name = "World" });
```

### Validation

```csharp
// Guard clauses
Guard.Against.Null(value);
Guard.Against.NullOrEmpty(name);
Guard.Against.OutOfRange(age, 0, 120);
Guard.Against.InvalidEmail(email);

// Fluent validation
var result = user.Validate()
    .NotEmpty(x => x.Name, "Name is required")
    .ValidEmail(x => x.Email, "Invalid email")
    .InRange(x => x.Age, 18, 120, "Invalid age")
    .Must(x => x.Password == x.ConfirmPassword, "Passwords must match")
    .Build();

if (!result.IsValid)
{
    foreach (var error in result.Errors)
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
}
```

### Result Pattern

```csharp
// Railway-oriented programming
Result<User> GetUser(int id)
{
    if (id <= 0)
        return Result.Failure<User>(Error.Validation("Invalid ID"));

    var user = repository.Find(id);
    return user is null
        ? Result.Failure<User>(Error.NotFound)
        : Result.Success(user);
}

// Chaining
var result = GetUser(1)
    .Map(u => u.Email)
    .Bind(ValidateEmail)
    .OnSuccess(email => SendNotification(email))
    .OnFailure(error => LogError(error));

// Pattern matching
var message = result.Match(
    onSuccess: user => $"Found: {user.Name}",
    onFailure: error => $"Error: {error.Message}"
);
```

### Maybe/Option Type

```csharp
Maybe<User> FindUser(int id)
{
    var user = repository.Find(id);
    return Maybe<User>.From(user);
}

var user = FindUser(1)
    .Map(u => u.Name)
    .GetValueOrDefault("Unknown");

// Dictionary lookup
var value = dictionary.TryGetValue("key");  // Returns Maybe<T>

// Collection operations
var first = items.FirstOrNone();
var filtered = maybes.WhereSome();
```

### Either Type

```csharp
Either<Error, User> ParseUser(string json)
{
    try
    {
        return json.FromJson<User>()!;
    }
    catch (Exception ex)
    {
        return new Error("Parse", ex.Message);
    }
}

var result = ParseUser(json)
    .MapRight(user => user.Name)
    .Match(
        onLeft: error => $"Error: {error.Message}",
        onRight: name => $"User: {name}"
    );
```

## License

MIT License - see [LICENSE](LICENSE) for details.

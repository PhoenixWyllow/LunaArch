// Suppress CA1707 for test method names - underscores are conventional in test names
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method naming convention uses underscores for readability", Scope = "namespaceanddescendants", Target = "~N:LunaArch.Domain.Tests")]

// Suppress CA1305 for ToString calls in tests
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "Not critical for test assertions", Scope = "namespaceanddescendants", Target = "~N:LunaArch.Domain.Tests")]

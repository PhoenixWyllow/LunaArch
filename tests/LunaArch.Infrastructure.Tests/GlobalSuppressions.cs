// Suppress CA1707 for test method names - underscores are conventional in test names
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "Test method naming convention uses underscores for readability", Scope = "namespaceanddescendants", Target = "~N:LunaArch.Infrastructure.Tests")]

// Suppress CA1848 in test projects - LoggerMessage delegates not needed for test code
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1848:Use the LoggerMessage delegates", Justification = "Not needed in test code", Scope = "namespaceanddescendants", Target = "~N:LunaArch.Infrastructure.Tests")]

// Suppress CA1711 for test event handlers - naming convention is clear in tests
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "EventHandler suffix is appropriate for domain event handlers", Scope = "namespaceanddescendants", Target = "~N:LunaArch.Infrastructure.Tests")]

// Suppress xUnit1051 in tests - TestContext.Current.CancellationToken not critical for these tests
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1051:Ensure TestContext.Current.CancellationToken is used", Justification = "Not critical for these unit tests", Scope = "namespaceanddescendants", Target = "~N:LunaArch.Infrastructure.Tests")]

// Suppress IL2026 and IL3050 from Vogen-generated code in test projects
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Trimming", "IL2026:Members annotated with RequiresUnreferencedCodeAttribute", Justification = "Vogen-generated code, not relevant for test projects")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("AOT", "IL3050:Members annotated with RequiresDynamicCodeAttribute", Justification = "Vogen-generated code, not relevant for test projects")]

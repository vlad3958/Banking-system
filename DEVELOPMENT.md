# Development Guide

This guide covers development choises

##  Project Architecture

### Layered Architecture

```
┌─────────────────────┐
│   Banking           │ ← Controllers (API endpoints), Repositories 
├─────────────────────┤
│   Banking.DAL       │ ← Entities, DTOs, DbContext
├─────────────────────┤
```

### Project Dependencies

```
Banking (API) 
├── Banking.DAL (Data Access Layer)
└── Banking.Tests → Banking + Banking.DAL
```


##  Testing Guidelines

### Test Structure (AAA Pattern)

```csharp
[Fact]
public async Task MethodName_Condition_ExpectedResult()
{
    // Arrange - Setup test data and mocks
    var mockRepo = new Mock<IRepository>();
    var controller = new Controller(mockRepo.Object);
    
    // Act - Execute the method being tested
    var result = await controller.Method(testData);
    
    // Assert - Verify the results
    Assert.IsType<OkResult>(result);
}
```

##  Security Guidelines

### Authentication & Authorization

**JWT Configuration**:
```csharp
[Authorize]                    // Requires authentication
[Authorize(Roles = "Admin")]   // Requires specific role
```

**Input Validation**:
```csharp
[Required]
[EmailAddress]
public string Email { get; set; }
```

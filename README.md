# Handsey - Currently in development

## Introduction

In short Handsey is automatic handler registration framework. Handlers are selected for a handler request based on type of Handler and then registered within a pre configured IOC container for instantiation.

Once the calling application has requested that handlers are triggered it can invoke any method on the list of handlers found.

As part of the automatic registration process Handsey also supports generic prarameter injection. Handlers are selected for generic parameter injection based on the generic parameters of the requested handler, an example is below.

#### Model

```C#
public class Employee : IVersionable
{
	public string EmailAddress { get; set; }
}

public interface IChangeHandler<TChanged>
{
     void Handle(TChanged changed);
}
```

#### Requested handler
```C#
IChangeHandler<Employee>
```

#### Matched handlers
```C#
public class SaveVersion<TVersionable> : IChangeHandler<TVersionable>
	where TVersionable : IVersionable
{ 
     void Handle(TVersionable versionable)
     {
        // do something
     }
}

public class AlertEmployeeChange<TEmployee> : IChangeHandler<TEmployee>
	where TEmployee : Employee
{
     void Handle(TEmployee employee)
     {
        // do something
     }
}
```
